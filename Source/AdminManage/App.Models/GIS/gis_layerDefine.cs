using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;
using NM.Model;
namespace App.Models.Gis
{

    /*gis_layerDefine
    *
    * exec ESP_ClassCodeGenerator  gis_layerDefine,1
    */
    public partial class gis_layerDefine : EntityBase//<gis_layerDefine>
    {
        [DisplayName("英文名称")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public String layercode
        {
            get { return GetString("layercode"); }
            set
            {
                SetString("layercode", value);
            }
        }
        [DisplayName("中文名称")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public String layername
        {
            get { return GetString("layername"); }
            set
            {
                SetString("layername", value);
            }
        }

        [IgnoreDataMember]
        public String dsource
        {
            get { return GetString("dsource"); }
            set
            {
                SetString("dsource", value);
            }
        }

        [IgnoreDataMember]
        public String addby
        {
            get { return GetString("addby"); }
            set
            {
                SetString("addby", value);
            }
        }

        [IgnoreDataMember]
        public DateTime addon
        {
            get { return GetDateTime("addon"); }
            set
            {
                SetDateTime("addon", value);
            }
        }

        [IgnoreDataMember]
        public String editby
        {
            get { return GetString("editby"); }
            set
            {
                SetString("editby", value);
            }
        }

        [IgnoreDataMember]
        public DateTime editon
        {
            get { return GetDateTime("editon"); }
            set
            {
                SetDateTime("editon", value);
            }
        }

        [IgnoreDataMember]
        public String deleteby
        {
            get { return GetString("deleteby"); }
            set
            {
                SetString("deleteby", value);
            }
        }

        [IgnoreDataMember]
        public DateTime deleteon
        {
            get { return GetDateTime("deleteon"); }
            set
            {
                SetDateTime("deleteon", value);
            }
        }

        [DisplayName("是否删除")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public Int32 FlagDelete
        {
            get { return GetInt32("FlagDelete"); }
            set
            {
                SetInt32("FlagDelete", value);
            }
        }

        [DisplayName("是否历史数据")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public Int32 ishistory
        {
            get { return GetInt32("ishistory"); }
            set
            {
                SetInt32("ishistory", value);
            }
        }
        [DisplayName("图层类型")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public Int32 layerType
        {
            get { return GetInt32("layerType"); }
            set
            {
                SetInt32("layerType", value);
            }
        }

        [DisplayName("图层类型")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public string  layerType_G
        {
            get { return GetString("layerType_G"); }
            set
            {
                SetString("layerType_G", value);
            }
        }

        [IgnoreDataMember]
        public Int32 dataType
        {
            get { return GetInt32("dataType"); }
            set
            {
                SetInt32("dataType", value);
            }
        }

        [DisplayName("服务地址")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public String serviceurl
        {
            get { return GetString("serviceurl"); }
            set
            {
                SetString("serviceurl", value);
            }
        }
        [DisplayName("服务序号")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public Int32 serviceIndex
        {
            get { return GetInt32("serviceIndex"); }
            set
            {
                SetInt32("serviceIndex", value);
            }
        }
        [DisplayName("元素类型")]
        [Required(ErrorMessage = "*")]
        [IgnoreDataMember]
        public Int32 shpType
        {
            get { return GetInt32("shpType"); }
            set
            {
                SetInt32("shpType", value);
            }
        }

    }
}
