using AS.Domain.Entities;
using AS.Infrastructure.Data.EF;

namespace AS.Infrastructure.Mappings
{
    public class UserActivityMapping : EntityTypeConfigurationBase<UserActivity, int>
    {
        public UserActivityMapping()
        {
            this.ToTable("UserActivity");
        }
    }
}