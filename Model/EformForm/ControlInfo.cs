using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZHONGJIAN_API.Model.EformForm
{
    public class ControlInfo
    {
        /// <summary>
        /// 控件Id
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 控件类型
        /// </summary>
        public string ControlType { get; set; } = string.Empty;

        //public string BlockId { get; set; } = string.Empty;

        //public string ColumnId { get; set; } = string.Empty;

        /// <summary>
        /// 控件配置
        /// </summary>
        public IList<PropertyInfo> Setting { get; set; } = new List<PropertyInfo>();

        /// <summary>
        /// 控件所包含的块
        /// </summary>
        public IList<ContainerInfo> Containers { get; set; } = null;
        public string Name
        {
            get
            {
                if (Setting != null)
                {
                    string vname = "";
                    foreach (var propertyInfo in Setting)
                    {
                        if (propertyInfo.id == "name")
                        {
                            vname = propertyInfo.value;
                            break;
                        }
                    }
                    return vname;
                }
                else
                {
                    return "";
                }
            }
        }

        public string FiledName
        {
            get
            {
                if (Setting != null)
                {
                    string vname = "";
                    foreach (var propertyInfo in Setting)
                    {
                        if (propertyInfo.id == "fieldName")
                        {
                            vname = propertyInfo.value;
                            break;
                        }
                    }
                    return vname;
                }
                else
                {
                    return "";
                }
            }
        }

        public string ControlId
        {
            get
            {
                if (Setting != null)
                {
                    string vid = "";
                    foreach (var propertyInfo in Setting)
                    {
                        if (propertyInfo.id == "controlId")
                        {
                            vid = propertyInfo.value;
                            break;
                        }
                    }
                    return vid;
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// 跟据属性名称获取属性对象
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyInfo GetPropertyInfo(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;
            return this.Setting.FirstOrDefault(x => x.id.ToLower().Equals(propertyName.ToLower()));
        }

    }
}
