using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;

namespace ZHONGJIAN_API.DAL.eformForm
{
    public class FormDao
    {
        /// <summary>
        /// 根据id查询表单数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetFormById(string id)
        {
            string sql = $"SELECT * from eform_forms  where Id = @id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@id",id),
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }

        /// <summary>
        /// 根据id查询指定表数据
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetDataById(string tablename,string id)
        {
            string sql = $"SELECT * from {tablename}  where Id = @id;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("@id",id),
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            DataTable dt = ds.Tables[0];
            return dt;
        }



    }
}
