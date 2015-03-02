using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using OR.Model;
using OR;

namespace OR
{
    /// <summary>
    ///dal 的摘要说明
    /// </summary>
    public class DAL
    {
        public DAL()
        {
            //
            //TODO: 在此处添加构造函数逻辑
            //
        }

        #region 针对查询的一些操作

        /// <summary>
        /// 查询指定条件的数据，以数据表的方式返回结果
        /// </summary>
        /// <typeparam name="T">查询的实体类类型，需继承Entity类型</typeparam>
        /// <param name="strWhere">查询条件</param>
        /// <param name="param">参数化查询参数列表</param>
        /// <returns>返回的数据表</returns>
        public static DataTable GetDataTable<T>(String strWhere, params SqlParameter[] param) where T : Entity
        {
            StringBuilder strSQL = new StringBuilder();

            strSQL.Append("SELECT ");
            strSQL.Append(Util.GetEntityMembersName<T>());
            strSQL.Append(" FROM " + Util.GetTableName<T>());

            if (!String.IsNullOrEmpty(strWhere))
            {
                strSQL.Append(" WHERE " + strWhere);
            }

            return SQLHelper.Query(strSQL.ToString(), Util.GetTableConnectionName<T>(), param).Tables[0];
        }

        /// <summary>
        /// 查询指定条件的数据，以数据表的方式返回结果。做分页处理
        /// </summary>
        /// <typeparam name="T">查询的实体类类型，需继承Entity类型</typeparam>
        /// <param name="strWhere">查询条件</param>
        /// <param name="param">参数化查询参数列表</param>
        /// <returns>返回的数据表</returns>
        public static DataTable GetDataTable<T>(String strWhere, int _PageIndex, ref int _RowCount) where T : Entity
        {
            return GetDataTable<T>(strWhere, _PageIndex, Util.GetPageSize(), ref _RowCount);
        }

        /// <summary>
        /// 查询指定条件的数据，以数据表的方式返回结果。做分页处理
        /// </summary>
        /// <typeparam name="T">查询的实体类类型，需继承Entity类型</typeparam>
        /// <param name="strWhere">查询条件</param>
        /// <param name="param">参数化查询参数列表</param>
        /// <returns>返回的数据表</returns>
        public static DataTable GetDataTable<T>(String strWhere, int _PageIndex, int _PageSize, ref int _RowCount) where T : Entity
        {
            StringBuilder strSQL = new StringBuilder();

            strSQL.Append("SELECT ");
            strSQL.Append(Util.GetEntityMembersName<T>());
            strSQL.Append(" FROM " + Util.GetTableName<T>());

            if (!String.IsNullOrEmpty(strWhere))
            {
                strSQL.Append(" WHERE " + strWhere);
            }

            int pageSize = _PageSize;
            int pageCount = 0;

            return SQLHelper.Query(strSQL.ToString(), Util.GetTableConnectionName<T>(), _PageIndex, pageSize, ref _RowCount, ref pageCount);
        }

        #endregion

        #region 数据集与实体类操作


        /// <summary>
        /// 查询指定条件的数据，以指定类型的实体类列表的方式返回结果
        /// </summary>
        /// <typeparam name="T">查询的实体类类型，需继承Entity类型</typeparam>
        /// <param name="strWhere">查询条件</param>
        /// <param name="param">参数化查询参数列表</param>
        /// <returns>List形式返回的实体类列表</returns>
        public static List<T> GetModelList<T>(String strWhere, params SqlParameter[] param) where T : Entity, new()
        {
            DataTable dt = GetDataTable<T>(strWhere, param);

            return DataTableToModel<T>(dt);
        }

        /// <summary>
        /// 查询指定条件的数据，以指定类型的实体类列表的方式返回结果。内置了分页处理
        /// </summary>
        /// <typeparam name="T">查询的实体类类型，需继承Entity类型</typeparam>
        /// <param name="strWhere">查询条件</param>
        /// <param name="param">参数化查询参数列表</param>
        /// <returns>List形式返回的实体类列表</returns>
        public static List<T> GetModelList<T>(String strWhere, int _PageIndex, ref int _RowCount) where T : Entity, new()
        {
            DataTable dt = GetDataTable<T>(strWhere, _PageIndex, ref _RowCount);

            return DataTableToModel<T>(dt);
        }

        /// <summary>
        /// 查询指定条件的数据，以指定类型的实体类列表的方式返回结果。内置了分页处理
        /// </summary>
        /// <typeparam name="T">查询的实体类类型，需继承Entity类型</typeparam>
        /// <param name="strWhere">查询条件</param>
        /// <param name="param">参数化查询参数列表</param>
        /// <returns>List形式返回的实体类列表</returns>
        public static List<T> GetModelList<T>(String strWhere, int _PageIndex, int _PageSize, ref int _RowCount) where T : Entity, new()
        {
            DataTable dt = GetDataTable<T>(strWhere, _PageIndex, _PageSize, ref _RowCount);

            return DataTableToModel<T>(dt);
        }

        /// <summary>
        /// 查询指定条件的数据，以指定类型的实体类列表的方式返回结果
        /// </summary>
        /// <typeparam name="T">查询的实体类类型，需继承Entity类型</typeparam>
        /// <param name="strWhere">查询条件</param>
        /// <param name="param">参数化查询参数列表</param>
        /// <returns>List形式返回的实体类列表</returns>
        public static T GetModel<T>(String strWhere, params SqlParameter[] param) where T : Entity, new()
        {
            DataTable dt = GetDataTable<T>(strWhere, param);

            if (dt.Rows.Count > 0)
            {
                return DataRowToModel<T>(dt.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 通过主键进行数据库的查询。当前只支持单主键的方式。
        /// </summary>
        /// <typeparam name="T">要查询的实体类类型</typeparam>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public static T GetModelByKey<T>(object keyValue) where T : Entity, new()
        {
            PropertyInfo key = Util.GetEntityPrimayKeys<T>()[0];

            String strWhere = key.Name + "=@" + key.Name;
            SqlParameter para = new SqlParameter("@" + key.Name, keyValue);

            return GetModel<T>(strWhere, para);
        }

        /// <summary>
        /// 将指定的数据表转换为指定类型的实体类列表
        /// </summary>
        /// <typeparam name="T">需转换的实体类类型，需继承Entity类型</typeparam>
        /// <param name="dt">需转换成列表的数据集</param>
        /// <returns>返回的指定类型的实体类列表</returns>
        public static List<T> DataTableToModel<T>(System.Data.DataTable dt) where T : Entity, new()
        {
            List<T> list = new List<T>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                T model = DataRowToModel<T>(dt.Rows[i]);
                list.Add(model);
            }

            return list;
        }

        /// <summary>
        /// 将数据行转换为指定类型的实体类
        /// </summary>
        /// <typeparam name="T">需转换的实体类类型，需继承Entity类型</typeparam>
        /// <param name="dr">需转换成实体类的数据行</param>
        /// <returns>返回的指定类型的实体类</returns>
        public static T DataRowToModel<T>(System.Data.DataRow dr) where T : Entity, new()
        {
            PropertyInfo[] pInfos = Util.GetEntityMembers<T>();

            T model = new T();

            for (int j = 0; j < pInfos.Length; j++)
            {
                if (dr[pInfos[j].Name] == DBNull.Value)
                {
                    continue;
                }

                if (pInfos[j].CanWrite)
                {
                    try
                    {
                        if (!Convert.IsDBNull(dr[pInfos[j].Name]))
                        {
                            if (!pInfos[j].PropertyType.IsGenericType)
                            {
                                pInfos[j].SetValue(model, Convert.ChangeType(dr[pInfos[j].Name], pInfos[j].PropertyType), null);
                            }
                            else
                            {
                                // 加入对 nullable字段类型的处理，主要用于： DateTime? 泛型。 2014/8/6
                                Type genericTypeDefinition = pInfos[j].PropertyType.GetGenericTypeDefinition();

                                if (genericTypeDefinition == typeof(Nullable<>))
                                {
                                    pInfos[j].SetValue(model, Convert.ChangeType(dr[pInfos[j].Name], Nullable.GetUnderlyingType(pInfos[j].PropertyType)), null);
                                }
                            }
                        }
                    }
                    catch (System.ArgumentException)
                    {
                        //
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// 将数据行数组转换为List实体类
        /// </summary>
        /// <typeparam name="T">需转换的实体类类型，需继承Entity类型</typeparam>
        /// <param name="drArray">需转换成实体类的数据行数组</param>
        /// <returns>返回的指定类型的实体类List集合</returns>
        public static List<T> DataRowsToList<T>(System.Data.DataRow[] drArray) where T : Entity, new()
        {
            List<T> list = new List<T>();

            for (int i = 0; i < drArray.Length; i++)
            {
                list.Add(DataRowToModel<T>(drArray[i]));
            }

            return list;
        }

        #endregion

        #region 基本数据库增删改操作

        /// <summary>
        /// 通过实体类新增一条记录
        /// </summary>
        /// <param name="entity">新增的实体类对象</param>
        /// <param name="refresh">插入数据后，是否已实体类的方式返回新增的数据记录。
        ///                         如当前值置为True，实体类必须有主键属性</param>
        /// <returns>新增的实体类对象。该方法暂时只支持单主键处理方式。</returns>
        public static T Add<T>(T model, bool refresh) where T : Entity, new()
        {
            Entity entity = null;

            if (model is Entity)
            {
                entity = model as Entity;
            }

            // 该字段记录当前实体类定义的主键赋值方式是否是自增的方式。如果是自增的，则插入的时候这个不需要赋值
            // 如果是序列方式，并且这个字段为空，则需要明确的对其进行赋值

            /*
             * 对于插入数据的处理判断过程如下：
             * 
             * 1、对主键进行赋值：当前属性是否是主键，
             *                      当前主键是否是自增类型的
             *                              如果是，则不对其进行赋值处理，直接跳过
             *                      当前主键是否是序列类型的
             *                              如果是，则通过序列查找应赋的值
             *                      当前是主键，但以上两类都不是，则对其正常赋值，使用实体类带过来的内容
             *                    当前属性不是主键，对其正常赋值，使用实体类带过来的内容
             *                      
             * 2、刷新处理
             * 
             */

            GenerationType keyType = GenerationType.Auto;

            PropertyInfo[] props = entity.GetType().GetProperties();

            StringBuilder strColumns = new StringBuilder();
            StringBuilder strValues = new StringBuilder();

            List<SqlParameter> param = new List<SqlParameter>();

            object nextKeyValue = null;

            for (int i = 0; i < props.Length; i++)
            {
                if (!Util.IsTransient(props[i]))
                {
                    object value = props[i].GetValue(entity, null);

                    if (value != null)
                    {
                        bool isPrimayKey = Util.IsPrimayKey(props[i]);

                        if (isPrimayKey)
                        {
                            keyType = Util.GetPrimayKeyGenerationType<T>();

                            if (keyType == GenerationType.Auto)
                            {
                                // 此类型暂时不处理，应该在这里加入一点判断：
                                // 当前数据库的类型是什么，如果是SQL Server，则为 GenerationType.Identity
                                //                         如果是    Oracle，则为 GenerationType.Senquence
                            }

                            if (keyType == GenerationType.Identity)
                            {
                                // 自增类型的，什么也不干，跳过，不赋值
                            }
                            else if (keyType == GenerationType.Senquence)
                            {
                                strColumns.Append(props[i].Name + ", ");
                                strValues.Append("@" + props[i].Name + ", ");

                                // 查找序列，对其进行赋值
                                nextKeyValue = OR.SQLHelper.GetSingle("Select OR_Senquence.nextval From dual");
                                param.Add(new SqlParameter("@" + props[i].Name, nextKeyValue));
                            }
                            else if (keyType == GenerationType.Manually)
                            {
                                strColumns.Append(props[i].Name + ", ");
                                strValues.Append("@" + props[i].Name + ", ");
                                param.Add(new SqlParameter("@" + props[i].Name, value));
                            }
                        }
                        else
                        {
                            strColumns.Append(props[i].Name + ", ");
                            strValues.Append("@" + props[i].Name + ", ");
                            param.Add(new SqlParameter("@" + props[i].Name, value));
                        }
                    }
                }
            }


            /*
             * 对主键取值的处理
             * 1、实现判断当前实体类是否需要刷新
             *                  需要刷新的话，判断当前实体类的赋值方式
             *                                  如果是自增方式，返回@@IDENTITY
             *                                  序列方式，直接获取实体类里的序列内容，该值在上面的赋值过程中已经处理过了
             *                  不需要刷新，什么也不用做
             */

            StringBuilder strSQL = new StringBuilder();

            strSQL.Append("Insert into ");
            strSQL.Append(Util.GetTableName<T>());
            strSQL.Append("( " + strColumns.ToString().Trim(new char[] { ' ', ',' }) + " ) ");
            strSQL.Append(" values ");
            strSQL.Append("( " + strValues.ToString().Trim(new char[] { ' ', ',' }) + " ) ");

            object id = null;

            // 判断当前实体类内容是否需要刷新
            if (refresh)
            {
                // 当前实体类需要刷新，判断主键的赋值方式，决定采用不同的取值方式

                if (keyType == GenerationType.Identity)
                {
                    // 自增类型，
                    strSQL.Append(";Select @@IDENTITY");

                    id = SQLHelper.GetSingle(strSQL.ToString(), param.ToArray());
                }
                else if (keyType == GenerationType.Senquence)
                {
                    // 序列方式，直接返回上面的内容即可
                    id = nextKeyValue;

                    SQLHelper.ExecuteSql(strSQL.ToString(), param.ToArray());
                }
                else if (keyType == GenerationType.Manually)
                {
                    id = Util.GetEntityPrimayKeys<T>()[0].GetValue(model, null);
                    SQLHelper.GetSingle(strSQL.ToString(), param.ToArray());
                }
                else
                {
                    // 如果实体类指定的刷新为True，必须设置主键
                    id = Util.GetEntityPrimayKeys<T>()[0].GetValue(model, null);
                }

                // 需要刷新实体类，根据上面的ID内容，查找实体类
                strSQL = new StringBuilder();
                strSQL.Append(" Select " + Util.GetEntityMembersName<T>() + " From " + Util.GetTableName<T>() + " WHERE " + Util.GetEntityPrimayKeysName<T>() + "=@id");
                DataSet ds = SQLHelper.Query(strSQL.ToString(), Util.GetTableConnectionName<T>(), new SqlParameter("@id", id));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    model = DataRowToModel<T>(ds.Tables[0].Rows[0]);
                }
            }
            else
            {
                // 不需要刷新实体类，直接执行SQL，返回即可
                SQLHelper.ExecuteSql(strSQL.ToString(), param.ToArray());
            }

            return model;
        }

        /// <summary>
        /// 通过实体类对象删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Delete<T>(T model) where T : Entity
        {
            return Delete<T>(Util.GetPrimayKeyValue<T>(model));
        }

        /// <summary>
        /// 通过实体类的主键值删除对应的数据。这里只支持单主键，联合主键的下一个版本处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelKey"></param>
        /// <returns></returns>
        public static int Delete<T>(Object modelKeyValue) where T : Entity
        {
            String strSQL = "Delete From " + Util.GetTableName<T>() + " where " + Util.GetEntityPrimayKeysName<T>() + " = @" + Util.GetEntityPrimayKeysName<T>();
            return SQLHelper.ExecuteSql(strSQL, Util.GetTableConnectionName<T>(), new SqlParameter("@" + Util.GetEntityPrimayKeysName<T>(), modelKeyValue));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        public static int Update<T>(T model) where T : Entity
        {
            Entity entity = null;

            if (model is Entity)
            {
                entity = model as Entity;
            }

            PropertyInfo[] props = entity.GetType().GetProperties();

            StringBuilder strWhere = new StringBuilder();

            List<SqlParameter> param = new List<SqlParameter>();

            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(" Update " + Util.GetTableName<T>() + " Set ");

            for (int i = 0; i < props.Length; i++)
            {
                if (!Util.IsTransient(props[i]))
                {
                    object value = props[i].GetValue(entity, null);

                    if (!Util.IsPrimayKey(props[i]))
                    {
                        strSQL.Append(props[i].Name + "=@" + props[i].Name + ", ");
                        param.Add(new SqlParameter("@" + props[i].Name, value));
                    }
                    else
                    {
                        strWhere.Append(props[i].Name + "=@" + props[i].Name);
                    }
                }
            }

            strSQL.Remove(strSQL.Length - 2, 2);
            strSQL.Append(" Where ");
            strSQL.Append(strWhere);
            param.Add(new SqlParameter("@" + Util.GetEntityPrimayKeys<T>()[0].Name, Util.GetEntityPrimayKeys<T>()[0].GetValue(model, null)));

            // 执行语句，返回结果
            return SQLHelper.ExecuteSql(strSQL.ToString(), Util.GetTableConnectionName<T>(), param.ToArray());
        }

        #endregion

    }
}