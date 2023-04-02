namespace Server
{
    public static class Extension
    {
        
        public static string GetSaltHash(this string password, string uuid)
        {
            string salt = uuid;
            byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSaltBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }
}