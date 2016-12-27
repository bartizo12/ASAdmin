using System;
using System.Text;
using System.Xml.Serialization;

namespace AS.Domain.Entities
{
    public class ASConfiguration : DbConnectionConfiguration
    {
        public ASConfiguration()
        {

        }
        public ASConfiguration(string dataProvider,string connectionString)
            :base(dataProvider,connectionString)
        {
        }
        public bool IsDemo { get; set; }
        public string RecaptchaPrivateKey { get; set; }
        public string RecaptchaPublicKey { get; set; }
        /// <summary>
        /// Symmetric encryption algorithm key
        /// </summary>
        public string SymmetricKey { get; set; }

        /// <summary>
        /// Name of the settinng
        /// </summary>
        public string SMTPName { get; set; }

        /// <summary>
        /// SMTP Host Address .e.g smtp.gmail.com
        /// </summary>
        public string SMTPHost { get; set; }

        /// <summary>
        /// 25/587 or whatever the port number is
        /// </summary>
        public int SMTPPort { get; set; }

        public int SMTPTimeOut { get; set; }//In miliseconds

        /// <summary>
        /// If SMTP server requires SSL connection
        /// </summary>
        public bool SMTPEnableSsl { get; set; }

        /// <summary>
        /// Is SMTP server accepts Default Credenttials
        /// </summary>
        public bool SMTPDefaultCredentials { get; set; }

        /// <summary>
        /// Username on SMTP server
        /// </summary>
        public string SMTPUserName { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [XmlIgnore]
        public string SMTPPassword { get; set; }

        [XmlElement(ElementName = "SMTPPassword")]
        public string SMTPPasswordXMLSafe
        {
            get
            {
                return Convert.ToBase64String(Encoding.UTF8
                    .GetBytes(this.SMTPPassword));
            }
            set
            {
                this.SMTPPassword = Encoding.UTF8.GetString(Convert
                    .FromBase64String(value));
            }
        }

        /// <summary>
        /// From Name that receiever see on mails
        /// </summary>
        public string SMTPFromDisplayName { get; set; }

        /// <summary>
        /// From address that receiever see on mails
        /// </summary>
        public string SMTPFromAddress { get; set; }
    }
}
