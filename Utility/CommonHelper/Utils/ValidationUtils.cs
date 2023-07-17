using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace CommonHelper
{
    public class ValidationUtils
    {
        private readonly bool multipleKey = false;
        private readonly string keyName;
        private readonly JsonNode _value;
        private readonly List<KeyValuePair<string, JsonNode>> _values;
        private string errorMessage;

        public ValidationUtils(string keyName, JsonNode value)
        {
            this.keyName = keyName;
            this._value = value;
        }

        public ValidationUtils(string keyName, JsonNode value, string errorMessage)
        {
            this.keyName = keyName;
            this._value = value;
            this.errorMessage = errorMessage;
        }

        public ValidationUtils(List<KeyValuePair<string, JsonNode>> _values, string errorMessage)
        {
            this.multipleKey = true;
            this._values = _values;
            this.errorMessage = errorMessage;
        }

        #region Shared
        private void ThrowErrorMessage(string message)
        {
            Exception logException = new Exception(message);
            if (string.IsNullOrEmpty(errorMessage))
            {
                throw new ArgumentException(message, logException);
            }

            throw new ArgumentException(errorMessage ?? "Something went error.", logException);
        }

        public ValidationUtils WithErrorMessage(string errorMessage)
        {
            this.errorMessage = errorMessage;

            return this;
        }

        public ValidationUtils NextNode(string nextKayName)
        {
            if (_value == null)
            {
                ThrowErrorMessage($@"{keyName} is Null or Empty.");
            }
            if (!_value.AsObject().ContainsKey(nextKayName))
            {
                ThrowErrorMessage($@"Node ({nextKayName}) of {keyName} is Null or Empty.");
            }

            return new ValidationUtils($@"{keyName}/{nextKayName}", _value[nextKayName], errorMessage);
        }

        public ValidationUtils Must<T>(Func<T?, bool> map)
        {
            var convertValue = _value.ConvertObject<T>();
            if (!map(convertValue))
            {
                ThrowErrorMessage($@"{keyName} must equal to map.");
            }

            return this;
        }
        public ValidationUtils Must(Func<List<KeyValuePair<string, JsonNode>>, bool> map)
        {
            if (!map(_values))
            {
                ThrowErrorMessage($@"{keyName} must equal to map.");
            }

            return this;
        }


        /// <param name="doHtmlEncoder">使用 JsonEncodedText.Encode 方法對輸入字串進行 JSON 編碼，將其中的特殊字元轉換為對應的 Unicode 轉義序列。這個方法只會編碼特殊字元，不會編碼中文字符，因此可以提高性能。</param>
        public T? GetValue<T>(bool returnNull = false, bool notJsonEncoded = false)
        {
            try
            {
                string safeValue = _value.ToSafeString();
                if (typeof(T).IsEnum)
                {
                    return safeValue.ToEnum<T>();
                }

                Type propertyType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

                if (propertyType == typeof(string) && RegexExtensions.RegexMatch(safeValue, @"[<>&""'\/()%+]"))
                {
                    if (notJsonEncoded)
                    {
                        return (T)Convert.ChangeType(safeValue, propertyType);
                    }
                    return (T)Convert.ChangeType(JsonEncodedText.Encode(safeValue).ToSafeString(), propertyType);
                }

                return (T)Convert.ChangeType(safeValue, propertyType);
            }
            catch (Exception ex)
            {
                if (returnNull)
                {
                    return default(T);
                }
                throw new ArgumentException($@"The type of {keyName} is wrong", ex);
            }
        }
        #endregion

        #region JsonNode
        public ValidationUtils NotNull()
        {
            if (_value == null)
            {
                ThrowErrorMessage($@"{keyName} is Null.");
            }

            return this;
        }

        public ValidationUtils NotNullOrEmpty()
        {
            if (_value == null)
            {
                ThrowErrorMessage($@"{keyName} is Null or Empty.");
            }

            if (string.IsNullOrEmpty(_value?.ToString()) || string.IsNullOrWhiteSpace(_value?.ToString()))
            {
                ThrowErrorMessage($@"{keyName} is Null or Empty.");
            }

            return this;
        }

        public ValidationUtils Contains(string keyword)
        {
            string? value = _value?.ToString();

            if (value.Contains(keyword))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} does not contains {keyword}");
            }

            return this;
        }

        public ValidationUtils Equal<T>(T equalValue)
        {
            if (!Equals(Convert.ChangeType(_value.ToString(), typeof(T)), equalValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} does not equal to {equalValue}");
            }

            return this;
        }
        public ValidationUtils NotEqual<T>(T equalValue)
        {
            if (Equals(Convert.ChangeType(_value.ToString(), typeof(T)), equalValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} does equal to {equalValue}");
            }

            return this;
        }

        /// <summary>
        /// 是否小於指定值
        /// </summary>
        public ValidationUtils LessThan(int minValue)
        {
            if (!int.TryParse(_value.ToString(), out int intValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than {minValue}");
            }

            if (intValue >= minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than {minValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否小於指定值
        /// </summary>
        public ValidationUtils LessThan(double minValue)
        {
            if (!double.TryParse(_value.ToString(), out double doubleValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than {minValue}");
            }

            if (doubleValue >= minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than {minValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否小於指定值
        /// </summary>
        public ValidationUtils LessThan(decimal minValue)
        {
            if (!decimal.TryParse(_value.ToString(), out decimal decimalValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} does not less than {minValue}");
            }

            if (decimalValue >= minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than {minValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否小於等於指定值
        /// </summary>
        public ValidationUtils LessThanOrEqualTo(int minValue)
        {
            if (!int.TryParse(_value.ToString(), out int intValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than or not equals to {minValue}");
            }

            if (intValue > minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than or not equals to {minValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否小於等於指定值
        /// </summary>
        public ValidationUtils LessThanOrEqualTo(double minValue)
        {
            if (!double.TryParse(_value.ToString(), out double doubleValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than or not equals to {minValue}");
            }

            if (doubleValue > minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than or not equals to {minValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否小於等於指定值
        /// </summary>
        public ValidationUtils LessThanOrEqualTo(decimal minValue)
        {
            if (!decimal.TryParse(_value.ToString(), out decimal decimalValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than or not equals to {minValue}");
            }

            if (decimalValue > minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not less than or not equals to {minValue}");
            }

            return this;
        }

        /// <summary>
        /// 是否大於指定值
        /// </summary>
        public ValidationUtils GreaterThan(int maxValue)
        {
            if (!int.TryParse(_value.ToString(), out int intValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than {maxValue}");
            }

            if (intValue <= maxValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than {maxValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否大於指定值
        /// </summary>
        public ValidationUtils GreaterThan(double maxValue)
        {
            if (!double.TryParse(_value.ToString(), out double doubleValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than {maxValue}");
            }

            if (doubleValue <= maxValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than {maxValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否大於指定值
        /// </summary>
        public ValidationUtils GreaterThan(decimal maxValue)
        {
            if (!decimal.TryParse(_value.ToString(), out decimal decimalValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than {maxValue}");
            }

            if (decimalValue <= maxValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than {maxValue}");
            }

            return this;
        }

        /// <summary>
        /// 是否大於等於指定值
        /// </summary>
        public ValidationUtils GreaterThanOrEqualTo(int maxValue)
        {
            if (!int.TryParse(_value.ToString(), out int intValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than and not equal to {maxValue}");
            }

            if (intValue < maxValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than and not equal to {maxValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否大於等於指定值
        /// </summary>
        public ValidationUtils GreaterThanOrEqualTo(double maxValue)
        {
            if (!double.TryParse(_value.ToString(), out double doubleValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than and not equal to {maxValue}");
            }

            if (doubleValue < maxValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than and not equal to {maxValue}");
            }

            return this;
        }
        /// <summary>
        /// 是否大於等於指定值
        /// </summary>
        public ValidationUtils GreaterThanOrEqualTo(decimal maxValue)
        {
            if (!decimal.TryParse(_value.ToString(), out decimal decimalValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than and not equal to {maxValue}");
            }

            if (decimalValue < maxValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not greater than and not equal to {maxValue}");
            }

            return this;
        }

        public ValidationUtils InRange(int minValue, int maxValue)
        {
            if (!int.TryParse(_value.ToString(), out int intValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not between {minValue} and {maxValue}.");
            }

            if (intValue > maxValue || intValue < minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not between {minValue} and {maxValue}.");
            }

            return this;
        }
        public ValidationUtils InRange(double minValue, double maxValue)
        {
            if (!double.TryParse(_value.ToString(), out double doubleValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not between {minValue} and {maxValue}.");
            }

            if (doubleValue > maxValue || doubleValue < minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not between {minValue} and {maxValue}.");
            }

            return this;
        }
        public ValidationUtils InRange(decimal minValue, decimal maxValue)
        {
            if (!decimal.TryParse(_value.ToString(), out decimal decimalValue))
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not between {minValue} and {maxValue}.");
            }

            if (decimalValue > maxValue || decimalValue < minValue)
            {
                ThrowErrorMessage($@"Value ({_value}) of {keyName} is not between {minValue} and {maxValue}.");
            }

            return this;
        }

        public ValidationUtils Length(int length, bool allowEmpty = false)
        {
            Type propertyType = Nullable.GetUnderlyingType(_value.GetType()) ?? _value.GetType();

            if (propertyType == typeof(JsonArray))
            {
                if (_value == null ||
                    _value.AsArray().Where(value =>
                        (allowEmpty && !string.IsNullOrEmpty(value?.ToString()) && value?.ToString().Length != length) ||
                        (!allowEmpty && (string.IsNullOrEmpty(value?.ToString()) || value?.ToString().Length != length)))
                    .Count() > 0)
                {
                    ThrowErrorMessage($@"Value length of {keyName} must equals to {length}");
                }
            }
            else
            {
                string? value = _value?.ToString();
                if ((allowEmpty && !string.IsNullOrEmpty(value) && value.Length != length) ||
                    (!allowEmpty && (string.IsNullOrEmpty(value) || value.Length != length)))
                {
                    ThrowErrorMessage($@"Value length of {keyName} must equals to {length}");
                }
            }

            return this;
        }

        public ValidationUtils MaximumLength(int max, bool allowEmpty = false)
        {
            string erroMsg = $@"Value length of {keyName} must be smaller than {max}.";
            var value = _value?.ToString();
            if ((allowEmpty && !string.IsNullOrEmpty(value) && value.Length > max) ||
                (!allowEmpty && (string.IsNullOrEmpty(value) || value.Length > max)))
            {
                ThrowErrorMessage(erroMsg);
            }

            return this;
        }

        public ValidationUtils MinimumLength(int min, bool allowEmpty = false)
        {
            string erroMsg = $@"Value length of {keyName} must be bigger than {min}.";

            var value = _value?.ToString();
            if ((allowEmpty && !string.IsNullOrEmpty(value) && value.Length < min) ||
                (!allowEmpty && (string.IsNullOrEmpty(value) || value.Length < min)))
            {
                ThrowErrorMessage(erroMsg);
            }

            return this;
        }

        public ValidationUtils Length(int min, int max)
        {
            var value = _value?.ToString();
            if (value.Length < min || value.Length > max)
            {
                ThrowErrorMessage($@"Value length of {keyName} must be between {min} max {max}");
            }

            return this;
        }

        public ValidationUtils IsNumber(bool allowEmpty = false)
        {
            Type propertyType = Nullable.GetUnderlyingType(_value.GetType()) ?? _value.GetType();

            if (propertyType == typeof(JsonArray))
            {
                if (_value == null ||
                    _value.AsArray().Where(value =>
                        (allowEmpty && !string.IsNullOrEmpty(value?.ToString()) && !RegexExtensions.isNumber(value?.ToString())) ||
                        (!allowEmpty && !RegexExtensions.isNumber(value?.ToString())))
                    .Count() > 0)
                {
                    ThrowErrorMessage($@"Value ({_value}) of {keyName} is not Number");
                }
            }
            else
            {
                string? value = _value.ToString();
                if ((allowEmpty && !string.IsNullOrEmpty(value) && !RegexExtensions.isNumber(value)) ||
                    (!allowEmpty && !RegexExtensions.isNumber(value)))
                {
                    ThrowErrorMessage($@"Value ({value}) of {keyName} is not Number");
                }
            }

            return this;
        }

        public ValidationUtils IsNaturalNumber(bool allowEmpty = false)
        {
            Type propertyType = Nullable.GetUnderlyingType(_value.GetType()) ?? _value.GetType();

            if (propertyType == typeof(JsonArray))
            {
                if (_value == null ||
                    _value.AsArray().Where(value =>
                        (allowEmpty && !string.IsNullOrEmpty(value?.ToString()) && !RegexExtensions.isNaturalNumber(value?.ToString())) ||
                        (!allowEmpty && !RegexExtensions.isNaturalNumber(value?.ToString())))
                    .Count() > 0)
                {
                    ThrowErrorMessage($@"Value ({_value}) of {keyName} is not NaturalNumber");
                }
            }
            else
            {
                string? value = _value.ToString();
                if ((allowEmpty && !string.IsNullOrEmpty(value) && !RegexExtensions.isNaturalNumber(value)) ||
                    (!allowEmpty && !RegexExtensions.isNaturalNumber(value)))
                {
                    ThrowErrorMessage($@"Value ({value}) of {keyName} is not NaturalNumber");
                }
            }

            return this;
        }


        public ValidationUtils IsNaturalNumberDash(bool allowEmpty = false)
        {
            string? value = _value.ToString();
            if ((allowEmpty && !string.IsNullOrEmpty(value) && !RegexExtensions.isNaturalNumberDash(value)) ||
                (!allowEmpty && !RegexExtensions.isNaturalNumberDash(value)))
            {
                ThrowErrorMessage($@"Value ({value}) of {keyName} is not NaturalNumberDash");
            }

            return this;
        }

        /// <summary>
        /// 這個檢查包含了 Unicode 碼點範圍，包括了 a-z、A-Z、中文字符、以及空格。
        /// </summary>
        public ValidationUtils IsValidInput(bool allowEmpty = false)
        {
            string pattern = @"^[a-zA-Z\s\u4e00-\u9fa5]+$";
            Type propertyType = Nullable.GetUnderlyingType(_value.GetType()) ?? _value.GetType();

            if (propertyType == typeof(JsonArray))
            {
                if (_value == null ||
                    _value.AsArray().Where(value =>
                        (allowEmpty && !string.IsNullOrEmpty(value?.ToString()) && !RegexExtensions.RegexMatch(value?.ToString(), pattern)) ||
                        (!allowEmpty && !RegexExtensions.RegexMatch(value?.ToString(), pattern)))
                    .Count() > 0)
                {
                    ThrowErrorMessage($@"Value ({_value}) of {keyName} is not valid Input");
                }
            }
            else
            {
                string? value = _value.ToString();
                if ((allowEmpty && !string.IsNullOrEmpty(value) && !RegexExtensions.RegexMatch(value, pattern)) ||
                    (!allowEmpty && !RegexExtensions.RegexMatch(value, pattern)))
                {
                    ThrowErrorMessage($@"Value ({value}) of {keyName} is not valid Input");
                }
            }

            return this;
        }

        public ValidationUtils IsXssInput(bool allowEmpty = false)
        {
            string pattern = @"^[<>&""'\/()%+]";
            Type propertyType = Nullable.GetUnderlyingType(_value.GetType()) ?? _value.GetType();

            if (propertyType == typeof(JsonArray))
            {
                if (_value == null ||
                    _value.AsArray().Where(value =>
                        (allowEmpty && !string.IsNullOrEmpty(value?.ToString()) && RegexExtensions.RegexMatch(value?.ToString(), pattern)) ||
                        (!allowEmpty && RegexExtensions.RegexMatch(value?.ToString(), pattern)))
                    .Count() > 0)
                {
                    ThrowErrorMessage($@"Value ({_value}) of {keyName} is not valid Input");
                }
            }
            else
            {
                string? value = _value.ToString();
                if ((allowEmpty && !string.IsNullOrEmpty(value) && RegexExtensions.RegexMatch(value, pattern)) ||
                    (!allowEmpty && RegexExtensions.RegexMatch(value, pattern)))
                {
                    ThrowErrorMessage($@"Value ({value}) of {keyName} is not valid Input");
                }
            }

            return this;
        }

        public ValidationUtils RegexMatch(string pattern, bool allowEmpty = false)
        {
            string? value = _value.ToString();
            if ((allowEmpty && !string.IsNullOrEmpty(value) && !RegexExtensions.RegexMatch(value, pattern)) ||
                (!allowEmpty && !RegexExtensions.RegexMatch(value, pattern)))
            {
                ThrowErrorMessage($@"Value ({value}) of {keyName} does not match Regex");
            }

            return this;
        }

        public ValidationUtils IsVaildEmail(bool allowEmpty = false)
        {
            string? value = _value.ToString();
            if ((allowEmpty && !string.IsNullOrEmpty(value) && !RegexExtensions.IsVaildEmail(value)) ||
                (!allowEmpty && !RegexExtensions.IsVaildEmail(value)))
            {
                ThrowErrorMessage($@"Value ({value}) of {keyName} is not valid Email");
            }

            return this;
        }


        public ValidationUtils IsEnum<T>(bool needUpperCase = false, bool allowEmpty = false)
        {
            Type propertyType = Nullable.GetUnderlyingType(_value.GetType()) ?? _value.GetType();

            if (propertyType == typeof(JsonArray))
            {
                if (_value == null ||
                    _value.AsArray().Where(value =>
                        (needUpperCase && !EnumExtensions.ExistInEnum<T>(value?.ToString().ToUpper())) ||
                        (!needUpperCase && !EnumExtensions.ExistInEnum<T>(value?.ToString())))
                    .Count() > 0)
                {
                    ThrowErrorMessage($@"{keyName} is not specific Enum");
                }
            }
            else
            {
                string? value = _value.ToString();
                if ((allowEmpty && !string.IsNullOrEmpty(value)) ||
                    (!allowEmpty))
                {
                    if (needUpperCase)
                    {
                        value = value.ToUpper();
                    }
                    if (!EnumExtensions.ExistInEnum<T>(value))
                    {
                        ThrowErrorMessage($@"{keyName} is not specific Enum");
                    }
                }
            }

            return this;
        }
        #endregion

        #region JsonArray
        public ValidationUtils ZeroCount()
        {
            if (multipleKey)
            {
                if (_values == null)
                {
                    ThrowErrorMessage($@"Data is empty.");
                }

                var keynames = _values.Where(node => node.Value == null || node.Value.AsArray().Count == 0).Select(node => node.Key).ToList();
                if (keynames.Count() == _values.Count())
                {
                    ThrowErrorMessage($@"Count of {string.Join(", ", keynames)} is 0.");
                }
            }
            else
            {
                if (_value == null || _value.AsArray().Count == 0)
                {
                    ThrowErrorMessage($@"Count of {keyName} is 0.");
                }
            }
            return this;
        }
        #endregion
    }
}
