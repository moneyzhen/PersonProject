using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Common
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class ConfigHelper
    {
        public static IConfiguration _configuration { get; set; }


        /// <summary>
        ///  主数据库链接
        /// </summary>
        public static string Conn
        {
            get
            {
                if (_configuration["Conn"] != null && _configuration["Conn"] != "")
                {
                    return _configuration["Conn"].Trim();
                }
                return "server=10.101.8.154;userid=user;password=1qaz2WSX;database= edoc2v5;port=30001;Charset=utf8;";
            }
        }

        /// <summary>
        /// Edoc2基础网址
        /// </summary>
        public static string Edoc2BaseUrl
        {
            get
            {
                if (_configuration["Edoc2BaseUrl"] != null && _configuration["Edoc2BaseUrl"] != "")
                {
                    return _configuration["Edoc2BaseUrl"].Trim('/');
                }
                return "http://10.101.8.154";
            }
        }

        /// <summary>
        ///     Edoc2 SDK登录帐号
        /// </summary>
        public static string Edoc2SdkLoginName
        {
            get
            {
                if (_configuration["Edoc2SdkLoginName"] != null && _configuration["Edoc2SdkLoginName"] != "")
                {
                    return _configuration["Edoc2SdkLoginName"].Trim();
                }
                return "admin";
            }
        }
        /// <summary>
        ///     Edoc2集成登录密钥
        /// </summary>
        public static string Edoc2SdkIntegrationKey
        {
            get
            {
                if (_configuration["Edoc2SdkIntegrationKey"] != null && _configuration["Edoc2SdkIntegrationKey"] != "")
                {
                    return _configuration["Edoc2SdkIntegrationKey"].Trim();
                }
                return "46aa92ec-66af-4818-b7c1-8495a9bd7f17";
            }
        }

        /// <summary>
        ///     获取当前时间全数字形式fff(24小时制-yyyyMMddHHmmssfff)
        /// </summary>
        public static string CurrentTimeNumberfff => DateTime.Now.ToString("yyyyMMddHHmmssfff");

    }
}
