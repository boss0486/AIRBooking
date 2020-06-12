using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIRService.AppService.Module.Flight.Services
{
    public interface ICMSUserSettingService : IEntityService<CMSUserSetting> { }
    public class CMSUserSettingService : EntityService<CMSUserSetting>, ICMSUserSettingService
    {
        public CMSUserSettingService() : base() { }
        public CMSUserSettingService(System.Data.IDbConnection db) : base(db) { }
    }
}
