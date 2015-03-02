using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace models
{
    [OR.Model.Table("UserInfo")]
    public class UserInfo : OR.Model.Entity
    {
        [OR.Model.ID(OR.Model.GenerationType.Identity)]
        public int UserID { get; set; }
        public String UserName { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
    }

    //
    public class UserInfoImpl<UserInfo> : OR.Model.CacheManage { }

}
