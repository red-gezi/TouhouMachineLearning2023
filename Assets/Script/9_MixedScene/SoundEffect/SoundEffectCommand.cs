using System;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class SoundEffectCommand
    {
        public static void Init()
        {
            //如果已经加载了则无需在加载
            if (!Info.SoundEffectInfo.SoundEfects.Any())
            {
                //for (int i = 0; i < Enum.GetValues(typeof(SoundEffectType)).Length; i++)
                //{
                //    Info.SoundEffectInfo.SoundEfects[(SoundEffectType)i] = AssetBundleCommand.Load<AudioClip>("SoundEffect", ((SoundEffectType)i).ToString());
                //}
                var soundEffectTypes = Enum.GetValues(typeof(SoundEffectType)).Cast<SoundEffectType>();
                Info.SoundEffectInfo.SoundEfects = soundEffectTypes
                    .ToDictionary(soundEffect => soundEffect,
                        soundEffect => AssetBundleCommand.Load<AudioClip>("SoundEffect", soundEffect.ToString()));
            }
        }
        public static async Task PlayAsync(SoundEffectType type)
        {
            var audioClip = Info.SoundEffectInfo.SoundEfects[type];
            AudioSource Source = Info.SoundEffectInfo.Instance.audioScoure.AddComponent<AudioSource>();
            Source.clip = audioClip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.volume = Config.GameConfig.Instance.MaxSoundEffectVolume;
            Source.Play();
            await Task.Delay((int)(audioClip.length * 1000));
            GameObject.DestroyImmediate(Source);
        }
    }
}