using System;
using WaterS.Application.Interfaces.Services;

namespace WaterS.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
        public DateTime NowLocal => DateTime.Now;
    }
}