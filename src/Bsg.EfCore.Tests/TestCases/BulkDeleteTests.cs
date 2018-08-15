namespace Bsg.EfCore.Tests.TestCases
{
    using System.Collections.Generic;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using NUnit.Framework;
    using TestInfrastructure;

    public class BulkDeleteTests : TestBase
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

            var alphasToDeleteQuery = alphaPrimaryRepo.FindAll(e => e.IsActive);

            // Assume
            var activeAlphasBeforeDelete = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(activeAlphasBeforeDelete, Is.GreaterThan(0));

            // Action
            var recordsDeleted = alphaPrimaryRepo.BulkDelete(alphasToDeleteQuery);

            // Assert
            var activeAlphasAfterDelete = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(activeAlphasAfterDelete, Is.EqualTo(0));
            Assert.That(recordsDeleted, Is.EqualTo(activeAlphasBeforeDelete));
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
            var activeAlphasBeforeDelete = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(activeAlphasBeforeDelete, Is.GreaterThan(0));

            // Action
            var recordsDeleted = alphaPrimaryRepo.BulkDelete(e => e.IsActive);

            // Assert
            var activeAlphasAfterDelete = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(activeAlphasAfterDelete, Is.EqualTo(0));
            Assert.That(recordsDeleted, Is.EqualTo(activeAlphasBeforeDelete));
        } 
        #endregion
    }
}
