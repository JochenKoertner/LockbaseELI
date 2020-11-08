namespace Lockbase.CoreDomain.Extensions
{
    public static class IntExtensions
    {
       public static int SwapEndianness(this int value)
		{
			var b1 = (value >> 0) & 0xff;
			var b2 = (value >> 8) & 0xff;
			var b3 = (value >> 16) & 0xff;
			var b4 = (value >> 24) & 0xff;

			return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
		}

		public static string ToHex(this int value) => value.ToString("X8");
    }
}