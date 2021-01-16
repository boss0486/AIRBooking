using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AL.NetFrame.Attributes;
using Dapper;
using WebCore.Model.Entities;
using WebCore.Services;

namespace WebCore.Entities
{
    [ConnectionString(DbConnect.ConnectionString.CMS)]
    [Dapper.Table("App_AirResBookDesig")]
    public class AirResBookDesig : WEBModel
    {
        public AirResBookDesig()
        {
            ID = Guid.NewGuid().ToString().ToLower();
        }
        [Key]
        [IgnoreUpdate]
        public string ID { get; set; }
        public string CodeID { get; set; }
        public int VoidBookTime { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
    }

    public class AirResBookDesigIDModel
    {
        public string ID { get; set; }
    }

    public class AirResBookDesigCreateModel
    {
        public string CodeID { get; set; }
        public int VoidBookTime { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
        public int Enabled { get; set; }
    }

    public class AirResBookDesigUpdateModel : AirResBookDesigCreateModel
    {
        public string ID { get; set; }
    }
    public class AirResBookDesigResult : WEBModelResult
    {
        public string ID { get; set; }
        public string CodeID { get; set; }
        public int VoidBookTime { get; set; }
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Summary { get; set; }
    }
    public class AirResBookDesigSetting
    {
        public string ID { get; set; }
        public int VoidBookTime { get; set; } 
    }
    public class AirResBookDesigOption
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string CodeID { get; set; }
    }
}
