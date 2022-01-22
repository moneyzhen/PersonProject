using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    public class BlockInfo
    {
        /// <summary>
        /// 区块Id
        /// </summary>
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 布局标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        ///是否向下合并
        /// </summary>
        public string Merge { get; set; } = string.Empty;

        /// <summary>
        /// 是否异步加载(this.Merge是true时有效)
        /// </summary>
        public string Async { get; set; } = string.Empty;

        /// <summary>
        /// 视图
        /// </summary>
        public string Views { get; set; } = string.Empty;

        /// <summary>
        ///是否分组
        /// </summary>
        public string Group { get; set; } = string.Empty;

        /// <summary>
        /// 是否展开(this.Group是true时有效)
        /// </summary>
        public string Unfold { get; set; } = string.Empty;
        /// <summary>
        /// 是否展开(this.Group是true时有效)
        /// </summary>
        public string GroupMutil { get; set; } = string.Empty;

        /// <summary>
        /// 是否显示
        /// </summary>
        public string Show { get; set; } = string.Empty;

        /// <summary>
        /// 是否标题栏
        /// </summary>
        public string ShowHeader { get; set; } = string.Empty;

        /// <summary>
        /// 区块所包含的列
        /// </summary>
        public IList<ColumnInfo> Columns { get; set; } = new List<ColumnInfo>();

        /// <summary>
        /// 根据id获取区块内的控件
        /// <param name="id"></param>
        /// </summary>
        public ControlInfo GetControl(string id)
        {
            var controls = GetControls();
            foreach (var control in controls)
            {
                if (control.Id == id)
                {
                    return control;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取区块内的所有控件
        /// </summary>
        public IList<ControlInfo> GetControls()
        {
            IList<ControlInfo> controls = new List<ControlInfo>();
            GetControls(this, controls);
            return controls;
        }


        /// <summary>
        /// 获取区块内的所有控件
        /// <param name="block"></param>
        /// <param name="controls"></param>
        /// </summary>
        public void GetControls(BlockInfo block, IList<ControlInfo> controls)
        {
            foreach (var column in block.Columns)
            {
                foreach (var control in column.Controls)
                {
                    controls.Add(control);
                    if (control.Containers == null) continue;
                    foreach (var container in control.Containers)
                    {
                        foreach (var innerBlock in container.Blocks)
                        {
                            GetControls(innerBlock, controls);
                        }
                    }
                }
            }
        }

    }
}
