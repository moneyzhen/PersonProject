using EDoc2.IAppService.ResultModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;
using ZHONGJIAN_API.DAL;
using ZHONGJIAN_API.DAL.eformForm;
using ZHONGJIAN_API.DAL.sect;
using ZHONGJIAN_API.Edoc2Api;
using ZHONGJIAN_API.Model;
using ZHONGJIAN_API.Model.Mobile;
using ZHONGJIAN_API.Model.Result;

namespace ZHONGJIAN_API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly MobileService mobileService = new MobileService();
        private readonly SectService sectService = new SectService();
        private readonly FormService formService = new FormService();

        private readonly static string archformId = "210824134108528_edrms";//项目库档案归档流程表单
        private readonly static string borrowformId = "200206210516_edrms";//档案借阅流程表单

        #region 登录信息
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UserLogin([FromBody] UserLoginDto param)
        {
            try
            {
                //参数检验
                if (string.IsNullOrEmpty(param.userName) || string.IsNullOrEmpty(param.password))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                //用户登录
                string token = EDoc2SdkHelper.GetToken();
                string url = $"{ConfigHelper.Edoc2BaseUrl}/api/services/Org/UserLogin?token={token}";
                var jsonobj = new { userName = param.userName, password = param.password };
                var resultStr = HttpClientHelper.Post(url, JsonConvert.SerializeObject(jsonobj));
                var jsonResult = JsonConvert.DeserializeObject<LoginResposeResult>(resultStr);
                if (jsonResult.result == 0)
                {
                    return new JsonResult(new ResultObj(true, "S", "成功", jsonResult));
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败", jsonResult));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"insertList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }

        /// <summary>
        /// 根据token获取用户信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserInfoByToken(string token)
        {
            try
            {
                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(token))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }
                var rand = mobileService.CreateRandom(13);//随机数
                //获取用户信息
                string url = $"{ConfigHelper.Edoc2BaseUrl}/api/services/OrgUser/GetUserInfoByToken?token={token}&_={rand}";
                string resultStr = HttpClientHelper.Get(url);
                var jsonResult = JsonConvert.DeserializeObject<UserResposeResult>(resultStr);
                if (jsonResult.result == 0)
                {
                    return new JsonResult(new ResultObj(true, "S", "成功", jsonResult.data));
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败", jsonResult));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"selectDataList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }

        #endregion

        #region 项目库
        /// <summary>
        /// 项目列表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetProjectList(string token, [FromBody] ProjectListDto param)
        {
            try
            {
                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(token))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }

                //参数检验
                if (string.IsNullOrEmpty(param.dynField) || string.IsNullOrEmpty(param.dynOrder) || string.IsNullOrEmpty(param.page.ToString()) || string.IsNullOrEmpty(param.rows.ToString()))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }
                //设置默认参数值
                if (string.IsNullOrEmpty(param.formId))
                {
                    param.formId = "21081915271614_edrms";//项目管理列表表单ID
                }
                int count = 0;
                param.page = (param.page - 1) * param.rows;
                DataTable dt = mobileService.SelectProjectList(param, out count);
                var obj = new { rows = dt, total = count };
                if (dt != null && count > 0)
                {
                    return new JsonResult(new ResultObj(true, "S", "成功", obj));
                }
                else
                {
                    return new JsonResult(new ResultObj(true, "S1", "无数据", obj));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"insertList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }

        /// <summary>
        /// 根据项目ID获取项目信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="project_manage_id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetProjectInfoById(string token, string project_manage_id)
        {
            try
            {
                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(token))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }
                //参数检验
                if (string.IsNullOrEmpty(project_manage_id))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                DataTable dt = mobileService.GetProjectInfoById(project_manage_id);
                if (dt != null && dt.Rows.Count > 0)
                {
                    return new JsonResult(new ResultObj(true, "S", "成功", dt));
                }
                else
                {
                    return new JsonResult(new ResultObj(true, "S1", "无数据"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"selectDataList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }

        #endregion

        #region 档案库
        /// <summary>
        /// 档案列表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult selectArchDataList(string token, [FromBody] ArchListDto param)
        {
            try
            {
                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(token))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }

                //参数检验
                if (string.IsNullOrEmpty(param.departmentName) || string.IsNullOrEmpty(param.dynField) || string.IsNullOrEmpty(param.dynOrder) || string.IsNullOrEmpty(param.page.ToString()) || string.IsNullOrEmpty(param.rows.ToString()))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                #region 档案列表权限控制（非领导只可查看当前公司下已归档档案，领导可查看所有归档档案）
                if (!string.IsNullOrEmpty(param.departmentName) && param.departmentName != "中建国际投资")
                {
                    //档案列表当前公司下已归档档案
                    if (param.filter != null)
                    {
                        //非项目详情中已归档档案
                        if (string.IsNullOrEmpty(param.filter.project_manage_id))
                        {
                            //if (!string.IsNullOrEmpty(param.filter.mainCompanyName))
                            //{
                            //    param.filter.mainCompanyName = param.filter.mainCompanyName + "," + param.departmentName;
                            //}
                            //else
                            //{
                            //    param.filter.mainCompanyName = param.departmentName;
                            //}
                            param.filter.mainCompanyName = param.departmentName;
                        }
                    }
                    else
                    {
                        param.filter.mainCompanyName = param.departmentName;
                    }
                }
                #endregion

                var rand = mobileService.CreateRandom(13);//随机数
                //获取档案信息
                string url = $"{ConfigHelper.Edoc2BaseUrl}/edrmscore/api/arch/selectArchDataList?token={token}&t={rand}";
                #region 调用档案列表接口参数
                var jsonobj = new
                {
                    archTypeId = param.archTypeId,
                    data = param.data,
                    dataIdPath = param.dataIdPath,
                    dynField = param.dynField,
                    dynOrder = param.dynOrder,
                    filter = param.filter,
                    entryState = "1",
                    isShow = "1",
                    curPage = param.page,
                    page = param.page,
                    pageSize = param.rows,
                    rows = param.rows,
                    vcFilter = new { }
                };
                #endregion
                var resultStr = HttpClientHelper.Post(url, JsonConvert.SerializeObject(jsonobj));
                var jsonResult = JsonConvert.DeserializeObject<ResponseModel<ArchModel>>(resultStr);
                if (jsonResult.code == 0)
                {
                    List<ArchListModel> querylist = new List<ArchListModel>();
                    #region 获取移动端展示列
                    foreach (var item in jsonResult.obj.entryDataEsQueryList)
                    {
                        ArchListModel model = new ArchListModel();
                        model.Id = item.id;
                        model.archType = item.archType;
                        model.archtypeid = item.archtypeid;
                        model.edition = item.edition;
                        model.mainCompanyName = item.mainCompanyName;
                        model.mainProjectMode = item.mainProjectCode;
                        model.mainProjectName = item.mainProjectName;
                        model.mainProjectType = item.mainProjectType;
                        model.name = item.name;
                        model.note = item.note;
                        model.project_manage_id = item.project_manage_id;
                        model.tableName = item.tableName;
                        model.writtendate = item.writtendate;
                        //添加到列表
                        querylist.Add(model);
                    }
                    #endregion

                    //档案列表对象
                    var obj = new { totalCount = jsonResult.obj.totalCount, entryDataEsQueryList = querylist };
                    if (jsonResult.obj.totalCount > 0)
                    {
                        return new JsonResult(new ResultObj(true, "S", "成功", obj));
                    }
                    else
                    {
                        return new JsonResult(new ResultObj(true, "S1", "无数据", obj));
                    }
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败", jsonResult.obj));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"selectDataList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }

        /// <summary>
        /// 根据进度ID和档案ID获取档案信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="archTypeId">进度ID</param>
        /// <param name="archId">档案ID</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetArchInfoById(string token, string archTypeId, string archId)
        {
            try
            {
                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(token))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }
                //参数检验
                if (string.IsNullOrEmpty(archTypeId) || string.IsNullOrEmpty(archId))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                DataTable archdt = mobileService.GetArchInfoById(token, archTypeId, archId);
                if (archdt != null && archdt.Rows.Count > 0)
                {
                    #region 获取当前档案电子文件
                    List<FileInfoForSdkResult> fileList = new List<FileInfoForSdkResult>();
                    //根据档案ID获取附件（电子文件）
                    DataTable filedt = mobileService.GetAttachInfoByArchId(archdt.Rows[0]["Id"].ToString());
                    foreach (DataRow item in filedt.Rows)
                    {
                        //根据文件id获取文件信息v
                        var fileinfo = sectService.GetFullFileInfoById(int.Parse(item["FileId"].ToString()));

                        fileList.Add(fileinfo);
                    }
                    #endregion

                    var obj = new { archInfo = archdt, filesInfo = fileList };
                    return new JsonResult(new ResultObj(true, "S", "成功", obj));
                }
                else
                {
                    return new JsonResult(new ResultObj(true, "S1", "无数据"));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"selectDataList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }
        #endregion

        #region 流程
        /// <summary>
        /// 流程统计
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult getCountByUserId([FromForm] FlowStatisticsDto param)
        {
            try
            {
                //参数检验
                if (string.IsNullOrEmpty(param.userId))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                var response = "";
                Dictionary<string, string> parameter = new Dictionary<string, string>();
                parameter.Add("userId", param.userId);
                parameter.Add("type", param.type);
                parameter.Add("groupType", param.groupType);

                var rand = "0." + mobileService.CreateRandom(16);//随机数
                string url = $"{ConfigHelper.Edoc2BaseUrl}/edoc2Flow-web/edoc-task/getCountByUserId?rand=" + rand;
                HttpClientHelper.CreatePostHttpResponse(url, parameter, 60000, string.Empty, Encoding.UTF8, ref response);
                if (response != null)
                {
                    var jsonResult = JsonConvert.DeserializeObject<FlowStatisticsModel>(response);

                    return new JsonResult(new ResultObj(true, "S", "成功", jsonResult));
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败", response));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"insertList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }

        /// <summary>
        /// 流程列表
        /// </summary>
        /// <param name="token"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetFlowList(string token, [FromBody] FlowListDto param)
        {
            try
            {
                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(token))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }

                //参数检验
                if (string.IsNullOrEmpty(param.userId) || string.IsNullOrEmpty(param.proType.ToString()) || string.IsNullOrEmpty(param.page.ToString()) || string.IsNullOrEmpty(param.rows.ToString()))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                int count = 0;
                DataTable dt = mobileService.SelectFlowList(param, out count);
                var obj = new { rows = dt, total = count };
                if (dt != null && count > 0)
                {
                    return new JsonResult(new ResultObj(true, "S", "成功", obj));
                }
                else
                {
                    return new JsonResult(new ResultObj(true, "S1", "无数据", obj));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"insertList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }


        /// <summary>
        /// 流程详情（档案列表）
        /// </summary>
        /// <param name="token"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetFlowArchList([FromQuery] FlowInfoDto param)
        {
            try
            {
                //参数检验
                if (string.IsNullOrEmpty(param.orgToken) || string.IsNullOrEmpty(param.taskType.ToString()) || string.IsNullOrEmpty(param.processId) || string.IsNullOrEmpty(param.incidentId) || string.IsNullOrEmpty(param.businessKey) || string.IsNullOrEmpty(param.taskId))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(param.orgToken))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }

                FlowJumpIndex jumpModel = mobileService.GetFlowFormUrlParms(param);
                if (jumpModel == null)
                {
                    return new JsonResult(new ResultObj(false, "E", "失败"));
                }
                string formId = jumpModel.formId;//默认项目库档案归档流程 //获取返回流程表单，判断是项目库档案归档流程（210824134108528_edrms）还是档案借阅流程（200206210516_edrms）          
                string recordId = jumpModel.id;//流程表单记录ID

                //项目库档案归档流程
                if (formId == archformId)
                {
                    #region 项目库档案归档流程
                    DataTable ResultDt = mobileService.GetArchPlaceByRecordId(recordId);

                    if (ResultDt != null && ResultDt.Rows.Count > 0)
                    {
                        return new JsonResult(new ResultObj(true, "S", "成功", ResultDt));
                    }
                    else
                    {
                        return new JsonResult(new ResultObj(true, "S1", "无数据", ResultDt));
                    }
                    #endregion

                }
                //档案借阅流程
                else if (formId == borrowformId)
                {
                    #region 档案借阅流程
                    DataTable archdt = mobileService.GetBorrowInfoByRecordId(recordId);
                    if (archdt != null && archdt.Rows.Count > 0)
                    {
                        DataTable brrowdt = mobileService.GetBorrowarchInfoById(recordId);
                        var obj = new { archList = archdt, brrowInfo = brrowdt };

                        return new JsonResult(new ResultObj(true, "S", "成功", obj));
                    }
                    else
                    {
                        return new JsonResult(new ResultObj(true, "S1", "无数据"));
                    }
                    #endregion
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败"));
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"insertList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }


        /// <summary>
        /// 根据流程ID获取流程日志
        /// </summary>
        /// <param name="processInstanceId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult getProcessLogs(string processInstanceId)
        {
            try
            {
                //参数检验
                if (string.IsNullOrEmpty(processInstanceId))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                var rand = "0." + mobileService.CreateRandom(17);//随机数
                //获取流程日志
                string url = $"{ConfigHelper.Edoc2BaseUrl}/edoc2Flow-web/edoc-processInstance/getProcessLogs.do?processInstanceId={processInstanceId}&r={rand}";
                string resultStr = HttpClientHelper.Get(url);
                var jsonResult = JsonConvert.DeserializeObject<ProcessLogsModel>(resultStr);
                if (jsonResult != null)
                {

                    return new JsonResult(new ResultObj(true, "S", "成功", jsonResult));
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败", jsonResult));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"selectDataList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }

        }

        /// <summary>
        /// 流程审批(同意)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult FlowApprove([FromForm] FlowInfoDto param)
        {
            try
            {
                #region 校验
                //参数检验
                if (string.IsNullOrEmpty(param.actionType.ToString()) || string.IsNullOrEmpty(param.summary) || string.IsNullOrEmpty(param.archName) || string.IsNullOrEmpty(param.orgToken) || string.IsNullOrEmpty(param.taskType.ToString()) || string.IsNullOrEmpty(param.processId) || string.IsNullOrEmpty(param.incidentId) || string.IsNullOrEmpty(param.businessKey) || string.IsNullOrEmpty(param.taskId))
                {
                    return new JsonResult(new ResultObj(false, "E2", "参数异常"));
                }

                //登录校验
                if (!EDoc2SdkHelper.CheckUserTokenValidity(param.orgToken))
                {
                    return new JsonResult(new ResultObj(false, "E1", "登录信息失效"));
                }

                if (param.taskType == taskTypeEnum.ArchiveTask)
                {
                    return new JsonResult(new ResultObj(false, "E", "完结流程不可审批"));
                }

                if (param.taskType == taskTypeEnum.CompleteTask && param.actionType != actionTypeEnum.cancel)
                {
                    return new JsonResult(new ResultObj(false, "E", "在途流程只有终止权限"));
                }

                if (param.actionType == actionTypeEnum.initiate)
                {
                    return new JsonResult(new ResultObj(false, "E", "无提交发起权限，请在PC端处理"));
                }

                //获取跳转url传参
                FlowJumpIndex jumpModel = mobileService.GetFlowFormUrlParms(param);
                if (jumpModel == null)
                {
                    return new JsonResult(new ResultObj(false, "E", "失败"));
                }
                #endregion

                string formId = jumpModel.formId;//默认项目库档案归档流程 //获取返回流程表单，判断是项目库档案归档流程（210824134108528_edrms）还是档案借阅流程（200206210516_edrms）          
                string recordId = jumpModel.id;//流程表单记录ID

                string orgToken = param.orgToken;

                #region 获取流程表单信息
                DataTable flowArchdt = null;//项目库档案归档流程记录数据
                DataTable archinfodt = null;//项目库档案归档表单中档案数据
                DataTable borrowdt = null;//档案借阅流程记录数据
                DataTable borrowarchdt = null;//档案借阅流程表单中档案数据
                //项目库档案归档流程表单
                if (formId == archformId)
                {
                    #region 根据流程表单信息获取档案信息

                    flowArchdt = mobileService.GetArchPlaceByRecordId(recordId);
                    if (flowArchdt != null && flowArchdt.Rows.Count > 0)
                    {
                        archinfodt = mobileService.GetArchInfoById(orgToken, flowArchdt.Rows[0]["archTypeId"].ToString(), flowArchdt.Rows[0]["detailsId"].ToString());
                    }
                    else
                    {
                        return new JsonResult(new ResultObj(false, "E", "项目库档案归档流程记录数据未找到", recordId));
                    }
                    #endregion
                }
                //档案借阅流程表单
                else if (formId == borrowformId)
                {
                    borrowdt = mobileService.GetBorrowarchInfoById(recordId);
                    if (borrowdt != null && borrowdt.Rows.Count > 0)
                    {
                        //借阅流程档案信息
                        borrowarchdt = mobileService.GetBorrowInfoByRecordId(recordId);
                    }
                    else
                    {
                        return new JsonResult(new ResultObj(false, "E", "档案借阅流程记录数据未找到", recordId));
                    }
                }
                #endregion

                var resultStr = string.Empty;

                #region 流程审批 项目库档案归档(同意、退回、终止操作）、档案借阅流程审批(同意、终止操作)

                //获取当前任务信息
                FlowActivityInfo currentActivity = mobileService.getActivityInfo(orgToken, param.processId, param.incidentId, param.taskId);

                //流程审批（同意）
                if (param.actionType == actionTypeEnum.approve)
                {
                    //流程变量
                    List<FlowVariableModel> varslist = formService.GetVarslist(formId, recordId, param.processId);

                    string url = $"{ConfigHelper.Edoc2BaseUrl}/edoc2Flow-web/edoc-task/{param.taskId}/completeTask?orgToken={orgToken}";
                    #region 完成任务接口参数
                    var jsonobj = new
                    {
                        taskId = param.taskId,
                        comment = param.summary,
                        variablesParams = JsonConvert.SerializeObject(varslist),
                        incidentRemark = param.archName,
                        signDataStr = "",
                    };
                    #endregion
                    resultStr = HttpClientHelper.Post(url, JsonConvert.SerializeObject(jsonobj));
                }
                //流程审批（退回）
                else if (param.actionType == actionTypeEnum.returns && formId == archformId)
                {
                    string url = $"{ConfigHelper.Edoc2BaseUrl}/edoc2Flow-web/edoc-task/rejectStarterActivity?orgToken={orgToken}";
                    #region 退回发起人接口参数
                    Dictionary<string, string> parameter = new Dictionary<string, string>();
                    parameter.Add("taskId", param.taskId);
                    parameter.Add("rejectOrgin", param.summary);
                    parameter.Add("isBackReturn", "true");
                    parameter.Add("incidentRemark", param.archName);
                    parameter.Add("signDataStr", "");
                    #endregion

                    HttpClientHelper.CreatePostHttpResponse(url, parameter, 60000, string.Empty, Encoding.UTF8, ref resultStr);

                }
                //流程审批（终止）
                else if (param.actionType == actionTypeEnum.cancel)
                {
                    //项目库档案归档只有发起人才有“终止”权限
                    if (formId == archformId)
                    {
                        var startUserId = flowArchdt.Rows[0]["user_id"].ToString();
                        //判断是否是发起人
                        if (startUserId != EDoc2SdkHelper.GetUserInfoByToken(orgToken).ID.ToString())
                        {
                            return new JsonResult(new ResultObj(false, "E", "项目库档案归档非发起人无终止权限"));
                        }
                    }
                    //终止流程接口
                    string url = $"{ConfigHelper.Edoc2BaseUrl}/edoc2Flow-web/edoc-processInstance/stopProcessInstance?orgToken={orgToken}&id={param.incidentId}&taskId={param.taskId}&summary={param.summary}&remark={param.archName}&signDataStr=callback=";
                    resultStr = HttpClientHelper.Get(url);
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败"));
                }
                #endregion

                //流程审批结果
                var jsonResult = JsonConvert.DeserializeObject<FlowApproveResult>(resultStr);
                if (jsonResult != null && jsonResult.isSuccess)
                {
                    string resultStr2 = string.Empty;
                    string resultStr3 = string.Empty;

                    #region 审批业务处理
                    //项目库档案归档流程审批
                    if (formId == archformId)
                    {
                        #region 项目库档案归档流程审批
                        //同意
                        if (param.actionType == actionTypeEnum.approve)
                        {
                            if (currentActivity != null && currentActivity.activityNo != "firstTask")
                            {
                                #region eform.wf.currentActivity.activityNo != "firstTask"
                                //流程表单同意
                                string url = $"{ConfigHelper.Edoc2BaseUrl}/edrmscore/api/flowFiled/approve?token=" + orgToken;
                                resultStr2 = HttpClientHelper.Post(url, JsonConvert.SerializeObject(new { recordId = recordId }));
                                var jsonResult2 = JsonConvert.DeserializeObject<ResponseDto>(resultStr2);
                                //第三方thirdTodoId
                                mobileService.getThirdTodoId(param, recordId, "thirdTodoId", 0, out resultStr3);
                                #endregion
                            }
                            else
                            {
                                #region 退回后发起人再次提交
                                //有第三方thirdTodoId
                                mobileService.getThirdTodoId(param, recordId, "thirdTodoId,startUserthirdTodoId", 0, out resultStr3);

                                if (flowArchdt != null && flowArchdt.Rows.Count > 0)
                                {
                                    //给归档人发OA
                                    var tmp_thirdTodoId = mobileService.addOAMessage(orgToken, null, archinfodt);

                                    if (!string.IsNullOrEmpty(tmp_thirdTodoId))
                                    {
                                        mobileService.UpdateThirdTodoIdArchplaceById(recordId, tmp_thirdTodoId);
                                    }
                                }
                                #endregion
                            }

                        }
                        //退回
                        else if (param.actionType == actionTypeEnum.returns)
                        {
                            mobileService.getThirdTodoId(param, recordId, "thirdTodoId", 1, out resultStr3);
                            if (flowArchdt != null && flowArchdt.Rows.Count > 0)
                            {
                                //给发起人发送消息
                                MessageUser startUserJson = new MessageUser();
                                startUserJson.id = flowArchdt.Rows[0]["user_id"].ToString();
                                startUserJson.account = flowArchdt.Rows[0]["user_account"].ToString();
                                startUserJson.name = flowArchdt.Rows[0]["user_name"].ToString();
                                var tmp_thirdTodoId = mobileService.addOAMessage(orgToken, startUserJson, archinfodt);
                                if (!string.IsNullOrEmpty(tmp_thirdTodoId))
                                {
                                    mobileService.UpdateStartUserThirdTodoIdArchplaceById(recordId, tmp_thirdTodoId);
                                }
                            }
                        }
                        //终止
                        else if (param.actionType == actionTypeEnum.cancel)
                        {
                            string url = $"{ConfigHelper.Edoc2BaseUrl}/edrmscore/api/flowFiled/cancel?token=" + orgToken;
                            resultStr2 = HttpClientHelper.Post(url, JsonConvert.SerializeObject(new { recordId = recordId }));
                            var jsonResult2 = JsonConvert.DeserializeObject<ResponseDto>(resultStr2);
                            //第三方thirdTodoId
                            mobileService.getThirdTodoId(param, recordId, "thirdTodoId", 1, out resultStr3);
                        }
                        #endregion
                    }
                    //档案借阅流程审批
                    else if (formId == borrowformId)
                    {
                        #region 档案借阅流程审批

                        #region 第三方数据获取
                        //第三方查询
                        resultStr3 = mobileService.getListByCommon(param.orgToken, "archuserThirdTodoId,assistanterThirdTodoId,supervisorThirdTodoId,managerThirdTodoId,plusThirdTodoId", " Id = '" + recordId + "'", "borrow");
                        var jsonResult3 = JsonConvert.DeserializeObject<ResponseListModel<borrowModel>>(resultStr3);
                        var borrow_data = jsonResult3.obj;

                        var thirdTodoId = string.Empty;
                        var assistanterThirdTodoId = string.Empty;
                        var supervisorThirdTodoId = string.Empty;
                        var archuserThirdTodoId = string.Empty;
                        var managerThirdTodoId = string.Empty;
                        var plusThirdTodoId = string.Empty;
                        if (borrow_data.Count > 0 && borrow_data[0] != null)
                        {
                            if (!string.IsNullOrEmpty(borrow_data[0].supervisorThirdTodoId))
                            {
                                thirdTodoId = borrow_data[0].supervisorThirdTodoId;
                                supervisorThirdTodoId = borrow_data[0].supervisorThirdTodoId;
                            }
                            if (!string.IsNullOrEmpty(borrow_data[0].assistanterThirdTodoId))
                            {
                                assistanterThirdTodoId = borrow_data[0].assistanterThirdTodoId;
                            }
                            if (!string.IsNullOrEmpty(borrow_data[0].archuserThirdTodoId))
                            {
                                archuserThirdTodoId = borrow_data[0].archuserThirdTodoId;
                            }
                            if (!string.IsNullOrEmpty(borrow_data[0].managerThirdTodoId))
                            {
                                managerThirdTodoId = borrow_data[0].managerThirdTodoId;
                            }
                            if (!string.IsNullOrEmpty(borrow_data[0].plusThirdTodoId))
                            {
                                plusThirdTodoId = borrow_data[0].plusThirdTodoId;
                            }
                        }
                        #endregion

                        #region 获取借阅流程表单审核人员信息
                        string BorrowDepartmentId = string.Empty;
                        string supervisorJsonStr = string.Empty;
                        string assistanterJsonStr = string.Empty;
                        string archuserJsonStr = string.Empty;
                        if (borrowdt != null && borrowdt.Rows.Count > 0)
                        {
                            BorrowDepartmentId = borrowdt.Rows[0]["borrowDeptId"].ToString();
                            supervisorJsonStr = borrowdt.Rows[0]["supervisor"].ToString();
                            assistanterJsonStr = borrowdt.Rows[0]["assistanter"].ToString();
                            archuserJsonStr = borrowdt.Rows[0]["archuser"].ToString();
                        }
                        #endregion

                        //同意
                        if (param.actionType == actionTypeEnum.approve)
                        {
                            //流程表单同意  借阅档案edrmscore/api/flowBorrow/approve (eform.recordId)
                            if (currentActivity != null)
                            {
                                if (currentActivity.activityNo == "endTask")//总部领导
                                {
                                    string url = $"{ConfigHelper.Edoc2BaseUrl}/edrmscore/api/flowBorrow/approve?token=" + orgToken;
                                    resultStr2 = HttpClientHelper.Post(url, JsonConvert.SerializeObject(new { recordId = recordId }));
                                    var jsonResult2 = JsonConvert.DeserializeObject<ResponseDto>(resultStr2);

                                    if (!string.IsNullOrEmpty(thirdTodoId))
                                    {
                                        mobileService.cancelOAMessage(orgToken, thirdTodoId, 0);
                                    }
                                }
                                else if (currentActivity.activityNo == "assistantTask")//总部区域对接人
                                {
                                    if (!string.IsNullOrEmpty(assistanterThirdTodoId))
                                    {
                                        mobileService.cancelOAMessage(orgToken, assistanterThirdTodoId, 0);
                                    }
                                    if (string.IsNullOrEmpty(supervisorThirdTodoId))
                                    {
                                        //发送OA消息给上级领导
                                        if (borrowdt != null && borrowdt.Rows.Count > 0 && !string.IsNullOrEmpty(supervisorJsonStr))
                                        {
                                            Leader leaderUser = JsonConvert.DeserializeObject<List<Leader>>(supervisorJsonStr)[0];
                                            //档案管理员
                                            supervisorThirdTodoId = mobileService.addOAMessageBorrow(orgToken, BorrowDepartmentId, leaderUser, borrowarchdt);
                                            if (!string.IsNullOrEmpty(supervisorThirdTodoId))
                                            {
                                                mobileService.UpdateSupervisorThirdTodoIdBorrowById(recordId, supervisorThirdTodoId);
                                            }
                                        }
                                    }
                                }
                                else if (currentActivity.activityNo == "archTask")//分公司领导
                                {
                                    if (!string.IsNullOrEmpty(archuserThirdTodoId))
                                    {
                                        mobileService.cancelOAMessage(orgToken, archuserThirdTodoId, 0);
                                    }

                                    if (string.IsNullOrEmpty(assistanterThirdTodoId))
                                    {
                                        //发送OA消息给上级领导
                                        if (borrowdt != null && borrowdt.Rows.Count > 0 && !string.IsNullOrEmpty(assistanterJsonStr))
                                        {
                                            Leader leaderUser = JsonConvert.DeserializeObject<List<Leader>>(assistanterJsonStr)[0];
                                            //档案管理员
                                            assistanterThirdTodoId = mobileService.addOAMessageBorrow(orgToken, BorrowDepartmentId, leaderUser, borrowarchdt);
                                            if (!string.IsNullOrEmpty(assistanterThirdTodoId))
                                            {
                                                mobileService.UpdateAssistanterThirdTodoIdBorrowById(recordId, assistanterThirdTodoId);
                                            }
                                        }
                                    }

                                }
                                else if (currentActivity.activityNo == "managerTask")//分公司负责人
                                {
                                    if (!string.IsNullOrEmpty(managerThirdTodoId))
                                    {
                                        mobileService.cancelOAMessage(orgToken, managerThirdTodoId, 0);
                                    }

                                    if (string.IsNullOrEmpty(archuserThirdTodoId))
                                    {
                                        //发送OA消息给上级领导
                                        if (borrowdt != null && borrowdt.Rows.Count > 0 && !string.IsNullOrEmpty(archuserJsonStr))
                                        {
                                            Leader leaderUser = JsonConvert.DeserializeObject<List<Leader>>(archuserJsonStr)[0];
                                            //档案管理员
                                            archuserThirdTodoId = mobileService.addOAMessageBorrow(orgToken, BorrowDepartmentId, leaderUser, borrowarchdt);
                                            if (!string.IsNullOrEmpty(archuserThirdTodoId))
                                            {
                                                mobileService.UpdateArchuserThirdTodoIdBorrowById(recordId, archuserThirdTodoId);
                                            }
                                        }
                                    }
                                }
                            }
                            else //为加签人
                            {
                                string url = $"{ConfigHelper.Edoc2BaseUrl}/edrmscore/api/flowBorrow/approve?token=" + orgToken;
                                resultStr2 = HttpClientHelper.Post(url, JsonConvert.SerializeObject(new { recordId = recordId }));
                                var jsonResult2 = JsonConvert.DeserializeObject<ResponseDto>(resultStr2);

                                if (!string.IsNullOrEmpty(thirdTodoId))
                                {
                                    mobileService.cancelOAMessage(orgToken, thirdTodoId, 0);
                                }
                            }
                        }
                        //终止
                        else if (param.actionType == actionTypeEnum.cancel)
                        {
                            if (currentActivity != null)
                            {
                                if (currentActivity.activityNo == "endTask")//总部领导
                                {
                                    thirdTodoId = supervisorThirdTodoId;
                                }
                                else if (currentActivity.activityNo == "assistantTask")//总部区域对接人
                                {
                                    thirdTodoId = assistanterThirdTodoId;
                                }
                                else if (currentActivity.activityNo == "archTask")//分公司领导
                                {
                                    thirdTodoId = archuserThirdTodoId;
                                }
                                else if (currentActivity.activityNo == "managerTask")//分公司负责人
                                {
                                    thirdTodoId = managerThirdTodoId;
                                }
                            }
                            else //为加签人
                            {
                                thirdTodoId = plusThirdTodoId;
                            }
                            if (!string.IsNullOrEmpty(thirdTodoId))
                            {
                                mobileService.cancelOAMessage(orgToken, thirdTodoId, 1);
                            }
                        }
                        //退回
                        else if (param.actionType == actionTypeEnum.returns)
                        {
                            return new JsonResult(new ResultObj(false, "E", "借阅档案无退回审批权限"));
                        }
                        #endregion
                    }
                    #endregion

                    return new JsonResult(new ResultObj(true, "S", "成功", jsonResult));
                }
                else
                {
                    return new JsonResult(new ResultObj(false, "E", "失败", jsonResult));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"insertList exception：{ex.Message}");
                return new JsonResult(new ResultObj(false, "E3", "服务出错"));
            }
        }

        #endregion
    }
}
