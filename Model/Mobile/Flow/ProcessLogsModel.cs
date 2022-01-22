using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class ProcessLogsModel
    {
        public int currPage { get; set; }
        public int pageSize { get; set; }

        public string customSql { get; set; } = string.Empty;

        public string orderType { get; set; } = string.Empty;

        public List<ProcessLogs> rows { get; set; }

        public int total { get; set; }
    }

    public class ProcessLogs
    {
        public string id { get; set; }
        public string taskState { get; set; }
        public string name { get; set; }
        public string assignee { get; set; }
        public string assigneeName { get; set; }
        public string comment { get; set; }
        public string endTime { get; set; }
        public string startTime { get; set; }
        public string currentTaskName { get; set; }
        public string ccMember { get; set; }
        public string remark_ { get; set; }
        public string task_Def_id { get; set; }
    }

}
