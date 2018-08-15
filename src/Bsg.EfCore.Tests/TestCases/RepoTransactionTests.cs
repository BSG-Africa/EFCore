namespace Bsg.EfCore.Tests.TestCases
{
    using System.Collections.Generic;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using Context;
    using NUnit.Framework;
    using TestInfrastructure;

    public class RepoTransactionTests : TestBase
    {
        [Test]
        public void EnsureBulkDeleteDeletesWhenUsingExternalTransaction()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var gammaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Gamma>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var gammas = new List<Gamma>();
            var noOfRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                gammas.Add(new Gamma
                {
                    Category = "Some Category",
                    Cost = 1,
                    Price = 1
                });
            }

            gammaPrimaryRepo.BulkAdd(gammas);

            // Assume
            Assert.That(gammaPrimaryRepo.CountAll(), Is.GreaterThanOrEqualTo(noOfRecordsToInsert));

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                gammaPrimaryRepo.BulkDelete(g => true, transaction);
                transaction.Commit();
            }

            // Assert
            Assert.That(gammaPrimaryRepo.CountAll(), Is.EqualTo(0));
        }

        [Test]
        public void EnsureBulkAddInsertsWhenUsingExternalTransaction()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);
            var gammaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Gamma>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var gammas = new List<Gamma>();
            var noOfRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                gammas.Add(new Gamma
                {
                    Category = "Some Category",
                    Cost = 1,
                    Price = 1
                });
            }

            // Assume
            Assert.That(gammaPrimaryRepo.CountAll(), Is.EqualTo(0));

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                gammaPrimaryRepo.BulkAdd(gammas, transaction);
                transaction.Commit();
            }

            // Assert
            Assert.That(gammaPrimaryRepo.CountAll(), Is.EqualTo(noOfRecordsToInsert));
        }

        [Test]
        public void EnsureCommitUpdatesWhenUsingExternalTransaction()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);
            var gammaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Gamma>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var gammas = new List<Gamma>();
            var noOfRecordsToInsert = 1;
            var someCategory = "Some Category";
            var otherCategory = "other Category";

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                gammas.Add(new Gamma
                {
                    Category = someCategory,
                    Cost = 1,
                    Price = 1
                });
            }

            gammaPrimaryRepo.BulkAdd(gammas);

            // Assume
            Assert.That(gammaPrimaryRepo.CountAll(e => e.Category == someCategory), Is.EqualTo(1));
            Assert.That(gammaPrimaryRepo.CountAll(e => e.Category == otherCategory), Is.EqualTo(0));

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                var item = gammaPrimaryRepo.FindOneTracked(e => e.Category == someCategory);
                item.Category = "Other Category";
                contextSession.CommitChanges();
                transaction.Commit();
            }

            // Assert
            Assert.That(gammaPrimaryRepo.CountAll(e => e.Category == someCategory), Is.EqualTo(0));
            Assert.That(gammaPrimaryRepo.CountAll(e => e.Category == otherCategory), Is.EqualTo(1));
        }

        [Test]
        public void EnsureHasCurrentTransactionReturnsFalseWhenNoTransactionCreated()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            // Assert
            Assert.That(contextSession.HasCurrentTransaction(), Is.False);
        }

        [Test]
        public void EnsureHasCurrentTransactionReturnsTrueWhenTransactionStarted()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();
            bool hasCurrentTransaction;

            using (var transaction = contextSession.StartNewTransaction())
            {
                hasCurrentTransaction = contextSession.HasCurrentTransaction();
            }

            // Assert
            Assert.That(hasCurrentTransaction, Is.True);
        }

        [Test]
        public void EnsureHasCurrentTransactionReturnsFalseWhenCommitted()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            using (var transaction = contextSession.StartNewTransaction())
            {
                transaction.Commit();
            }

            var hasCurrentTransaction = contextSession.HasCurrentTransaction();

            // Assert
            Assert.That(hasCurrentTransaction, Is.False);
        }

        [Test]
        public void EnsureBulkDeleteDeletesWhenUsingExistingTransactionWithoutPassingItIntoRepo()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var gammaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Gamma>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var gammas = new List<Gamma>();
            var noOfRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                gammas.Add(new Gamma
                {
                    Category = "Some Category",
                    Cost = 1,
                    Price = 1
                });
            }

            gammaPrimaryRepo.BulkAdd(gammas);

            // Assume
            Assert.That(gammaPrimaryRepo.CountAll(), Is.GreaterThanOrEqualTo(noOfRecordsToInsert));

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                gammaPrimaryRepo.BulkDelete(g => true);
                transaction.Commit();
            }

            // Assert
            Assert.That(gammaPrimaryRepo.CountAll(), Is.EqualTo(0));
        }
    }
}
