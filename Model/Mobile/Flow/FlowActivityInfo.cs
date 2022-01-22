using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class FlowActivityInfo
    {
        public string processInstanceId { get; set; }
        public string activityId { get; set; }
        public string thirdTodoId { get; set; }
        public string activityNo { get; set; }
        public string completionTime { get; set; }
        public string completionTimeUnit { get; set; }
        public string performerCategory { get; set; }
        public string performerType { get; set; }
        public string minReplyNum { get; set; }
        public string plusSign { get; set; }
        public string plusSignLogic { get; set; }
        public string ccCategory { get; set; }
        public string ccMember { get; set; }
        public string ccMemberType { get; set; }
        public string taskId { get; set; }
        public string jumpRule { get; set; }
        public string flowNodeBeans { get; set; }
        public string extProps { get; set; }
        public string assignee { get; set; }
        public string expression { get; set; }
        public string currentNodeTaskCount { get; set; }
        public string advanceNoticeTime { get; set; }
        public string advanceNoticeTimeUnit { get; set; }
        public string candidateNotice { get; set; }
        public string returnPreStep { get; set; }
        public string returnStarter { get; set; }
        public string cdirectBack { get; set; }
        public string taskCCNotice { get; set; }
        public string sendAssign { get; set; }
        public string toNextActivity { get; set; }
        public string taskArrivedNotice { get; set; }
        public string taskCompletionNotice { get; set; }
        public string taskTimeOutNotice { get; set; }
        public string taskTurnBackNotice { get; set; }
        public string includeMutiTask { get; set; }
        public string multiInstance { get; set; }
        public string noticeSended { get; set; }
        public string subProcess { get; set; }
        public string endSendNotice { get; set; }
    }
}
