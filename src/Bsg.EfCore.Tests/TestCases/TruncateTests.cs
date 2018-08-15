namespace Bsg.EfCore.Tests.TestCases
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using Context;
    using NUnit.Framework;
    using TestInfrastructure;

    public class TruncateTests : TestBase
    {
        #region Tests
        [Test]
        public void EnsureTruncateRemovesAllRecords()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var betaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Beta>>();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            alphaPrimaryRepo.AddOne(alpha);
            contextSession.CommitChanges();
            Assert.That(alpha.Id, Is.GreaterThan(0));

            var betas = new List<Beta>();
            var noOfRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                betas.Add(new Beta
                {
                    Code = idx.ToString(),
                    AlphaId = alpha.Id
                });
            }

            betaPrimaryRepo.BulkAdd(betas);

            // Assume
            var betasBeforeTruncate = betaPrimaryRepo.CountAll();
            Assert.That(betasBeforeTruncate, Is.GreaterThan(0));

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                betaPrimaryRepo.Truncate(transaction);
                transaction.Commit();
            }

            // Assert
            var betasAfterTruncate = betaPrimaryRepo.CountAll();
            Assert.That(betasAfterTruncate, Is.EqualTo(0));
        }

        [Test]
        public void EnsureTruncateReseedsIdFrom1AfterTruncate()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var betaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Beta>>();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            alphaPrimaryRepo.AddOne(alpha);
            contextSession.CommitChanges();
            Assert.That(alpha.Id, Is.GreaterThan(0));

            var betas = new List<Beta>();
            var noOfRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                betas.Add(new Beta
                {
                    Code = idx.ToString(),
                    AlphaId = alpha.Id
                });
            }

            betaPrimaryRepo.BulkAdd(betas);

            // Assume
            var betasBeforeTruncate = betaPrimaryRepo.CountAll();
            Assert.That(betasBeforeTruncate, Is.GreaterThan(0));

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                betaPrimaryRepo.Truncate(transaction);
                transaction.Commit();
            }

            betas = new List<Beta>
            {
                new Beta
                {
                    Code = "XYZ",
                    AlphaId = alpha.Id
                }
            };

            betaPrimaryRepo.BulkAdd(betas);

            // Assert
            var betasAfterTruncateAndOneAdd = betaPrimaryRepo.CountAll();
            var onlyBetaPrimaryRepoId = betaPrimaryRepo.FindOne().Id;

            Assert.That(betasAfterTruncateAndOneAdd, Is.EqualTo(1));
            Assert.That(onlyBetaPrimaryRepoId, Is.EqualTo(1));
        }

        [Test]
        public void EnsureTruncateWithForeignKeyConstraintsRemovesAllRecords()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alphas = new List<Alpha>();
            var noOfRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                alphas.Add(new Alpha
                {
                    Name = idx.ToString(),
                    IsActive = idx % 2 == 0
                });
            }

            alphaPrimaryRepo.BulkAdd(alphas);

            // Assume
            var alphasBeforeTruncate = alphaPrimaryRepo.CountAll();
            Assert.That(alphasBeforeTruncate, Is.GreaterThan(0));

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                alphaPrimaryRepo.TruncateWithForeignKeys(transaction);
                transaction.Commit();
            }

            // Assert
            var alphasAfterTruncate = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(alphasAfterTruncate, Is.EqualTo(0));
        }

        [Test]
        public void EnsureTruncateWithForeignKeyConstraintsReseedsIdFrom1AfterTruncate()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alphas = new List<Alpha>();
            var noOfRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                alphas.Add(new Alpha
                {
                    Name = idx.ToString(),
                    IsActive = idx % 2 == 0
                });
            }

            alphaPrimaryRepo.BulkAdd(alphas);

            // Action
            using (var transaction = contextSession.StartNewTransaction())
            {
                alphaPrimaryRepo.TruncateWithForeignKeys(transaction);
                transaction.Commit();
            }

            alphas = new List<Alpha>
            {
                new Alpha
                {
                    Name = "After Truncate",
                    IsActive = true
                }
            };

            alphaPrimaryRepo.BulkAdd(alphas);

            // Assert
            var alphasAfterTruncateAndOneAdd = alphaPrimaryRepo.CountAll();
            var onlyAlphaId = alphaPrimaryRepo.FindOne().Id;

            Assert.That(alphasAfterTruncateAndOneAdd, Is.EqualTo(1));
            Assert.That(onlyAlphaId, Is.EqualTo(1));
        }

        [Test]
        public void TruncateWillThrowSqlExceptionWhenTableHasForeignKeyConstraints()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            // Action
            var result = this.ActionWithException<SqlException>(() =>
            {
                using (var transaction = contextSession.StartNewTransaction())
                {
                    alphaPrimaryRepo.Truncate(transaction);
                }
            });

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo("Cannot truncate table 'Test.Alpha' because it is being referenced by a FOREIGN KEY constraint."));
        }
        #endregion
    }
}
