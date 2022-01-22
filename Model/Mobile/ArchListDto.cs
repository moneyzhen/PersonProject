using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class ArchListDto
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string departmentName { get; set; }
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
        /// 当前页
        /// </summary>
        public int curPage { get; set; }
        /// <summary>
        /// 条数
        /// </summary>
        public int pageSize { get; set; }
        /// <summary>
        /// 条件过滤
        /// </summary>
        public Filters filter { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public string isShow { get; set; } = "1";
        /// <summary>
        /// 进度ID
        /// </summary>
        public string archTypeId { get; set; } = string.Empty;
        /// <summary>
        /// 档案状态
        /// </summary>
        public string entryState { get; set; } = "1";
        /// <summary>
        /// 加密数据
        /// </summary>
        public string data { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string dataIdPath { get; set; } = "/0";
    }

    public class Filters
    {
        /// <summary>
        /// 项目ID（项目详情项目档案必传)
        /// </summary>
        public string project_manage_id { get; set; }
        /// <summary>
        /// 地区公司（可多选，选择内容以逗号隔开  如：“安徽公司,江苏公司”）
        /// </summary>
        public string mainCompanyName { get; set; }
        /// <summary>
        /// 项目模式（可多选，选择内容以逗号隔开  如：“BOT,产业园投资”）
        /// </summary>
        public string mainProjectMode { get; set; }
        /// <summary>
        /// 档案名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string mainProjectName { get; set; }

    }

}
