using System;
using System.Threading;

namespace Lockbase.CoreDomain
{
    public class AtomicValue<T> where T : class
	{
		private T state;

		public AtomicValue(T seed)
		{
			this.state = seed;
		}

		public static implicit operator T(AtomicValue<T> value)
		{
			return value.state;
		}

		public void SetValue(Func<T, T> func)
		{
			T oldValue;
			T newValue;
			do
			{
				oldValue = this.state;
				newValue = func(oldValue);
			} while (Interlocked.CompareExchange<T>(ref this.state, newValue, oldValue) != oldValue);
		}
	}
}
