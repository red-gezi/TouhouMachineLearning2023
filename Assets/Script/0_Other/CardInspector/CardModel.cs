using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using UnityEngine;

namespace TouhouMachineLearningSummary.Model
{
    [Serializable]
    public class CardModel
    {
        [HorizontalGroup("Split", 55, LabelWidth = 70)]
        [HideLabel, PreviewField(55, ObjectFieldAlignment.Right)]
        //[LabelText("卡片贴图")]
        public Texture2D cardFace;
        public Texture2D cardBack;
        [VerticalGroup("Split/Meta")]
        [LabelText("ID")]
        public string cardID;

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("所属关卡")]
        public string level;
        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("所属系列")]
        public string series;
        public Dictionary<string, string> Name { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> CardTags { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Describe { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Ability { get; set; } = new Dictionary<string, string>();

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("名字")]
        public string TranslateName => Name["Name-" + (Name.ContainsKey("Name-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("标签")]
        public string TranslateTags
        {
            get
            {
                try
                {
                    string language = (CardTags.ContainsKey(TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch");
                    if (CardTags.ContainsKey(language))
                    {
                        return CardTags[language];
                    }
                    else
                    {
                        Debug.LogError("标签无法检索到，请在表格中补充" + CardTags.ToJson());
                        return "";
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("介绍")]
        public string TranslateDescribe => Describe["Describe-" + (Describe.ContainsKey("Describe-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];

        [ShowInInspector, VerticalGroup("Split/Meta"), LabelText("效果")]
        public string TranslateAbility => Ability["Ability-" + (Ability.ContainsKey("Ability-" + TranslateManager.currentLanguage) ? TranslateManager.currentLanguage : "Ch")];

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("点数")]
        public int point;

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("卡片类型"), EnumToggleButtons]
        public CardType cardType;

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("卡片等级"), EnumToggleButtons]
        public CardRank cardRank;

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("所属势力"), EnumToggleButtons]
        public Camp cardCamp;

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("部署区域"), EnumToggleButtons]
        public BattleRegion cardDeployRegion = BattleRegion.All;

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("部署所属"), EnumToggleButtons]
        public Territory cardDeployTerritory = Territory.My;

        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("关联卡牌"), EnumToggleButtons]
        public List<string> refCardIDs = new();
        [DisableInEditorMode]
        [VerticalGroup("Split/Meta")]
        [LabelText("正体/衍生"), EnumToggleButtons]
        public int ramification;


        public Sprite GetCardSprite() => cardFace.ToSprite();
        public CardModel() { }
        /// <summary>
        /// 根据卡牌参数补充对应的卡牌id，卡牌插画等
        /// 正常游戏从AB包加载，卡组编辑器从本地加载
        /// </summary>
        /// <param name="isSingle"></param>
        public CardModel Init(bool isSingle)
        {

            cardID = $"{(isSingle ? "S_" + level : "M_" + series)}_{(int)cardRank}{cardRank.ToString()[0]}_{cardID.PadLeft(3, '0')}";
            //编辑器下由从编辑器中加载图片
            //发布后从AB包加载图片
            if (Application.isEditor)
            {
                var target = InspectorInfo.CardTexture.FirstOrDefault(file => file.Name == cardID + ".png");
                if (target == null)
                {
                    target = InspectorInfo.CardTexture.FirstOrDefault(file => file.Name == "default.png");
                }
                cardFace = target.ToTexture2D();
            }
            else
            {
                cardFace = AssetBundleCommand.Load<Texture2D>("CardTex", cardID) ?? AssetBundleCommand.Load<Texture2D>("CardTex", "default");
            }
            return this;
        }
        public Texture2D GetCardBack(string cardBackID)
        {
            if (Application.isEditor)
            {
                var target = InspectorInfo.CardTexture.FirstOrDefault(file => file.Name == cardBackID + ".png");
                if (target == null)
                {
                    target = InspectorInfo.CardTexture.FirstOrDefault(file => file.Name == "default.png");
                }
                cardBack = target.ToTexture2D();
            }
            else
            {
                cardBack = AssetBundleCommand.Load<Texture2D>("CardTex", cardBackID + ".png") ?? AssetBundleCommand.Load<Texture2D>("CardTex", "default");
            }
            return cardBack;
        }
        [Button("打开脚本")]
        public void OpenCardScript()
        {
            string targetPath = Application.dataPath + $@"\Script\9_MixedScene\CardSpace\Card{cardID}.cs";
            InspectorCommand.CreatScript(cardID);
            System.Diagnostics.Process.Start(targetPath);
        }
    }
}
