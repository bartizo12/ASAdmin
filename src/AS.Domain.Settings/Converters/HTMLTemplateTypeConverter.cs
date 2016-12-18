using AS.Domain.Entities;
using System;
using System.ComponentModel;
using System.Globalization;

namespace AS.Domain.Settings
{
    /// <summary>
    /// Type converter for HTMLTemplates. Converts from SettingValue entity to HTMLTemplate object
    /// HTMLTemplate is used for generating an HTML output/HTML based E-Mail from a common template.
    /// E.g. Generating e-mail from template , to send to the user.
    /// </summary>
    internal class HTMLTemplateTypeConverter : SettingTypeConverterBase
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            SettingValue settingValue = value as SettingValue;

            if (settingValue != null)
            {
                HTMLTemplate template = new HTMLTemplate();
                template.SettingValueID = settingValue.Id;
                template.BodyFilePath = base.GetFieldValue<string>(settingValue, "BodyFilePath");
                template.Name = base.GetFieldValue<string>(settingValue, "Name");
                template.Subject = base.GetFieldValue<string>(settingValue, "Subject");
                template.Comment = base.GetFieldValue<string>(settingValue, "Comment");

                return template;
            }
            else
            {
                throw new InvalidCastException();
            }
        }
    }
}