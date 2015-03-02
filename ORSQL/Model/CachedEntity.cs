using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OR.Model
{
    public abstract class CachedEntity<T> where T : OR.Model.Entity, new()
    {
        /// <summary>
        /// 控制并发锁问题
        /// </summary>
        protected static object _lockObj = new object();

        private static Dictionary<object, T> _cached = new Dictionary<object, T>();

        public static void Refresh()
        {
            _cached.Clear();
        }

        /// <summary>
        /// 根据Key从缓存里拿东西，如果没有则到数据库里查
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetModelByKey(object key)
        {
            if (!_cached.ContainsKey(key))
            {
                lock (_lockObj)
                {
                    if (!_cached.ContainsKey(key))
                    {
                        T entity = OR.DAL.GetModelByKey<T>(key);

                        if (entity != null)
                        {
                            _cached.Add(key, entity);

                            return entity;
                        }
                        else
                        {
                            return new T();
                        }
                    }
                    else
                    {
                        return _cached[key];
                    }
                }
            }
            else
            {
                return _cached[key];
            }
        }
    }
}
