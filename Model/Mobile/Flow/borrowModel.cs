using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class borrowModel
    {
        public string archuserThirdTodoId { get; set; }
        public string assistanterThirdTodoId { get; set; }
        public string supervisorThirdTodoId { get; set; }
        public string managerThirdTodoId { get; set; }
        public string plusThirdTodoId { get; set; }
    }

    public class Leader
    {
        public string id { get; set; }
        public string guid { get; set; }
        public string text { get; set; }
        public int memberType { get; set; }
        public int identityId { get; set; }
        public string userEmail { get; set; }
        public string account { get; set; }
    }

    public class borrowLeaderModel
    {
        public string manager { get; set; }
        public string manager_account { get; set; }
        public string leader { get; set; }
        public string leader_account { get; set; }
        public string boss { get; set; }
        public string boss_account { get; set; }
        public string assistanter { get; set; }
        public string assistanter_account { get; set; }
        public string dept_name { get; set; }
    }

}
