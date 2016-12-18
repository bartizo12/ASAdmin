using AS.Infrastructure.Collections;
using System.Linq;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class PagedListTest
    {
        [Theory]
        [InlineData(911, 100, 11)]
        [InlineData(8, 10, 8)]
        [InlineData(10, 10, 0)]
        public void PagedList_TestList_Should_Paginate_Correctly(int totalCount, int pageSize,
            int lastExpectedPageSize)
        {
            var testUsernameList = TestHelper.GenerateUsernameList(totalCount);
            var pagedList = new PagedList<string>(testUsernameList.AsQueryable(), totalCount / pageSize, pageSize);
            Assert.Equal<int>(pagedList.Count, lastExpectedPageSize);

            if (lastExpectedPageSize != 0)
            {
                Assert.Equal<string>(pagedList.First(), testUsernameList.ElementAt(totalCount - lastExpectedPageSize));
            }
        }
    }
}