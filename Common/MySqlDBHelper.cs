using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Common
{
    public class MySqlDBHelper
    {
        private MySqlCommand command = null;
        private MySqlConnection connection = null;

        //初始化
        public MySqlDBHelper(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            //判断状态
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }

        #region 执行带参数的SQL语句
        /// <summary>
        /// 执行SQL语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(string sql, params MySqlParameter[] parms)
        {
            //初始化对象
            command = new MySqlCommand(sql, connection);
            command.CommandType = CommandType.Text; //cmdType
            if (parms != null && parms.Length > 0)
            {
                //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value
                foreach (MySqlParameter parameter in parms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
            {
                DataSet ds = new DataSet();
                try
                {
                    adapter.Fill(ds);
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                connection.Close();
                return ds;
            }
        }

        /// <summary>
        /// 执行存储过程，返回DataSet
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="parms">查询参数</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteProcDataSet(string procName, params MySqlParameter[] parms)
        {
            //初始化对象
            command = new MySqlCommand(procName, connection);
            command.CommandType = CommandType.StoredProcedure; //cmdType
            if (parms != null && parms.Length > 0)
            {
                //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value
                foreach (MySqlParameter parameter in parms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    command.Parameters.Add(parameter);
                }
            }
            using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
            {
                DataSet ds = new DataSet();
                try
                {
                    adapter.Fill(ds);
                }
                catch (MySqlException ex)
                {
                    throw new Exception(ex.Message);
                }
                connection.Close();
                return ds;
            }
        }

        #region 存储过程调用
        /// <summary>
        /// 存储过程执行并返回总条数
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="totalRecord">存储过程返回总条数</param>
        /// <param name="parms">存储过程输入参数</param>
        /// <returns></returns>
        public DataTable GetDataByProcedureName(string procName, out int totalRecord, MySqlParameter[] parms)
        {
            // 获取存储过程所有参数
            var procParams = GetProcedureParams(procName);
            MySqlParameter totalRecordParam = null;//输入总数
            if (procParams != null && procParams.Count > 0 && parms != null && parms.Length > 0)
            {
                // 存储过程输入参数赋值
                foreach (MySqlParameter proparameter in procParams)
                {
                    var name = proparameter.ParameterName.ToLower();
                    //输出总数
                    if (totalRecordParam == null && (name.ToLower() == "@totalrecord" || name.ToLower() == "totalrecord"))
                    {
                        totalRecordParam = proparameter;
                    }
                    foreach (MySqlParameter parameter in parms)
                    {
                        if (name == parameter.ParameterName.ToLower() || (name == "@" + parameter.ParameterName.ToLower()))
                        {
                            // 存储过程输入参数赋值
                            proparameter.Value = parameter.Value;
                            break;
                        }
                    }
                }
            }

            totalRecord = 0;
            DataTable dataTable = ExecuteProcDataSet(procName, procParams.ToArray()).Tables[0];
            //输出总数
            if (totalRecordParam != null)
            {
                totalRecord = Convert.ToInt32(totalRecordParam.Value.ToString());
            }
            else
            {
                totalRecord = dataTable.Rows.Count;
            }

            return dataTable;

        }
        /// <summary>
        /// 获取存储过程所有参数
        /// </summary>
        /// <param name="procedureName">存储过程名称</param>
        /// <returns></returns>
        public List<MySqlParameter> GetProcedureParams(string procedureName)
        {
            string sql = @"select `PARAMETER_NAME` as `NAME`,`PARAMETER_MODE` as `PARAMDIRECTION`,`DATA_TYPE` as `TYPENAME`,`DTD_IDENTIFIER` AS `CHARTYPE`,`CHARACTER_OCTET_LENGTH` AS `CHARLENGTH`,`NUMERIC_PRECISION` AS `NUMLENGTH` from information_schema.PARAMETERS where SPECIFIC_SCHEMA=@database and SPECIFIC_NAME = @procedureName and ROUTINE_TYPE = 'PROCEDURE'";

            List<MySqlParameter> data = null;
            try
            {
                var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@database","edoc2v5"),//数据库名
                new MySqlParameter("@procedureName",procedureName),
            };
                DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
                DataTable table = ds.Tables[0];

                data = new List<MySqlParameter>();
                foreach (System.Data.DataRow row in table.Rows)
                {
                    string name = row["NAME"].ToString();
                    MySqlDbType myDbtype = MySqlDbTypeToDataType(row["TYPENAME"].ToString());
                    DbType dbtype = DbTypeToDataType(row["TYPENAME"].ToString());
                    string strParamDirection = row["PARAMDIRECTION"].ToString().ToUpper();
                    string chartype = row["CHARTYPE"].ToString();
                    string charlength = row["CHARLENGTH"].ToString();
                    string numlength = row["NUMLENGTH"].ToString();

                    ParameterDirection parameterDirection = ParameterDirection.Input;
                    if (strParamDirection == "OUT")
                    {
                        parameterDirection = ParameterDirection.Output;
                    }
                    else if (strParamDirection == "IN/OUT")
                    {
                        parameterDirection = ParameterDirection.InputOutput;
                    }
                    else if (strParamDirection == "INOUT")
                    {
                        parameterDirection = ParameterDirection.InputOutput;
                    }
                    else
                    {
                        parameterDirection = ParameterDirection.Input;
                    }
                    int size = 0;
                    if (!string.IsNullOrEmpty(chartype))
                    {
                        int splitIndex = chartype.IndexOf('(');
                        int splitIndex2 = chartype.IndexOf(')');
                        string sizestr = chartype.Substring(splitIndex + 1, splitIndex2 - splitIndex - 1);
                        size = int.Parse(sizestr);
                    }

                    data.Add(new MySqlParameter()
                    {
                        ParameterName = "@" + name,
                        Value = string.Empty,
                        MySqlDbType = myDbtype,
                        //DbType = dbtype,
                        Size = size,
                        Direction = parameterDirection
                    });

                }

            }
            catch (Exception ex)
            {
                data = new List<MySqlParameter>();
            }
            return data;
        }
        /// <summary>
        /// 根据数据库缓存TYPENAME转换MySqlDbType类型
        /// </summary>
        /// <param name="typename"></param>
        /// <returns></returns>
        public MySqlDbType MySqlDbTypeToDataType(string typename)
        {
            MySqlDbType dataTypes = MySqlDbType.String;
            switch (typename.ToLower())
            {
                case "int":
                    dataTypes = MySqlDbType.Int32;
                    break;
                case "nvarchar":
                case "varchar":
                    dataTypes = MySqlDbType.VarChar;
                    break;
                case "string":
                case "text":
                case "char":
                case "nchar":
                    dataTypes = MySqlDbType.String;
                    break;
                case "bit":
                    dataTypes = MySqlDbType.Bit;
                    break;
                case "datetime":
                    dataTypes = MySqlDbType.DateTime;
                    break;
                case "decimal":
                case "number":
                case "numeric":
                    dataTypes = MySqlDbType.Decimal;
                    break;
                case "float":
                    dataTypes = MySqlDbType.Double;
                    break;
                case "smalldatetime":
                    dataTypes = MySqlDbType.DateTime;
                    break;
                case "smallint":
                    dataTypes = MySqlDbType.Int16;
                    break;
                case "bigint":
                    dataTypes = MySqlDbType.Int64;
                    break;
                case "binary":
                    dataTypes = MySqlDbType.Binary;
                    break; ;
                case "timestamp":
                    dataTypes = MySqlDbType.DateTime;
                    break;
                default:
                    dataTypes = MySqlDbType.String;
                    break;
            }

            return dataTypes;
        }
        /// <summary>
        /// 根据数据库缓存TYPENAME转换DbType类型
        /// </summary>
        /// <param name="dbType"></param>
        /// <returns></returns>
        public DbType DbTypeToDataType(string typename)
        {
            DbType dataTypes = DbType.String;
            switch (typename.ToLower())
            {
                case "int":
                    dataTypes = DbType.Int32;
                    break;
                case "nvarchar":
                case "varchar":
                    dataTypes = DbType.String;
                    break;
                case "string":
                case "text":
                case "char":
                case "nchar":
                    dataTypes = DbType.String;
                    break;
                case "bit":
                    dataTypes = DbType.Boolean;
                    break;
                case "datetime":
                    dataTypes = DbType.DateTime;
                    break;
                case "decimal":
                case "number":
                case "numeric":
                    dataTypes = DbType.Decimal;
                    break;
                case "float":
                    dataTypes = DbType.Double;
                    break;
                case "smalldatetime":
                    dataTypes = DbType.DateTime2;
                    break;
                case "smallint":
                    dataTypes = DbType.Int16;
                    break;
                case "bigint":
                    dataTypes = DbType.Int64;
                    break;
                case "binary":
                    dataTypes = DbType.Binary;
                    break; ;
                case "timestamp":
                    dataTypes = DbType.DateTimeOffset;
                    break;
                default:
                    dataTypes = DbType.String;
                    break;
            }

            return dataTypes;
        } 
        #endregion

        /// <summary>
        /// 执行SQL语句，返回影响的行数
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="cmdParms"></param>
        /// <returns>影响的行数</returns>
        public int ExecuteNonQuery(string sql, params MySqlParameter[] parms)
        {
            try
            {
                //初始化对象
                command = new MySqlCommand(sql, connection);
                command.CommandType = CommandType.Text; //cmdType
                if (parms != null && parms.Length > 0)
                {
                    //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value
                    foreach (MySqlParameter parameter in parms)
                    {
                        if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }
                int rows = command.ExecuteNonQuery();
                command.Parameters.Clear();
                connection.Close();
                return rows;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 执行SQL语句，返回结果集中的第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="parms"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params MySqlParameter[] parms)
        {
            try
            {
                //初始化对象
                command = new MySqlCommand(sql, connection);
                command.CommandType = CommandType.Text; //cmdType
                if (parms != null && parms.Length > 0)
                {
                    //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value
                    foreach (MySqlParameter parameter in parms)
                    {
                        if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }
                        command.Parameters.Add(parameter);
                    }
                }

                object retval = command.ExecuteScalar();
                command.Parameters.Clear();
                connection.Close();
                return retval;
            }
            catch (MySqlException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region     多sql操作
        public int ExecuteTran(List<MultipleSql> items)
        {
            using (connection)
            {
                //connection.Open();
                int records = 0;
                MySqlCommand command = connection.CreateCommand();
                MySqlTransaction transaction;

                //启动事务
                transaction = connection.BeginTransaction();
                //设定SqlCommand的事务和连接对象
                command.Connection = connection;
                command.Transaction = transaction;
                try
                {
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            command.CommandText = item.SqlStr;
                            if (item.Params != null)
                            {
                                //预处理MySqlParameter参数数组，将为NULL的参数赋值为DBNull.Value
                                foreach (MySqlParameter parameter in item.Params)
                                {
                                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
                                    {
                                        parameter.Value = DBNull.Value;
                                    }
                                    command.Parameters.Add(parameter);
                                }
                                //command.Parameters.AddRange(item.Params);
                            }
                            records += command.ExecuteNonQuery();
                            command.Parameters.Clear();
                        }
                    }
                    if (records > 0)
                    {
                        // 完成提交
                        transaction.Commit();
                    }
                }
                catch (Exception ex)
                {
                    //数据回滚
                    transaction.Rollback();
                    records = 0;
                }
                return records;
            }
        }
        #endregion

        #endregion
        public class MultipleSql
        {
            public MySqlParameter[] Params { get; set; }
            public string SqlStr { get; set; }
        }
    }

}
