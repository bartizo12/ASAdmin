using System.Web;
using System.Web.Optimization;

namespace AS.Infrastructure.Web.Optimization
{
    /// <summary>
    /// Resolves issues such as ; file pathing, @import statement ...etc  that causes faulty bundling
    /// </summary>
    public class ASCssTransform : IItemTransform
    {
        public string Process(string includedVirtualPath, string input)
        {
            int importStartIndex = input.IndexOf("@import");

            if (importStartIndex >= 0)
            {
                int count = input.IndexOf(";", importStartIndex);
                input = input.Remove(importStartIndex, count + 1);
            }
            return new CssRewriteUrlTransform().Process("~" +
                VirtualPathUtility.ToAbsolute(includedVirtualPath), input);
        }
    }
}