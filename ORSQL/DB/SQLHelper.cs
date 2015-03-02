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
    /// ���ݷ��ʳ��������
    /// </summary>
    public abstract class SQLHelper
    {
        //���ݿ������ַ���(web.config������)�����Զ�̬����connectionString֧�ֶ����ݿ�.		
        //public static String connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnection"].ConnectionString;

        public static String defaultConnectionName = "SQLConnection";

        #region ExecuteSql

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSql(String SQLString, params SqlParameter[] cmdParms)
        {
            return ExecuteSql(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <returns>Ӱ��ļ�¼��</returns>
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
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="SQLString">�����ѯ������</param>
        /// <returns>��ѯ�����object��</returns>
        public static object GetSingle(string SQLString, params SqlParameter[] cmdParms)
        {
            return GetSingle(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="SQLString">�����ѯ������</param>
        /// <param name="connectionName">�����ַ�������</param>
        /// <param name="cmdParms">��ѯ�����object��</param>
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
        /// ִ�в�ѯ��䣬����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="strSQL">��ѯ���</param>
        /// <returns>SqlDataReader</returns>
        public static SqlDataReader ExecuteReader(string SQLString, params SqlParameter[] cmdParms)
        {
            return ExecuteReader(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����SqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��SqlDataReader����Close )
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <param name="connectionName">�����ַ�������</param>
        /// <param name="cmdParms">��ѯ����</param>
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
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SqlParameter[] cmdParms)
        {
            return Query(SQLString, defaultConnectionName, cmdParms);
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
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
        /// sql��ҳ��������
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
        /// sql��ҳ��������
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

            strPageQuery.Append(" DECLARE @CurrentPageIndex int ");             // ��ǰ������ҳ��
            strPageQuery.Append(" DECLARE @PageSize int ");                     // ÿҳ��С

            strPageQuery.Append(" DECLARE @RowCount int ");                     // �ڲ�ʹ�õı��������ڷ��ؼ����������

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
