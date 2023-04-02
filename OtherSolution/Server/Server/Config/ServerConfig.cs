using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Server
{
    //记录服务器配置信息的模板
    public class ServerConfig
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        //可抽取的卡牌id列表(测试服)
        public List<string> DrawAbleCardIDs_Test { get; set; } = new List<string>();
        //可抽取的卡牌id列表(正式服)
        public List<string> DrawAbleCardIDs_Release { get; set; } = new List<string>();
        //操作指令的哈希值
        public string CommandPassword { get; set; }
    }
}
