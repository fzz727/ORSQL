using System;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Common;
using System.Collections.Generic;

namespace OR
{
    /// <summary>
    /// 数据访问抽象基础类
    /// </summary>
    public abstract class SQLHelper
    {
        //数据库连接字符串(web.config来配置)，可以动态更改connectionString支持多数据库.		
        //public static String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnection"].ConnectionString;

        public static String defaultConnectionName = "SQLConnection";

        #region ExecuteSql

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(String SQLString, params SqlParameter[] cmdParms)
        {
            return ExecuteSql(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(String SQLString, String connectionName, params SqlParameter[] cmdParms)
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
        #endregion
        
        #region GetSingle

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params SqlParameter[] cmdParms)
        {
            return GetSingle(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <param name="connectionName">连接字符串名称</param>
        /// <param name="cmdParms">查询结果（object）</param>
        /// <returns></returns>
        public static object GetSingle(string SQLString, String connectionName, params SqlParameter[] cmdParms)
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        #endregion

        #region ExecuteReader
        
        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
        {
            return ExecuteReader(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// 执行查询语句，返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <param name="connectionName">链接字符串名称</param>
        /// <param name="cmdParms">查询参数</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString,String connectionName, params SqlParameter[] cmdParms)
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        SqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.Parameters.Clear();
                        return myReader;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
        
        #endregion
        
        #region Query

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            return Query(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString,String connectionName, params SqlParameter[] cmdParms)
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }

        /// <summary>
        /// sql分页处理方法。
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="_RowCount"></param>
        /// <param name="_PageCount"></param>
        /// <returns></returns>
        public static DataTable Query(String strSQL, int pageIndex, int pageSize, ref int _RowCount, ref int _PageCount)
        {
            return Query(strSQL, defaultConnectionName, pageIndex, pageSize, ref _RowCount, ref _PageCount);
        }

        /// <summary>
        /// sql分页处理方法。
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="_RowCount"></param>
        /// <param name="_PageCount"></param>
        /// <returns></returns>
        public static DataTable Query(String strSQL, String connectionName, int pageIndex, int pageSize, ref int _RowCount, ref int _PageCount)
        {
            System.Text.StringBuilder strPageQuery = new System.Text.StringBuilder();

            strPageQuery.Append(" DECLARE @CurrentPageIndex int ");             // 当前检索的页数
            strPageQuery.Append(" DECLARE @PageSize int ");                     // 每页大小

            strPageQuery.Append(" DECLARE @RowCount int ");                     // 内部使用的变量，用于返回检索结果数量

            strPageQuery.Append(" SET @PageSize=" + pageSize.ToString() + " ");
            strPageQuery.Append(" SET @CurrentPageIndex = " + (pageIndex * pageSize + 1).ToString() + " ");

            strPageQuery.Append(" DECLARE @p1 int ");

            strPageQuery.Append(" EXEC    sp_cursoropen  @p1 output, @SelectCommandText, @scrollopt = 1, @ccopt = 1, @RowCount = @RowCount output; ");
            strPageQuery.Append(" EXEC    sp_cursorfetch @p1, 16, @CurrentPageIndex, @PageSize; ");
            strPageQuery.Append(" EXEC    sp_cursorclose @p1 ");

            strPageQuery.Append(" Select @RowCount ");

            SqlParameter param = new SqlParameter("@SelectCommandText", strSQL);

            DataSet dataSet = Query(strPageQuery.ToString(), param);

            _RowCount = Convert.ToInt32(dataSet.Tables[2].Rows[0][0].ToString());

            _PageCount = (int)(_RowCount + pageSize - 1) / pageSize;

            return dataSet.Tables[1];

        }
        
        #endregion

        #region ExecSPDataSet

        public DataSet ExecSPDataSet(string procName, params SqlParameter[] cmdParms)
        {
            return ExecSPDataSet(procName, defaultConnectionName, cmdParms);
        }

        public DataSet ExecSPDataSet(string procName, string connectionName, params SqlParameter[] cmdParms)
        {
            String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand sqlcom = new SqlCommand(procName, conn);
            sqlcom.CommandType = CommandType.StoredProcedure;

            foreach (System.Data.IDataParameter paramer in cmdParms)
            {
                sqlcom.Parameters.Add(paramer);
            }

            conn.Open();

            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = sqlcom;
            DataSet ds = new DataSet();
            da.Fill(ds);

            conn.Close();
            return ds;
        }

        #endregion

        #region PrepareCommand

        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

    }
}
