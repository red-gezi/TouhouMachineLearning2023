using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;
namespace TouhouMachineLearningSummary.Info
{
    public class CardSet
    {
        /// <summary>
        /// 卡牌数据集的最源头，可直接修改
        /// </summary>
        [ShowInInspector]
        public static List<List<Card>> GlobalCardList { get; set; } = new List<List<Card>>();
        /// <summary>
        /// 每个区域的管理器，按编号排序
        /// </summary>
        public List<RowManager> RowManagers { get; set; } = new List<RowManager>();
        [ShowInInspector]
        private List<Card> cardList = null;
        /// <summary>
        /// 全局数据集通过各种筛选后剩下的卡牌集合
        /// </summary>
        public List<Card> CardList { get => cardList ?? GlobalCardList.SelectMany(x => x).ToList(); set => cardList = value; }
        public List<Card> GetSameIdCard(string cardId, int num) => CardList.Where(card => card.CardID == cardId).Take(num).ToList();

        public CardSet()
        {
            GlobalCardList.Clear();
            Enumerable.Range(0, 18).ToList().ForEach(x => GlobalCardList.Add(new List<Card>()));
        }
        public CardSet(List<RowManager> singleRowInfos, List<Card> cardList = null)
        {
            this.RowManagers = singleRowInfos;
            this.CardList = cardList;
        }
        public List<Card> this[int rank]
        {
            get => GlobalCardList[rank];
            set => GlobalCardList[rank] = value;
        }
        public CardSet this[params GameRegion[] regions]
        {
            get
            {
                List<RowManager> targetRows = new List<RowManager>();
                regions.ToList().ForEach(region =>
                {
                    if (region == GameRegion.Battle)
                    {
                        targetRows.AddRange(RowManagers.Where(row =>
                        row.gameRegion == GameRegion.Water ||
                        row.gameRegion == GameRegion.Fire ||
                        row.gameRegion == GameRegion.Wind ||
                        row.gameRegion == GameRegion.Soil
                        ));
                    }
                    else
                    {
                        targetRows.AddRange(RowManagers.Where(row => row.gameRegion == region));
                    }
                });
                List<Card> filterCardList = CardList ?? GlobalCardList.SelectMany(x => x).ToList();
                filterCardList = filterCardList.Intersect(targetRows.SelectMany(x => x.CardList)).ToList();
                return new CardSet(targetRows, filterCardList);
            }
        }
        public CardSet this[Orientation orientation]
        {
            get
            {
                List<RowManager> targetRows = new List<RowManager>();
                switch (orientation)
                {
                    case Orientation.Up:
                        targetRows = RowManagers.Where(row => row.orientation == orientation).ToList(); break;
                    case Orientation.Down:
                        targetRows = RowManagers.Where(row => row.orientation == orientation).ToList(); break;
                    case Orientation.My:
                        return this[AgainstInfo.IsMyTurn ? Orientation.Down : Orientation.Up];
                    case Orientation.Op:
                        return this[AgainstInfo.IsMyTurn ? Orientation.Up : Orientation.Down];
                    case Orientation.All:
                        targetRows = RowManagers; break;
                }
                List<Card> filterCardList = CardList;
                filterCardList = filterCardList.Intersect(targetRows.SelectMany(x => x.CardList)).ToList();
                return new CardSet(targetRows, filterCardList);
            }
        }
        //待补充
        public CardSet this[CardState cardState]
        {
            get
            {
                List<Card> filterCardList = CardList.Where(card => card.cardStates.Contains(cardState)).ToList();
                return new CardSet(RowManagers, filterCardList);
            }
        }
        //待补充
        public CardSet this[CardField cardField]
        {
            get
            {
                List<Card> filterCardList = CardList.Where(card => card.cardFields.ContainsKey(cardField)).ToList();
                return new CardSet(RowManagers, filterCardList);
            }
        }
        /// <summary>
        /// 根据Tag检索卡牌
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public CardSet this[params CardTag[] tags]
        {
            get
            {
                CardList = CardList ?? GlobalCardList.SelectMany(x => x).ToList();
                List<Card> filterCardList = CardList.Where(card =>
                    tags.Any(tag =>
                        card.TranslateTags.Contains(tag.ToString().TranslationGameText())))
                    .ToList();
                return new CardSet(RowManagers, filterCardList);
            }
        }
        /// <summary>
        /// 根据一些特征筛选卡牌
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public CardSet this[params CardFeature[] cardFeatures]
        {
            get
            {
                var filterCardList = CardList.Where(card => card.ShowPoint != 0);
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
                return new CardSet(RowManagers, filterCardList.ToList());
            }
        }
        //按卡牌阶级筛选
        public CardSet this[params CardRank[] ranks]
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
                List<Card> filterCardList = CardList
                    .Where(card => ranks.Any(rank => card.Rank == rank))
                    .ToList();
                return new CardSet(RowManagers, filterCardList);
            }
        }
        //按卡牌类型筛选
        public CardSet this[CardType type]
        {
            get
            {
                List<Card> filterCardList = CardList
                    .Where(card => card.Type == type)
                    .ToList();
                return new CardSet(RowManagers, filterCardList);
            }
        }
        public void Add(Card card, int rank = -1)
        {
            if (RowManagers.Count != 1)
            {
                Debug.LogWarning("选择区域异常，数量为" + RowManagers.Count);
            }
            var targetRowInfo = RowManagers.FirstOrDefault();
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
            if (RowManagers.Count != 1)
            {
                Debug.LogWarning("选择区域异常，数量为" + RowManagers.Count);
            }
            RowManagers.FirstOrDefault()?.CardList.Remove(card);
        }
    }
}