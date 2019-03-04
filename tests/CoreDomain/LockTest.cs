using System;
using Xunit; 

using Lockbase.CoreDomain.Entities; 

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class LockTest {

		[Fact]
		public void TestLockCreation() {
			Lock entity = new Lock("000000t00nuiu","W1",null, "Tor West");
			Assert.Equal("000000t00nuiu", entity.Id);
			Assert.Equal("W1", entity.Name);
			Assert.Null(entity.AppId);
			Assert.Equal("Tor West", entity.ExtData);
		}

		[Fact]
		public void TestLockIdMandatory() {
			Assert.Throws<ArgumentNullException>(() => new Lock(string.Empty,"W1",null,"Tor West"));
			Assert.Throws<ArgumentNullException>(() => new Lock(null,"W1",null,"Tor West"));
		}
	}
}
