using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    /// <summary>
    /// 流程统计参数
    /// </summary>
    public class FlowStatisticsDto
    {
        /// <summary>
        /// 当前登录用户ID
        /// </summary>
        public string userId { get; set; }
        /// <summary>
        /// 类型（默认“inbox"）
        /// </summary>
        public string type { get; set; } = "inbox";
        /// <summary>
        /// 分组类型（默认“Process”）
        /// </summary>
        public string groupType { get; set; } = "Process";

    }





}
