using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    public class ContainerInfo
    {
        /// <summary>
        /// 容器Id
        /// </summary>
        public string Id { get; set; } = string.Empty;


        /// <summary>
        /// 列所包含的块
        /// </summary>
        public IList<BlockInfo> Blocks { get; set; } = new List<BlockInfo>();
    }
}
