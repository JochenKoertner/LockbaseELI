using System;
using Lockbase.CoreDomain.Services;
using Xunit; 
using FakeItEasy;
using Lockbase.CoreDomain.Contracts;

namespace Lockbase.Tests.CoreDomain {

	public class IdTest {

		[Fact]
		public void TestNewId() {

			var dateProvider = A.Fake<IDateTimeProvider>();
			A.CallTo( () => dateProvider.Now).Returns(new DateTime(2020,2,29, 8,12,36));

			var subject = new Id( dateProvider);
			var act = subject.NewId( 0x2a, 0x03);

			Assert.Equal("580000qub8ef8", act);
		}

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
	}
}
