using MongoDB.Bson.Serialization.Attributes;

namespace Server
{
    public class ServerConfigManager
    {
        
        public static ServerConfig config;
        public static string CommandPassword => config.CommandPassword;
        public static List<string> DrawAbleCardIDs_Test => config.DrawAbleCardIDs_Test;
        public static List<string> DrawAbleCardIDs_Release => config.DrawAbleCardIDs_Release;
    }
}
