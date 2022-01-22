using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;

namespace ZHONGJIAN_API.DAL.sect
{
    public class SectDao
    {
        /// <summary>
        /// Edoc数据连接字符串
        /// </summary>
        private string edocMySqlConnectString = ConfigHelper.Conn;

        /// <summary>
        /// 根据ID获取档案表名
        /// </summary>
        /// <param name="archTypeId"></param>
        /// <returns></returns>
        public string GetArchTableName(string archTypeId)
        {

            string sql = @"SELECT arch_table_name FROM eform_edrms_sect_and_arch_type where id=?id";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("?id",archTypeId)
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            return ds.Tables[0].Rows[0][0].ToString();
        }

        /// <summary>
        /// 根据ID获取册表名
        /// </summary>
        /// <param name="archTypeId"></param>
        /// <returns></returns>
        public string GetDossierTableName(string archTypeId)
        {

            string sql = @"SELECT dossier_table_name FROM eform_edrms_sect_and_arch_type where id=?id";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("?id",archTypeId)
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(sql, mysqlParams);
            return ds.Tables[0].Rows[0][0].ToString();
        }

        /// <summary>
        /// 根据文件版本ID获取存储路径
        /// </summary>
        /// <param name="verId"></param>
        /// <returns></returns>
        public string GetFileFullPathByVerId(int verId)
        {
            string result = "";
            string strSql = @"select e.storage_path from dms_fileentityinfo as e 
                    join dms_filestorageinfo as s on e.fileEntity_id = s.fileEntity_id 
                    where s.file_verId = ?file_verId order by id asc limit 1;";
            var mysqlParams = new MySqlParameter[] {
                new MySqlParameter("?file_verId",verId)
            };
            DataSet ds = new MySqlDBHelper(ConfigHelper.Conn).ExecuteDataSet(strSql, mysqlParams);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                result = ds.Tables[0].Rows[0]["storage_path"].ToString();
            }

            return result;
        }


    }
}
