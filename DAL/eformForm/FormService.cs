using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZHONGJIAN_API.Common;
using ZHONGJIAN_API.Model.EformForm;
using ZHONGJIAN_API.Model.Mobile;

namespace ZHONGJIAN_API.DAL.eformForm
{
    public class FormService
    {
        private readonly MobileService mobileService = new MobileService();
        private readonly MobileDao mobileDao = new MobileDao();
        private readonly FormDao formDao = new FormDao();

        /// <summary>
        /// 获取当前表单指定记录所有控件和控件值
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="recordId"></param>
        /// <returns></returns>
        public List<FlowControlsDatas> GetFormInfo(string formId,string recordId)
        {
            List<FlowControlsDatas> controlsDataList = new List<FlowControlsDatas>();
            DataTable formdt = formDao.GetFormById(formId);
            List<FormData> dbModelList = CommonHelper.GetModelFromDB<FormData>(formdt);
            if (dbModelList != null && dbModelList.Count >0)
            {
                foreach (var dbModel in dbModelList)
                {
                    FormInfo formInfo = JsonConvert.DeserializeObject<FormInfo>(dbModel.Content);
                    var tableName = dbModel.TableName;//当前表单表名
                    DataTable dt = formDao.GetDataById(tableName, recordId);
                    if (dt != null && dt.Rows.Count == 0 )
                    {
                        return null;
                    }
                    // 解析控件
                    List<ControlInfo> controls = formInfo.GetControls().ToList();
                    if (controls != null && controls.Any())
                    {
                        foreach (var control in controls)
                        {
                            if (dt.Columns.Contains(control.ControlId))
                            {
                                FlowControlsDatas controlData = new FlowControlsDatas();
                                controlData.Id = control.Id;
                                controlData.controlType = control.ControlType;
                                controlData.key = control.ControlId;
                                controlData.value = dt.Rows[0][control.ControlId].ToString();
                                controlData.json = controlData.value;

                                controlsDataList.Add(controlData);

                            }
                        }

                        return controlsDataList;
                    }

                }
            }

            return null;
        }

        /// <summary>
        /// 匹配流程表单所有控件查询流程变量列表
        /// </summary>
        /// <param name="formId"></param>
        /// <param name="recordId"></param>
        /// <param name="processId"></param>
        /// <returns></returns>
        public List<FlowVariableModel> GetVarslist(string formId, string recordId,string processId)
        {
            //流程变量
            List<FlowVariableModel> varslist = new List<FlowVariableModel>();
            try
            {
                //取当前流程表单所有变量参数及值
                List<FlowControlsDatas> controlslist = GetFormInfo(formId, recordId);
                //获取所有变量
                DataTable vardt = mobileService.GetProcessVars(processId);
                List<FlowVariableData> variables = CommonHelper.GetModelFromDB<FlowVariableData>(vardt);
                if (variables != null && controlslist != null)
                {
                    foreach (var modelVariable in variables)
                    {
                        if (!string.IsNullOrEmpty(modelVariable.RALATION_ID_) && modelVariable.TYPE_ == "1")
                        {
                            FlowControlsDatas data = controlslist.FirstOrDefault(t => t.Id == modelVariable.RALATION_ID_);
                            if (data != null)
                            {
                                FlowVariableModel varsmodel = new FlowVariableModel();
                                varsmodel.name = modelVariable.VAR_NAME_;
                                varsmodel.value = data.value;
                                varslist.Add(varsmodel);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
            }

            return varslist;
        }

    }
}
