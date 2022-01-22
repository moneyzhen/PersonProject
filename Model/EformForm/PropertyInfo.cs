using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; } = string.Empty;

        /// <summary>
        /// 编号
        /// </summary>
        public string id { get; set; } = string.Empty;

        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; } = string.Empty;

        public PropertyInfo()
        {
        }

        public PropertyInfo(string id, string name, string value)
        {
            this.id = id;
            this.name = name;
            this.value = value;
        }
    }
}
