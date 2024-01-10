using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Extension;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Manager;
using TouhouMachineLearningSummary.Model;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
using UnityEngine.UI;

namespace TouhouMachineLearningSummary.Command
{

    /// <summary>
    /// 剧情演出指令
    /// </summary>
    public class DialogueCommand
    {
        ///////////////////////////////////////////////////////////////////////立绘生成与参数初始化///////////////////////////////////////////////////////////////////////
        public static void Init()
        {
            for (int i = 0; i < DialogueInfo.instance.models.transform.childCount; i++)
            {
                var model = DialogueInfo.instance.models.transform.GetChild(i).gameObject;
                var leftModel = GameObject.Instantiate(model, DialogueInfo.instance.left.transform);
                leftModel.GetComponent<Live2dManager>().defaultSightPoint = new Vector3(30, 0, 0);
                var rightModel= GameObject.Instantiate(model, DialogueInfo.instance.right.transform);
                rightModel.GetComponent<Live2dManager>().defaultSightPoint = new Vector3(-30, 0, 0);
                
                //临时显形
                rightModel.SetActive(true);
                leftModel.SetActive(true);
            }
        }
        ///////////////////////////////////////////////////////////////////////剧本加载///////////////////////////////////////////////////////////////////////
        //加载剧情文件
        public static void Load()
        {
            string storyData = Application.isEditor ? File.ReadAllText(@"Assets\GameResources\GameData\Story.json") : AssetBundleCommand.Load<TextAsset>("GameData", "Story").text;
            Info.DialogueInfo.DialogueModels = storyData.ToObject<List<DialogueModel>>();
        }
        //重新加载新的剧情文件，刷新当前的对话集合信息，并检查对话指针未知合法性
        public static void ReLoad()
        {
            Load();
            DialogueInfo.currnetDialogueModel = Info.DialogueInfo.DialogueModels.FirstOrDefault(model => model.Tag == $"{DialogueInfo.StageTag}-{DialogueInfo.StageRank}");
            if (DialogueInfo.currnetDialogueModel != null)
            {
                int newDialogueCount = Info.DialogueInfo.currnetDialogueModel.Operations.Count;
                DialogueInfo.CurrentPoint = Mathf.Min(DialogueInfo.CurrentPoint, newDialogueCount);
            }
            else
            {
                Debug.LogError("剧情重载失败");
            }
        }
        ///////////////////////////////////////////////////////////////////////剧本播放///////////////////////////////////////////////////////////////////////
        public static async Task Play(string stageTag, int stageRank)
        {
            DialogueInfo.IsSelectOver = true;
            DialogueInfo.IsSkip = false;
            DialogueInfo.IsFastForward = false;
            DialogueInfo.IsShowNextText = false;
            DialogueInfo.CurrentPoint = 0;
            DialogueInfo.StageTag = stageTag;
            DialogueInfo.StageRank = stageRank;
            DialogueInfo.isLeftCharaActive = false;
            DialogueInfo.isRightCharaActive = false;
            DialogueInfo.DialogueSummary = new();


            Transform left = Info.DialogueInfo.instance.left.transform;
            for (int i = 0; i < left.childCount; i++)
            {
                left.GetChild(i).gameObject.SetActive(false);
            }
            Transform right = Info.DialogueInfo.instance.right.transform;
            for (int i = 0; i < right.childCount; i++)
            {
                right.GetChild(i).gameObject.SetActive(false);
            }

            DialogueInfo.instance.dialogueCanvas.SetActive(true);
            Debug.LogWarning("对话组件开启");
            //加载剧情文本
            DialogueInfo.currnetDialogueModel = DialogueInfo.DialogueModels.FirstOrDefault(model => model.Tag == $"{stageTag}-{stageRank}");
            if (DialogueInfo.currnetDialogueModel != null)
            {
                await AnalyzeDialogue();
            }
            else
            {
                Debug.LogError("剧情加载失败");
                DialogueInfo.instance.dialogueCanvas.SetActive(false);
            }

            static async Task AnalyzeDialogue()
            {
                Debug.LogWarning($"当前对话节点{Info.DialogueInfo.CurrentPoint}，总对话节点{ Info.DialogueInfo.currnetDialogueModel.Operations.Count}");
                //如果没执行完则运行下一个指令，否则直接结束
                if (Info.DialogueInfo.CurrentPoint < Info.DialogueInfo.currnetDialogueModel.Operations.Count)
                {
                    var currentOperations = Info.DialogueInfo.currnetDialogueModel.Operations[Info.DialogueInfo.CurrentPoint];
                    await RunOperationsAsync(currentOperations);
                    Info.DialogueInfo.CurrentPoint++;
                    await AnalyzeDialogue();
                }
                else//读取完毕
                {
                    Info.DialogueInfo.instance.dialogueCanvas.SetActive(false);
                    Debug.LogWarning("对话组件关闭");
                };
            }
        }
        /// <summary>
        /// 传入播放剧情参数，若当前剧情与玩家节点相等则解锁下个阶段剧情
        /// </summary>
        /// <param name="stageTag"></param>
        /// <param name="stageRank"></param>
        /// <returns></returns>
        public static async Task UnlockAsync(string stageTag, int stageRank)
        {
            if (AgainstInfo.OnlineUserInfo.GetStage(stageTag) == stageRank)
            {
                Debug.LogWarning("玩家进度更新至" + stageTag + "-" + stageRank + 1);
                await AgainstInfo.OnlineUserInfo.UpdateUserStateAsync(stageTag, stageRank + 1);
            }
        }
        ///////////////////////////////////////////////////////////////////////界面按钮///////////////////////////////////////////////////////////////////////
        public static void FastForward() => Info.DialogueInfo.IsFastForward = true;
        public static void Skip() => Info.DialogueInfo.IsSkip = !Info.DialogueInfo.IsSkip;
        public static void Log() { }
        public static void SetBranch(int index)
        {
            Info.DialogueInfo.IsSelectOver = true;
            Info.DialogueInfo.SelectBranch = index;
            Info.DialogueInfo.instance.selectUi.SetActive(false);
        }
        public static async Task LastDialogue()
        {
            //重载
            ReLoad();
            //向上解析指令
            DialogueInfo.instance.selectUi.SetActive(false);
            DialogueInfo.CurrentPoint = Mathf.Max(-1, DialogueInfo.CurrentPoint - 2);
            DialogueInfo.IsSelectOver = true;
            DialogueInfo.IsShowNextText = true;
        }
        public static async Task NextDialogue()
        {
            //处于需要选择选项时点击对话框也不会跳转到下一句话
            if (!Info.DialogueInfo.IsSelectOver) return;
            DialogueInfo.IsSkip = false;
            DialogueInfo.IsFastForward = false;
            DialogueInfo.IsShowNextText = true;
        }
        ///////////////////////////////////////////////////////////////////////剧本台词与指令的记录与解析///////////////////////////////////////////////////////////////////////
        //记录对话过程中的指令操作
        public static async Task SummaryOperationsAsync(DialogueModel.Operation currentOperations)
        {
            //DialogueInfo.
        }
        public static async Task RunOperationsAsync(DialogueModel.Operation currentOperations)
        {
            //分支结束，返回主路线
            if (currentOperations.Branch == "")
            {
                DialogueInfo.SelectBranch = 0;
            }
            //如果不是所选分支则直接跳过
            if (DialogueInfo.SelectBranch != 0 && currentOperations.Branch != DialogueInfo.SelectBranch.ToString())
            {
                return;
            }
            if (currentOperations.Chara == "指令")//是指令的情况
            {
                //解析和执行指令
                string command = currentOperations.Text["Ch"];
                if (command.StartsWith("select"))
                {
                    //到选择选项时停止自动跳转
                    DialogueInfo.IsSkip = false;
                    DialogueInfo.IsSelectOver = false;
                    Info.DialogueInfo.instance.selectUi.SetActive(true);
                    var options = command.Replace("select:", "").Split('@').ToList();
                    for (int i = 0; i < 3; i++)
                    {
                        if (i < options.Count)
                        {
                            Info.DialogueInfo.instance.selectUi.transform.GetChild(i).gameObject.SetActive(true);
                            Info.DialogueInfo.instance.selectUi.transform.GetChild(i).GetChild(0).GetComponent<Text>().text = options[i];
                        }
                        else
                        {
                            Info.DialogueInfo.instance.selectUi.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                    //暂停界面，直到玩家选择完毕
                    while (!DialogueInfo.IsSelectOver)
                    {
                        await Task.Delay(100);
                    }
                }
                else
                {
                    if (command.StartsWith("rename"))
                    {
                        //到命名选项时停止自动跳转
                        DialogueInfo.IsSkip = false;
                        await NoticeCommand.ShowAsync("请输入你的名字", NotifyBoardMode.Input, inputAction: async (name) =>
                        {
                            await Info.AgainstInfo.OnlineUserInfo.UpdateName(name);
                        }, inputField: "村中人");
                    }
                    if (command.StartsWith("music"))
                    {
                        var music = command.Replace("music:", "");
                        Debug.LogWarning("播放音乐" + music);
                    }
                    if (command.StartsWith("backImage"))
                    {
                        var music = command.Replace("backImage:", "");
                        Debug.LogWarning("切换背景图片" + music);
                    }
                }
            }
            else//是对话的情况
            {
                if (currentOperations.Position == "左侧")
                {
                    //关闭所有左侧live2d
                    Transform left = Info.DialogueInfo.instance.left.transform;
                    for (int i = 0; i < left.childCount; i++)
                    {
                        left.GetChild(i).gameObject.SetActive(false);
                    }
                    //先判断上个激活的立绘是否存在且是右侧，如果是则变灰移回原位或者关闭
                    if (Info.DialogueInfo.targetLive2dChara != null && Info.DialogueInfo.isRightCharaActive)
                    {
                        Debug.Log("关闭右侧立绘");
                        Transform target = Info.DialogueInfo.targetLive2dChara;
                        _ = CustomThread.TimerAsync(0.2f, process =>
                        {
                            target.localPosition = new Vector3((1 - process) * -100, 0, 0);
                            target.GetComponent<Live2dManager>().ToActive(1 - process);
                        });
                        Info.DialogueInfo.isRightCharaActive = false;
                    }
                    //获取左侧立绘，没激活则激活
                    Info.DialogueInfo.targetLive2dChara = Info.DialogueInfo.instance.left.transform.Find(currentOperations.Chara);
                    Info.DialogueInfo.targetLive2dChara.gameObject.SetActive(true);
                    if (!Info.DialogueInfo.isLeftCharaActive)
                    {
                        Debug.Log("打开左侧立绘");
                        Transform target = Info.DialogueInfo.targetLive2dChara;
                        _ = CustomThread.TimerAsync(0.5f, process =>
                        {
                            target.localPosition = new Vector3(process * 100, 0, 0);
                            target.GetComponent<Live2dManager>().ToActive(process);
                        });
                        Info.DialogueInfo.isLeftCharaActive = true;
                    }
                }
                else if (currentOperations.Position == "右侧")
                {
                    Transform right = Info.DialogueInfo.instance.right.transform;
                    for (int i = 0; i < right.childCount; i++)
                    {
                        right.GetChild(i).gameObject.SetActive(false);
                    }
                    //先判断上个激活的立绘是否存在且是左侧，如果是则变灰移回原位
                    if (Info.DialogueInfo.targetLive2dChara != null && Info.DialogueInfo.isLeftCharaActive)
                    {
                        Debug.Log("关闭左侧立绘");
                        Transform target = Info.DialogueInfo.targetLive2dChara;
                        _ = CustomThread.TimerAsync(0.2f, process =>
                        {
                            target.localPosition = new Vector3((1 - process) * 100, 0, 0);
                            target.GetComponent<Live2dManager>().ToActive(1 - process);
                        });
                        Info.DialogueInfo.isLeftCharaActive = false;
                    }
                    //获取右侧立绘，没激活则激活
                    Info.DialogueInfo.targetLive2dChara = Info.DialogueInfo.instance.right.transform.Find(currentOperations.Chara);
                    Info.DialogueInfo.targetLive2dChara.gameObject.SetActive(true);
                    if (!Info.DialogueInfo.isRightCharaActive)
                    {
                        Debug.Log("打开右侧立绘");
                        Transform target = Info.DialogueInfo.targetLive2dChara;
                        _ = CustomThread.TimerAsync(0.5f, process =>
                        {
                            target.localPosition = new Vector3(process * -100, 0, 0);
                            target.GetComponent<Live2dManager>().ToActive(process);
                        });
                        Info.DialogueInfo.isRightCharaActive = true;
                    }
                }
                if (currentOperations.Face != "")
                {
                    Info.DialogueInfo.targetLive2dChara.GetComponent<Live2dManager>().Play(currentOperations.Face);
                }
                Info.DialogueInfo.instance.charaName.text = currentOperations.Chara;
                Info.DialogueInfo.instance.charaText.text = currentOperations.Text["Ch"];
                //Info.DialogueInfo.CurrentPoint++;
                DialogueInfo.IsShowNextText = false;

                //卡住对话，直到点击了对话框或者选择了跳过
                while (!(DialogueInfo.IsShowNextText || DialogueInfo.IsSkip))
                {
                    Command.TaskThrowCommand.Throw();
                    await Task.Delay(50);
                    //如果选择了加速则延迟0.5秒后自动跳出等待循环并播放下一句话
                    if (DialogueInfo.IsFastForward)
                    {
                        await Task.Delay(500);
                        break;
                    }
                }
            }
        }
    }
}