using System;
using Xunit; 

using Lockbase.CoreDomain.Entities; 

namespace Lockbase.Tests.CoreDomain {

	public class LockTest {

		[Fact]
		public void TestLockCreation() {
			Lock entity = new Lock("000000t00nuiu","100",null, "Meeting Room, Administration");
			Assert.Equal("000000t00nuiu", entity.Id);
			Assert.Equal("100", entity.Name);
			Assert.Null(entity.AppId);
			Assert.Equal("Meeting Room, Administration", entity.ExtData);
		}

		[Fact]
		public void TestLockIdMandatory() {
			Assert.Throws<ArgumentNullException>(() => new Lock(string.Empty,"W1",null,"Tor West"));
			Assert.Throws<ArgumentNullException>(() => new Lock(null,"W1",null,"Tor West"));
		}

		[Fact]
		public void TestIdEqualString() {
			Lock entityA = new Lock("000000t00nuiu","W1",null, "Tor West");
			Lock entityB = new Lock("000000t00nuiu","W1d",null, "Torded West");

			Assert.Equal(entityA, entityB);
			Assert.True(entityA.Equals(entityB.Id));
		}
	}
}
