using EDoc2.IAppService.Model.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model
{
    public class LoginResposeResult
    {
        public string data { get; set; }
        public string dataDescription { get; set; }
        public int result { get; set; }
        public string message { get; set; }
    }
    public class UserResposeResult
    {
        public UserInfo data { get; set; }
        public string dataDescription { get; set; }
        public int result { get; set; }
        public string message { get; set; }
    }
    public class FlowApproveResult
    {
        public bool isSuccess { get; set; }
        public string code { get; set; }
        public string context { get; set; }
        public string data { get; set; }
    }

    public class ResponseModel<T>
    {
        public bool result { get; set; }
        public string message { get; set; }
        public T obj { get; set; }
        public int code { get; set; }
    }

    public class ResponseListModel<T>
    {
        public bool result { get; set; }
        public string message { get; set; }
        public List<T> obj { get; set; }
        public int code { get; set; }
    }

    public class ResponseModel
    {
        public string code { get; set; }
        public string message { get; set; }
        public string obj { get; set; }
        public bool result { get; set; }
        public object data { get; set; }
    }

    public class ResponseDto
    {
        public int code { get; set; }
        public string message { get; set; }
        public string obj { get; set; }
        public bool result { get; set; }
    }

}
