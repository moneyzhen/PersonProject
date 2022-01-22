using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class FlowInfoDto
    {
        /// <summary>
        /// 审批操作类型（1同意approve、2终止cancel、3退回returns）
        /// </summary>
        public actionTypeEnum actionType { get; set; }
        public string orgToken { get; set; }
        /// <summary>
        /// 操作类型（我的待办InboxTask、在途流程CompleteTask、完结流程ArchiveTask）
        /// </summary>
        public taskTypeEnum taskType { get; set; }
        /// <summary>
        /// 流程ID（包含实例编号）
        /// </summary>
        public string processId { get; set; }
        /// <summary>
        /// 流程ID
        /// </summary>
        public string incidentId { get; set; }
        /// <summary>
        /// 流程表单记录ID
        /// </summary>
        public string businessKey { get; set; }
        /// <summary>
        /// 任务ID(我的待办、在途流程不可为空，完结流程为空)
        /// </summary>
        public string taskId { get; set; }
        /// <summary>
        /// 任务状态(我的待办必传默认值“0”,在途流程和完结流程不传)
        /// </summary>
        public string state { get; set; }
        /// <summary>
        /// 审批意见(可为空)
        /// </summary>
        public string summary { get; set; }
        /// <summary>
        /// 档案名称
        /// </summary>
        public string archName { get; set; }


    }
}
