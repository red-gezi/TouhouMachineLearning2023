using System;
using System.Collections.Generic;
using TouhouMachineLearningSummary.GameEnum;
using TouhouMachineLearningSummary.Model;

namespace TouhouMachineLearningSummary.Config
{
    /// <summary>
    /// 剧情模式的卡牌配置信息，管理每个关卡的卡组
    /// </summary>
    public class DeckConfig
    {


        public static PlayerInfo GetPlayerCardDeck(string Stage)
        {
            return Stage switch
            {
                "default" => new PlayerInfo(
                        "NPC", "神秘的妖怪", "1-0", "2-0", "",
                        new List<Deck>
                        {
                            new Deck("gezi", "M_N0_0L_001","N0_0", new List<string>
                            {
                                "M_N0_1G_001","M_N0_1G_002","M_N0_1G_003","M_N0_1G_004",
                                "M_N0_2S_001","M_N0_2S_002","M_N0_2S_003","M_N0_2S_004","M_N0_2S_005","M_N0_2S_006",
                                "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                                "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                                "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                            })
                        }),
                "test" => new PlayerInfo(
                        "NPC", "神秘的妖怪", "1-0", "2-0", "",
                        new List<Deck>
                        {
                            new Deck("gezi", "M_T1_0L_001","T1_0", new List<string>
                            {
                                "M_T1_1G_001","M_T1_1G_002","M_T1_1G_003","M_T1_1G_004",
                                "M_T1_2S_001","M_T1_2S_002","M_T1_2S_003","M_T1_2S_004","M_T1_2S_005","M_T1_2S_005",
                                "M_T1_3C_001","M_T1_3C_002","M_T1_3C_003","M_T1_3C_004","M_T1_3C_005",
                                "M_T1_3C_001","M_T1_3C_002","M_T1_3C_003","M_T1_3C_004","M_T1_3C_005",
                                "M_T1_3C_001","M_T1_3C_002","M_T1_3C_003","M_T1_3C_004","M_T1_3C_005",
                            })
                        }),
                "1-1" => new PlayerInfo(
                        "NPC", "神秘的妖怪", "1-0", "2-0", "",
                        new List<Deck>
                        {
                            new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                        }),
                "1-2" => new PlayerInfo(
                        "NPC", "神秘的妖怪", "1-0", "2-0", "",
                        new List<Deck>
                        {
                         new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                        }),
                _ => Info.AgainstInfo.OnlineUserInfo.GetSampleInfo(),
            };
        }
        public static PlayerInfo GetOpponentCardDeck(string Stage)
        {

            return Stage switch
            {
                "test" => new PlayerInfo(
                     "NPC", "gezi", "1-0", "2-0", "",
                    new List<Deck>
                    {
                       new Deck("gezi", "M_F1_0L_001","T0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                    }),
                "1-1" => new PlayerInfo(
                     "NPC", "神秘的妖怪", "1-0", "2-0", "",
                     new List<Deck>
                     {
                        new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                     }),
                "1-2" => new PlayerInfo(
                    "NPC", "神秘的妖怪", "1-0", "2-0", "",
                    new List<Deck>
                    {
                       new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                    }),
                _ => Info.AgainstInfo.OnlineUserInfo.GetSampleInfo(),
            };
        }
        public static PlayerInfo GetPracticeCardDeck(PracticeLeader practiceLeader)
        {

            return practiceLeader switch
            {
                PracticeLeader.Reimu_Hakurei => new PlayerInfo(
                     "NPC", "神秘的妖怪", "1-0", "2-0", "",
                     new List<Deck>
                     {
                        new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                     }),
                //奇迹的代言人
                PracticeLeader.Sanae_Kotiya => new PlayerInfo(
                          "NPC", "东风谷早苗", "1-0", "2-0", "",
                          new List<Deck>
                          {
                           new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                          }),
                PracticeLeader.Mononobe_no_Futo => throw new NotImplementedException(),
                PracticeLeader.Kaku_Seiga => throw new NotImplementedException(),
                PracticeLeader.Hijiri_Byakuren => throw new NotImplementedException(),
                PracticeLeader.Koishi_Komeiji => throw new NotImplementedException(),
                //卡牌系统的设计师
                PracticeLeader.Nitori_Kawasiro => new PlayerInfo(
                     "NPC", "荷城河取", "1-0", "2-0", "",
                     new List<Deck>
                     {
                        new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                     }),
                PracticeLeader.Kaguya_Houraisan => throw new NotImplementedException(),
                //雾之湖的霸主
                PracticeLeader.Cirno => new PlayerInfo(
                     "NPC", "琪露诺", "1-0", "2-0", "",
                     new List<Deck>
                     {
                        new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                     }),
                PracticeLeader.Remilia_Scarlet => throw new NotImplementedException(),
                //逆转胜负的王牌
                PracticeLeader.Kijin_Seija => new PlayerInfo(
                     "NPC", "鬼人正邪", "1-0", "2-0", "",
                     new List<Deck>
                     {
                        new Deck("gezi", "M_F1_0L_001","N0_0", new List<string>
                            {
                                "M_F1_1G_001","M_F1_1G_002","M_F1_1G_003","M_F1_1G_004",
                                "M_F1_2S_001","M_F1_2S_002","M_F1_2S_003","M_F1_2S_004","M_F1_2S_005","M_F1_2S_006",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                                "M_F1_3C_001","M_F1_3C_002","M_F1_3C_003","M_F1_3C_004","M_F1_3C_005",
                            })
                     }),
                _ => Info.AgainstInfo.OnlineUserInfo.GetSampleInfo(),
            };
        }
    }
}
