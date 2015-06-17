using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Quilt4.Interface;
using Quilt4.Web.Business;

namespace Quilt4.Web.Tests.Business.CounterBusinessTests
{
    [TestFixture]
    internal class CounterBusinessTests
    {
        [Test]
        [Ignore("Ongoing")]
        public void Different_counters_should_never_be_returned()
        {
            var allFakeData = GivenSomeDifferentFakeCounters().ToArray();

            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => allFakeData);

            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            var data = counterBusiness.GetRawData("Session");

            var result = data.GroupBy(x => x.CounterName);
            
            Assert.That(result.Count(), Is.EqualTo(1));
            //Assert.That(!data.Any(x => string.IsNullOrEmpty(x.CounterName)), Is.True, "There are counters without a name");
        }

        private IEnumerable<ICounter> GivenSomeDifferentFakeCounters()
        {
            var fakeSessions = GetSessionDataFake();
            var fakeApplications = GetApplicationDataFake();
            var fakeIssueTypes = GetIssueTypeDataFake();
            var fakeIssues = GetIssueDataFake();
            var customDataFake1 = GetCustomDataDataFake();
            var customDataFake2 = GetCustomDataDataFake();
            var customDataFake3 = GetCustomDataDataFake();
            var allFakeData = fakeSessions.Union(fakeApplications).Union(fakeIssueTypes).Union(fakeIssues).Union(customDataFake1).Union(customDataFake2).Union(customDataFake3).ToArray();
            return allFakeData;
        }

        private IEnumerable<ICounter> GetCustomDataDataFake()
        {
            var rng = new Random();
            var counterName = "Counter_" + rng.Next();

            return Mocks.Of<ICounter>(x => x.CounterName == counterName).Take(1000);
        }

        private IEnumerable<ICounter> GetIssueDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "Issue").Take(1000);
        }

        private IEnumerable<ICounter> GetIssueTypeDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "IssueType").Take(1000);
        }

        private IEnumerable<ICounter> GetApplicationDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "Application").Take(1000);
        }

        private IEnumerable<ICounter> GetSessionDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "Session").Take(1000);
        }
    }
} 