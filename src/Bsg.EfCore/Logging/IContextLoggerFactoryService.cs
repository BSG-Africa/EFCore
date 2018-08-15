namespace Bsg.EfCore.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public interface IContextLoggerFactoryService : IDisposable
    {
        LoggerFactory GetLoggerFactory();
    }
}