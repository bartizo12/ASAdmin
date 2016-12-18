using AS.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Extended Role class
    /// </summary>
    public class ASRole : IdentityRole<int, ASUserRole>, IRole, IXmlSerializable
    {
        public DateTime? ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public string Note { get; set; }

        public ASRole()
        {
        }

        public ASRole(string name)
        {
            Name = name;
        }

        #region IXmlSerializable

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("Name", this.Name);
            writer.WriteElementString("Id", this.Id.ToString());
            writer.WriteElementString("CreatedBy", this.CreatedBy ?? string.Empty);
            writer.WriteElementString("CreatedOn", this.CreatedOn.ToString());
            writer.WriteElementString("ModifiedBy", this.ModifiedBy ?? string.Empty);

            if (this.ModifiedOn != null)
                writer.WriteElementString("ModifiedOn", this.ModifiedOn.Value.ToString());
        }

        #endregion IXmlSerializable
    }
}