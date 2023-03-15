using System.Threading.Tasks;
using TouhouMachineLearningSummary.Control;
using TouhouMachineLearningSummary.Info;
using UnityEngine;
namespace TouhouMachineLearningSummary.Command
{
    public static class EffectCommand
    {
       
        public static void ParticlePlay(int Rank, Card card)
        {
            ParticleSystem TargetParticle = GameObject.Instantiate(Info.ParticleInfo.Instance.ParticleEffect[Rank]);
            TargetParticle.transform.position = card.transform.position;
            TargetParticle.Play();
            GameObject.Destroy(TargetParticle.gameObject, 2);
        }
        public static void Bullet_Gain(Event e)
        {
            GameObject Bullet = GameObject.Instantiate(Info.ParticleInfo.Instance.GainBullet);
            BulletControl bulletControl = Bullet.GetComponent<BulletControl>();
            bulletControl.Init(e.TriggerCard, e.TargetCard);
            bulletControl.Play();
            Bullet.GetComponent<ParticleSystem>().Play();
        }
        internal static void Bullet_Hurt(Event e)
        {
            GameObject Bullet = GameObject.Instantiate(Info.ParticleInfo.Instance.HurtBullet);
            BulletControl bulletControl = Bullet.GetComponent<BulletControl>();
            bulletControl.Init(e.TriggerCard, e.TargetCard);
            bulletControl.Play();
            Bullet.GetComponent<ParticleSystem>().Play();
        }
        public static async Task TheWorldPlayAsync(Card card)
        {
            SceneEffectInfo.theWorldEffect.gameObject.SetActive(true);
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(card.transform.position);
            SceneEffectInfo.theWorldEffect.transform.position = Camera.main.ScreenPointToRay(screenPoint).GetPoint(5);
            TheWorldEffect.state = 0;
            await Task.Delay(1000);
            TheWorldEffect.state = 1;
            await Task.Delay(2000);
            TheWorldEffect.state = 0;
            await Task.Delay(1000);
            SceneEffectInfo.theWorldEffect.gameObject.SetActive(false);
        }


    }
}

