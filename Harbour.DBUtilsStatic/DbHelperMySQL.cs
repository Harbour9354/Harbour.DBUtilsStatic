using System;
using System.Collections;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
namespace Harbour.DBUtilsStatic
{
    /// <summary>
    /// MySQL���ݿ���ʰ�����
    /// </summary>
    public abstract class DbHelperMySQL
    {

        /// <summary>
        /// ���ݿ������ַ���(web.config������)�����Զ�̬����connectionString֧�ֶ����ݿ�.		
        /// </summary>
        public static string connectionString;

        #region ���÷���

        /// <summary>
        /// �ж��Ƿ����ĳ���ĳ���ֶ�
        /// </summary>
        /// <param name="tableName">������</param>
        /// <param name="columnName">������</param>
        /// <returns>�Ƿ����</returns>
        public static bool ColumnExists(string tableName, string columnName)
        {
            string sql = " desc " + " tableName " + " columnName ";
            object res = GetSingle(sql);
            if (res == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// �õ����ֵ
        /// </summary>
        /// <param name="FieldName">������</param>
        /// <param name="TableName">����</param>
        /// <returns></returns>
        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        /// <summary>
        /// ���Ƿ����
        /// </summary>
        /// <param name="TableName">����</param>
        /// <returns></returns>
        public static bool TabExists(string TableName)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                            connection.Open();
                        string strsql = "select count(1) from INFORMATION_SCHEMA.TABLES where TABLE_SCHEMA='" + connection.Database + "' and TABLE_NAME='" + TableName + "'";
                        cmd.Connection = connection;
                        cmd.CommandText = strsql;
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            return false;
                        else
                            return int.Parse(obj.ToString()) > 0 ? true : false;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// �Ƿ����
        /// </summary>
        /// <param name="strSql">SQL</param>
        /// <param name="cmdParms">����</param>
        /// <returns></returns>
        public static bool Exists(string strSql, params MySqlParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region ִ��SQL���

        /// <summary>
        /// ִ��SQL��䣬����Ӱ��ļ�¼��
        /// </summary>
        /// <param name="SQLString">SQL���</param>
        /// <param name="cmdParms">����</param>
        /// <returns>Ӱ��ļ�¼��</returns>
        public static int ExecuteSql(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }
        /// <summary>
        /// ִ��һ�������ѯ�����䣬���ز�ѯ�����object����
        /// </summary>
        /// <param name="SQLString">�����ѯ������</param>
        /// <param name="cmdParms">�����б�</param>
        /// <returns>��ѯ�����object��</returns>
        public static object GetSingle(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                            return null;
                        else
                            return obj;
                    }
                    catch (MySql.Data.MySqlClient.MySqlException e)
                    {
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����MySqlDataReader ( ע�⣺���ø÷�����һ��Ҫ��MySqlDataReader����Close )
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <param name="cmdParms">�����б�</param>
        /// <returns>MySqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(string SQLString, params MySqlParameter[] cmdParms)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                MySqlDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (MySql.Data.MySqlClient.MySqlException e)
            {
                throw e;
            }
        }
        /// <summary>
        /// ��ȡʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQLString">��ѯ���</param>
        /// <param name="cmdParms">�����б�</param>
        /// <returns></returns>
        public static T ExeReaEntity<T>(string SQLString, params MySqlParameter[] cmdParms) where T : new()
        {
            return EntityConverter.ToEntity<T>(ExecuteReader(SQLString, cmdParms));
        }
        /// <summary>
        /// ��ȡʵ�弯��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQLString">��ѯ���</param>
        /// <param name="cmdParms">�����б�</param>
        /// <returns></returns>
        public static List<T> ExeReaEntList<T>(string SQLString, params MySqlParameter[] cmdParms) where T : new()
        {
            return EntityConverter.ToList<T>(ExecuteReader(SQLString, cmdParms));
        }

        /// <summary>
        /// ִ�в�ѯ��䣬����DataSet
        /// </summary>
        /// <param name="SQLString">��ѯ���</param>
        /// <param name="cmdParms">�����б�</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }
        /// <summary>
        /// ��ѯ�����ʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQLString">��ѯ���</param>
        /// <param name="cmdParms">�����б�</param>
        /// <returns></returns>
        public static T QueryEntity<T>(string SQLString, params MySqlParameter[] cmdParms) where T : new()
        {
            DataSet ds = Query(SQLString, cmdParms);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return EntityConverter.ToEntity<T>(ds.Tables[0].Rows[0]);
            }
            else
            {
                return default(T);
            }
        }
        /// <summary>
        /// ��ѯ�����ʵ��
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="SQLString">��ѯ���</param>
        /// <param name="cmdParms">�����б�</param>
        /// <returns></returns>
        public static List<T> QueryEntList<T>(string SQLString, params MySqlParameter[] cmdParms) where T : new()
        {
            DataSet ds = Query(SQLString, cmdParms);
            return EntityConverter.ToList<T>(ds.Tables[0]);
        }

        /// <summary>
        /// ׼������
        /// </summary>
        /// <param name="cmd">MySqlCommand</param>
        /// <param name="conn">MySqlConnection</param>
        /// <param name="trans">MySqlTransaction</param>
        /// <param name="cmdText">SQL</param>
        /// <param name="cmdParms">MySqlParameter</param>
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms != null)
            {
                foreach (MySqlParameter parameter in cmdParms)
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

        #region ���ݿ��ҳ

        /// <summary>
        /// ���ݿ��ҳ
        /// </summary>
        /// <param name="SQLString">SQL</param>
        /// <param name="PageSize">��ҳ��С</param>
        /// <param name="PageIndex">ҳ��</param>
        /// <param name="cmdParms">����</param>
        /// <returns></returns>
        public static DataSet DBPaging(string SQLString, int PageSize, int PageIndex, params MySqlParameter[] cmdParms)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand cmd = new MySqlCommand();
                PrepareCommand(cmd, connection, null, SQLString + " LIMIT " + (PageIndex - 1) * PageSize + 1 + "," + PageSize, cmdParms);
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    return ds;
                }
            }
        }
        /// <summary>
        /// ���ݿ��ҳ
        /// </summary>
        /// <param name="SQLString">SQL</param>
        /// <param name="PageSize">��ҳ��С</param>
        /// <param name="PageIndex">ҳ��</param>
        /// <param name="cmdParms">����</param>
        /// <returns></returns>
        public static List<T> DBPagEntList<T>(string SQLString, int PageSize, int PageIndex, params MySqlParameter[] cmdParms) where T : new()
        {
            return EntityConverter.ToList<T>(DBPaging(SQLString, PageSize, PageIndex, cmdParms).Tables[0]);
        }
        #endregion

        #region �������

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">����SQL���</param>
        /// <returns>��Ӱ���������</returns>
        public static int ExecuteSqlTran(List<String> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    int count = 0;
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n];
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            count += cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return count;
                }
                catch
                {
                    tx.Rollback();
                    return 0;
                }
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">SQL���Ĺ�ϣ��keyΪsql��䣬value�Ǹ�����MySqlParameter[]��</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="cmdList">List(CommandInfo)�б�</param>
        /// <returns>��Ӱ�������</returns>
        public static int ExecuteSqlTran(List<CommandInfo> cmdList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int count = 0;
                        //ѭ��
                        foreach (CommandInfo myDE in cmdList)
                        {
                            string cmdText = myDE.CommandText;
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                            {
                                if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                {
                                    trans.Rollback();
                                    return 0;
                                }

                                object obj = cmd.ExecuteScalar();
                                bool isHave = false;
                                if (obj == null && obj == DBNull.Value)
                                {
                                    isHave = false;
                                }
                                isHave = Convert.ToInt32(obj) > 0;

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                continue;
                            }
                            int val = cmd.ExecuteNonQuery();
                            count += val;
                            if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                            {
                                trans.Rollback();
                                return 0;
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                        return count;
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">List(CommandInfo)�б�</param>
        /// <returns>��Ӱ�������</returns>
        public static void ExecuteSqlTranWithIndentity(List<CommandInfo> SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int indentity = 0;
                        //ѭ��
                        foreach (CommandInfo myDE in SQLStringList)
                        {
                            string cmdText = myDE.CommandText;
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// ִ�ж���SQL��䣬ʵ�����ݿ�����
        /// </summary>
        /// <param name="SQLStringList">List(CommandInfo)�б�</param>
        /// <returns>��Ӱ�������</returns>
        public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                using (MySqlTransaction trans = conn.BeginTransaction())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    try
                    {
                        int indentity = 0;
                        //ѭ��
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Value;
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.InputOutput)
                                {
                                    q.Value = indentity;
                                }
                            }
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            foreach (MySqlParameter q in cmdParms)
                            {
                                if (q.Direction == ParameterDirection.Output)
                                {
                                    indentity = Convert.ToInt32(q.Value);
                                }
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /*
        /// <summary>
        /// ִ��Sql��Oracle�λ������
        /// </summary>
        /// <param name="list">SQL�������б�</param>
        /// <param name="oracleCmdSqlList">Oracle�������б�</param>
        /// <returns>ִ�н�� 0-����SQL�������ʧ�� -1 ����Oracle�������ʧ�� 1-��������ִ�гɹ�</returns>
        public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    foreach (CommandInfo myDE in list)
                    {
                        string cmdText = myDE.CommandText;
                        MySqlParameter[] cmdParms = (MySqlParameter[])myDE.Parameters;
                        PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
                        if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
                        {
                            if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                            {
                                tx.Rollback();
                                throw new Exception("Υ��Ҫ��" + myDE.CommandText + "�������select count(..�ĸ�ʽ");
                                //return 0;
                            }

                            object obj = cmd.ExecuteScalar();
                            bool isHave = false;
                            if (obj == null && obj == DBNull.Value)
                            {
                                isHave = false;
                            }
                            isHave = Convert.ToInt32(obj) > 0;
                            if (isHave)
                            {
                                //�����¼�
                                myDE.OnSolicitationEvent();
                            }
                        }
                        if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                        {
                            if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                            {
                                tx.Rollback();
                                throw new Exception("SQL:Υ��Ҫ��" + myDE.CommandText + "�������select count(..�ĸ�ʽ");
                                //return 0;
                            }

                            object obj = cmd.ExecuteScalar();
                            bool isHave = false;
                            if (obj == null && obj == DBNull.Value)
                            {
                                isHave = false;
                            }
                            isHave = Convert.ToInt32(obj) > 0;

                            if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                            {
                                tx.Rollback();
                                throw new Exception("SQL:Υ��Ҫ��" + myDE.CommandText + "����ֵ�������0");
                                //return 0;
                            }
                            if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                            {
                                tx.Rollback();
                                throw new Exception("SQL:Υ��Ҫ��" + myDE.CommandText + "����ֵ�������0");
                                //return 0;
                            }
                            continue;
                        }
                        int val = cmd.ExecuteNonQuery();
                        if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                        {
                            tx.Rollback();
                            throw new Exception("SQL:Υ��Ҫ��" + myDE.CommandText + "������Ӱ����");
                            //return 0;
                        }
                        cmd.Parameters.Clear();
                    }
                    string oraConnectionString = PubConstant.GetConnectionString("ConnectionStringPPC");
                    bool res = OracleHelper.ExecuteSqlTran(oraConnectionString, oracleCmdSqlList);
                    if (!res)
                    {
                        tx.Rollback();
                        throw new Exception("ִ��ʧ��");
                        // return -1;
                    }
                    tx.Commit();
                    return 1;
                }
                catch (MySql.Data.MySqlClient.MySqlException e)
                {
                    tx.Rollback();
                    throw e;
                }
                catch (Exception e)
                {
                    tx.Rollback();
                    throw e;
                }
            }
        }
        */

        #endregion

    }

}
