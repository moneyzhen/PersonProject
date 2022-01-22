using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    public class ColumnInfo
    {
        /// <summary>
        /// 列Id
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 是否显示
        /// </summary>
        public string Show { get; set; } = string.Empty;

        /// <summary>
        /// 列标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 列宽
        /// </summary>
        public string Width { get; set; } = string.Empty;
        /// <summary>
        /// 是否使用erms样式
        /// </summary>
        public string IsErms { get; set; } = string.Empty;
        /// <summary>
        /// 是否显示列
        /// </summary>
        public string ShowHeader { get; set; } = string.Empty;

        /// <summary>
        /// 列所包含的控件
        /// </summary>
        public IList<ControlInfo> Controls { get; set; } = new List<ControlInfo>();
    }
}
