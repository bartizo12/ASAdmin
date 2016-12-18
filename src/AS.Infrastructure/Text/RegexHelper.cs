using System.Text.RegularExpressions;

namespace AS.Infrastructure
{
    /// <summary>
    /// Regex helper functions
    /// </summary>
    public static class RegexHelper
    {
        private static readonly string EmailMaskRegex = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";

        /// <summary>
        /// Masks e-mail address.Replaces characters with '*' except the last and
        /// the first character of the local part of  e-mail address
        /// For example if address is : "test@testSmtp.com" it will be masked as ; "t**t@testSmtp.com"
        /// </summary>
        /// <param name="emailAddress">E-Mail Address to be masked</param>
        /// <returns>Masked e-mail address</returns>
        public static string MaskEmailAddress(string emailAddress)
        {
            return Regex.Replace(emailAddress, EmailMaskRegex, m => new string('*', m.Length));
        }

        /// <summary>
        /// Masks whole string with '*' character (just like password input fields)
        /// </summary>
        /// <param name="input">Input to be masked</param>
        /// <returns>Masked string</returns>
        public static string Mask(string input)
        {
            return Mask(input, '*');
        }

        /// <summary>
        /// Masks whole string with <paramref name="maskChar"/>
        /// Example :  input : 'test', maskChar = 'x' ,  this will return : xxxx
        /// </summary>
        /// <param name="input">String to be masked</param>
        /// <param name="maskChar">Mask character</param>
        /// <returns>Masked string</returns>
        public static string Mask(string input, char maskChar)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return new string(maskChar, input.Length);
        }
    }
}