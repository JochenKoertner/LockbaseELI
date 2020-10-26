using System;
using Lockbase.CoreDomain.Services;
using Xunit; 
using FakeItEasy;
using Lockbase.CoreDomain.Contracts;
using Lockbase.CoreDomain;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.Tests.CoreDomain {

	public class IdTest {
		
		[Fact]
		public void TestBase32hexEncode() {
			var actual = 0x2A000003A005FD2F.Base32HexEncode();
			Assert.Equal("580000T00NUIU===", actual);
		}

		[Fact]
		public void TestBase32hexEncodeShort() {
			var actual = 0x2A000003A005FD2F.Base32HexEncodeShort();
			Assert.Equal("580000t00nuiu", actual);
		}

		[Theory]
		[InlineData(0x07, TableIds.Lock, "2020-01-23T16:21:05",  "0s0000vhoskls")]
		[InlineData(0x09, TableIds.Key, "2020-01-23T16:14:35",   "140000jbookls")]
		[InlineData(0x03, TableIds.Event, "2020-02-29T08:12:36", "0c0001nk3hd5s")]
		public void TestGivenSamples(int itemId, TableIds tableId, string time, string expected)
		{		
			var subject = new Id( time.FakeNow());
			var act = subject.NewId( tableId, itemId);

			Assert.Equal(expected, act);
		}

		[Theory]
		[InlineData("2020-01-23T16:21:05", 0x5e29c7f1)]
		[InlineData("2020-01-23T16:14:35", 0x5e29c66b)]
		public void TestUnixTime(string time, int expected) {
			var act = Convert.ToDateTime(time).UnixTime();
			Assert.Equal(expected, act);
		}

	}

	public static class TestExtensions {
		public static IDateTimeProvider FakeNow(this string time) 
		{
			var stamp = Convert.ToDateTime(time);

			var dateProvider = A.Fake<IDateTimeProvider>();
			A.CallTo( () => dateProvider.Now).Returns(stamp);

			return dateProvider;
		}
	}
}