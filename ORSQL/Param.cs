using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR
{
    public class Param
    {
        public static System.Data.SqlClient.SqlParameter NamedParam(String pName, object pValue)
        {
            System.Data.SqlClient.SqlParameter p = new System.Data.SqlClient.SqlParameter();
            p.ParameterName = pName;
            p.SqlValue = pValue;
            return p;
        }
    }
}
