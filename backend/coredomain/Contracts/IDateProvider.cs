using System;

namespace Lockbase.CoreDomain.Contracts
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
    }
}