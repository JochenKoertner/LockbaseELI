using System;
using Xunit; 

using Lockbase.CoreDomain.ValueObjects;

namespace Lockbase.ui.UnitTest.CoreDomain {

	public class NumberOfLockingsTest {


		[Fact]
		public void NumberOfLockingsNothing() {
			NumberOfLockings definition = "";
			Assert.Null(definition);
		}

		[Fact]
		public void NumberOfLockingsRange() {
			NumberOfLockings definition = "12-47";
			
			Assert.Equal(12,definition.Minimum); 
			Assert.Equal(47,definition.Maximum);
		}

		[Fact]
		public void NumberOfLockingsSingle() {
			NumberOfLockings definition = "12";
			
			Assert.Equal(12,definition.Minimum); 
			Assert.Equal(12,definition.Maximum);
		}

		[Fact]
		public void NumberOfLockingsOutofRange() {
			Assert.Throws<ArgumentOutOfRangeException>( () => { NumberOfLockings definition = "42-12"; });
		}

	}
}
