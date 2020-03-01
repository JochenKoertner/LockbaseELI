using System;
using System.Collections.Generic;
using System.Linq;
using Lockbase.CoreDomain.Contracts;

namespace Lockbase.CoreDomain.Services 
{

	using Bytes = IEnumerable<byte>;

	public class Id {
        private readonly IDateTimeProvider timeProvider;

		private readonly Lazy<DateTime> unixTime = 
			new Lazy<DateTime>( () => new DateTime(1970, 1, 1));

        public Id(IDateTimeProvider timeProvider)
		{
            this.timeProvider = timeProvider;
        }

		public string NewId(Int32 tblId, Int32 ItemId) 
		{
			var left = ItemId ^ (tblId << 24);
			var right = (Int32)(this.timeProvider.Now.Subtract(unixTime.Value)).TotalSeconds;
			var value = (long)left << 32 | (long)right;
			return value.Base32HexEncodeShort();

			// Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(unixTime.Value).TotalSeconds);
			// 2A000003 A005FD2F; 2684747055  // 3026418965162622255
			// 0000000B 0E0C045C;  235668572

			// 580000t00nuiu -> 2A000003 A005FD2F  2684747055
			// 000002oe1g25o -> 0000000B 0E0C045C  235668572

			// 1582963956 => 29.2.2020 08:12:36 => 0x5E5A1CF4
			// 580000QUB8EF8 -> 2A0000035E5A1CF4 => 719680000
		}

	}

	public static class Base32HexExtension {

		const string ALPHA = "0123456789ABCDEFGHIJKLMNOPQRSTUV=";
		const byte MASK = 0b11111;
		public static string Base32HexEncode(this long x) 
		=> x.Base32Hex().AppendPostfix().Format();
		public static string Base32HexEncodeShort(this long x) 
		=> x.Base32Hex().Format().ToLower();

		internal static Bytes Base32Hex(this long x)
		=> Enumerable.Range(0,64/5)
			.Select( i =>  64 - ((i+1)*5))
			.Aggregate( Enumerable.Empty<byte>(), 
				(accu, shift) => accu.Append((byte)((x >> shift) & MASK)))
			.Append( (byte)(x << 1 & MASK));

		internal static Bytes AppendPostfix(this Bytes bytes) 
		=> bytes.Concat(Enumerable.Repeat((byte)(ALPHA.Length-1),(64%40)/8));

		internal static string Format(this Bytes seq) =>  String.Concat(seq.Select( v => ALPHA[v]));
	}
}