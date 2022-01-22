using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    public class FormData
    {
        /// <summary>
        /// 表单编号
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        public virtual string FormName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// 绑定的表名称
        /// </summary>
        public virtual string TableName { get; set; }

        /// <summary>
        /// 绑定的主题名称
        /// </summary>
        public virtual string Theme { get; set; }

        /// <summary>
        /// 绑定的或者程序集Id
        /// </summary>
        public virtual string Assembly { get; set; }

        /// <summary>
        /// js前端绑定
        /// </summary>
        public virtual string JsExtend { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        public virtual DateTime ModifiedTime { get; set; }

        /// <summary>
        /// 发布状态
        /// </summary>
        public virtual PublishStatus PublishStatus { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Content { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public virtual string CreatorId { get; set; }

        /// <summary>
        /// 样式
        /// </summary>
        public virtual string Style { get; set; }



        /// <summary>
        /// 表单是否为只读
        /// </summary>
        public virtual int Readonly { get; set; }

        /// <summary>
        /// 表单是否为只读
        /// </summary>
        public virtual int RequiredMark { get; set; }

        /// <summary>
        /// label位置
        /// </summary>
        public virtual string PageLayoutOptions { get; set; }
        /// 表单多语言
        /// </summary>
        public string LanguageContent { get; set; }

        // <summary>
        /// 图表主题
        /// </summary>
        public virtual string ChartTheme { get; set; }

        /// <summary>
        /// 应用分类
        /// </summary>
        public virtual FormGroupData FormGroup { get; set; }

        /// <summary>
        /// 应用分类ID
        /// </summary>
        public virtual string GroupId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        public virtual DataSourceType DataSourceType { get; set; }

        /// <summary>
        /// 表单类型
        /// </summary>
        public virtual FormType FormType { get; set; }

        /// <summary>
        /// 页面类型，标识PC/移动端
        /// </summary>
        public virtual PageType PageType { get; set; }

        /// <summary>
        /// 表单配置编号，通过该编号在控件属性表中查找表单控件的属性配置(表单的移动版和PC版的该字段值相同)
        /// </summary>
        public virtual string FormSettingId { get; set; }



        /// <summary>
        /// 对应的表单版本编号,多版本使用
        /// </summary>
        public virtual int FormVer { get; set; }


        /// <summary>
        /// 对应的表单版本备注,多版本使用
        /// </summary>
        public virtual string VerRemark { get; set; }






        /// <summary>
        /// 数据源连接key，数据从formsetting中获取(该属性不创建字段)
        /// </summary>
        public virtual string ConnStringKey { get; set; }

        /// <summary>
        /// 业务表为外部数据表时的主键字段，数据从formsetting中获取(该属性不创建字段)
        /// </summary>
        public virtual string PrimaryKey { get; set; }




        /// <summary>
        /// 解析前事件,数据从formsetting中获取(该属性不创建字段)
        /// </summary>
        public virtual string OnLoadBefore { get; set; }

        /// <summary>
        /// 解析后事件,数据从formsetting中获取(该属性不创建字段)
        /// </summary>
        public virtual string OnLoaded { get; set; }


        /// <summary>
        /// 公共扩展，数据从formsetting中获取(该属性不创建字段)
        /// </summary>
        public virtual string PublicExtend { get; set; }


        /// <summary>
        /// 公共css，数据从formsetting中获取(该属性不创建字段)
        /// </summary>
        public virtual string PublicCss { get; set; }


        /// <summary>
        /// 资源
        /// </summary>
        public virtual string Resources { get; set; } // Resource在Oracle是关键字，属性名不改，字段名改成Resources


        /// <summary>
        /// 多语言Key
        /// </summary>
        public virtual string Langs { get; set; }

        ///// <summary>
        ///// 控件验证方式
        ///// </summary>
        //public virtual ValidateType ValidateType { get; set; }

        ///// <summary>
        ///// 验证信息显示方式
        ///// </summary>
        //public virtual ValidateDisplayType ValidateDisplayType { get; set; }



        ///// <summary>
        ///// 必填项是否加标记
        ///// </summary>
        //public virtual MustInputType MustInputType { get; set; }

        /// <summary>
        /// 是否自增长
        /// </summary>
        public virtual string IsIncreaseAttribute { get; set; }

        /// <summary>
        /// 自增字段
        /// </summary>
        public virtual string AttributeCode { get; set; }
        /// <summary>
        /// 自增值
        /// </summary>
        public virtual string AttributeValue { get; set; }
        /// <summary>
        /// 自增前缀内容
        /// </summary>
        public virtual string AttributePrefix { get; set; }
        /// <summary>
        /// 自增后缀内容
        /// </summary>
        public virtual string AttributeSuffix { get; set; }


    }
}
