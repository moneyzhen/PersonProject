using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;

namespace ZHONGJIAN_API.DAL.Edoc2Api
{
    public class Edoc2DownLoadHelper
    {
        public static string Edoc2BaseUrl { get; set; }
        static Edoc2DownLoadHelper()
        {
            Edoc2BaseUrl = ConfigHelper.Edoc2BaseUrl;
        }

        /// <summary>
        /// 验证下载
        /// </summary>
        /// <returns></returns>
        public static DownLoadCheckResult DownLoadCheck(string token, string fileIds, string wmk, string rn)
        {
            try
            {
                var builder = new UriBuilder(Edoc2BaseUrl);
                builder.Path = Path.Combine(builder.Path, $"/downLoad/DownLoadCheck");
                var uri = builder.ToString();
                //请求信息
                string getUrl = $"{uri}?fileIds={fileIds}&token={token}&wmk={wmk}&r={rn}";
                var rData = HttpClientHelper.Get(getUrl);
                var result = JsonConvert.DeserializeObject<DownLoadCheckResult>(rData);

                return result;
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                return new DownLoadCheckResult();
            }
        }

        /// <summary>
        /// 下载流(单个文件下载)
        /// </summary>
        /// <returns></returns>
        public static Stream DownLoadIndexSingle(string uploadServer, string token, string fileId, string regionHash, string wmk, string rn)
        {
            try
            {
                var builder = new UriBuilder(uploadServer);
                builder.Path = Path.Combine(builder.Path, $"/downLoad/index");
                var uri = builder.ToString();
                //请求信息
                string getUrl = $"{uri}?regionHash={regionHash}&fileIds={fileId}&token={token}&wmk={wmk}&r={rn}";
                Stream stream = HttpClientHelper.GetStream(getUrl);
                return stream;
            }
            catch (Exception ex)
            {
                LogHelper.ExceptionLog(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 获取压缩包文件流
        /// </summary>
        /// <returns></returns>
        public static Stream GetFileStream(string fileId,string token)
        {
            var uploadServer = ConfigHelper.Edoc2BaseUrl;
            string rn = ConfigHelper.CurrentTimeNumberfff;
            //检查文件
            var downLoadResult = Edoc2DownLoadHelper.DownLoadCheck(token, fileId, "测试水印001", rn);
            if (downLoadResult != null && downLoadResult.nResult == 0)
            {
                //分区域改变地址
                if (downLoadResult.RegionType != 1)
                {
                    uploadServer = $"http://{downLoadResult.RegionUrl}";
                }

                var stream = Edoc2DownLoadHelper.DownLoadIndexSingle(uploadServer, token, fileId, downLoadResult.RegionHash, "测试水印001", rn);
                return stream;
            }
            return null;
        }
    }

    /// <summary>
    /// 检查下载结果
    /// </summary>
    public class DownLoadCheckResult
    {
        public DownLoadCheckResult()
        {
            nResult = -1;
        }

        /// <summary>
        /// 本次下载的hash码
        /// </summary>
        public string RegionHash { get; set; }
        // 区域编号
        public int RegionId { get; set; }
        /// <summary>
        ///  区域类型,1: 主区域，2: 分区域
        /// </summary>
        public int RegionType { get; set; }
        /// <summary>
        /// 区域地址（RegionType=1时为空）
        /// </summary>
        public string RegionUrl { get; set; }
        /// <summary>
        /// // 外发里$&quot; etc..{outLength}&quot;;//这个是约定格式，前端将etc.. 解析为等
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        ///  错误码，值=0表示成功
        /// </summary>
        public int nResult { get; set; }
    }

    /// <summary>
    /// 下载流结果
    /// </summary>
    public class DownLoadIndexResult
    {
        public DownLoadIndexResult()
        {
            nResult = -1;
        }
        /// <summary>
        /// 若是批量下载且async=true时，用于客户端请求批量下载的文件是否已经压缩完成，压缩完成表示可以请求下载压缩文件了。
        /// </summary>
        public string pTaskId { get; set; }
        /// <summary>
        ///  错误码，值=0表示成功
        /// </summary>
        public int nResult { get; set; }
    }

}
