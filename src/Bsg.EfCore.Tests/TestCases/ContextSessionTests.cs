namespace Bsg.EfCore.Tests.TestCases
{
    using Bsg.EfCore.Tests.Data.Context;
    using Bsg.EfCore.Tests.Data.Domain;
    using Bsg.EfCore.Tests.Data.Repo;
    using Container;
    using Context;
    using NUnit.Framework;
    using TestInfrastructure;

    public class ContextSessionTests : TestBase
    {
        [Test]
        public void EnsureSessionSharesChangesWithRepositories()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            // Assume
            Assert.That(contextSession.HasChanges(), Is.False);

            // Action
            alphaPrimaryRepo.AddOne(alpha);

            // Assert
            Assert.That(contextSession.HasChanges(), Is.True);
        }

        [Test]
        public void EnsureSessionDoesNotShareChangesWithOtherScopedSession()
        {
            // Arrange
            var requestContainer1 = this.BuildRequestContainer();
            var requestContainer2 = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer1.GetService<IPrimaryRepository<Alpha>>();
            var contextSession1 = requestContainer1.GetService<IDbContextSession<PrimaryContext>>();
            var contextSession2 = requestContainer2.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            // Assume
            Assert.That(contextSession1.HasChanges(), Is.False);
            Assert.That(contextSession2.HasChanges(), Is.False);
            Assert.That(contextSession1, Is.Not.SameAs(contextSession2));

            // Action
            alphaPrimaryRepo.AddOne(alpha);

            // Assert
            Assert.That(contextSession1.HasChanges(), Is.True);
            Assert.That(contextSession2.HasChanges(), Is.False);
        }

        [Test]
        public void EnsureCommitChangesDoesNotUpdateChangedNonTrackedEntries()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            alphaPrimaryRepo.AddOne(alpha);
            contextSession.CommitChanges();

            var untrackAlpha = alphaPrimaryRepo.FindOne(e => e.Id == alpha.Id);

            // Assume
            Assert.That(contextSession.HasChanges(), Is.False);
            untrackAlpha.Name = "Changed Name";
            Assert.That(contextSession.HasChanges(), Is.False);

            // Action
            contextSession.CommitChanges();

            // Assert
            var reRetrievedAlpha = alphaPrimaryRepo.FindOne(e => e.Id == alpha.Id);
            Assert.That(reRetrievedAlpha.Name, Is.EqualTo("Some Alpha"));
        }

        [Test]
        public void EnsureCommitChangesUpdatesChangedTrackedEntries()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            alphaPrimaryRepo.AddOne(alpha);
            contextSession.CommitChanges();

            var untrackAlpha = alphaPrimaryRepo.FindOneTracked(e => e.Id == alpha.Id);

            // Assume
            Assert.That(contextSession.HasChanges(), Is.False);
            untrackAlpha.Name = "Changed Name";
            Assert.That(contextSession.HasChanges(), Is.True);

            // Action
            contextSession.CommitChanges();

            // Assert
            var reRetrievedAlpha = alphaPrimaryRepo.FindOne(e => e.Id == alpha.Id);
            Assert.That(reRetrievedAlpha.Name, Is.EqualTo("Changed Name"));
        }

        public void EnsureCommitChangesDoesNotUpdateRevertedChangedTrackedEntries()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha"
            };

            alphaPrimaryRepo.AddOne(alpha);
            contextSession.CommitChanges();

            var untrackAlpha = alphaPrimaryRepo.FindOneTracked(e => e.Id == alpha.Id);

            // Assume
            Assert.That(contextSession.HasChanges(), Is.True);
            untrackAlpha.Name = "Changed Name";

            // Action
            contextSession.RevertChanges();
            contextSession.CommitChanges();

            // Assert
            Assert.That(contextSession.HasChanges(), Is.False);
            var reRetrievedAlpha = alphaPrimaryRepo.FindOne(e => e.Id == alpha.Id);
            Assert.That(reRetrievedAlpha.Name, Is.EqualTo("Some Alpha"));
        }

        public void EnsureRevertChangesDoesNotPreventFutureCommits()
        {
            // Arrange
            var requestContainer = this.BuildRequestContainer();
            var alphaPrimaryRepo = requestContainer.GetService<IPrimaryRepository<Alpha>>();
            var contextSession = requestContainer.GetService<IDbContextSession<PrimaryContext>>();

            var alpha1 = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha 1"
            };

            var alpha2 = new Alpha
            {
                IsActive = true,
                Name = "Some Alpha 2"
            };

            alphaPrimaryRepo.AddOne(alpha1);
            alphaPrimaryRepo.AddOne(alpha2);
            contextSession.CommitChanges();

            var untrackAlpha1 = alphaPrimaryRepo.FindOneTracked(e => e.Id == alpha1.Id);
            untrackAlpha1.Name = "Changed Name 1";
            contextSession.RevertChanges();

            var untrackAlpha2 = alphaPrimaryRepo.FindOneTracked(e => e.Id == alpha1.Id);
            untrackAlpha2.Name = "Changed Name 2";

            // Action
            contextSession.CommitChanges();

            // Assert
            Assert.That(contextSession.HasChanges(), Is.False);
            var reRetrievedAlpha = alphaPrimaryRepo.FindOne(e => e.Id == alpha2.Id);
            Assert.That(reRetrievedAlpha.Name, Is.EqualTo("Changed Name 2"));
        }
    }
}
