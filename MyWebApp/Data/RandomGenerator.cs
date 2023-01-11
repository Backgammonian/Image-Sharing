using System.Text;
using System.Security.Cryptography;
using MyWebApp.Data.Interfaces;

namespace MyWebApp.Data
{
    public sealed class RandomGenerator : IRandomGenerator
    {
        public int GetRandomInt()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(int));

            return BitConverter.ToInt32(randomBytes, 0);
        }

        public uint GetRandomUInt()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(uint));

            return BitConverter.ToUInt32(randomBytes, 0);
        }

        public long GetRandomLong()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(long));

            return BitConverter.ToInt64(randomBytes, 0);
        }

        public ulong GetRandomULong()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(ulong));

            return BitConverter.ToUInt64(randomBytes, 0);
        }

        public string GetRandomString(int length)
        {
            var result = new StringBuilder();
            for (var j = 0; j < length; j++)
            {
                var randomNumber = RandomNumberGenerator.GetInt32(0, Constants.Chars.Length - 1);
                result.Append(Constants.Chars[randomNumber]);
            }

            return result.ToString();
        }

        public string GetRandomId() => GetRandomString(Constants.IdLength);
    }
}
