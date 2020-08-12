using System;
using Xunit; 

using Lockbase.CoreDomain.Entities;
using Lockbase.CoreDomain.Extensions;

namespace Lockbase.Tests.CoreDomain {

	public class KeyTest {

		[Fact]
		public void TestKeyCreation() {
			Key entity = new Key("000000hqvs1lo","103-1",null, "Fender, Klaus");
			Assert.Equal("000000hqvs1lo", entity.Id);
			Assert.Equal("103-1", entity.Name);
			Assert.Null(entity.AppId);
			Assert.Equal("Fender, Klaus", entity.ExtData);			
		}

		[Fact]
		public void TestKeyIdMandatory() {
			Assert.Throws<ArgumentNullException>(() => new Key(string.Empty,"103-1",null,"Fender, Klaus"));
			Assert.Throws<ArgumentNullException>(() => new Key(null,"103-1",null,"Fender, Klaus"));
		}
	}
}
