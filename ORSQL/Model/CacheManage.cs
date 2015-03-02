using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace OR.Model
{
    public class CacheManage
    {
        #region 各实体类成员变量存储管理，字符串型的

        // 用于记录各个实体类模型的成员列表，Key型的
        private static Dictionary<String, String> _dictEntityMembersName = new Dictionary<string, string>();
        private static object _dictEntityMembersNameLock = new object();

        /// <summary>
        /// 获取实体类成员变量字符串缓存字典表
        /// </summary>
        public static Dictionary<String, String> CacheEntityMembersName
        {
            get
            {
                if (_dictEntityMembersName == null)
                {
                    lock (_dictEntityMembersNameLock)
                    {
                        /*
                         * 此处必须重新判断一次，保证这里不会由于并发问题造成数据丢失。
                         * 这里其实执行的次数很少，只有第一次会
                         */
                        if (_dictEntityMembersName == null)
                        {
                            _dictEntityMembersName = new Dictionary<string, string>();
                        }
                    }
                }
                return _dictEntityMembersName;
            }
        }

        /// <summary>
        /// 返回值为字符串行的，适用于表名，连接串，等单一值范围
        /// </summary>
        /// <param name="EntityName"></param>
        /// <returns></returns>
        public static String GetEntityMembersName(String EntityName)
        {
            if (CacheEntityMembersName.ContainsKey(EntityName))
            {
                return _dictEntityMembersName[EntityName];
            }
            return String.Empty;
        }

        /// <summary>
        /// 更新成员表缓存变量，增加新内容。字符串型的。适用于表名，连接串等单一值范围。
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddCacheMembersNameDict(String key, String value)
        {
            // 改部分必须保证只有一个同时执行的代码，否则会有问题
            lock (_dictEntityMembersNameLock)
            {
                if (!_dictEntityMembersName.ContainsKey(key))
                {
                    try
                    {
                        _dictEntityMembersName.Add(key, value);
                    }
                    catch (Exception)
                    {
                        // 已经存储了，不用管他
                    }
                }
            }
        }

        #endregion

        #region 各实体类成员变量存储管理，成员变量数组型的

        // 用于记录各个实体类模型的成员列表，Key型的
        private static Dictionary<String, PropertyInfo[]> _dictEntityMembers = new Dictionary<string, PropertyInfo[]>();
        private static object _dictEntityMembersLock = new object();

        /// <summary>
        /// 获取实体类成员变量字符串缓存字典表
        /// </summary>
        public static Dictionary<String, PropertyInfo[]> CacheEntityMembers
        {
            get
            {
                if (_dictEntityMembers == null)
                {
                    lock (_dictEntityMembersLock)
                    {
                        /*
                         * 此处必须重新判断一次，保证这里不会由于并发问题造成数据丢失。
                         * 这里其实执行的次数很少，只有第一次会
                         */
                        if (_dictEntityMembers == null)
                        {
                            _dictEntityMembers = new Dictionary<string, PropertyInfo[]>();
                        }
                    }
                }
                return _dictEntityMembers;
            }
        }

        public static PropertyInfo[] GetEntityMembersField(String EntityName)
        {
            if (CacheEntityMembers.ContainsKey(EntityName))
            {
                return _dictEntityMembers[EntityName];
            }
            return null;
        }

        /// <summary>
        /// 更新成员表缓存变量，增加新内容
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void AddCacheMembersFields(String key, PropertyInfo[] value)
        {
            // 改部分必须保证只有一个同时执行的代码，否则会有问题
            lock (_dictEntityMembersLock)
            {
                if (!_dictEntityMembers.ContainsKey(key))
                {
                    try
                    {
                        _dictEntityMembers.Add(key, value);
                    }
                    catch (Exception)
                    {
                        // 已经存储了，不用管他
                    }
                }
            }
        }

        #endregion
    }
}
