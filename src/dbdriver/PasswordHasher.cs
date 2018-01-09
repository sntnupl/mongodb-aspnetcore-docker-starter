using System.Security.Cryptography;
using System.Text;

namespace MongoCore.DbDriver
{
    public static class PasswordHasher
    {
        private static SHA1 _sha1;

        public static string Generate(string password)
        {
            _sha1 = SHA1.Create();
            var data = Encoding.ASCII.GetBytes(password);
            var hashBytes =  _sha1.ComputeHash(data);
            return Encoding.ASCII.GetString(hashBytes);
        }

        public static bool Match(string password, string hash)
        {
            string result = Generate(password);
            return result.Equals(hash);
        }
    }
}