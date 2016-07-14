using System.Security.Cryptography;
using System.Text;

namespace GTI619_Lab5.Utils
{
    public static class HashingUtil
    {
        public const string DefaultSalt = "df2d3cea-38f8-43ff-8abf-399bd3408124";

        public static string SaltAndHash(string data, string salt)
        {
            return Encoding.Unicode.GetString(Hash(ApplySalt(data, salt)));
        }
        
        private const int SALTED_LENGTH = 1000;
        private const int NUMBER_HASH_OF_ITERATION = 25;

        private static string ApplySalt(string originalData, string salt)
        {
            var salted = salt + originalData + salt;

            while (salted.Length < SALTED_LENGTH)
            {
                salted += salt;
            }

            return salted;
        }

        private static byte[] Hash(string data)
        {
            var asByteArray = Encoding.UTF8.GetBytes(data);
            using (SHA512 sha = new SHA512CryptoServiceProvider())
            {
                for (int i = 0; i < NUMBER_HASH_OF_ITERATION; i++)
                {
                    asByteArray = sha.ComputeHash(asByteArray);
                }
            }
            return asByteArray;
        }
    }
}