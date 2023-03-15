
using Newtonsoft.Json;

namespace Server
{
    public static class Extension
    {
        public static string ToJson(this object DataObject) => JsonConvert.SerializeObject(DataObject, Formatting.Indented);
        public static T ToObject<T>(this string target) => JsonConvert.DeserializeObject<T>(target);
        public static T To<T>(this object target) => target.ToJson().ToObject<T>();
        public static string GetSaltHash(this string password, string uuid)
        {
            string salt = uuid;
            byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSaltBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}