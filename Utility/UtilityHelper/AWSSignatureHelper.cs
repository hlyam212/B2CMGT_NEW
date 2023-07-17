using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EB2CUtility_46.AWSSignatureV4_S3
{
    public static class AWSSignatureHelper
    {
        public static string PostToken(String url, String data, String aws_key, String secret_key, String region)
        {
            string result = "";
            try
            {
                String method = "POST";
                String service = "execute-api";
                String algorithm = "AWS4-HMAC-SHA256";
                String content_type = "application/x-amz-json-1.0";
                Uri u = new Uri(url);
                String endpoint = "";
                String request_parameters = "";
                if (String.IsNullOrEmpty(u.Query) == true)
                {
                    endpoint = url;
                }
                else
                {
                    endpoint = url.Replace(u.Query, "");
                }

                String host = u.Host;

                String signed_headers = "content-type;host;x-amz-date";
                String payload_hash = Hash(data);
                if (string.IsNullOrEmpty(aws_key))
                {
                    return result;
                }
                else if (string.IsNullOrEmpty(secret_key))
                {
                    return result;
                }
                else if (string.IsNullOrEmpty(region))
                {
                    return result;
                }
                var t = DateTimeOffset.UtcNow;//標準UTC時間
                var amzdate = t.ToString("yyyyMMddTHHmmssZ");
                var datestamp = t.ToString("yyyyMMdd");

                var credential_scope = $"{datestamp}/{region}/{service}/aws4_request";
                var canonical_uri = u.AbsolutePath;
                var canonical_querystring = request_parameters;
                var canonical_headers = "content-type:" + content_type + '\n' + "host:" + host + '\n' + "x-amz-date:" + amzdate + '\n';
                var canonical_request = method + '\n' + canonical_uri + '\n' + canonical_querystring + '\n' + canonical_headers + '\n' + signed_headers + '\n' + payload_hash;
                var string_to_sign = $"{algorithm}\n{amzdate}\n{credential_scope}\n" + Hash(canonical_request.ToString());

                var signing_key = GetSignatureKey(secret_key, datestamp, region, service);
                var signature = ToHexString(HmacSHA256(signing_key, string_to_sign));
                var authorization_header = algorithm + " " + "Credential=" + aws_key + "/" + credential_scope + ", " + "SignedHeaders=" + signed_headers + ", " + "Signature=" + signature;
                result = authorization_header;
            }
            catch (Exception ex)
            {
                
            }
            return result;
        }
        private static SHA256 _sha256 = SHA256.Create();
        private static string Hash(string stringToHash)
        {
            var result = _sha256.ComputeHash(Encoding.UTF8.GetBytes(stringToHash));
            return ToHexString(result);
        }
        private static string ToHexString(byte[] array)
        {
            var hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
        private static byte[] HmacSHA256(byte[] key, string data)
        {
            var hashAlgorithm = new HMACSHA256(key);

            return hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(data));
        }
        private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName)
        {
            byte[] kSecret = Encoding.UTF8.GetBytes("AWS4" + key);
            byte[] kDate = HmacSHA256(kSecret, dateStamp);
            byte[] kRegion = HmacSHA256(kDate, regionName);
            byte[] kService = HmacSHA256(kRegion, serviceName);
            byte[] kSigning = HmacSHA256(kService, "aws4_request");
            return kSigning;
        }
    }
}
