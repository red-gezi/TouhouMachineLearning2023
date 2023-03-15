using System;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class SoundEffectCommand
    {
        public static void Init()
        {
            Info.SoundEffectInfo.AudioScoure = GameObject.FindGameObjectWithTag("Audio");
            //如果已经加载了则无需在加载
            if (Info.SoundEffectInfo.SoundEfects.Count == 0)
            {
                for (int i = 0; i < Enum.GetValues(typeof(SoundEffectType)).Length; i++)
                {
                    Info.SoundEffectInfo.SoundEfects[(SoundEffectType)i] = AssetBundleCommand.Load<AudioClip>("SoundEffect", ((SoundEffectType)i).ToString());
                }
            }

        }
        public static async Task PlayAsync(SoundEffectType type)
        {
            var result = Info.SoundEffectInfo.SoundEfects;
            var audioClip = Info.SoundEffectInfo.SoundEfects[type];
            AudioSource Source = Info.SoundEffectInfo.AudioScoure.AddComponent<AudioSource>();
            Source.clip = audioClip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.Play();
            await Task.Delay((int)(audioClip.length * 1000));
            GameObject.DestroyImmediate(Source);
        }
    }
}