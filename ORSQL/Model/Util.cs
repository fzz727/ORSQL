using System;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace OR.Model
{
    /// <summary>
    ///ModelUtil 的摘要说明
    /// </summary>
    public class Util
    {
        #region 成员变量操作方法

        /// <summary>
        /// 返回当前实体类的所有成员列表。已经做了缓存处理，效率应该不错
        /// </summary>
        /// <returns></returns>
        public static string GetEntityMembersName<T>() where T : Entity
        {
            String strMemebers = string.Empty;

            String strEntityKey = typeof(T).FullName;

            strMemebers = CacheManage.GetEntityMambersName(strEntityKey);
            // 检查hash里有没有，已经有了则直接返回，没有则将其添加进去
            if (String.IsNullOrEmpty(strMemebers))
            {
                PropertyInfo[] props = GetEntityMembers<T>();

                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = props[i];
                    strMemebers += prop.Name + ", ";
                }

                strMemebers = strMemebers.Trim(new char[] { ' ', ',' });

                CacheManage.AddCacheMembersNameDict(strEntityKey, strMemebers);
            }

            return strMemebers;
        }

        /// <summary>
        /// 返回当前实体类的所有成员列表
        /// </summary>
        /// <returns></returns>
        public static PropertyInfo[] GetEntityMembers<T>() where T : Entity
        {
            String strEntityKey = typeof(T).FullName;

            PropertyInfo[] props;

            props = CacheManage.GetEntityMambers(strEntityKey);

            if (props == null)
            {
                props = typeof(T).GetProperties();

                List<PropertyInfo> members = new List<PropertyInfo>();

                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = props[i];

                    object[] attrs = prop.GetCustomAttributes(typeof(Transient), false);

                    // 此字段不实例化
                    if (attrs == null || attrs.Length == 0)
                    {
                        members.Add(prop);
                    }
                }

                props = members.ToArray();

                // 记录一下缓存
                CacheManage.AddCacheMembersDict(strEntityKey, props);
            }

            return props;
        }

        /// <summary>
        /// 返回当前实体类的所有主键成员列表。支持联合主键，所以返回的是数组。
        /// 已做缓存处理
        /// </summary>
        /// <returns></returns>
        public static PropertyInfo[] GetEntityPrimayKeys<T>() where T : Entity
        {
            String strEntityKey = typeof(T).FullName + "@Key";

            PropertyInfo[] props = CacheManage.GetEntityMambers(strEntityKey);

            if (props == null)
            {
                props = typeof(T).GetProperties();

                List<PropertyInfo> propArray = new List<PropertyInfo>();

                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = props[i];

                    object[] attrs = prop.GetCustomAttributes(typeof(ID), false);

                    if (attrs != null && attrs.Length > 0)
                    {
                        propArray.Add(prop);
                    }
                }

                props = propArray.ToArray();

                CacheManage.AddCacheMembersDict(strEntityKey, props);
            }
            return props;
        }

        /// <summary>
        /// 返回当前实体类的所有主键成员列表。因为一个实体类会有联合主键，
        /// 所以这里返回的逗号分割的多主键列表字符串。
        /// 已做缓存处理。
        /// </summary>
        /// <returns></returns>
        public static string GetEntityPrimayKeysName<T>() where T : Entity
        {
            String strEntityKey = typeof(T).FullName + "@Key";

            String strMemebers = CacheManage.GetEntityMambersName(strEntityKey);

            if (String.IsNullOrEmpty(strMemebers))
            {
                PropertyInfo[] props = GetEntityPrimayKeys<T>();

                for (int i = 0; i < props.Length; i++)
                {
                    strMemebers += props[i].Name + ", ";
                }

                strMemebers = strMemebers.Trim(new char[] { ' ', ',' });

                CacheManage.AddCacheMembersNameDict(strEntityKey, strMemebers);
            }

            return strMemebers;
        }

        /// <summary>
        /// 判断当前字段是否是主键
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static bool IsPrimayKey<T>(String propName) where T : Entity
        {
            PropertyInfo prop = typeof(T).GetProperty(propName);
            return IsPrimayKey(prop);
        }

        /// <summary>
        /// 判断当前字段是否是主键
        /// </summary>
        /// <param name="PropName"></param>
        /// <returns></returns>
        public static bool IsPrimayKey(PropertyInfo prop)
        {
            object[] attrs = prop.GetCustomAttributes(typeof(ID), false);
            if (attrs != null && attrs.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 该字段是否不做持久化
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static bool IsTransient(PropertyInfo prop)
        {
            object[] attrs = prop.GetCustomAttributes(typeof(Transient), false);
            if (attrs != null && attrs.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 返回实体类的主键值。注意：该方法暂时只支持单主键，复合主键暂时不支持。以后扩展。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static Object GetPrimayKeyValue<T>(T model) where T : Entity
        {
            return Util.GetEntityPrimayKeys<T>()[0].GetValue(model, null);
        }

        #endregion

        #region 针对实体类的一些处理方法

        /// <summary>
        /// 返回当前实体类映射数据库表名。已做缓存处理
        /// </summary>
        /// <returns></returns>
        public static string GetTableName<T>() where T : Entity
        {
            String strKeyName = typeof(T).FullName + "@TableName";

            String strTableName = CacheManage.GetEntityMambersName(strKeyName);

            if (String.IsNullOrEmpty(strTableName))
            {

                object[] attrs = typeof(T).GetCustomAttributes(typeof(Table), false);

                if (attrs != null && attrs.Length > 0 && attrs[0] is Table)
                {
                    Table table = (Table)attrs[0];
                    strTableName = table.TableName;
                }
                else
                {
                    strTableName = typeof(T).Name;
                }

                CacheManage.AddCacheMembersNameDict(strKeyName, strTableName);
            }

            return strTableName;
        }

        /// <summary>
        /// 获取主键的取值方式
        /// </summary>
        /// <returns></returns>
        public static GenerationType GetPrimayKeyGenerationType<T>() where T : Entity
        {
            PropertyInfo[] props = GetEntityPrimayKeys<T>();

            if (props.Length > 0)
            {
                object[] attrs = props[0].GetCustomAttributes(typeof(ID), false);

                if (attrs != null && attrs.Length > 0)
                {
                    ID id = (ID)attrs[0];
                    return id.GeneratedValue;
                }
            }
            return GenerationType.Auto;
        }

        #endregion

        /// <summary>
        /// 获取配置文件中，每页分页的大小
        /// </summary>
        /// <returns></returns>
        public static int GetPageSize()
        {
            String sPage = System.Configuration.ConfigurationManager.AppSettings["PageSize"];
            if (String.IsNullOrEmpty(sPage))
            {
                return 10;
            }
            else
            {
                int p = 0;
                if (int.TryParse(sPage, out p))
                {
                    return p;
                }
                else
                {
                    return 10;
                }
            }
        }
    }
}