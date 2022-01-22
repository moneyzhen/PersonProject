using EDoc2.IAppService.ResultModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;
using ZHONGJIAN_API.Edoc2Api;

namespace ZHONGJIAN_API.DAL.sect
{
    public class SectService
    {
        private readonly SectDao sectDao = new SectDao();

        /// <summary>
        /// 根据文件id获取文件信息
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public FileInfoForSdkResult GetFullFileInfoById(int fileId)
        {
            return EDoc2SdkHelper.GetFullFileInfoById(fileId);
        }

        /// <summary>
        /// 根据文件ID获取文件id值列表
        /// </summary>
        /// <param name="atFolderId"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public IList<int> GetFileIdsByFolderId(int atFolderId, string token = null)
        {
            return EDoc2SdkHelper.GetFileIdsByFolderId(atFolderId);
        }

        /// <summary>
        /// 根据文件版本ID获取存储路径
        /// </summary>
        /// <param name="verId"></param>
        /// <returns></returns>
        public string GetFileFullPathByVerId(int verId)
        {
            return sectDao.GetFileFullPathByVerId(verId);
        }


    }
}
