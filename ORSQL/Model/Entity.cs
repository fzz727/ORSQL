using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace OR.Model
{
    /// <summary>
    ///Entity 的摘要说明
    /// </summary>
    [Serializable]
    public class Entity
    {
        #region 继承重载早期方法 ToString()

        /// <summary>
        /// 对象ToString()方法重写。不传参数，默认将所有的成员变量和值全输出
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary>
        /// 对象ToString()方法重写。根据传的参数决定是否显示全部成员变量
        /// </summary>
        /// <param name="showMembers">是否显示成员变量，true: 显示全部变量，false：显示主键变量</param>
        /// <returns></returns>
        public virtual string ToString(bool showMembers)
        {
            String strMembers = string.Empty;

            StringBuilder strString = new StringBuilder();

            PropertyInfo[] props = this.GetType().GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo curMem = props[i];

                if (showMembers)
                {
                    object[] attrs = curMem.GetCustomAttributes(typeof(Transient), false);

                    //排除掉非持久化属性对象
                    if (attrs == null || attrs.Length == 0)
                    {
                        strString.Append(String.Format("{0} = [{1}], ", curMem.Name, curMem.GetValue(this, null)));
                    }
                }
                else
                {
                    object[] attrs = curMem.GetCustomAttributes(typeof(ID), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        strString.Append(String.Format("{0} = [{1}], ", curMem.Name, curMem.GetValue(this, null)));
                    }
                }
            }

            return strString.ToString().Trim(new char[] { ' ', ',' });
        }

        #endregion
    }

    #region 实体类定义中所需的各类属性

    /// <summary>
    /// 映射数据库表。如设置此属性，以此属性设置的为准，否则默认与实体类名一致。
    /// </summary>
    public class Table : Attribute
    {
        private String _tableName = string.Empty;

        public String TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }

        private String _connName;

        public String ConnectionName
        {
            get { return _connName; }
            set { _connName = value; }
        }

        public Table(String TableName)
        {
            this._tableName = TableName;
            // 如果没有传入连接串名称，则使用默认的SQLConection
            this._connName = "SQLConnection";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TableName">数据库表名</param>
        /// <param name="ConnectionName">链接字符串名称</param>
        public Table(String ConnectionName, String TableName)
        {
            if (!String.IsNullOrEmpty(ConnectionName))
            {
                this._connName = ConnectionName;
            }
            else
            {
                this._connName = "SQLConnection";
            }

            this.ConnectionName = TableName;
        }
    }

    /// <summary>
    /// 定义主键字段的生成方式，自动编号或者手工编号，将来扩展序列编号
    /// Auto：手工指定值内容，默认值
    /// Identity：自动编号
    /// Senquence：待扩展，序列方式
    /// </summary>
    public enum GenerationType
    {
        Auto = 0,
        Identity = 1,
        Senquence = 2,
        Manually = 3
    }

    /// <summary>
    /// 实体类主键标识，可指定主键的生成方式
    /// </summary>
    [Serializable]
    public class ID : Attribute
    {
        GenerationType _generationType = GenerationType.Auto;

        public GenerationType GeneratedValue
        {
            get
            {
                return _generationType;
            }
            set
            {
                _generationType = value;
            }
        }

        // 当前逐渐的序列名称
        private String _Senquence;

        /// <summary>
        /// 获取当前实体类所需的实体类名称。该名称优先级：ID属性 > 配置文件 > 默认值
        /// </summary>
        public String Senquence
        {
            get
            {
                if (String.IsNullOrEmpty(_Senquence))
                {
                    String strName = System.Configuration.ConfigurationManager.AppSettings["Senquence"];
                    if (String.IsNullOrEmpty(strName))
                    {
                        _Senquence = "OR_Senquence";
                    }
                    else
                    {
                        _Senquence = strName;
                    }
                }
                return _Senquence;
            }
            set
            {
                _Senquence = value;
            }
        }


        public ID()
        {
            this._generationType = GenerationType.Auto;
        }

        public ID(GenerationType generationType)
        {
            this._generationType = generationType;
        }

        public ID(GenerationType generationType, String SenquenceName)
        {
            this._generationType = generationType;
            this._Senquence = SenquenceName;
        }
    }

    /// <summary>
    /// 用于标识属性，不做具体工作。有此属性的字段不做数据库实例化操作。
    /// </summary>
    public class Transient : Attribute
    {
        /*
         * 用于标识属性，不做具体工作。有此属性的字段不做数据库实例化操作。
         */
    }

    #endregion

}