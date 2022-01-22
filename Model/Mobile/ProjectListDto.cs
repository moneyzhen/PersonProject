using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class ProjectListDto
    {
        /// <summary>
        /// //项目管理列表表单ID （默认值“21081915271614_edrms”）
        /// </summary>
        public string formId { get; set; } = "21081915271614_edrms";

        /// <summary>
        /// 排序字段
        /// </summary>
        public string dynField { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public string dynOrder { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 行数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 条件过滤
        /// </summary>
        public Filter filter { get; set; }
    }

    public class Filter
    {
        /// <summary>
        /// 地区公司（可多选，选择内容以逗号隔开 如：“安徽公司,江苏公司”）
        /// </summary>
        public string company_name { get; set; }
        /// <summary>
        /// 项目模式（可多选，选择内容以逗号隔开 如：“BOT,产业园投资”）
        /// </summary>
        public string project_mode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string project_name { get; set; }

    }

}
