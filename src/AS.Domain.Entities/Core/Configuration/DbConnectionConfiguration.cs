namespace AS.Domain.Entities
{
    public class DbConnectionConfiguration
    {
        public string ConnectionString { get; set; }
        public string DataProvider { get; set; }

        public DbConnectionConfiguration() { }
        public DbConnectionConfiguration(string dataProvider,string connectionString)
        {
            this.ConnectionString = connectionString;
            this.DataProvider = dataProvider;
        }
    }
}
