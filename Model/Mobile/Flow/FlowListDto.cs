using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class FlowListDto
    {
        /// <summary>
        /// 当前登录用户ID
        /// </summary>
        public string userId { get; set; }

        /// <summary>
        /// 操作类型（0我的待办、1在途流程、2完结流程）
        /// </summary>
        public int proType { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int rows { get; set; }

    }
}
