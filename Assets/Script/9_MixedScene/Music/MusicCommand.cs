using System;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.Config;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;
using TouhouMachineLearningSummary.Thread;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class MusicCommand
    {
        public static void Init()
        {
            //如果已经加载了,有数据了则无需再次加载
            if (!Info.SoundEffectInfo.SoundEfects.Any())
            {
                foreach (MusicType musicType in Enum.GetValues(typeof(MusicType)))
                {
                    Info.MusicInfo.Musics[musicType] = AssetBundleCommand.Load<AudioClip>("Music", musicType.ToString());
                }
            }
        }
        public static async void Play(MusicType type)
        {
            var audioClip = Info.MusicInfo.Musics[type];
            AudioSource Source = Info.MusicInfo.Instance.audioScoure;
            if (Source.clip != null)
            {
                await CustomThread.TimerAsync(0.5f, progress =>
                {
                    Source.volume = GameConfig.Instance.MaxMusicVolume * (1 - progress);
                });
            }
            Source.clip = audioClip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.Play();
            await CustomThread.TimerAsync(0.5f, progress =>
            {
                Source.volume = GameConfig.Instance.MaxMusicVolume * progress;
            });
        }
    }
}