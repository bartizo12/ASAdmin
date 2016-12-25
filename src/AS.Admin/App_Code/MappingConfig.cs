using AS.Admin.Models;
using AS.Domain.Entities;
using Mapster;

namespace AS.Admin
{
    /// <summary>
    /// Contains mapster mapping configs
    /// </summary>
    internal class MappingConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<ConfigurationModel, ASConfiguration>.NewConfig()
                .Map(dest => dest.SMTPName, src => src.EMailSetting.Name)
                .Map(dest => dest.SMTPDefaultCredentials, src => src.EMailSetting.DefaultCredentials)
                .Map(dest => dest.SMTPEnableSsl, src => src.EMailSetting.EnableSsl)
                .Map(dest => dest.SMTPFromAddress, src => src.EMailSetting.FromAddress)
                .Map(dest => dest.SMTPFromDisplayName, src => src.EMailSetting.FromDisplayName)
                .Map(dest => dest.SMTPHost, src => src.EMailSetting.Host)
                .Map(dest => dest.SMTPPassword, src => src.EMailSetting.Password)
                .Map(dest => dest.SMTPPort, src => src.EMailSetting.Port)
                .Map(dest => dest.SMTPTimeOut, src => src.EMailSetting.TimeOut)
                .Map(dest => dest.SMTPUserName, src => src.EMailSetting.UserName);
        }
    }
}