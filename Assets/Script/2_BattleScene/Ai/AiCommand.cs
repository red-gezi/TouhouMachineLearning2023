using System;
using System.Linq;
using System.Threading.Tasks;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Info;

namespace TouhouMachineLearningSummary.Command
{
    public class AiCommand
    {
        static Random rand = new Random("gezi".GetHashCode());
        public static void Init() => rand = new Random("gezi".GetHashCode());
        public static int GetRandom() => rand.Next();
        public static int GetRandom(int Min, int Max) => rand.Next(Min, Max);
        public static void RoundStartExchange(bool isControlPlayer)
        {
            //如果超时，自动设置当前操作者结束换牌
            if (isControlPlayer)
            {
                if (AgainstInfo.IsPlayer1)
                {
                    AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                }
                else
                {
                    AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                }
                AgainstInfo.IsSelectCardOver = true;
            }
            else//操纵对面Ai
            {
                if (AgainstInfo.IsPVE)
                {
                    if (AgainstInfo.IsPlayer1)
                    {
                        AgainstInfo.isPlayer2RoundStartExchangeOver = true;
                    }
                    else
                    {
                        AgainstInfo.isPlayer1RoundStartExchangeOver = true;
                    }
                }
            }
        }
        public static async Task TempPlayOperation()
        {
            if ((Info.AgainstInfo.isDownPass && Info.AgainstInfo.TotalDownPoint < Info.AgainstInfo.TotalUpPoint) ||
                Info.AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList.Count == 0)
            {
                //设置pass标记位
                AgainstInfo.IsPlayerPass = true;
            }
            else
            {
                Card targetCard = Info.AgainstInfo.GameCardsFilter[Orientation.My][GameRegion.Leader, GameRegion.Hand].ContainCardList.First();
                Info.AgainstInfo.playerPlayCard = targetCard;
            }
        }
    }
}