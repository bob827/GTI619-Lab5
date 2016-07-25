using System;
using System.Security.Cryptography;
using System.Text;

namespace GTI619_Lab5.Utils
{
    public static class HashingUtil
    {
        public static string Version { get { return "SHA512"; } }

        /// <summary>
        /// Applique le sel sur les donnees et ensuite les hache 
        /// </summary>
        public static string SaltAndHash(string data, string salt)
        {
            return Convert.ToBase64String(Hash(ApplySalt(data, salt)));
        }

        private const int MIN_SALTED_LENGTH = 1000;
        private const int NUMBER_HASH_OF_ITERATION = 25;

        /// <summary>
        /// Applique le sel sur le originalData et retourne la valeur
        /// </summary>
        private static string ApplySalt(string originalData, string salt)
        {
            var salted = salt + originalData + salt;

            while (salted.Length < MIN_SALTED_LENGTH)
            {
                salted += originalData + salt;
            }

            return salted;
        }

        /// <summary>
        /// Applique l'algo the hachage sur le data
        /// </summary>
        private static byte[] Hash(string data)
        {
            var asByteArray = Encoding.UTF8.GetBytes(data);
            using (SHA512 sha = new SHA512Cng())
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