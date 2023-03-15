using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Video;

namespace TouhouMachineLearningSummary.Test
{
    internal class PremiumCardCommand : MonoBehaviour
    {
        public List<Transform> cards = new List<Transform>();
        [Button]
        public void LoadPremiumTexture()
        {
            var PremiumTexturePath = Application.streamingAssetsPath;
            var PremiumTexturesFiles = new DirectoryInfo(PremiumTexturePath).GetFiles("*.mp4", SearchOption.AllDirectories);
            
            cards.ForEach(async card =>
            {
                card.GetComponent<VideoPlayer>().url = PremiumTexturesFiles.OrderBy(x => UnityEngine.Random.value).FirstOrDefault().FullName;
                card.GetComponent<VideoPlayer>().targetTexture = new RenderTexture(105,150,24);
                card.GetComponent<Renderer>().material.SetTexture("_Front", card.GetComponent<VideoPlayer>().targetTexture);
                card.GetComponent<VideoPlayer>().enabled = true;
                await System.Threading.Tasks.Task.Delay(100);
            });

        }
    }
}
//PremiumTexturesFiles.ForEach(File =>
//{
//    Process.Start("E:\\UnityProject\\TouhouMachineLearning2022\\Assets\\StreamingAssets\\PremiumCard\\ffmpeg.exe",$"ffmpeg -i {File.Name} out-{File.Name}");
//});