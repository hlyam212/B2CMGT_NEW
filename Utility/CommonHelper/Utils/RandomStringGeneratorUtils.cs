using System.Security.Cryptography;
using System.Text;

namespace CommonHelper
{
    public class RandomStringGenerator
    {
        private readonly string _characters;
        private readonly Random _random;

        public RandomStringGenerator()
        {
            _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_";
            _random = new Random();
        }

        public RandomStringGenerator(string randomCharacters)
        {
            _characters = randomCharacters;
            _random = new Random();
        }

        public string GenerateStringID(int length)
        {
            var result = new StringBuilder(length);
            var bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            for (int i = 0; i < length; i++)
            {
                int index = bytes[i] % _characters.Length;
                result.Append(_characters[index]);
            }
            return result.ToString();
        }
    }
}
