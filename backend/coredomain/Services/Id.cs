using System;
using System.Collections.Generic;
using System.Linq;
using Lockbase.CoreDomain.Contracts;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.CoreDomain.Services 
{
	using Bytes = IEnumerable<byte>;

	public enum TableIds {
		Lock = 0x03,
		Key = 0x02,
		Event = 0x06 
	}

	public class Id {
        private readonly IDateTimeProvider timeProvider;

        public Id(IDateTimeProvider timeProvider)
		{
            this.timeProvider = timeProvider;
        }

		public string NewId(TableIds tblId, Int32 ItemId) 
		{
			var left = (ItemId ^ ((Int32)tblId << 24)).SwapEndianness();
			var right = this.timeProvider.Now.UnixTime().SwapEndianness();
			var value = (long)left << 32 | (uint)right;
			return value.Base32HexEncodeShort();
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