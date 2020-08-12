using System;
using Lockbase.CoreDomain.Contracts;

namespace Lockbase.ui.Resources
{
    public class DateTimeProvider : IDateTimeProvider
    {
        DateTime IDateTimeProvider.Now => DateTime.Now;
    }
}