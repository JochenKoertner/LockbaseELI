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
	}
}
