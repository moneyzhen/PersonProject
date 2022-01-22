using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    /// <summary>
    /// 表单信息
    /// </summary>
    public class FormInfo
    {
        /// <summary>
        /// 表单Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string FormName { get; set; }

        /// <summary>
        /// 表单绑定的主题名称
        /// </summary>
        public string Theme { get; set; } = "Default";


        /// <summary>
        /// 表单是否为只读
        /// </summary>
        public bool Readonly { get; set; } = false;


        /// <summary>
        /// 控件必填标记
        /// </summary>
        public bool RequiredMark { get; set; } = false;
        /// <summary>
        /// label位置
        /// </summary>
        public string PageLayoutOptions { get; set; }
        /// <summary>
        /// 表单多语言
        /// </summary>
        public string LanguageContent { get; set; }
        /// <summary>
        /// 图表主题
        /// </summary>
        public string ChartTheme { get; set; }

        /// <summary>
        /// 表单描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 表单创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 表单创建人
        /// </summary>
        public string CreatorId { get; set; }

        /// <summary>
        /// 表单修改时间
        /// </summary>
        public DateTime ModifiedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 表单发布状态
        /// </summary>
        public PublishStatus Status { get; set; } = PublishStatus.UnPublish;

        /// <summary>
        /// 风格
        /// </summary>
        public string Style { get; set; }

        /// <summary>
        /// 数据源类型
        /// </summary>
        public DataSourceType DataSourceType { get; set; } = DataSourceType.Table;


        /// <summary>
        /// 绑定的 表,视图 名称
        /// </summary>
        public string TableName { get; set; }


        /// <summary>
        /// 表单所属分组ID
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 所属一级分组编号
        /// </summary>
        public string RootGroupId { get; set; }

        /// <summary>
        /// 页面类型，标识PC/移动端
        /// </summary>
        public PageType PageType { get; set; } = PageType.Pc;


        public string FormSettingId { get; set; }


        public int FormVer { get; set; } = 0;


        public string VerRemark { get; set; }


        public string ConnStringKey { get; set; }


        public string PrimaryKey { get; set; }

        /// <summary>
        /// 绑定的程序集Id
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 绑定的程序集Id
        /// </summary>
        public string JsExtend { get; set; }



        /// <summary>
        /// 外部资源
        /// </summary>
        public string Resources { get; set; }

        /// <summary>
        /// 当前表单多语言
        /// </summary>
        public string Langs { get; set; }

        ///// <summary>
        ///// 控件验证方式
        ///// </summary>
        //public ValidateType ValidateType = ValidateType.Single;

        ///// <summary>
        ///// 验证信息显示方式
        ///// </summary>
        //public ValidateDisplayType ValidateDisplayType = ValidateDisplayType.Around;


        ///// <summary>
        ///// 必填项是否加标记
        ///// </summary>
        //public MustInputType MustInputType = MustInputType.No;

        /// <summary>
        /// 是否自增长
        /// </summary>
        public string IsIncreaseAttribute { get; set; }

        /// <summary>
        /// 自增字段
        /// </summary>
        public string AttributeCode { get; set; }
        /// <summary>
        /// 自增值
        /// </summary>
        public string AttributeValue { get; set; }
        /// <summary>
        /// 自增前缀内容
        /// </summary>
        public string AttributePrefix { get; set; }
        /// <summary>
        /// 自增后缀内容
        /// </summary>
        public string AttributeSuffix { get; set; }


        /// <summary>
        /// 表单所包括的容器
        /// </summary>
        public ContainerInfo Container { get; set; } = new ContainerInfo();
        /// <summary>
        /// 列所包含的块
        /// </summary>
        public IList<BlockInfo> Blocks { get; set; } = new List<BlockInfo>();
        /// <summary>
        /// 获取表单的所有控件
        /// </summary>
        /// <returns></returns>
        public IList<ControlInfo> GetControls()
        {
            var controls = new List<ControlInfo>();
            GetControlsByContainer(this.Container, controls);
            foreach (var control in controls)
            {
                //兼容老数据,修复流程中的几个控件不是以edoc2开头的,加载js会用到,window.edoc2Form.edoc2WorkFlowButton
                if (!control.ControlType.StartsWith("edoc2"))
                {
                    control.ControlType = "edoc2" + control.ControlType.Substring(0, 1).ToUpper() + control.ControlType.Substring(1);
                }
            }
            return controls;
        }

        public bool RemoveControl(string controlGuid)
        {
            return RemoveControlByContainer(this.Container, controlGuid);
        }

        /// <summary>
        /// 在给定的container内获取所有控件,支持容器嵌套
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        public void GetControlsByContainer(ContainerInfo container, IList<ControlInfo> controls)
        {
            //兼容V4老数据
            if (container.Blocks.Count == 0)
            {
                if (this.Blocks.Count > 0)
                {
                    container.Blocks = this.Blocks;
                }
            }
            foreach (var block in container.Blocks)
            {
                foreach (var column in block.Columns)
                {
                    foreach (var control in column.Controls)
                    {
                        controls.Add(control);
                        if (control.Containers == null) continue;
                        foreach (var _container in control.Containers)
                        {
                            GetControlsByContainer(_container, controls);
                        }
                    }
                }
            }
        }



        public bool RemoveControlByContainer(ContainerInfo container, string controlGuid)
        {
            foreach (var block in container.Blocks)
            {
                foreach (var column in block.Columns)
                {
                    foreach (var control in column.Controls)
                    {
                        if (control.Id == controlGuid)
                        {
                            column.Controls.Remove(control);
                            return true;
                        }
                        if (control.Containers == null) continue;
                        foreach (var _container in control.Containers)
                        {
                            return RemoveControlByContainer(_container, controlGuid);
                        }
                    }
                }
            }

            return false;
        }




        /// <summary>
        /// 在给定的block内获取所有控件,支持容器嵌套
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        public void GetControlsByBlock(BlockInfo block, IList<ControlInfo> controls)
        {
            foreach (var column in block.Columns)
            {
                foreach (var control in column.Controls)
                {
                    controls.Add(control);
                    if (control.Containers == null) continue;
                    foreach (var _container in control.Containers)
                    {
                        GetControlsByContainer(_container, controls);
                    }
                }
            }
        }


        /// <summary>
        /// 在给定的column内获取所有控件,支持容器嵌套
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        public void GetControlsByColumn(ColumnInfo column, IList<ControlInfo> controls)
        {
            foreach (var control in column.Controls)
            {
                controls.Add(control);
                if (control.Containers == null) continue;
                foreach (var _container in control.Containers)
                {
                    GetControlsByContainer(_container, controls);
                }
            }
        }



        /// <summary>
        /// 根据控件id获取control,支持容器嵌套
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ControlInfo GetControlByID(string controlId)
        {
            var controls = GetControls();
            return controls.SingleOrDefault(control => control.ControlId.ToLower().Equals(controlId.ToLower()));

        }

        /// <summary>
        /// 根据字段
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ControlInfo GetControlByFiledName(string filedName)
        {
            var controls = GetControls();
            return controls.FirstOrDefault(control => control.FiledName.ToLower().Equals(filedName.ToLower()));
        }

        //获取表单的所有控件
        public IList<ControlInfo> GetControlsByType(string type)
        {
            var result = GetControls();
            return result.Where(t => t.ControlType.Equals(type)).ToList();
        }


        /// <summary>
        /// 在给定的容器内获取块对象
        /// </summary>
        /// <param name="blocks"></param>
        /// <param name="controls"></param>
        /// <returns></returns>
        public void GetBlocksByContainer(ContainerInfo container, IList<BlockInfo> blocks)
        {
            foreach (var block in container.Blocks)
            {
                blocks.Add(block);
                foreach (var column in block.Columns)
                {
                    foreach (var control in column.Controls)
                    {
                        if (control.Containers == null) continue;
                        foreach (var _container in control.Containers)
                        {
                            GetBlocksByContainer(_container, blocks);
                        }
                    }
                }
            }
        }

    }
}
