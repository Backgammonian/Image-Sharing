using System.Text;
using System.Security.Cryptography;

namespace MyWebApp.Data
{
    public static class RandomGenerator
    {
        private const string _chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public static int GetRandomInt()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(int));

            return BitConverter.ToInt32(randomBytes, 0);
        }

        public static uint GetRandomUInt()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(uint));

            return BitConverter.ToUInt32(randomBytes, 0);
        }

        public static long GetRandomLong()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(long));

            return BitConverter.ToInt64(randomBytes, 0);
        }

        public static ulong GetRandomULong()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(ulong));

            return BitConverter.ToUInt64(randomBytes, 0);
        }

        public static string GetRandomString(int length)
        {
            var result = new StringBuilder();
            for (var j = 0; j < length; j++)
            {
                var randomNumber = RandomNumberGenerator.GetInt32(0, _chars.Length - 1);
                result.Append(_chars[randomNumber]);
            }

            return result.ToString();
        }
    }
}
