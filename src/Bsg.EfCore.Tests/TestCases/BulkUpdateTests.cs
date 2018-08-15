namespace Bsg.EfCore.Tests.TestCases
{
    using System.Collections.Generic;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using NUnit.Framework;
    using TestInfrastructure;

    public class BulkUpdateTests : TestBase
    {
        #region Tests
        [Test]
        public void EnsureBulkDeleteRemovesRecordsUsingIQueryable()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();

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

            var alphasToUpdateQuery = alphaPrimaryRepo.FindAll(e => e.IsActive);

            // Assume
            var activeAlphasBeforeUpdate = alphaPrimaryRepo.CountAll(e => e.IsActive);
            var allAlphasBeforeUpdate = alphaPrimaryRepo.CountAll();
            Assert.That(activeAlphasBeforeUpdate, Is.GreaterThan(0));
            Assert.That(allAlphasBeforeUpdate, Is.GreaterThan(0));

            // Action
            var recordsUpdated = alphaPrimaryRepo.BulkUpdate(alphasToUpdateQuery, e => new Alpha { IsActive = false });

            // Assert
            var activeAlphasAfterUpdate = alphaPrimaryRepo.CountAll(e => e.IsActive);
            var allAlphasAfterUpdate = alphaPrimaryRepo.CountAll();
            Assert.That(activeAlphasAfterUpdate, Is.EqualTo(0));
            Assert.That(recordsUpdated, Is.EqualTo(activeAlphasBeforeUpdate));
            Assert.That(allAlphasAfterUpdate, Is.EqualTo(allAlphasBeforeUpdate));
        }

        [Test]
        public void EnsureBulkDeleteRemovesRecordsUsingExpression()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();

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
            var activeAlphasBeforeUpdate = alphaPrimaryRepo.CountAll(e => e.IsActive);
            var allAlphasBeforeUpdate = alphaPrimaryRepo.CountAll();
            Assert.That(activeAlphasBeforeUpdate, Is.GreaterThan(0));
            Assert.That(allAlphasBeforeUpdate, Is.GreaterThan(0));

            // Action
            var recordsUpdated = alphaPrimaryRepo.BulkUpdate(e => e.IsActive, e => new Alpha { IsActive = false });

            // Assert
            var activeAlphasAfterUpdate = alphaPrimaryRepo.CountAll(e => e.IsActive);
            var allAlphasAfterUpdate = alphaPrimaryRepo.CountAll();
            Assert.That(activeAlphasAfterUpdate, Is.EqualTo(0));
            Assert.That(recordsUpdated, Is.EqualTo(activeAlphasBeforeUpdate));
            Assert.That(allAlphasAfterUpdate, Is.EqualTo(allAlphasBeforeUpdate));
        }
        #endregion
    }
}
