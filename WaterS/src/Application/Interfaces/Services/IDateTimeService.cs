using System;

namespace WaterS.Application.Interfaces.Services
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTime NowLocal { get; }
    }
}