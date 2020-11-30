using System.Linq;
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

		public static string ToBase64(this string value)
		=> System.Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
		
        public static string RemoveTrailingZero(this string value) {
            if (string.IsNullOrEmpty(value))
                return value;
             if (value.LastIndexOf('\0')==(value.Length-1))
                value = value.Remove(value.Length - 1);
            return value;
        }

		public static string Shorten(this string value, int maxLen = 13, int takeLen = 5)
		=> value.Length <= maxLen ? value :
			string.Concat(value.Take(takeLen)
            .Concat(Enumerable.Repeat('.', 3))
            .Concat(value.TakeLast(takeLen)));

		public static int FromHex(this string hexValue) =>
			int.Parse(hexValue, System.Globalization.NumberStyles.HexNumber);
    }
}