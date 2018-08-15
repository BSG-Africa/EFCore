namespace Bsg.EfCore.Tests.TestCases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using NUnit.Framework;
    using TestInfrastructure;

    public class BulkSelectAndUpdateTests : TestBase
    {
        #region Tests
        [Test]
        public void EnsureBulkSelectAndUpdateModifiedRecords()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var gammaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Gamma>>();

            var gammas = new List<Gamma>();
            var noOfGammaRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfGammaRecordsToInsert; idx++)
            {
                var categoryName = idx % 10 == 0 ? "Category A" : idx % 3 == 0 ? "Category B" : "Other";

                gammas.Add(new Gamma
                {
                    Category = categoryName,
                    Cost = (idx * 1000m) - 100,
                    Price = (idx * 1000m) + idx
                });
            }

            gammaPrimaryRepo.BulkAdd(gammas);

            var alphas = new List<Alpha>
            {
                new Alpha
                {
                    IsActive = false,
                    Name = "Category A"
                },
                new Alpha
                {
                    IsActive = false,
                    Name = "Category B"
                },
            };

            alphaPrimaryRepo.BulkAdd(alphas);

            var gammasProjections =
                gammaPrimaryRepo.FindAll()
                    .GroupBy(e => e.Category)
                    .Select(g => new
                    {
                        Catgeory = g.Key,
                        Profit = g.Sum(e => e.Price - e.Cost)
                    });

            var allAlphas = alphaPrimaryRepo.FindAll();

            var updateAlphaQuery = from gammaProjection in gammasProjections
                join alpha in allAlphas on gammaProjection.Catgeory equals alpha.Name
                select new Alpha
                {
                    Id = alpha.Id,
                    IsActive = gammaProjection.Profit > 100000m
                };

            // Assume
            var activeAlphasBeforeUpdate = alphaPrimaryRepo.CountAll(e => e.IsActive);
            Assert.That(activeAlphasBeforeUpdate, Is.EqualTo(0));

            // Action
            alphaPrimaryRepo.BulkSelectAndUpdate(updateAlphaQuery);

            // Assert
            var activeAlphasAfterUpdate = alphaPrimaryRepo.CountAll(e => e.IsActive);
            var categoryAAlpha = alphaPrimaryRepo.FindOne(e => e.Name == "Category B");
            Assert.That(activeAlphasAfterUpdate, Is.EqualTo(1));
            Assert.That(categoryAAlpha.IsActive, Is.True);
        }

        [Test]
        public void BulkSelectAndUpdateThrowsNotSupportedExceptionWhenUsingDomainTypeForProjection()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            this.CleanPrimarySchema(requestContainer);

            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var gammaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Gamma>>();

            var gammas = new List<Gamma>();
            var noOfGammaRecordsToInsert = 1000;

            for (var idx = 1; idx <= noOfGammaRecordsToInsert; idx++)
            {
                var categoryName = idx % 10 == 0 ? "Category A" : idx % 3 == 0 ? "Category B" : "Other";

                gammas.Add(new Gamma
                {
                    Category = categoryName,
                    Cost = (idx * 1000m) - 100,
                    Price = (idx * 1000m) + idx
                });
            }

            gammaPrimaryRepo.BulkAdd(gammas);

            var alphas = new List<Alpha>
            {
                new Alpha
                {
                    IsActive = false,
                    Name = "Category A"
                },
                new Alpha
                {
                    IsActive = false,
                    Name = "Category B"
                },
            };

            alphaPrimaryRepo.BulkAdd(alphas);

            var gammasProjections =
                gammaPrimaryRepo.FindAll()
                    .GroupBy(e => e.Category)
                    .Select(g => new
                    {
                        Catgeory = g.Key,
                        Profit = g.Sum(e => e.Price - e.Cost)
                    });

            var allAlphas = alphaPrimaryRepo.FindAll();

            var updateAlphaQuery = from gammaProjection in gammasProjections
                                   join alpha in allAlphas on gammaProjection.Catgeory equals alpha.Name
                                   select new Alpha
                                   {
                                       Id = alpha.Id,
                                       IsActive = gammaProjection.Profit > 100000m
                                   };

            // Action
            var result = this.ActionWithException<NotSupportedException>(
               () =>
               {
                   alphaPrimaryRepo.BulkSelectAndUpdate(updateAlphaQuery);
               });

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Message, Is.EqualTo("The entity or complex type 'Bsg.Ef6.Tests.Data.Context.Alpha' cannot be constructed in a LINQ to Entities query."));
        }
        #endregion
    }
}
