using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    /// <summary>
    /// 流程统计
    /// </summary>
    public class FlowStatisticsModel
    {
        public AllFlowStatistics all { get; set; }
        public List<FlowGroupData> GroupData { get; set; }
    }

    public class AllFlowStatistics {
        /// <summary>
        /// 我的待办数
        /// </summary>
        public int InboxCount { get; set; }
        /// <summary>
        /// 我的归档数(完结流程数)
        /// </summary>
        public int ArchiveCount { get; set; }
        /// <summary>
        /// 我的申请数
        /// </summary>
        public int InitCount { get; set; }
        /// <summary>
        /// 抄送给我数
        /// </summary>
        public int CctomeCount { get; set; }
        /// <summary>
        /// 流程申请数
        /// </summary>
        public int ApplyCount { get; set; }
        /// <summary>
        /// 我的已办数（在途流程数）
        /// </summary>
        public int CompleteCount { get; set; }
    }

    public class FlowGroupData {
        public string name { get; set; }
        public int count { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public string id { get; set; }
        public string targetType { get; set; }
        public string version { get; set; }
        public string formId { get; set; }
        public string children { get; set; }
    }

}
