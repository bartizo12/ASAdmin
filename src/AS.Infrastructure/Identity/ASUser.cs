using AS.Domain.Entities;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AS.Infrastructure.Identity
{
    /// <summary>
    /// Extended user class
    /// </summary>
    [Serializable]
    public class ASUser : IdentityUser<int, ASUserLogin, ASUserRole, ASUserClaim>, IUser, IXmlSerializable
    {
        public virtual DateTime? LastActivity { get; set; }
        public virtual DateTime? LastLogin { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }

        public List<int> RoleIds
        {
            get
            {
                return this.Roles.Select(r => r.RoleId).ToList();
            }
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
            writer.WriteElementString("Id", this.Id.ToString());
            writer.WriteElementString("UserName", this.UserName);
            writer.WriteElementString("AccessFailedCount", this.AccessFailedCount.ToString());
            writer.WriteElementString("Email", this.Email);
            writer.WriteElementString("EmailConfirmed", this.EmailConfirmed.ToString());
            writer.WriteElementString("LastActivity", this.LastActivity.ToString());
            writer.WriteElementString("LastLogin", this.LastLogin.ToString());
            writer.WriteElementString("LockoutEnabled", this.LockoutEnabled.ToString());

            if (this.LockoutEndDateUtc != null)
                writer.WriteElementString("LockoutEndDateUtc", this.LockoutEndDateUtc.ToString());

            writer.WriteElementString("PasswordHash", this.PasswordHash);
            writer.WriteElementString("PhoneNumber", this.PhoneNumber);
            writer.WriteElementString("PhoneNumberConfirmed", this.PhoneNumberConfirmed.ToString());
            writer.WriteElementString("SecurityStamp", this.SecurityStamp.ToString());
            writer.WriteElementString("TwoFactorEnabled", this.TwoFactorEnabled.ToString());

            writer.WriteElementString("CreatedBy", this.CreatedBy ?? string.Empty);
            writer.WriteElementString("CreatedOn", this.CreatedOn.ToString());
            writer.WriteElementString("ModifiedBy", this.ModifiedBy ?? string.Empty);

            if (this.ModifiedOn != null)
                writer.WriteElementString("ModifiedOn", this.ModifiedOn.Value.ToString());
        }

        #endregion IXmlSerializable
    }
}