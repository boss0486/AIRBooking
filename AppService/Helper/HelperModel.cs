using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper.Model
{
    public class RoleModel
    {
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Details { get; set; }
        public bool Delete { get; set; }
    }

    public class RoleAccountModel
    {
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Details { get; set; }
        public bool Delete { get; set; }
        public bool Block { get; set; }
        //public bool Unlock { get; set; }
        public bool Active { get; set; }
        //public bool UnActive { get; set; }
    }
    public class RoleLeaveApproveModel
    {
        public bool Approve { get; set; }
        public bool UnApprove { get; set; }
        public bool Details { get; set; }
    }

    public class RoleLeaveModel
    {
        public bool Create { get; set; }
        public bool Update { get; set; }
        public bool Delete { get; set; }
        public bool Send { get; set; }
        public bool Unsend { get; set; }
        public bool Details { get; set; }
    }
    
}
