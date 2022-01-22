using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public enum taskTypeEnum
    {
        /// <summary>
        /// 我的待办
        /// </summary>
        InboxTask = 0,
        /// <summary>
        /// 我的已办
        /// </summary>
        CompleteTask = 1,
        /// <summary>
        /// 我的归档
        /// </summary>
        ArchiveTask = 2,
        /// <summary>
        /// 发起流程
        /// </summary>
        beginTask = 3,
        /// <summary>
        /// 我的申请
        /// </summary>
        myStartTask = 4


    }

    /// <summary>
    /// 流程事件类型
    /// </summary>
    public enum actionTypeEnum
    {
        /// <summary>
        /// 同意
        /// </summary>
        approve = 0,
        /// <summary>
        /// 退回
        /// </summary>
        returns = 1,
        /// <summary>
        /// 终止
        /// </summary>
        cancel = 2,
        /// <summary>
        /// 发起
        /// </summary>
        initiate = 3,
        /// <summary>
        /// 退回上一步
        /// </summary>
        returnPreStep = 4,
        /// <summary>
        /// 退回发起人
        /// </summary>
        returnStarter = 5,

    }

}
