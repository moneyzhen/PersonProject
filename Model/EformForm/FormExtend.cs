using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{

    /// <summary>
    /// 表单发布状态
    /// </summary>
    public enum PublishStatus
    {
        /// <summary>
        /// 未发布
        /// </summary>
        UnPublish = 0,
        /// <summary>
        /// 已发布
        /// </summary>
        Publish = 1
    }

    /// <summary>
    /// 表单数据源类型
    /// </summary>
    public enum DataSourceType
    {
        Table,
        DbView,
        Procedure,
        /// <summary>
        /// sql执行脚本
        /// </summary>
        SqlScript = 998,
        /// <summary>
        /// es 索引
        /// </summary>
        ESIndex = 999
    }

    public enum PageType
    {
        /// <summary>
        /// PC
        /// </summary>
        Pc = 0,

        /// <summary>
        /// Mobile
        /// </summary>
        Mobile = 1
    }


    public enum FormType
    {
        /// <summary>
        /// 系统预置
        /// </summary>
        System = 0,

        /// <summary>
        /// 用户自定义
        /// </summary>
        Custom = 1
    }

    /// <summary>
    /// 多语言类型
    /// </summary>
    public enum LangType
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        GroupName = 0,

        /// <summary>
        /// 表单名称
        /// </summary>
        FormName = 1,
        /// <summary>
        /// 其他多语言
        /// </summary>
        Other = 2,
        /// <summary>
        /// 系统多语言
        /// </summary>
        SystemLang = 3,

        Default = -1

    }


    public enum GroupType
    {
        /// <summary>
        /// 系统预置
        /// </summary>
        System = 0,

        /// <summary>
        /// 用户自定义
        /// </summary>
        Custom = 1
    }
}
