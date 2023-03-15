using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;

namespace TouhouMachineLearningSummary.Command
{
    internal class PlayerDataCommand : MonoBehaviour
    {
        ///////////////////////////////玩家信息面板////////////////////////////////////
        public static void ShowPlayerDataCanve(string uid)
        {
            Info.PageComponentInfo.Instance.playerDataCanve.SetActive(true);
            InitChart(uid);
            _ = Command.SoundEffectCommand.PlayAsync(GameEnum.SoundEffectType.UiButton);
        }
        public static void ClosePlayerDataCanve()
        {
            Info.PageComponentInfo.Instance.playerDataCanve.SetActive(false);
            _ = Command.SoundEffectCommand.PlayAsync(GameEnum.SoundEffectType.UiButton);
        }
        ///////////////////////////////编辑面板////////////////////////////////////
        public static void ShowCanve(GameObject targetCanve)
        {
            Info.PageComponentInfo.Instance.editCanve.SetActive(false);
            //如果要初始化前缀选择界面
            if (targetCanve == Info.PageComponentInfo.Instance.editPrefixTitleCanve)
            {
                var unlockTitles = Command.TitleCommand.GetUnlockTitles(true);
                var model = Info.PageComponentInfo.Instance.prefixTitleModel;
                var content = model.transform.parent;
                //根据称号数目更新高度

                //根据称号数目创建等数量选项
                for (int i = content.childCount; i < unlockTitles.Count + 1; i++)
                {
                    var newModel = Instantiate(model, model.transform.parent);
                }
                for (int i = 1; i < content.childCount; i++)
                {
                    var newModel = content.GetChild(i);
                    newModel.gameObject.SetActive(true);
                    var titleModel = unlockTitles[i - 1];
                    newModel.name = titleModel.tag;
                    newModel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = titleModel.titles[Manager.TranslateManager.currentLanguage];
                }
            }
            //如果要初始化后缀选择界面
            if (targetCanve == Info.PageComponentInfo.Instance.editSuffixTitleCanve)
            {
                var unlockTitles = Command.TitleCommand.GetUnlockTitles(false);
                var model = Info.PageComponentInfo.Instance.suffixTitleModel;
                var content = model.transform.parent;
                //根据称号数目更新高度

                //根据称号数目创建等数量选项
                for (int i = content.childCount; i < unlockTitles.Count + 1; i++)
                {
                    var newModel = Instantiate(model, model.transform.parent);
                }
                for (int i = 1; i < content.childCount; i++)
                {
                    var newModel = content.GetChild(i);
                    newModel.gameObject.SetActive(true);
                    var titleModel = unlockTitles[i - 1];
                    newModel.name = titleModel.tag;
                    newModel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = titleModel.titles[Manager.TranslateManager.currentLanguage];
                }
            }
            targetCanve.SetActive(true);
            //
            _ = Command.SoundEffectCommand.PlayAsync(GameEnum.SoundEffectType.UiButton);
        }
        public static void CloseCanve()
        {
            Info.PageComponentInfo.Instance.editCanve.SetActive(false);
            Info.PageComponentInfo.Instance.editNameCanve.SetActive(false);
            Info.PageComponentInfo.Instance.editSignatureCanve.SetActive(false);
            Info.PageComponentInfo.Instance.editPrefixTitleCanve.SetActive(false);
            Info.PageComponentInfo.Instance.editSuffixTitleCanve.SetActive(false);
            _ = Command.SoundEffectCommand.PlayAsync(GameEnum.SoundEffectType.UiButton);
        }
        public static void SaveEdit(GameObject targetCanve)
        {
            if (targetCanve == Info.PageComponentInfo.Instance.editNameCanve)
            {
                Debug.Log("保存");
            }
            CloseCanve();
        }
        public static async void SaveTitle(GameObject item)
        {
            string tag = item.name;
            var updateType = tag.StartsWith("1") ? UpdateType.PrefixTitle : UpdateType.SuffixTitle;
            bool result = await Command.NetCommand.UpdateInfoAsync(updateType, tag);
            Debug.Log("称号更新" + result);
            if (updateType== UpdateType.PrefixTitle)
            {
                Info.AgainstInfo.OnlineUserInfo.UsePrefixTitleTag = tag;
            }
            if (updateType == UpdateType.SuffixTitle)
            {
                Info.AgainstInfo.OnlineUserInfo.UseSuffixTitleTag = tag;
            }
            Info.PageComponentInfo.Instance.title.GetComponent<TextMeshProUGUI>().text= $"———{Command.TitleCommand.GetCurrentTitle()}";
            CloseCanve();
        }
        ///////////////////////////////雷达图////////////////////////////////////
        public static void InitChart(string uid)
        {
            float length = 130;
            MeshFilter filter = Info.PageComponentInfo.Instance.chart.GetComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            List<int> value = new() { 1, 2, 3, 4, 5, 0 };
            _ = CustomThread.TimerAsync(2, process =>
            {
                var vertices = new List<Vector3> { new Vector3(0, 0, 0) };
                vertices.AddRange(Enumerable.Range(0, 6).Select(i =>
                {
                    float angel = Mathf.PI * ((i * 60 + 30) * 1f / 180);
                    return new Vector3(Mathf.Cos(angel), Mathf.Sin(angel)) * length;
                }));
                vertices.Add(new Vector3(0, 0, 0));
                vertices.AddRange(Enumerable.Range(0, 6).Select(i =>
                {
                    float angel = Mathf.PI * ((i * 60 + 30) * 1f / 180);
                    return new Vector3(Mathf.Cos(angel), Mathf.Sin(angel)) * length * Mathf.Min(value[i] / 5f, process * 5);
                }));
                mesh.vertices = vertices.ToArray();
                Color colorA = new Color(0.05f, 0.25f, 0.35f, 1);
                Color colorB = new Color(0.05f, 0.25f, 0.35f, 1);
                Color colorC = new Color(0, 0.4f, 0.6f, 0.5f);
                mesh.SetColors(new List<Color>()
               {
                   colorA,
                   colorB,colorB,colorB,colorB,colorB,colorB,
                   colorC,
                   colorC,colorC,colorC,colorC,colorC,colorC
               });
                mesh.SetTriangles(new int[]
                {
                    //0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5, 0, 5, 6, 0, 6, 1,
                    7, 8, 9, 7, 9, 10, 7, 10, 11, 7, 11, 12, 7, 12, 13, 7, 13, 8
                }, 0);
                mesh.RecalculateNormals();
                mesh.RecalculateBounds();
                filter.mesh = mesh;
                LineRenderer lineRenderer = Info.PageComponentInfo.Instance.chart.GetComponent<LineRenderer>();
                lineRenderer.positionCount = 7;
                var linePos = vertices.Skip(8).ToList();
                linePos.Add(linePos[0]);
                lineRenderer.SetPositions(linePos.ToArray());
            });
        }
    }
}
