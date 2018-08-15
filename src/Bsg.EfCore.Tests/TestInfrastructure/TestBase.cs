namespace Bsg.EfCore.Tests.TestInfrastructure
{
    using System;
    using Autofac;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using Context;
    using NUnit.Framework;

    [TestFixture]
    public class TestBase
    {
        private ILifetimeScope applicationContainer;

        [OneTimeSetUp]
        public void Setup()
        {
            this.applicationContainer = new TestContainerBootstrap().BuildAutofacContainer();

            var contextFactory = this.BuildRequestContainer().GetService<IDbContextFactory>();
            var primaryConext = contextFactory.BuildContext<PrimaryContext>();
            primaryConext.Database.EnsureDeleted();
            primaryConext.Database.EnsureCreated();

            var secondaryConext = contextFactory.BuildContext<SecondaryContext>();
            secondaryConext.Database.EnsureDeleted();
            secondaryConext.Database.EnsureCreated();
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            var contextFactory = this.BuildRequestContainer().GetService<IDbContextFactory>();
            var primaryConext = contextFactory.BuildContext<PrimaryContext>();
            primaryConext.Database.EnsureDeleted();

            var secondaryConext = contextFactory.BuildContext<SecondaryContext>();
            secondaryConext.Database.EnsureDeleted();

            this.applicationContainer = null;
        }

        protected ILifetimeScope BuildRequestContainer()
        {
            return this.applicationContainer.CreateScopedContainer();
        }

        protected TException ActionWithException<TException>(Action action) 
            where TException : Exception
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            try
            {
                action();
            }
            catch (TException exception)
            {
                return exception;
            }

            return null;
        }

        protected void CleanPrimarySchema(ILifetimeScope requestContainer)
        {
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var betaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Beta>>();
            var gammaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Gamma>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            using (var transaction = contextSession.StartNewTransaction())
            {
                betaPrimaryRepo.Truncate(transaction);
                gammaPrimaryRepo.Truncate(transaction);
                alphaPrimaryRepo.TruncateWithForeignKeys(transaction);
                transaction.Commit();
            }
        }
    }
}
