using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Quilt4.BusinessEntities;
using Quilt4.Interface;
using Quilt4.Web.Business;

namespace Quilt4.Web.Tests.Business.CounterBusinessTests
{
    [TestFixture]
    internal class CounterBusinessTests
    {
        [Test]
        public void Different_counters_should_never_be_returned()
        {
            //Arrange
            var allFakeData = GivenSomeDifferentFakeCounters().ToArray();

            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns((string counterName) => allFakeData.Where(x => x.CounterName == counterName));

            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetRawData("Session").ToArray();

            //Assert
            var result = data.GroupBy(x => x.CounterName);
            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Raw_data_should_not_grouped()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1),
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetRawData("Session").ToArray();

            //Assert
            Assert.That(data.Count(), Is.EqualTo(2));
            Assert.That(data.All(x => x.Count == 1), Is.True);
            Assert.That(data.Sum(x => x.Count), Is.EqualTo(2));
        }

        [Test]
        public void Aggregated_data_should_be_grouped()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1 && x.Environment == "A" && x.Level == "X"),
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1 && x.Environment == "B" && x.Level == "Y"),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(1) && x.Count == 1 && x.Environment == "A"),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount("Session", Precision.Ticks, x => new { x.Environment });

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(),Is.EqualTo(2));
            Assert.That(data.Lines.Sum(x => x.Counts.First()), Is.EqualTo(3));
            Assert.That(data.Lines.Sum(x => x.Counts.Sum()), Is.EqualTo(data.Lines.Sum(x => x.Counts.First()) * 2));
            Assert.That(data.Names.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Aggregated_data_should_be_grouped_when_environments_differs()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1 && x.Environment == "A" && x.Level == "X"),
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1 && x.Environment == "B" && x.Level == "Y"),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(1) && x.Count == 1 && x.Environment == "A"),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session");

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(),Is.EqualTo(2));
            Assert.That(data.Lines.Sum(x => x.Counts.First()), Is.EqualTo(3));
            Assert.That(data.Names.Count(), Is.EqualTo(1));
        }

        [Test]
        public void Aggregated_when_using_ticks()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddTicks(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddSeconds(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMinutes(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddHours(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMonths(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddYears(2)),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session", Precision.Ticks);

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(8));
        }

        [Test]
        public void Aggregated_when_using_seconds()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddTicks(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddSeconds(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMinutes(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddHours(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMonths(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddYears(2)),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session", Precision.Seconds);

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(7));
        }

        [Test]
        public void Aggregated_when_using_minutes()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddTicks(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddSeconds(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMinutes(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddHours(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMonths(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddYears(2)),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session", Precision.Minutes);

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(6));
        }

        [Test]
        public void Aggregated_when_using_hours()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddTicks(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddSeconds(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMinutes(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddHours(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMonths(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddYears(2)),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session", Precision.Hours);

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(5));
        }

        [Test]
        public void Aggregated_when_using_days()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddTicks(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddSeconds(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMinutes(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddHours(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMonths(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddYears(2)),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session", Precision.Days);

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(4));
        }

        [Test]
        public void Aggregated_when_using_months()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddTicks(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddSeconds(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMinutes(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddHours(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMonths(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddYears(2)),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session", Precision.Months);

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Aggregated_when_using_years()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddTicks(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddSeconds(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMinutes(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddHours(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddMonths(2)),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddYears(2)),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount<string>("Session", Precision.Years);

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(2));
        }

        [Test]
        public void Aggregated_data_should_not_be_grouped_when_environments_differs_and_the_separation_is_requested()
        {
            //Arrange
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var now = DateTime.UtcNow;
            var fakeData = new List<ICounter>
                               {
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1 && x.Environment == "A" && x.Level == "X"),
                                   Mock.Of<ICounter>(x => x.DateTime == now && x.Count == 1 && x.Environment == "B" && x.Level == "Y"),
                                   Mock.Of<ICounter>(x => x.DateTime == now.AddDays(1) && x.Count == 1 && x.Environment == "A"),
                               };
            repositoryMock.Setup(x => x.GetAllCounters("Session")).Returns(() => fakeData);
            var counterBusiness = new CounterBusiness(repositoryMock.Object);

            //Act
            var data = counterBusiness.GetAggregatedCount("Session", Precision.Ticks, x => new { x.Environment, x.Level });

            //var skv = data.ToSkv();

            //Assert
            Assert.That(data.Lines.Count(), Is.EqualTo(2));
            Assert.That(data.Lines.Sum(x => x.Counts.First()), Is.EqualTo(3));
            Assert.That(data.Lines.Sum(x => x.Counts.Sum()), Is.EqualTo(data.Lines.Sum(x => x.Counts.First()) * 2));
            Assert.That(data.Names.Count(), Is.EqualTo(4));
        }

        private IEnumerable<ICounter> GivenSomeDifferentFakeCounters()
        {
            var fakeSessions = GetSessionDataFake().ToArray();
            var fakeApplications = GetApplicationDataFake().ToArray();
            var fakeIssueTypes = GetIssueTypeDataFake().ToArray();
            var fakeIssues = GetIssueDataFake().ToArray();
            var customDataFake1 = GetCustomDataDataFake().ToArray();
            var customDataFake2 = GetCustomDataDataFake().ToArray();
            var customDataFake3 = GetCustomDataDataFake().ToArray();
            var fakeInitiatives = GetInitiativeDataFake().ToArray();
            var allFakeData = fakeSessions.Union(fakeApplications).Union(fakeIssueTypes).Union(fakeIssues).Union(customDataFake1).Union(customDataFake2).Union(customDataFake3).Union(fakeInitiatives).ToArray();
            return allFakeData;
        }

        private IEnumerable<ICounter> GetCustomDataDataFake()
        {
            var rng = new Random();
            var counterName = "Counter_" + rng.Next();

            return Mocks.Of<ICounter>(x => x.CounterName == counterName && x.Count == GetRandomNumber(1,5) && x.Data == new Dictionary<string, string>() && x.DateTime == GetRandomTime() && x.Duration == GetRandomNumber(0,1000) && x.Environment == GetRandomEnvironment() && x.Path == GetRandomPathForIssue()).Take(1000);
        }

        private IEnumerable<ICounter> GetIssueDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "Issue" && x.Count == 1 && x.Data == GetRandomDataForIssue() && x.DateTime == GetRandomTime() && x.Duration == null && x.Environment == GetRandomEnvironment() && x.Level == (string)null && x.Path == GetRandomPathForIssue()).Take(1000);
        }

        private IEnumerable<ICounter> GetIssueTypeDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "IssueType" && x.Count == 1 && x.Data == GetRandomDataForIssueType() && x.DateTime == GetRandomTime() && x.Duration == null && x.Environment == GetRandomEnvironment() && x.Level == (string)null && x.Path == GetRandomPathForIssueType()).Take(1000);
        }

        private IEnumerable<ICounter> GetApplicationDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "Application" && x.Count == 1 && x.Data == GetRandomDataForApplication() && x.DateTime == GetRandomTime() && x.Duration == null && x.Environment == GetRandomEnvironment() && x.Level == (string)null && x.Path == GetRandomPathForSession()).Take(1000);
        }

        private IEnumerable<ICounter> GetSessionDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "Session" && x.Count == 1 && x.Data == GetRandomDataForSession() && x.DateTime == GetRandomTime() && x.Duration == null && x.Environment == GetRandomEnvironment() && x.Level == (string)null && x.Path == GetRandomPathForSession()).Take(1000);
        }

        private IEnumerable<ICounter> GetInitiativeDataFake()
        {
            return Mocks.Of<ICounter>(x => x.CounterName == "Initiative" && x.Count == 1 && x.Data == GetRandomDataForInitiative() && x.DateTime == GetRandomTime() && x.Duration == null && x.Environment == (string)null && x.Level == (string)null && x.Path == GetRandomPathForInitiative()).Take(1000);
        }

        private Dictionary<string, string> GetRandomPathForSession()
        {
            return new Dictionary<string, string>
            { 
                { "Initiative", GetRandomStringFromRandomList("Initiative") },
                { "Developer", GetRandomStringFromRandomList("Developer") },
                { "Application", GetRandomStringFromRandomList("Application") },
                { "Version", GetRandomStringFromRandomList("Version") },
                { "Machine", GetRandomStringFromRandomList("Machine") },
            };
        }

        private Dictionary<string, string> GetRandomPathForInitiative()
        {
            return new Dictionary<string, string>
            { 
                { "Developer", GetRandomStringFromRandomList("Developer") },
            };
        }

        private Dictionary<string, string> GetRandomPathForIssue()
        {
            return new Dictionary<string, string>
            { 
                { "Initiative", GetRandomStringFromRandomList("Initiative") },
                { "Developer", GetRandomStringFromRandomList("Developer") },
                { "Application", GetRandomStringFromRandomList("Application") },
                { "Version", GetRandomStringFromRandomList("Version") },
                { "Machine", GetRandomStringFromRandomList("Machine") },
                { "IssueTypeTicket", (_issueTypeTicket++).ToString() },
            };
        }

        private Dictionary<string, string> GetRandomPathForIssueType()
        {
            return new Dictionary<string, string>
            { 
                { "Initiative", GetRandomStringFromRandomList("Initiative") },
                { "Developer", GetRandomStringFromRandomList("Developer") },
                { "Application", GetRandomStringFromRandomList("Application") },
                { "Version", GetRandomStringFromRandomList("Version") },
                { "Machine", GetRandomStringFromRandomList("Machine") },
            };
        }

        private int _issueInstanceTicket = 0;
        private int _issueTypeTicket = 0;
        private Dictionary<string, string> GetRandomDataForIssue()
        {
            return new Dictionary<string, string>
            {
                { "User", GetRandomStringFromRandomList("User") },
                { "IssueTypeTicket", (_issueTypeTicket++).ToString() },
                { "IssueInstanceTicket", (_issueInstanceTicket++).ToString() }
            };
        }

        private Dictionary<string, string> GetRandomDataForIssueType()
        {
            return new Dictionary<string, string>
            {
                { "User", GetRandomStringFromRandomList("User") },
                { "IssueTypeTicket", (_issueTypeTicket++).ToString() }
            };
        }

        private Dictionary<string, string> GetRandomDataForApplication()
        {
            return new Dictionary<string, string>
                       {                           
                       };
        }

        private Dictionary<string, string> GetRandomDataForSession()
        {
            return new Dictionary<string, string>
                       {
                           { "User", GetRandomStringFromRandomList("User") },
                           { "Handle", GetRandomStringFromRandomList("Handle") }
                       };
        }

        private Dictionary<string, string> GetRandomDataForInitiative()
        {
            return new Dictionary<string, string>
                       {
                            { "Initiative", GetRandomStringFromRandomList("Initiative") },
                       };
        }

        private readonly Dictionary<string,List<string>> _stringList = new Dictionary<string, List<string>>();
        private string GetRandomStringFromRandomList(string listName)
        {
            if (!_stringList.ContainsKey(listName))
            {
                var values = new List<string>();
                var cnt = GetRandomNumber(3, 8);
                for (var i = 0; i < cnt; i++)
                    values.Add(GetRandomString());
                _stringList.Add(listName, values);
            }
            return _stringList[listName].TakeRandom();
        }

        private string GetRandomString()
        {
            return RandomUtility.GetRandomString(10);
        }

        private int GetRandomNumber(int min, int max)
        {
            var rng = new Random();
            return rng.Next(min, max);
        }

        private string GetRandomEnvironment()
        {
            var rng = new Random();
            var i = rng.Next(0, 2);
            var arr = new[] { "Dev", "CI", "Prod" };
            return arr[i];
        }

        private DateTime GetRandomTime()
        {
            var rng = new Random();
            var i = rng.Next(100000);
            return DateTime.UtcNow.AddSeconds(i);
        }
    }

    public static class ListExtensions
    {
        public static T TakeRandom<T>(this List<T> items)
        {
            if (!items.Any())
                return default(T);

            var rng = new Random();
            var i = rng.Next(0, items.Count());
            return items[i];
        }

        //TODO: Break up this function into separate parts. One to provide data and another to format as skv.
        public static string ToSkv(this ICounterCollection cc)
        {
            var sb = new StringBuilder();

            sb.Append("DateTime");
            foreach(var name in cc.Names)
                sb.AppendFormat("\t{0}", name);
            sb.AppendLine();
            
            foreach (var item in cc.Lines)
            {
                sb.AppendFormat("{0}", item.Key);

                foreach(var count in item.Counts)
                    sb.AppendFormat("\t{0}", count);

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
} 