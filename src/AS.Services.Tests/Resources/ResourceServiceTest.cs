using AS.Domain.Entities;
using AS.Domain.Interfaces;
using AS.Infrastructure.Tests;
using AS.Services.Interfaces;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AS.Services.Tests
{
    public class ResourceServiceTest : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;

        public ResourceServiceTest(TestFixture fixture)
        {
            this._fixture = fixture;
        }

        [Fact]
        public void GetResourcesByCulture_Should_Fetch_Correct_Resources()
        {
            Mock<IContextProvider> contextProvider = new Mock<IContextProvider>();
            IResourceService service = new ResourceService(_fixture.DbContext);
            List<int> idList = new List<int>();
            StringResource testSr;

            for (int i = 0; i < 10; i++)
            {
                testSr = new StringResource()
                {
                    Name = "Test" + i.ToString(),
                    Value = "Test" + i.ToString(),
                    CultureCode = "en-US"
                };
                service.Insert(testSr);
                idList.Add(testSr.Id);
            }
            for (int i = 0; i < 10; i++)
            {
                testSr = new StringResource()
                {
                    Name = "Test" + i.ToString(),
                    Value = "Test" + i.ToString(),
                    CultureCode = "tr-TR"
                };
                service.Insert(testSr);
                idList.Add(testSr.Id);
            }
            Assert.Equal(service.FetchAll().Count(s => s.CultureCode == "en-US"), 10);
            service.DeleteById(idList.First());
            idList.RemoveAt(0);
            service.DeleteById(idList.First());
            idList.RemoveAt(0);
            testSr = service.GetResourceById(idList.First());
            testSr.CultureCode = "de-DE";
            service.Update(testSr);
            Assert.Equal(service.FetchAll().Count(s => s.CultureCode == "en-US"), 7);
        }
    }
}