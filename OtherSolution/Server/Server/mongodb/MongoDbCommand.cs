﻿using AntDesign;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

namespace Server
{
    class MongoDbCommand
    {
        static MongoClient client;
        static IMongoDatabase db;
        static IMongoCollection<PlayerInfo>? PlayerInfoCollection { get; set; }
        static IMongoCollection<CardConfig>? CardConfigCollection { get; set; }
        static IMongoCollection<ServerConfig>? ServerConfigCollection { get; set; }
        static IMongoCollection<AgainstSummary>? SummaryCollection { get; set; }
        public static IMongoCollection<DiyCardInfo> DiyCardCollection { get; set; }
        public static void Init()
        {

            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

            Log.Summary("/////////////////////////////");
            Log.Summary("链接数据库");
            //读取服务器配置保密文件
            if (!File.Exists("Config.ini"))
            {
                File.WriteAllLines("Config.ini", new List<string> { "MongodbIP", "mongodb://106.15.38.165:28020" });
                Console.WriteLine("检测不到配置文件，开始创建");
            }

            client = new MongoClient(File.ReadAllLines("Config.ini")[1]);
            db = client.GetDatabase("Gezi");
            PlayerInfoCollection = db.GetCollection<PlayerInfo>("PlayerInfo");
            CardConfigCollection = db.GetCollection<CardConfig>("CardConfig");
            ServerConfigCollection = db.GetCollection<ServerConfig>("ServerConfig");
            SummaryCollection = db.GetCollection<AgainstSummary>("AgainstSummary");
            DiyCardCollection = db.GetCollection<DiyCardInfo>("DiyCards");

            Log.Summary($"查询到用户信息{PlayerInfoCollection.AsQueryable().Count()}条");
            Log.Summary($"查询到卡牌配置信息{CardConfigCollection.AsQueryable().Count()}条");
            Log.Summary($"查询到对局记录信息{SummaryCollection.AsQueryable().Count()}条");
            Log.Summary($"查询到diy卡牌信息{DiyCardCollection.AsQueryable().Count()}条");
            Log.Summary("/////////////////////////////");
            LoadServerConfig();
        }
        //加载服务器的配置信息
        public static void LoadServerConfig()
        {
            ServerConfigManager.config = ServerConfigCollection?.AsQueryable().FirstOrDefault();
            if (ServerConfigManager.config == null)
            {
                Console.WriteLine("检索不到数据库中的服务器配置文件，请重新生成新配置文件");
                ServerConfigManager.config = new ServerConfig() { CommandPassword = "" };
                ServerConfigCollection?.InsertOne(ServerConfigManager.config);
            }
        }
        //////////////////////////////////////////////////账号系统///////////////////////////////////////////////////////////////////
        public static int Register(string account, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account);
            if (PlayerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0)
            {
                return -1;//已存在
            }
            else
            {
                PlayerInfoCollection.InsertOne(
                    new PlayerInfo().Creat(account, password, "萌新",
                    new List<Deck>()
                    {
                         new Deck("萌新的第一套卡组", "M_N0_0L_001","N0_0", new List<string>
                         {
                             "M_N0_1G_001","M_N0_1G_002","M_N0_1G_003","M_N0_1G_004",
                             "M_N0_2S_001","M_N0_2S_002","M_N0_2S_003","M_N0_2S_004","M_N0_2S_005","M_N0_2S_006",
                             "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                             "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                             "M_N0_3C_001","M_N0_3C_002","M_N0_3C_003","M_N0_3C_004","M_N0_3C_005",
                         })
                    },
                    new Dictionary<string, int>
                    {
                        {  "M_N0_1G_001",1  },
                        {  "M_N0_1G_002",1  },
                    }));
                return 1;//成功
            }
        }
        public static PlayerInfo? Login(string account, string password)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account);
            PlayerInfo? userInfo = null;
            if (PlayerInfoCollection.Find(CheckUserExistQuery).Any())
            {
                userInfo = PlayerInfoCollection.Find(CheckUserExistQuery).FirstOrDefault();
                if (userInfo.Password == password.GetSaltHash(userInfo.UID))
                {
                    var result = UpdateInfo(account, userInfo.Password, (x => x.LastLoginTime), DateTime.Now);
                }
                else
                {
                    userInfo = null;
                }
            }
            //1正确,-1密码错误,-2无此账号
            //return (UserInfo != null ? 1 : playerInfoCollection.Find(CheckUserExistQuery).CountDocuments() > 0 ? -1 : -2, UserInfo);
            return userInfo;
        }
        public static List<string> DrawCard(string account, string password, List<Faith> selectFaiths)
        {
            PlayerInfo? userInfo = PlayerInfoCollection.AsQueryable().FirstOrDefault(info => info.Account == account);
            if (userInfo != null)
            {
                if (userInfo.Password == password.GetSaltHash(userInfo.UID))
                {
                    var result = UpdateInfo(account, userInfo.Password, (x => x.LastLoginTime), DateTime.Now);
                }
                else
                {
                    userInfo = null;
                }

                bool HaveEnoughFaith = true;
                //判断能否扣除仓库中的信念
                selectFaiths.GroupBy(x => x.BelongUser).ToList().ForEach(group =>
                {
                    //如果有一项不足，则跳过
                    if (userInfo.Faiths.Where(faith => faith.BelongUser == group.Key).Count() >= group.Count())
                    {
                        HaveEnoughFaith = false;
                    }
                });
                //满足抽卡条件
                //if (HaveEnoughFaith)
                if (true)
                {
                    //如果是第一次，固定返回
                    if (userInfo.IsFirstDraw)
                    {
                        var result = UpdateInfo(account, userInfo.Password, (x => x.LastLoginTime), DateTime.Now);
                        return new List<string> { "M_N0_1G_001" };
                    }
                    else
                    {
                        //卡池计算公式
                        Random rand = new Random();
                        return ServerConfigManager.DrawAbleCardIDs_Test
                                    .OrderBy(x => rand.Next())
                                    .Take(selectFaiths.Count)
                                    .ToList();
                    }
                    //否则平等的随机抽卡
                }
                else
                {
                    //扣除失败
                    return new List<string> { "0" };
                }
            }
            //账号验证失败
            return new List<string> { "1" };
        }
        public static int GetRegisterPlayerCount() => PlayerInfoCollection.AsQueryable().Count();
        //////////////////////////////////////////////////用户信息更新///////////////////////////////////////////////////////////////////
        public static bool UpdateInfo<TField>(string account, string password, Expression<Func<PlayerInfo, TField>> setOperator, TField field)
        {
            var CheckUserExistQuery = Builders<PlayerInfo>.Filter.Where(info => info.Account == account && info.Password == password);
            var updateUserState = Builders<PlayerInfo>.Update.Set(setOperator, field);
            IFindFluent<PlayerInfo, PlayerInfo> findFluent = PlayerInfoCollection.Find(CheckUserExistQuery);
            if (findFluent.CountDocuments() > 0)
            {
                PlayerInfoCollection.UpdateOne(CheckUserExistQuery, updateUserState);
                return true;//修改成功
            }
            else
            {
                return false;//修改失败
            }
        }
        //////////////////////////////////////////////////对战记录///////////////////////////////////////////////////////////////////
        public static void InsertAgainstSummary(AgainstSummary againstSummary) => SummaryCollection.InsertOne(againstSummary);
        public static List<AgainstSummary> QueryAgainstSummary(int skipCount, int takeCount) => SummaryCollection.AsQueryable().Skip(skipCount).Take(takeCount).ToList();
        public static List<AgainstSummary> QueryAgainstSummary(string playerAccount, int skipCount, int takeCount)
        {
            return SummaryCollection.AsQueryable().Where(summary => summary.Player1Info.Account == playerAccount || summary.Player1Info.Account == playerAccount).Skip(skipCount).Take(takeCount).ToList();
        }

        //////////////////////////////////////////////////卡牌配置///////////////////////////////////////////////////////////////////
        public static string InsertOrUpdateCardConfig(CardConfig newConfig, List<string> drawAbleList, string commandPassword)
        {
            if (commandPassword.GetSaltHash("514") == ServerConfigManager.CommandPassword)
            {
                if (newConfig.Type == "Test")
                {
                    ServerConfigCollection.UpdateOne(x => true, Builders<ServerConfig>.Update.Set(x => x.DrawAbleCardIDs_Test, drawAbleList));
                    Log.Summary(DateTime.Now + "更新测试版可抽取卡牌列表");
                }
                if (newConfig.Type == "Release")
                {
                    ServerConfigCollection.UpdateOne(x => true, Builders<ServerConfig>.Update.Set(x => x.DrawAbleCardIDs_Release, drawAbleList));
                    Log.Summary(DateTime.Now + "更新正式版可抽取卡牌列表");
                }
                var CheckConfigExistQuery = Builders<CardConfig>.Filter.Where(config => config.Version == newConfig.Version && config.Type == newConfig.Type);
                IFindFluent<CardConfig, CardConfig> findFluent = CardConfigCollection.Find(CheckConfigExistQuery);
                if (findFluent.CountDocuments() > 0)
                {
                    Console.WriteLine("修改卡组配置文件");
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.UpdataTime, newConfig.UpdataTime));
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.AssemblyFileData, newConfig.AssemblyFileData));
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.SingleCardFileData, newConfig.SingleCardFileData));
                    CardConfigCollection.UpdateOne(CheckConfigExistQuery, Builders<CardConfig>.Update.Set(x => x.MultiCardFileData, newConfig.MultiCardFileData));
                    return "已修改配置";//修改成功
                }
                else
                {
                    Console.WriteLine("增加卡组配置文件");
                    CardConfigCollection.InsertOne(newConfig);
                    return "已新增配置";//修改失败
                }

            }
            else
            {
                return "指令密码输入错误，服务器拒绝修改";
            }
        }
        public static CardConfig GetCardConfig(string version)
        {
            if (version == "")
            {
                version = CardConfigCollection.AsQueryable().Max(x => x.Version);
            }
            var target = CardConfigCollection.Find(x => x.Version == version).FirstOrDefault();
            return target;
        }
        public static string GetLastCardUpdateTime() => CardConfigCollection.AsQueryable().Max(x => x.UpdataTime).ToString();
        public static string GetLastCardUpdateVersion() => CardConfigCollection.AsQueryable().Max(x => x.Version).ToString();


    }
}