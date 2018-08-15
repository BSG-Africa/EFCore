namespace Bsg.EfCore.Tests.TestCases
{
    using System.Collections.Generic;
    using System.Linq;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using Context;
    using NUnit.Framework;
    using TestInfrastructure;

    public class NonBulkTests : TestBase
    {
        #region Tests
        [Test]
        public void EnsureFindAllReturnsCorrectResult()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();

            var alphas = new List<Alpha>();
            var noOfRecordsToInsert = 100;

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
            var activeAlphas = alphaPrimaryRepo.FindAll(e => e.IsActive).ToList();

            // Assert
            var activeAlphasCount = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(activeAlphas.Count, Is.GreaterThan(0));
            Assert.That(activeAlphas.Count, Is.EqualTo(activeAlphasCount));
        }

        [Test]
        public void EnsureFindAllWithOrderingAndPagingReturnsCorrectResult()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();

            var alphas = new List<Alpha>();
            var noOfRecordsToInsert = 100;

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
            var activeAlphas = alphaPrimaryRepo.FindAll(e => e.IsActive, "Name", "DESC", 2, 1).ToList();

            // Assert
            Assert.That(activeAlphas.Count, Is.EqualTo(2));
            Assert.That(activeAlphas[0].Name, Is.EqualTo("94"));
            Assert.That(activeAlphas[1].Name, Is.EqualTo("92"));
        }

        [Test]
        public void EnsureCountAllReturnsCorrectResult()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();

            var alphas = new List<Alpha>();
            var noOfRecordsToInsert = 100;

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
            var activeAlphasDbCount = alphaPrimaryRepo.CountAll(e => e.IsActive);

            // Assert
            var activeAlphasMemoryCount = alphaPrimaryRepo.FindAll(e => e.IsActive).ToList().Count;
            Assert.That(activeAlphasDbCount, Is.GreaterThan(0));
            Assert.That(activeAlphasDbCount, Is.EqualTo(activeAlphasMemoryCount));
        }

        [Test]
        public void EnsureDeleteAllRemovesAllRecords()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alphas = new List<Alpha>();
            var noOfRecordsToInsert = 100;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                alphas.Add(new Alpha
                {
                    Name = idx.ToString(),
                    IsActive = idx % 2 == 0
                });
            }

            alphaPrimaryRepo.BulkAdd(alphas);
            var activeBefore = alphaPrimaryRepo.CountAll(e => e.IsActive);

            // Assume
            Assert.That(activeBefore, Is.GreaterThan(0));

            // Action
            alphaPrimaryRepo.DeleteAll(e => e.IsActive);
            contextSession.CommitChanges();

            // Assert
            var activeafter = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(activeafter, Is.EqualTo(0));
        }

        #endregion
    }
}
