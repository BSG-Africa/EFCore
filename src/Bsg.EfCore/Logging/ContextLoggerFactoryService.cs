namespace Bsg.EfCore.Logging
{
    using System;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Console;

    public class ContextLoggerFactoryService : IContextLoggerFactoryService
    {
        private LoggerFactory loggerFactory;
        private bool isSetupComplete;
        private object lockObj = new object();

        public LoggerFactory GetLoggerFactory()
        {
            if (!this.isSetupComplete)
            {
                this.Setup();
            }

            return this.loggerFactory;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DisposeFactory();
            }
        }

        private void DisposeFactory()
        {
            if (this.isSetupComplete && this.loggerFactory != null)
            {
                this.loggerFactory.Dispose();
                this.loggerFactory = null;
                this.isSetupComplete = false;
            }
        }

        private void Setup()
        {
            if (!this.isSetupComplete)
            {
                lock (this.lockObj)
                {
                    if (!this.isSetupComplete)
                    {
                        this.loggerFactory = new LoggerFactory(new[] { new ConsoleLoggerProvider((category, level) => true, true) });
                        this.isSetupComplete = true;
                    }
                }
            }
        }
    }
}
