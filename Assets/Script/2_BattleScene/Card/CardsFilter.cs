using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
namespace TouhouMachineLearningSummary.Other
{
    /// <summary>
    /// 卡牌数据集合刷选起
    /// </summary>
    public class CardsFilter
    {
        /// <summary>
        /// 当前对局中的所有卡牌
        /// </summary>
        [ShowInInspector]
        public static List<List<Card>> GlobalCardList => RowsInfo.rowInfoList.Select(rowInfo => rowInfo.CardList).ToList();
        /// <summary>
        /// 当前过滤器包含的所有行信息
        /// </summary>
        public List<RowInfo> ContainRowInfos { get; set; } = new List<RowInfo>();
        [ShowInInspector]
        private List<Card> cardList  = null;
        /// <summary>
        /// 全局数据集通过各种筛选后剩下的卡牌集合
        /// </summary>
        public List<Card> ContainCardList { get => cardList ?? GlobalCardList.SelectMany(x => x).ToList(); set => cardList = value; }
        public List<Card> GetCardByID(string cardId, int num) => ContainCardList.Where(card => card.CardID == cardId).Take(num).ToList();

        public CardsFilter() 
        {
            ContainRowInfos = RowsInfo.rowInfoList;
        }
        public CardsFilter(List<RowInfo> singleRowInfos, List<Card> cardList = null)
        {
            ContainRowInfos = singleRowInfos;
            ContainCardList = cardList;
        }
        public List<Card> this[int rank]
        {
            get => GlobalCardList[rank];
            set => GlobalCardList[rank] = value;
        }
        public CardsFilter this[params GameRegion[] regions]
        {
            get
            {
                List<RowInfo> targetRows = new();
                regions.ToList().ForEach(region =>
                {
                    if (region == GameRegion.Battle)
                    {
                        targetRows.AddRange(ContainRowInfos.Where(row =>
                        row.gameRegion == GameRegion.Water ||
                        row.gameRegion == GameRegion.Fire ||
                        row.gameRegion == GameRegion.Wind ||
                        row.gameRegion == GameRegion.Soil
                        ));
                    }
                    else
                    {
                        targetRows.AddRange(ContainRowInfos.Where(row => row.gameRegion == region));
                    }
                });
                List<Card> filterCardList = ContainCardList ?? GlobalCardList.SelectMany(x => x).ToList();
                filterCardList = filterCardList.Intersect(targetRows.SelectMany(x => x.CardList)).ToList();
                return new CardsFilter(targetRows, filterCardList);
            }
        }
        public CardsFilter this[Orientation orientation]
        {
            get
            {
                List<RowInfo> targetRows = new();
                switch (orientation)
                {
                    case Orientation.Up:
                        targetRows = ContainRowInfos.Where(row => row.orientation == orientation).ToList(); break;
                    case Orientation.Down:
                        targetRows = ContainRowInfos.Where(row => row.orientation == orientation).ToList(); break;
                    case Orientation.My:
                        return this[AgainstInfo.IsMyTurn ? Orientation.Down : Orientation.Up];
                    case Orientation.Op:
                        return this[AgainstInfo.IsMyTurn ? Orientation.Up : Orientation.Down];
                    case Orientation.All:
                        targetRows = ContainRowInfos; break;
                }
                List<Card> filterCardList = ContainCardList;
                filterCardList = filterCardList.Intersect(targetRows.SelectMany(x => x.CardList)).ToList();
                return new CardsFilter(targetRows, filterCardList);
            }
        }
        //待补充
        public CardsFilter this[CardState cardState]
        {
            get
            {
                List<Card> filterCardList = ContainCardList.Where(card => card.cardStates.Contains(cardState)).ToList();
                return new CardsFilter(ContainRowInfos, filterCardList);
            }
        }
        //待补充
        public CardsFilter this[CardField cardField]
        {
            get
            {
                List<Card> filterCardList = ContainCardList.Where(card => card.cardFields.ContainsKey(cardField)).ToList();
                return new CardsFilter(ContainRowInfos, filterCardList);
            }
        }
        /// <summary>
        /// 根据Tag检索卡牌
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public CardsFilter this[params CardTag[] tags]
        {
            get
            {
                ContainCardList = ContainCardList ?? GlobalCardList.SelectMany(x => x).ToList();
                List<Card> filterCardList = ContainCardList.Where(card =>
                    tags.Any(tag =>
                        card.TranslateTags.Contains(tag.ToString().TranslationGameText())))
                    .ToList();
                return new CardsFilter(ContainRowInfos, filterCardList);
            }
        }
        /// <summary>
        /// 根据一些特征筛选卡牌
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public CardsFilter this[params CardFeature[] cardFeatures]
        {
            get
            {
                var filterCardList = ContainCardList.Where(card => card.ShowPoint != 0);
                if (filterCardList.Any())
                {
                    cardFeatures.ToList().ForEach(cardFeature =>
                    {
                        switch (cardFeature)
                        {
                            case CardFeature.LargestPointUnits:
                                int largestPoint = filterCardList.Max(card => card.ShowPoint);
                                filterCardList = filterCardList.Where(card => card.ShowPoint == largestPoint);
                                break;
                            case CardFeature.LowestPointUnits:
                                int lowestPoint = filterCardList.Min(card => card.ShowPoint);
                                filterCardList = filterCardList.Where(card => card.ShowPoint == lowestPoint);
                                break;
                            case CardFeature.LargestRankUnits:
                                CardRank largestRank = filterCardList.Min(card => card.Rank);
                                filterCardList = filterCardList.Where(card => card.Rank == largestRank);
                                break;
                            case CardFeature.LowestRankUnits:
                                CardRank lowestRank = filterCardList.Min(card => card.Rank);
                                filterCardList = filterCardList.Where(card => card.Rank == lowestRank);
                                break;
                            case CardFeature.NotZero:
                                //已经排除了非0单位，可以空
                                break;
                            default:
                                break;
                        }
                    });

                }
                return new CardsFilter(ContainRowInfos, filterCardList.ToList());
            }
        }
        //按卡牌阶级筛选
        public CardsFilter this[params CardRank[] ranks]
        {
            get
            {
                if (ranks[0] == CardRank.NoGold)
                {
                    ranks = new CardRank[] { CardRank.Silver, CardRank.Copper };
                }
                if (ranks[0] == CardRank.GoldAndLeader)
                {
                    ranks = new CardRank[] { CardRank.Gold, CardRank.Leader };
                }
                List<Card> filterCardList = ContainCardList
                    .Where(card => ranks.Any(rank => card.Rank == rank))
                    .ToList();
                return new CardsFilter(ContainRowInfos, filterCardList);
            }
        }
        //按卡牌类型筛选
        public CardsFilter this[CardType type]
        {
            get
            {
                List<Card> filterCardList = ContainCardList
                    .Where(card => card.Type == type)
                    .ToList();
                return new CardsFilter(ContainRowInfos, filterCardList);
            }
        }
        public void Add(Card card, int rank = -1)
        {
            if (ContainRowInfos.Count != 1)
            {
                Debug.LogWarning("选择区域异常，数量为" + ContainRowInfos.Count);
            }
            var targetRowInfo = ContainRowInfos.FirstOrDefault();
            if (rank < 0)
            {
                rank = Mathf.Max(0, targetRowInfo.CardList.Count + rank + 1);
            }
            else
            {
                rank = Mathf.Min(rank, targetRowInfo.CardList.Count);
            }
            targetRowInfo?.CardList.Insert(rank, card);
        }
        public void Remove(Card card)
        {
            if (ContainRowInfos.Count != 1)
            {
                Debug.LogWarning("选择区域异常，数量为" + ContainRowInfos.Count);
            }
            ContainRowInfos.FirstOrDefault()?.CardList.Remove(card);
        }
    }
}