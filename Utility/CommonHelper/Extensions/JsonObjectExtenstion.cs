using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace CommonHelper
{
    public static class JsonObjectExtenstion
    {
        public static ValidationUtils Validate(this JsonObject input, string keyName)
        {
            if (input == null || !input.ContainsKey(keyName))
            {
                Exception logException = new Exception($@"Key {keyName} does not exist in the JsonObject");
                throw new ArgumentException("Something went error.", logException);
            }

            return new ValidationUtils(keyName, input[keyName]);
        }

        public static ValidationUtils Validate(this JsonObject? input, string keyName, string errorMessage)
        {
            if (input == null || !input.ContainsKey(keyName))
            {
                Exception logException = new Exception($@"Key {keyName} does not exist in the JsonObject");
                throw new ArgumentException(errorMessage, logException);
            }

            return new ValidationUtils(keyName, input[keyName], errorMessage);
        }
        public static ValidationUtils Validate(this JsonObject? input, string[] keyNames, string errorMessage)
        {
            if (input == null || keyNames.Where(keyName => !input.ContainsKey(keyName)).Count() > 0)
            {
                Exception logException = new Exception($@"Key {keyNames} does not exist in the JsonObject");
                throw new ArgumentException(errorMessage, logException);
            }
            List<KeyValuePair<string, JsonNode>> inputNodes = keyNames.Select(keyName => new KeyValuePair<string, JsonNode>(keyName, input[keyName])).ToList();

            return new ValidationUtils(inputNodes, errorMessage);
        }

        public static ValidationUtils Validate(this string input, [CallerArgumentExpression("input")] string inputExp = "")
        {
            return new ValidationUtils(inputExp, input);
        }

        public static ValidationUtils Validate(this string input, string errorMessage, [CallerArgumentExpression("input")] string inputExp = "")
        {
            return new ValidationUtils(inputExp, input, errorMessage);
        }
    }
}
