using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GTI619_Lab5.Utils
{
    public static class PasswordValidator
    {
        private static readonly Regex s_lowerCaseRegex = new Regex("^.*[a-z].*$", RegexOptions.Compiled);
        private static readonly Regex s_upperCaseRegex = new Regex("^.*[A-Z].*$", RegexOptions.Compiled);
        private static readonly Regex s_numberRegex = new Regex("^.*[\\d].*$", RegexOptions.Compiled);
        private static readonly Regex s_specialCharRegex = new Regex("^.*[-+_!@#$%^&*.,?<>;:'\"\\[\\]\\\\|\\/].*$", RegexOptions.Compiled);

        /// <summary>
        /// Valide le mot de passe selon les options et l'historique de mots de passe
        /// </summary>
        /// <returns></returns>
        public static bool Validate(string password, AdminOption options, List<PasswordHistory> passwordHistories)
        {
            bool valid = password.Length >= options.MinPasswordLength;

            if (options.IsLowerCaseCharacterRequired)
            {
                valid &= s_lowerCaseRegex.IsMatch(password);
            }
            if (options.IsUpperCaseCharacterRequired)
            {
                valid &= s_upperCaseRegex.IsMatch(password);
            }
            if (options.IsNumberRequired)
            {
                valid &= s_numberRegex.IsMatch(password);
            }
            if (options.IsSpecialCharacterRequired)
            {
                valid &= s_specialCharRegex.IsMatch(password);
            }

            for (int i = 0; i < passwordHistories.Count && i < options.NumberOfPasswordToKeepInHistory; i++)
            {
                valid &= HashingUtil.SaltAndHash(password, passwordHistories[i].PasswordSalt) != passwordHistories[i].PasswordHash;
            }

            return valid;
        }
    }
}