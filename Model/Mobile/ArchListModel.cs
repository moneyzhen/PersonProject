using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.Mobile
{
    public class ArchListModel
    {
        /// <summary>
        /// 档案ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 档案类型
        /// </summary>
        public string archType { get; set; }
        /// <summary>
        /// 项目进度ID
        /// </summary>
        public string archtypeid { get; set; }
        /// <summary>
        /// 版次
        /// </summary>
        public string edition { get; set; }
        /// <summary>
        /// 地区公司
        /// </summary>
        public string mainCompanyName { get; set; }
        /// <summary>
        /// 项目模式
        /// </summary>
        public string mainProjectMode { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string mainProjectName { get; set; }
        /// <summary>
        /// 项目类型
        /// </summary>
        public string mainProjectType { get; set; }
        /// <summary>
        /// 档案名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string note { get; set; }
        /// <summary>
        /// 项目ID
        /// </summary>
        public string project_manage_id { get; set; }
        /// <summary>
        /// 档案表名
        /// </summary>
        public string tableName { get; set; }
        /// <summary>
        /// 成文日期
        /// </summary>
        public string writtendate { get; set; }
    }
}
