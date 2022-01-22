using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;
using ZHONGJIAN_API.DAL.sect;
using ZHONGJIAN_API.Edoc2Api;
using ZHONGJIAN_API.Model;
using ZHONGJIAN_API.Model.Mobile;
using ZHONGJIAN_API.Model.Result;

namespace ZHONGJIAN_API.DAL
{
    public class MobileService
    {
        private readonly MobileDao mobileDao = new MobileDao();
        private readonly SectDao sectDao = new SectDao();

        #region 项目库
        /// <summary>
        /// 项目列表
        /// </summary>
        /// <returns></returns>
        public DataTable SelectProjectList(ProjectListDto param, out int count)
        {
            DataTable dt = mobileDao.SelectProjectList(param, out count);
            return dt;
        }

        /// <summary>
        /// 根据项目ID获取项目信息
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public DataTable GetProjectInfoById(string projectId)
        {
            DataTable dt = mobileDao.GetProjectInfoById(projectId);
            return dt;
        }
        #endregion

        #region 档案库
        /// <summary>
        /// 根据进度ID和档案ID获取档案信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="archTypeId"></param>
        /// <param name="archId"></param>
        /// <returns></returns>
        public DataTable GetArchInfoById(string token, string archTypeId, string archId)
        {
            string arch_table_name = sectDao.GetArchTableName(archTypeId);
            DataTable dt = mobileDao.GetArchInfoById(arch_table_name, archId);
            return dt;
        }

        /// <summary>
        /// 根据档案ID获取附件（电子文件）
        /// </summary>
        /// <param name="archId"></param>
        /// <returns></returns>
        public DataTable GetAttachInfoByArchId(string archId)
        {
            DataTable dt = mobileDao.GetAttachInfoByArchId(archId);
            return dt;
        }

        #endregion

        #region 流程

        /// <summary>
        /// 获取当前流程跳转流程表单传参值
        /// </summary>
        /// <param name="param">请求参数</param>
        /// <param name="formId">输出流程表单ID</param>
        /// <param name="recordId">输出流程表单提交记录ID</param>
        /// <returns></returns>
        public FlowJumpIndex GetFlowFormUrlParms(FlowInfoDto param)
        {
            FlowJumpIndex jumpindex = new FlowJumpIndex();

            string state = "";//任务状态(我的待办必传默认值“0”,在途流程和完结流程不传)
            if (param.taskType == taskTypeEnum.InboxTask)
            {
                state = "&state=0";
            }

            var rand = CreateRandom(13);//随机数
             //获取跳转流程表单url
            string url = $"{ConfigHelper.Edoc2BaseUrl}/edoc2Flow-web/edoc2-form/jumpIndex?orgToken={param.orgToken}&t={rand}{state}&taskType={param.taskType}&processId={param.processId}&taskId={param.taskId}&incidentId={param.incidentId}&businessKey={param.businessKey}";
            string resultStr = HttpClientHelper.Get(url);

            if (!string.IsNullOrEmpty(resultStr))
            {
                #region 截取字符串获取返回参数
                int splitIndex = resultStr.IndexOf('{');
                int splitIndex2 = resultStr.IndexOf('}');
                string paramstr = resultStr.Substring(splitIndex + 1, splitIndex2 - splitIndex - 1);

                int splitIndex3 = paramstr.IndexOf('?');
                int splitIndex4 = paramstr.IndexOf(',');
                paramstr = paramstr.Substring(splitIndex3 + 1, splitIndex4 - splitIndex3 - 2);
                #endregion
                Dictionary<string, string> paramDic = new Dictionary<string, string>();
                string[] arr = paramstr.Split('&');
                for (int i = 0; i < arr.Length; i++)
                {
                    var key = arr[i].Split('=')[0];
                    var value = arr[i].Split('=')[1];
                    paramDic.Add(key, value);
                }

                if (paramDic.Count > 0)
                {
                    jumpindex.formId = paramDic["formId"].ToString();
                    jumpindex.skin = paramDic["skin"].ToString();
                    jumpindex.processId = paramDic["processId"].ToString();
                    jumpindex.taskType = paramDic["taskType"].ToString();
                    jumpindex.formver = paramDic["formver"].ToString();
                    jumpindex.taskId = paramDic["taskId"].ToString();
                    jumpindex.incidentId = paramDic["incidentId"].ToString();
                    jumpindex.id = paramDic["id"].ToString();
                    if (paramDic.Keys.Contains("taskState"))
                    {
                        jumpindex.taskState = paramDic["taskState"].ToString();
                    }
                }
            }

            return jumpindex;

        }


        /// <summary>
        /// 流程列表
        /// </summary>
        /// <returns></returns>
        public DataTable SelectFlowList(FlowListDto param, out int count)
        {
            DataTable dt = mobileDao.SelectFlowList(param, out count);
            return dt;
        }

        /// <summary>
        /// 根据recordId查询项目库档案归档流程档案列表
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public DataTable GetArchPlaceByRecordId(string recordId)
        {
            DataTable dt = mobileDao.GetArchPlaceByRecordId(recordId);
            return dt;
        }

        /// <summary>
        /// 根据recordId查询档案借阅流程档案列表
        /// </summary>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public DataTable GetBorrowInfoByRecordId(string recordId)
        {
            DataTable dt = mobileDao.GetBorrowInfoByRecordId(recordId);
            return dt;
        }

        /// <summary>
        /// 根据Id查询档案借阅流程信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public DataTable GetBorrowarchInfoById(string id)
        {
            DataTable dt = mobileDao.GetBorrowarchInfoById(id);
            return dt;
        }

        /// <summary>
        /// 根据流程ID（包含版本）获取流程所有变量
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public DataTable GetProcessVars(string processId)
        {
            DataTable dt = mobileDao.GetProcessVars(processId);
            return dt;
        }


        #region 档案第三方
        /// <summary>
        /// 更新流程的第三方 thirdTodoId
        /// </summary>
        /// <param name="archplaceId"></param>
        /// <param name="thirdTodoId"></param>
        /// <returns></returns>
        public int UpdateThirdTodoIdArchplaceById(string archplaceId, string thirdTodoId)
        {
            return mobileDao.UpdateThirdTodoIdArchplaceById(archplaceId, thirdTodoId);
        }

        /// <summary>
        /// 修改归档发起人流程的第三方id
        /// </summary>
        /// <param name="archplaceId"></param>
        /// <param name="thirdTodoId"></param>
        /// <returns></returns>
        public int UpdateStartUserThirdTodoIdArchplaceById(string archplaceId, string thirdTodoId)
        {
            return mobileDao.UpdateStartUserThirdTodoIdArchplaceById(archplaceId, thirdTodoId);
        }

        #endregion

        #region 借阅第三方
        /// <summary>
        /// 修改借阅流程的负责人第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="managerThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateManagerThirdTodoIdBorrowById(string Id, string managerThirdTodoId)
        {
            return mobileDao.UpdateManagerThirdTodoIdBorrowById(Id, managerThirdTodoId);
        }

        /// <summary>
        /// 修改借阅流程的档案第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="archuserThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateArchuserThirdTodoIdBorrowById(string Id, string archuserThirdTodoId)
        {
            return mobileDao.UpdateArchuserThirdTodoIdBorrowById(Id, archuserThirdTodoId);
        }

        /// <summary>
        /// 修改借阅流程的总部区域对接人第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="assistanterThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateAssistanterThirdTodoIdBorrowById(string Id, string assistanterThirdTodoId)
        {
            return mobileDao.UpdateAssistanterThirdTodoIdBorrowById(Id, assistanterThirdTodoId);
        }

        /// <summary>
        /// 修改借阅流程的总部第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="supervisorThirdTodoId"></param>
        /// <returns></returns>
        public int UpdateSupervisorThirdTodoIdBorrowById(string Id, string supervisorThirdTodoId)
        {
            return mobileDao.UpdateSupervisorThirdTodoIdBorrowById(Id, supervisorThirdTodoId);
        }

        /// <summary>
        /// 修改借阅流程的加签人第三方id
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="plusThirdTodoId"></param>
        /// <returns></returns>
        public int UpdatePlusThirdTodoIdBorrowById(string Id, string plusThirdTodoId)
        {
            return mobileDao.UpdatePlusThirdTodoIdBorrowById(Id, plusThirdTodoId);
        }

        #endregion

        #endregion

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="length">生成随机数长度</param>
        /// <returns></returns>
        public string CreateRandom(int length)
        {
            string rand = string.Empty;
            string str = "";
            int maxLength = 9;
            int newLenth = maxLength;
            for (int i = 0; i < length; i++)
            {
                if (i >= newLenth)
                {
                    str += ",";
                    newLenth += maxLength;
                }
                str += "9";
            }
            string[] arr = str.Split(',');
            if (arr.Length > 0)
            {
                Random random = new Random();//随机数
                for (int i = 0; i < arr.Length; i++)
                {
                    rand += random.Next(0, Convert.ToInt32(arr[i].ToString())).ToString();
                }
            }
            return rand.ToString();
        }

        #region 接口调用

        /// <summary>
        /// 通用接口
        /// </summary>
        /// <param name="token"></param>
        /// <param name="fields"></param>
        /// <param name="conditions"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string getListByCommon(string token, string fields, string conditions, string tableName)
        {
            string url = $"{ConfigHelper.Edoc2BaseUrl}/edrmscore/api/common/list?token={token}&conditions={conditions}&fields={fields}&tableName={tableName}";
            return HttpClientHelper.Get(url);
        }

        /// <summary>
        /// 获取流程任务信息
        /// </summary>
        /// <param name="orgToken"></param>
        /// <param name="processId"></param>
        /// <param name="incidentId"></param>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public FlowActivityInfo getActivityInfo(string orgToken,string processId,string incidentId,string taskId)
        {
            var jsonobj = new
            {
                processId = processId,
                incidentId = incidentId,
                taskId = taskId
            };

            string url = $"{ConfigHelper.Edoc2BaseUrl}/edoc2Flow-web/edoc-task/{taskId}/getActivityInfo?orgToken={orgToken}";
            var resultStr = HttpClientHelper.Post(url, JsonConvert.SerializeObject(jsonobj));
            var jsonResult = JsonConvert.DeserializeObject<FlowActivityInfo>(resultStr);

            return jsonResult;
        }


        #endregion

        //测试环境
        public readonly string oa_url = "http://10.101.8.248:8088";
        //正式环境
        // var oa_url = "http://10.101.8.229:8088";
        // var oa_url = "http://iks.csiil.cn";
        public readonly string oa_return_url = "http://10.101.8.248:8989?returnUrl=/wcm/edrms/wodedaiban";

        #region 第三方

        /// <summary>
        /// 第三方thirdTodoId
        /// </summary>
        /// <param name="param"></param>
        /// <param name="recordId"></param>
        /// <param name="resultStr3"></param>
        public void getThirdTodoId(FlowInfoDto param, string recordId, string fields, int subState, out string resultStr3)
        {
            //有第三方thirdTodoId
            //string fields = "thirdTodoId";
            //string fields = "thirdTodoId,startUserthirdTodoId";
            resultStr3 = getListByCommon(param.orgToken, fields, " Id = '" + recordId + "'", "archplace");
            var jsonResult3 = JsonConvert.DeserializeObject<ResponseListModel<ArchPlaceModel>>(resultStr3);
            var archplace_data = jsonResult3.obj;
            string thirdTodoId = string.Empty;
            string startUserthirdTodoId = string.Empty;
            if (archplace_data.Count() > 0 && archplace_data[0] != null)
            {
                if (fields == "thirdTodoId" && !string.IsNullOrEmpty(archplace_data[0].thirdTodoId))
                {
                    thirdTodoId = archplace_data[0].thirdTodoId;
                }
                else
                {
                    startUserthirdTodoId = archplace_data[0].startUserthirdTodoId;
                }
            }
            if (!string.IsNullOrEmpty(thirdTodoId))
            {
                //调取OA撤销消息
                //subState 0-同意，1-不同意
                //cancelOAMessage(thirdTodoId, 0);
                cancelOAMessage(param.orgToken, thirdTodoId, subState);
            }
            else if (!string.IsNullOrEmpty(startUserthirdTodoId))
            {
                //调取OA撤销消息
                //subState 0-同意，1-不同意
                //cancelOAMessage(startUserthirdTodoId, 0);
                cancelOAMessage(param.orgToken, startUserthirdTodoId, subState);
            }
        }

        /// <summary>
        /// 甲方OA token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string getOAToken(string token)
        {
            var param = new { restPassword = "restAdmin1", restUsername = "restAdmin", userName = EDoc2SdkHelper.GetUserInfoByToken(token).Account };

            var targeturl = oa_url + "/auth/sso_encrypt_token";

            var resultStr = HttpClientHelper.Post(targeturl, JsonConvert.SerializeObject(param));

            var jsonResult = JsonConvert.DeserializeObject<ResponseModel>(resultStr);

            if (jsonResult.code == "0")
            {
                return jsonResult.data.ToString();
            }
            return null;
        }

        /// <summary>
        /// 调取OA撤销消息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public void cancelOAMessage(string token, string thirdTodoId, int subState)
        {
            var oa_token = getOAToken(token);
            if (!string.IsNullOrEmpty(oa_token))
            {
                var head_token = "Bearer " + oa_token;
                //subState  0-同意，1-不同意
                var param = new
                {
                    thirdTodoId = thirdTodoId,
                    subState = subState
                };
                var targeturl = oa_url + "/rest/thirdToDo/finishaffair";
                var resultStr = HttpClientHelper.PostToken(targeturl, JsonConvert.SerializeObject(param), head_token);
                var jsonResult = JsonConvert.DeserializeObject<ResponseModel>(resultStr);
            }

        }

        /// <summary>
        /// 项目库档案归档 查询当前操作的领导
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userinfo"></param>
        public leaderModel findLeader(string token, EDoc2.IAppService.Model.Organization.UserInfo userinfo)
        {
            if (userinfo != null)
            {
                var cur_dept_id = userinfo.MainDepartmentId;
                var resultStr = getListByCommon(token, "leader,user_account,dept_name ", " company_name = '" + cur_dept_id + "'", "company_leader_mapping");
                var jsonResult = JsonConvert.DeserializeObject<ResponseListModel<leaderModel>>(resultStr);
                return jsonResult.obj[0];
            }
            return null;
        }

        /// <summary>
        /// 档案借阅 查询当前操作的领导
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userinfo"></param>
        /// <param name="BorrowDepartmentId"></param>
        /// <returns></returns>
        public borrowLeaderModel findLeader(string token, EDoc2.IAppService.Model.Organization.UserInfo userinfo, string BorrowDepartmentId)
        {
            if (userinfo != null)
            {
                var cur_dept_id = userinfo.MainDepartmentId;
                if (!string.IsNullOrEmpty(BorrowDepartmentId))
                {
                    cur_dept_id = BorrowDepartmentId;
                }                
                var resultStr = getListByCommon(token, "manager,manager_account,leader,leader_account,boss,boss_account,assistanter,assistanter_account,dept_name ", " company_name = '" + cur_dept_id + "'", "borrow_company_leader_mapping");
                var jsonResult = JsonConvert.DeserializeObject<ResponseListModel<borrowLeaderModel>>(resultStr);
                return jsonResult.obj[0];
            }
            return null;
        }

        /// <summary>
        /// 项目库档案 调取OA新增消息
        /// </summary>
        public string addOAMessage(string token, MessageUser messageUser = null, DataTable archinfodt = null)
        {
            var oa_token = getOAToken(token);
            if (!string.IsNullOrEmpty(oa_token))
            {
                var head_token = "Bearer " + oa_token;

                //当前流程档案信息
                EDoc2.IAppService.Model.Organization.UserInfo userinfo = EDoc2SdkHelper.GetUserInfoByToken(token);

                var thirdReceiverId = "";
                var noneBindingReceiver = "";
                var title = "";
                var content = "档案归档";
                if (messageUser != null)
                {
                    noneBindingReceiver = messageUser.account;
                    thirdReceiverId = messageUser.id;
                    if (archinfodt != null && archinfodt.Rows.Count > 0)
                    {
                        title += archinfodt.Rows[0]["name"].ToString() + "，已回退。";
                        content = archinfodt.Rows[0]["mainCompanyName"].ToString() + "的" + archinfodt.Rows[0]["mainProjectName"].ToString() + "项目，现在" + userinfo.Name + "来申请归档";
                    }
                }
                else
                {
                    //查询当前操作者的领导
                    var leaderModel = findLeader(token, userinfo);
                    noneBindingReceiver = leaderModel.user_account;
                    var leader = JsonConvert.DeserializeObject<List<Leader>>(leaderModel.leader);
                    thirdReceiverId = leader[0].id;
                    if (archinfodt != null && archinfodt.Rows.Count > 0)
                    {
                        title += archinfodt.Rows[0]["name"].ToString() + "。";
                        content = archinfodt.Rows[0]["mainCompanyName"].ToString() + "的" + archinfodt.Rows[0]["mainProjectName"].ToString() + "项目，现在" + userinfo.Name + "来申请归档";
                    }
                }

                var param = new
                {
                    classify = "档案归档",
                    h5url = oa_return_url,
                    thirdReceiverId = thirdReceiverId,
                    thirdSenderId = userinfo.ID,
                    senderName = userinfo.Name,
                    noneBindingReceiver = noneBindingReceiver,
                    title = title,
                    content = content,
                    url = oa_return_url,
                    creationDate = DateTime.Now
                };
                var targeturl = $"{oa_url}/rest/thirdToDo/bpmSend";
                var resultStr = HttpClientHelper.PostToken(targeturl, JsonConvert.SerializeObject(param), head_token);
                var jsonResult = JsonConvert.DeserializeObject<ResponseModel>(resultStr);

                try
                {
                   var data = JsonConvert.DeserializeObject<ArchPlaceModel>(jsonResult.data.ToString());
                    return data.thirdTodoId.ToString();
                }
                catch (Exception)
                {
                    return jsonResult.data == null ? "" : jsonResult.data.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// 档案借阅 调取OA新增消息
        /// </summary>
        public string addOAMessageBorrow(string token, string BorrowDepartmentId, Leader leaderUser , DataTable archinfodt)
        {
            var oa_token = getOAToken(token);
            if (!string.IsNullOrEmpty(oa_token))
            {
                var head_token = "Bearer " + oa_token;

                //当前流程档案信息
                EDoc2.IAppService.Model.Organization.UserInfo userinfo = EDoc2SdkHelper.GetUserInfoByToken(token);

                var thirdReceiverId = "";
                var noneBindingReceiver = "";
                var title = "";
                var content = "借阅申请";
                //查询当前操作者的领导
                borrowLeaderModel leaderConfig = findLeader(token,userinfo, BorrowDepartmentId);

                var is_flag = true;

                if (leaderUser != null && leaderConfig != null)
                {
                    if (!string.IsNullOrEmpty(leaderConfig.manager.ToString()) && leaderUser.id == JsonConvert.DeserializeObject<List<Leader>>(leaderConfig.manager.ToString())[0].id)//负责人管理员
                    {
                        noneBindingReceiver = leaderConfig.manager_account;
                        thirdReceiverId = leaderUser.id;
                    }
                    else if (!string.IsNullOrEmpty(leaderConfig.leader.ToString()) && leaderUser.id == JsonConvert.DeserializeObject<List<Leader>>(leaderConfig.leader.ToString())[0].id)//档案管理员
                    {
                        noneBindingReceiver = leaderConfig.leader_account;
                        thirdReceiverId = leaderUser.id;
                    }
                    else if (!string.IsNullOrEmpty(leaderConfig.assistanter.ToString()) && leaderUser.id == JsonConvert.DeserializeObject<List<Leader>>(leaderConfig.assistanter.ToString())[0].id)//总部区域对接人
                    {
                        noneBindingReceiver = leaderConfig.assistanter_account;
                        thirdReceiverId = leaderUser.id;
                    }
                    else if (!string.IsNullOrEmpty(leaderConfig.boss.ToString()) && leaderUser.id == JsonConvert.DeserializeObject<List<Leader>>(leaderConfig.boss.ToString())[0].id)//总部管理员
                    {
                        noneBindingReceiver = leaderConfig.boss_account;
                        thirdReceiverId = leaderUser.id;
                    }
                    else if (!string.IsNullOrEmpty(leaderUser.account))//加签人
                    {
                        noneBindingReceiver = leaderUser.account;
                        thirdReceiverId = leaderUser.id;
                    }
                    else
                    {
                        is_flag = false;
                    }

                    if (is_flag)
                    {
                        if (archinfodt != null && archinfodt.Rows.Count > 0)
                        {
                            title += archinfodt.Rows[0]["name"].ToString() + "档案。";
                            content = archinfodt.Rows[0]["borrower"].ToString() + "申请";
                        }
                        for (var i = 0; i < archinfodt.Rows.Count; i++)
                        {
                            content += "借阅档案【" + archinfodt.Rows[i]["name"].ToString() + "】，借阅天数" + archinfodt.Rows[0]["brrowday"].ToString() + "天;";
                        }

                        var param = new
                        {
                            classify = "档案借阅",
                            h5url = oa_return_url,
                            thirdReceiverId = thirdReceiverId,
                            thirdSenderId = userinfo.ID,
                            senderName = userinfo.Name,
                            noneBindingReceiver = noneBindingReceiver,
                            title = title,
                            content = content,
                            url = oa_return_url,
                            creationDate = DateTime.Now
                        };
                        var targeturl = $"{oa_url}/rest/thirdToDo/bpmSend";
                        var resultStr = HttpClientHelper.PostToken(targeturl, JsonConvert.SerializeObject(param), head_token);
                        var jsonResult = JsonConvert.DeserializeObject<ResponseModel>(resultStr);

                        try
                        {
                            var data = JsonConvert.DeserializeObject<ArchPlaceModel>(jsonResult.data.ToString());
                            return data.thirdTodoId.ToString();
                        }
                        catch (Exception)
                        {
                            return jsonResult.data == null ? "" : jsonResult.data.ToString();
                        }
                    }
                }
            }
            return null;
        }

        #endregion


    }
}
