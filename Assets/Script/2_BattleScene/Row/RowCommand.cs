using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    public static class RowCommand
    {
        public static Location GetLocation(Card TargetCard)
        {
            int RankX = -1;
            int RankY = -1;
            for (int i = 0; i < CardSet.GlobalCardList.Count; i++)
            {
                if (CardSet.GlobalCardList[i].Contains(TargetCard))
                {
                    RankX = i;
                    RankY = CardSet.GlobalCardList[i].IndexOf(TargetCard);
                }
            }
            return new Location(RankX, RankY);
        }
        public static int GetLocationX(Orientation orientation, GameRegion gameRegion)
        {
            var targetRow = AgainstInfo.cardSet.RowManagers.FirstOrDefault(row => row.orientation == orientation && row.gameRegion == gameRegion);

            if (targetRow == null) return -1;
            return targetRow.RowRank;
        }
        public static Card GetCard(int x, int y) => x == -1 ? null : CardSet.GlobalCardList[x][y];
        public static Card GetCard(Location location) => location.X == -1 ? null : CardSet.GlobalCardList[location.X][location.Y];
        public static List<Card> GetCardList(Card targetCard) => CardSet.GlobalCardList.First(list => list.Contains(targetCard));

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void SetPlayCardMoveFree(bool isFree) => AgainstInfo.cardSet[Orientation.Down][GameRegion.Leader, GameRegion.Hand].CardList.ForEach(card => card.Manager.IsFree = isFree);
        public static void SetRegionSelectable(Territory territory = Territory.All, GameRegion region = GameRegion.None)
        {
            if (region == GameRegion.None)
            {
                AgainstInfo.cardSet[GameRegion.Battle].RowManagers.ForEach(row =>
                {
                    row.CanBeSelected = false;
                    row.RowMaterial.SetColor("_GlossColor", Color.black);
                });
            }
            else
            {
                AgainstInfo.cardSet[region][(Orientation)territory].RowManagers.ForEach(row =>
                {
                    row.CanBeSelected = true;
                    row.RowMaterial.SetColor("_GlossColor", row.color);
                });
            }
        }
        /// <summary>
        ///刷新所有行管理下的卡牌信息、位置等
        /// </summary>
        public static void RefreshAllRowsCards() => AgainstInfo.cardSet.RowManagers.ForEach(row => row.RefreshRowCards());
    }
}