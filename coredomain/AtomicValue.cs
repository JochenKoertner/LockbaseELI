using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lockbase.CoreDomain
{
    public class AtomicValue<T>
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
    }
}
