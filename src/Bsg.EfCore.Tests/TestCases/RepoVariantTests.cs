namespace Bsg.EfCore.Tests.TestCases
{
    using System.Collections.Generic;
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using Context;
    using NUnit.Framework;
    using Repo;
    using Services;
    using TestInfrastructure;

    public class RepoVariantTests : TestBase
    {
        #region Tests
        [Test]
        public void EnsureThatAll3VariantsOfReposQuerySameTableCorrectly()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaGenericRepo = requestContainer.GetService<IGenericRepository<Alpha, PrimaryContext>>();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var alphaRepo = requestContainer.GetService<IAlphaRepository>();

            // Assume
            var firstcount = alphaGenericRepo.CountAll();
            Assert.That(firstcount, Is.EqualTo(alphaPrimaryRepo.CountAll()));
            Assert.That(firstcount, Is.EqualTo(alphaRepo.CountAll()));

            // Action
            var alphas = new List<Alpha>();
            var noOfRecordsToInsert = 10000;

            for (var idx = 1; idx <= noOfRecordsToInsert; idx++)
            {
                alphas.Add(new Alpha
                {
                    Name = idx.ToString(),
                    IsActive = idx % 2 == 0
                });
            }

            alphaRepo.BulkAdd(alphas);

            // Assert
            var secondcount = alphaGenericRepo.CountAll();
            Assert.That(firstcount + noOfRecordsToInsert, Is.EqualTo(secondcount));
            Assert.That(secondcount, Is.EqualTo(alphaPrimaryRepo.CountAll()));
            Assert.That(secondcount, Is.EqualTo(alphaRepo.CountAll()));
        }

        [Test]
        public void EnsureThatAll3VariantsOfReposTrackSameChanges()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaGenericRepo = requestContainer.GetService<IGenericRepository<Alpha, PrimaryContext>>();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var alphaRepo = requestContainer.GetService<IAlphaRepository>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            alphaPrimaryRepo.AddOne(alpha);
            contextSession.CommitChanges();

            var genericTracked = alphaGenericRepo.FindOneTracked(e => e.Id == alpha.Id);

            // Assume
            Assert.That(alpha.Id, Is.GreaterThan(0));

            // Action
            var changed = "Changed";
            genericTracked.Name = changed;

            // Assert
            var primaryTracked = alphaPrimaryRepo.FindOneTracked(e => e.Id == alpha.Id);
            var alphaTracked = alphaRepo.FindOneTracked(e => e.Id == alpha.Id);
            Assert.That(primaryTracked.Name, Is.EqualTo(changed));
            Assert.That(alphaTracked.Name, Is.EqualTo(changed));
        } 
        #endregion
    }
}
