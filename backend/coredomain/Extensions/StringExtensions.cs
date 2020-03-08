using System.Text;

namespace Lockbase.CoreDomain.Extensions
{
    public static class StringExtensions
    {
        public static string FromBase64(this string base64Encoded) {
            if (string.IsNullOrEmpty(base64Encoded))
                return base64Encoded;
            byte[] data = System.Convert.FromBase64String(base64Encoded);
            return Encoding.UTF8.GetString(data);
        }

        public static string RemoveTrailingZero(this string value) {
            if (string.IsNullOrEmpty(value))
                return value;
             if (value.LastIndexOf('\0')==(value.Length-1))
                value = value.Remove(value.Length - 1);
            return value;
        }

		public static int FromHex(this string hexValue) =>
			int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
    }
}