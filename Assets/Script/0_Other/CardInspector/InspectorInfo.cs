using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using UnityEngine;

namespace TouhouMachineLearningSummary.Info
{

    public class InspectorInfo
    {
        public static List<FileInfo> CardTexture { get; set; }
        public static Dictionary<Camp, Texture2D> SectarianIcons { get; set; } = new Dictionary<Camp, Texture2D>();
        public static Dictionary<CardRank, Texture2D> RankIcons { get; set; } = new Dictionary<CardRank, Texture2D>();
        public static InspectorInfo Instance { get; set; } = new InspectorInfo();

        [HorizontalGroup("Button", 120, LabelWidth = 70)]
        [Button("载入卡牌数据从表格")]
        public void Load() => InspectorCommand.LoadFromJson();

        [HorizontalGroup("Button", 120, LabelWidth = 70)]
        [Button("清空卡牌数据")]
        public void Clear() => InspectorCommand.ClearCardData();

        [ShowInInspector]
        [LabelText("单人模式牌库卡牌数量")]
        public int singleModeCardCount => singleModeCards.Count;
        [ShowInInspector]
        [LabelText("多人模式牌库卡牌数量")]
        int multiModeCardCount => multiModeCards.Count;
        [LabelText("使用语言")]
        public static string useLanguage => TranslateManager.currentLanguage;
        //[ShowInInspector]
        public List<CardModel> singleModeCards = new List<CardModel>();
        public List<CardModel> multiModeCards = new List<CardModel>();
        [HideInInspector]
        public List<LevelLibrary> levelLibries = new List<LevelLibrary>();
        [ShowInInspector]
        public List<string> includeLevel => singleModeCards.Select(x => x.level).Distinct().ToList();







        public partial class LevelLibrary
        {
            internal bool isSingleMode;
            public string level;
            public List<SectarianCardLibrary> sectarianCardLibraries = new List<SectarianCardLibrary>();
            [TabGroup("卡片管理")]
            public List<CardModel> cardModelInfos;
            public List<Camp> includeSectarian => cardModelInfos.Select(x => x.cardCamp).Distinct().ToList();

            public LevelLibrary(List<CardModel> singleModeCardsModels, string level)
            {
                this.level = level;
                isSingleMode = level != "多人";
                cardModelInfos = singleModeCardsModels.Where(cards => cards.level == level).ToList();
            }

            public partial class SectarianCardLibrary
            {
                [HideLabel, PreviewField(55, ObjectFieldAlignment.Right)]
                [HorizontalGroup("Split", 55, LabelWidth = 70)]
                public Texture2D icon;
                [VerticalGroup("Split/Meta")]
                [LabelText("当前卡牌数量")]
                public int cardNum;
                //public bool isSingleMode;
                [TabGroup("卡片制作")]
                [HideLabel, PreviewField(128, ObjectFieldAlignment.Right)]
                public Texture2D cardIcon;

                [TabGroup("卡片制作")]
                [LabelText("所属势力")]
                public Camp sectarian;

                [TabGroup("卡片制作")]
                [LabelText("卡片名称")]
                public string cardName;

                [TabGroup("卡片制作")]
                public int point;

                [TabGroup("卡片管理")]
                public List<CardModel> cardModelInfos;
                public List<RankLibrary> rankLibraries = new List<RankLibrary>();
                public List<CardRank> includeRank => cardModelInfos.Select(x => x.cardRank).Distinct().ToList();


                public SectarianCardLibrary(List<CardModel> CardsModels, Camp sectarian)
                {
                    this.sectarian = sectarian;
                    icon = SectarianIcons[sectarian];
                    cardModelInfos = CardsModels.Where(card => card.cardCamp == sectarian).ToList();
                }
                public partial class RankLibrary
                {
                    [HideLabel, PreviewField(55, ObjectFieldAlignment.Right)]
                    [HorizontalGroup("Split", 55, LabelWidth = 70)]
                    public Texture2D icon;
                    public CardRank rank;
                    [TabGroup("卡片管理")]
                    public List<CardModel> cardModelInfos;
                    public RankLibrary(List<CardModel> cardsModels, CardRank rank)
                    {
                        this.rank = rank;
                        icon = RankIcons[rank];
                        cardModelInfos = cardsModels.Where(cards => cards.cardRank == rank).ToList();
                    }
                }
            }
        }
    }
}