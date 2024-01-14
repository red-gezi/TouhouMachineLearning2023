using System;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public class MusicCommand
    {
        public static void Init()
        {
            //如果已经加载了则无需在加载
            if (Info.SoundEffectInfo.SoundEfects.Count == 0)
            {
                for (int i = 0; i < Enum.GetValues(typeof(MusicType)).Length; i++)
                {
                    Info.MusicInfo.Musics[(MusicType)i] = AssetBundleCommand.Load<AudioClip>("Music", ((MusicType)i).ToString());
                }
            }

        }
        public static void Play(MusicType type)
        {
            var audioClip = Info.MusicInfo.Musics[type];
            AudioSource Source = Info.MusicInfo.Instance.audioScoure;
            Source.clip = audioClip;
            Source.spatialBlend = 1;
            Source.pitch = 1.3f;
            Source.Play();
        }
    }
}