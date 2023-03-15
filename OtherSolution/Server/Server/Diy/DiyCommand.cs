using AntDesign;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    public class DiyCommand
    {
        public static List<DiyCardInfo> GetDiyCardsInfo() => MongoDbCommand.DiyCardCollection.AsQueryable().ToList();
        public static void AddDiyCardInfos(string uploadUID, string uploadName, string name,string tags, int point,string rank, string camp, string describe,string ability,string imageUrl)
        {
            int uid = GetDiyCardsInfo().Count();
            DiyCardInfo diyCard = new DiyCardInfo()
            {
                UpLoadUID = uploadUID,
                UpLoadName = uploadName,
                UpLoadTime = DateTime.Now,
                CardName = name,
                Tag = tags.Split(" ").ToList(),
                Point = point,
                Rank = rank,
                Camp = camp,
                Ability = ability,
                Describe = describe,
                ImageUrl = imageUrl,
                Commits = new List<DiyCardInfo.Commit>
                {
                    new DiyCardInfo.Commit()
                    {
                        User="gezi",
                        Text="好烂"
                    },
                    new DiyCardInfo.Commit()
                    {
                        User="gezi",
                        Text="好烂"
                    }
                }
            };
            MongoDbCommand.DiyCardCollection.InsertOne(diyCard);
        }
    }
}