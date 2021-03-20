using System.Security.Cryptography;
using System.Text;

namespace BusinessLogic
{
    public static class HashHelper
    {
        public static string Sha256(string plainText)
        {
            using (var sha256Hash = SHA256.Create())
            {
                var stringBuilder = new StringBuilder();

                foreach (var b in sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plainText)))
                    stringBuilder.Append(b.ToString("x2"));

                return stringBuilder.ToString();
            }
        }
    }
}