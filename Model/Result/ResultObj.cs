using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Result
{
    public class ResultObj
    {

        /// <summary>
        /// 接口返回状态，true为成功，false为失败
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 接口状态码（S1:成功、S2无数据、S3无权限、E0失败、E1登录失败、E2参数异常、E3服务失败、E4其他异常）
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 接口提示信息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 接口返回数据对象
        /// </summary>
        public object obj { get; set; }

        public ResultObj()
        {
        }
        public ResultObj(bool success, string code, string message)
        {
            this.success = success;
            this.code = code;
            this.message = message;
        }

        public ResultObj(bool success, string code, string message, object obj)
        {
            this.success = success;
            this.code = code;
            this.message = message;
            this.obj = obj;
        }


    }
}
