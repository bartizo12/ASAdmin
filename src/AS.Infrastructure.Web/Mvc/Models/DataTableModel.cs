using Newtonsoft.Json;
using System.Collections;
using System.Web.Mvc;

namespace AS.Infrastructure.Web.Mvc
{
    /// <summary>
    /// Model for datatabl(datatables.net , jquery table plug-in)
    /// </summary>
    [ModelBinder(typeof(DataTableModelBinder))]
    public class DataTableModel : ASModelBase
    {
        private const int DEFAULT_PAGE_SIZE = 10;
        private const int DEFAULT_START = 0;

        public string Ordering { get; set; }

        public int PageIndex
        {
            get
            {
                return start / PageSize;
            }
        }

        public int start { get; set; }

        [JsonProperty(PropertyName = "length")]
        public int PageSize { get; set; }

        [JsonProperty(PropertyName = "draw")]
        public int Draw { get; set; }

        [JsonProperty(PropertyName = "recordsTotal")]
        public int TotalCount { get; set; } //Total records, before filtering

        public int iTotalDisplayRecords { get { return this.TotalCount; } }

        [JsonProperty(PropertyName = "recordsFiltered")]
        public int FilteredCount
        {
            get
            {
                return this.data == null ? 0 : this.data.Count;
            }
        }

        public string error { get; set; } //Optional. Error to be displayed
        public IList data { get; set; }

        public DataTableModel()
        {
            this.PageSize = DEFAULT_PAGE_SIZE;
            this.start = DEFAULT_START;
            this.error = string.Empty;
        }
    }
}