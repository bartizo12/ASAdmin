using AS.Domain.Entities;
using AS.Domain.Interfaces;
using System;
using System.Text;
using System.Xml.Serialization;
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
        [Fact]
        public void Serialized_Object_Should_Equal_Deserialized_Object()
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