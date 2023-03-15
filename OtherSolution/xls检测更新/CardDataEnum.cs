using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xls检测更新
{
    class CardDataEnum
    {
        public enum Territory { My, Op, All }
        public enum Camp
        {
            Neutral,
            Taoism,
            Shintoism,
            Buddhism,
            science
        }
        public enum GameRegion
        {
            Water,
            Fire,
            Wind,
            Soil,
           
            Leader,
            Hand,
            Uesd,
            Deck,
            Grave,
            Battle=99,
            None=100,
        }
        public enum BattleRegion
        {
            Water, Fire, Wind, Soil, All = 99, None = 100
        }
        public enum CardState
        {
            Spy,
            Seal
        }
        public enum CardType
        {
            Unite = 0,
            Special = 1,
        }
        public enum CardFeature
        {
            Largest,
            Lowest
        }
        public enum CardRank
        {
            Leader = 1,
            Gold = 2,
            Silver = 3,
            Copper = 4,
        }
    }
}
