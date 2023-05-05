#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System.Linq;
using TouhouMachineLearningSummary.Command;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Model;
using UnityEditor;
using static TouhouMachineLearningSummary.Info.InspectorInfo.LevelLibrary;
using static TouhouMachineLearningSummary.Info.InspectorInfo.LevelLibrary.SectarianCardLibrary;

namespace TouhouMachineLearningSummary.Editor
{
    public class CardMenu : OdinMenuEditorWindow
    {

        private static CardMenu Instance { get; set; }
        [MenuItem("TML_Tools/卡组编辑器")]
        private static void OpenWindow()
        {
            CardMenu window = GetWindow<CardMenu>();
            InspectorCommand.LoadFromJson();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        }
        public static void UpdateInspector() => Instance?.ForceMenuTreeRebuild();
        //构造界面树系统
        protected override OdinMenuTree BuildMenuTree()
        {
            UnityEngine.Debug.Log("构造树形结构");
            InspectorInfo cardLibraryInfo = InspectorInfo.Instance;
            var tree = new OdinMenuTree(true);
            tree.DefaultMenuStyle.Height = 60;
            tree.DefaultMenuStyle.IconSize = 48.00f;
            tree.Config.DrawSearchToolbar = true;

            tree.Add("单人模式牌库", cardLibraryInfo);
            foreach (var levelLibrary in cardLibraryInfo.levelLibries.Where(library => library.isSingleMode))
            {
                tree.Add($"单人模式牌库/{levelLibrary.level}", levelLibrary);
                foreach (var sectarianCardLibrary in levelLibrary.sectarianCardLibraries)
                {
                    tree.Add($"单人模式牌库/{levelLibrary.level}/{sectarianCardLibrary.sectarian}", sectarianCardLibrary);
                    foreach (var rankLibrary in sectarianCardLibrary.rankLibraries)
                    {
                        tree.Add($"单人模式牌库/{levelLibrary.level}/{sectarianCardLibrary.sectarian}/{rankLibrary.rank}", rankLibrary);
                        foreach (var cardModel in rankLibrary.cardModelInfos)
                        {
                            tree.Add($"单人模式牌库/{levelLibrary.level}/{sectarianCardLibrary.sectarian}/{rankLibrary.rank}/{cardModel.TranslateName}", cardModel);
                        }
                    }
                }
            }

            tree.Add("多人模式牌库", cardLibraryInfo);
            foreach (var levelLibrary in cardLibraryInfo.levelLibries.Where(library => !library.isSingleMode))
            {
                foreach (var sectarianCardLibrary in levelLibrary.sectarianCardLibraries)
                {
                    tree.Add($"多人模式牌库/{sectarianCardLibrary.sectarian}", sectarianCardLibrary);
                    foreach (var rankLibrary in sectarianCardLibrary.rankLibraries)
                    {
                        tree.Add($"多人模式牌库/{sectarianCardLibrary.sectarian}/{rankLibrary.rank}", rankLibrary);
                        foreach (var cardModel in rankLibrary.cardModelInfos)
                        {
                            tree.Add($"多人模式牌库/{sectarianCardLibrary.sectarian}/{rankLibrary.rank}/{cardModel.TranslateName}", cardModel);
                        }
                    }
                }
            }
            //tree.EnumerateTree().AddIcons<CardLibraryInfo>(x => x.singleIcon);
            tree.EnumerateTree().AddIcons<SectarianCardLibrary>(x => x.icon);
            tree.EnumerateTree().AddIcons<RankLibrary>(x => x.icon);
            tree.EnumerateTree().AddIcons<CardModel>(x => x.cardFace);
            Instance = this;
            return tree;
        }
    }
}
#endif