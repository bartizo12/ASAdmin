using AS.Domain.Interfaces;
using System;
using Xunit;

namespace AS.Infrastructure.Tests
{
    public class XmlSerializerTest
    {
        [Serializable]
        public class Customer : IEquatable<Customer>
        {
            public string Name { get; set; }
            public long CustomerNo { get; set; }
            public DateTime CreatedOn { get; set; }
            public DateTime? ModifiedOn { get; set; }

            public bool Equals(Customer customer)
            {
                return Name == customer.Name &&
                     CustomerNo == customer.CustomerNo &&
                      CreatedOn == customer.CreatedOn &&
                       ModifiedOn == customer.ModifiedOn;
            }
        }

        public class DummyClass
        {
            public DateTime CreatedOn { get; set; }
        }

        [Fact]
        public void Serialize_Should_Equal_After_Deserialize()
        {
            var customer = new Customer()
            {
                Name = Faker.Name.FullName(),
                CustomerNo = TestHelper.RandomLong(),
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now
            };
            IXmlSerializer serializer = new ASXmlSerializer();

            string xml = serializer.SerializeToXML(customer);
            var deserializedCustomer = serializer.DeserializeFromXML<Customer>(xml);

            Assert.Equal<Customer>(customer, deserializedCustomer);
        }
    }
}