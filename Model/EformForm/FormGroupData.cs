using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    public class FormGroupData
    {
        /// <summary>
        /// 表单编号
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 分組名称
        /// </summary>
        public virtual string GroupName { get; set; }

        /// <summary>
        /// 分組编号(1级分组有效)
        /// </summary>
        public virtual string GroupNo { get; set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        public virtual PublishStatus PublishStatus { get; set; }


        /// <summary>
        /// 创建
        /// </summary>
        public virtual string CreatorId { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime ModifiedTime { get; set; }


        /// <summary>
        /// 该分组的上级分组编号
        /// </summary>
        public virtual string ParentId { get; set; }

        /// <summary>
        /// 该分组的顶级分组编号
        /// </summary>
        public virtual string RootId { get; set; }


        /// <summary>
        /// 该分组的秘钥
        /// </summary>
        public virtual string GroupSecretKey { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        public virtual string AppId { get; set; }

        /// <summary>
        /// 分组类型
        /// </summary>
        public virtual GroupType GroupType { get; set; }

        /// <summary>
        /// 是否存在文控流程lic
        /// 0表示不存在，1表示存在
        /// </summary>
        public long LicDocControlProcessCode = 0;
    }
}
