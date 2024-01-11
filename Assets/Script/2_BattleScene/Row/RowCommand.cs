using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Other;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    public static class RowCommand
    {
        ////////////////////////////////////////获取游戏中行信息的某个数据///////////////////////////////////////////////////////////////////
        public static Card GetCard(int row, int rank) => row == -1 ? null : RowsInfo.rowInfoList[row].CardList[rank];
        public static Card GetCard(Location location) => location.Row == -1 ? null : RowsInfo.rowInfoList[location.Row].CardList[location.Rank];
        public static RowInfo GetRowInfo(Card targetCard) => RowsInfo.rowInfoList.FirstOrDefault(row => row.CardList.Contains(targetCard));
        public static RowInfo GetRowInfo(int rowRank) => RowsInfo.rowInfoList[rowRank];
        public static RowInfo GetRowInfo(GameObject rowPrefab) => RowsInfo.rowInfoList.FirstOrDefault(row => row.rowPrefab == rowPrefab);
        public static List<RowInfo> GetAllRowsInfo(Card targetCard) => RowsInfo.rowInfoList;
        public static List<List<Card>> GetAllCardInfo(Card targetCard) => RowsInfo.rowInfoList.Select(row => row.CardList).ToList();
        public static int GetRowRank(Card targetCard) => GetRowInfo(targetCard)?.RowRank ?? -1;
        public static int GetCardRank(Card targetCard) => GetRowInfo(targetCard)?.CardList?.IndexOf(targetCard) ?? -1;
        public static Location GetLocation(Card targetCard) => new Location(GetRowRank(targetCard), GetCardRank(targetCard));
        //判断玩家的焦点区域位置次序
        public static int GetFocusRank(RowInfo rowInfo)
        {
            float posx = -(AgainstInfo.FocusPoint.x - rowInfo.rowPrefab.transform.position.x);
            int cardNum = rowInfo.CardList.Where(card => !card.IsGray).Count();
            int rank = Enumerable.Range(0, cardNum).FirstOrDefault(i => posx > i * 1.6 - (cardNum - 1) * 0.8) + 1;
            return rank;
        }
        ////////////////////////////////////////设置卡牌行区域表现效果///////////////////////////////////////////////////////////////////
        public static void SetPlayCardMoveFree(bool isFree) => AgainstInfo.GameCardsFilter[Orientation.Down][GameRegion.Leader, GameRegion.Hand].ContainCardList.ForEach(card => card.Manager.IsFree = isFree);
        public static void SetRegionSelectable(GameRegion region, Territory territory = Territory.All)
        {
            //设置可选区域为null时关闭所有区域部署颜色显示
            if (region == GameRegion.None)
            {
                AgainstInfo.GameCardsFilter[GameRegion.Battle].ContainRowInfos.ForEach(row =>
                {
                    row.CanBeSelected = false;
                    row.RowMaterial.SetColor("_GlossColor", Color.black);
                });
            }
            else
            {
                AgainstInfo.GameCardsFilter[region][(Orientation)territory].ContainRowInfos.ForEach(row =>
                {
                    row.CanBeSelected = true;
                    row.RowMaterial.SetColor("_GlossColor", row.color);
                });
            }
        }
        //通过对局记录加载行数据
        public static void SetCardListFromSummary(List<List<SimpleCardModel>> sampleCardInfos)
        {
            for (int i = 0; i < Info.RowsInfo.rowInfoList.Count; i++)
            {
                Info.RowsInfo.rowInfoList[i].CardList = sampleCardInfos[i].Select(CardCommand.GenerateCard).ToList();
            }
        }
        ////////////////////////////////////////根据数据刷新行和卡牌的状态///////////////////////////////////////////////////////////////////

        /// <summary>
        ///刷新所有行管理下的卡牌信息、位置等
        /// </summary>
        public static void RefreshAllRowsCards()
        {
            Info.RowsInfo.rowInfoList.ForEach(rowInfo =>
            {
                //设置Card可见性
                rowInfo.CardList.ForEach(card => card.Manager.RefreshCardVisible());
                //设置点数
                rowInfo.CardList.ForEach(card => card.Manager.RefreshCardUi());
                //执行位置、角度移动动画
                rowInfo.CardList.ForEach(card => card.Manager.RefreshTransform());
            });
        }

        public static void RefreshTempCard()
        {
            Info.RowsInfo.rowInfoList.ForEach(rowInfo =>
            {
                if (AgainstInfo.IsMyTurn)
                {
                    Card tempCard = rowInfo.TempCard;
                    int focusRank = GetFocusRank(rowInfo);
                    //创建临时Card
                    if (tempCard == null && rowInfo.CanBeSelected && AgainstInfo.PlayerFocusRow == rowInfo)
                    {
                        Card modelCard = AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Used].ContainCardList.LastOrDefault();
                        tempCard = Command.CardCommand.GenerateTempCard(new Event(null, targetCard: null)
                                .SetTargetCardId(modelCard.CardID)
                                .SetLocation(rowInfo.orientation, rowInfo.gameRegion, focusRank));
                    }
                    //改变临时Card的位置
                    if (tempCard != null && focusRank != rowInfo.CardList.IndexOf(tempCard))
                    {
                        rowInfo.CardList.Remove(tempCard);
                        rowInfo.CardList.Insert(focusRank, tempCard);
                    }
                    //销毁临时Card
                    if (tempCard != null && !(rowInfo.CanBeSelected && AgainstInfo.PlayerFocusRow == rowInfo))
                    {
                        rowInfo.CardList.Remove(tempCard);
                        GameObject.Destroy(tempCard.gameObject);
                        tempCard = null;
                    }
                }
                //可部署时区域高亮
                rowInfo.RowMaterial.SetFloat("_Strength", Mathf.PingPong(Time.time * 10, 10) + 10);
            });
        }
    }
}