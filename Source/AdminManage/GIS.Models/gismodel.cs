using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Model;
using System.Runtime.Serialization;
using NM.Util;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Gis.Models
{

    public partial class GIS_BASEMAP : EntityBase, IComparable<GIS_BASEMAP>//<gis_basemap>
    {
        [IgnoreDataMember]
        public Int32 LayerID
        {
            get { return GetInt32("LayerID"); }
            set { SetInt32("LayerID", value); }
        }
        [IgnoreDataMember]
        public String LayerCode
        {
            get { return GetString("LayerCode"); }
            set
            {
                SetString("LayerCode", value);
            }
        }
        [IgnoreDataMember]
        public String LAYERNAME
        {
            get { return GetString("LAYERNAME"); }
            set { SetString("LAYERNAME", value); }
        }
        [IgnoreDataMember]
        public String DSOURCE
        {
            get { return GetString("DSOURCE"); }
            set { SetString("DSOURCE", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [IgnoreDataMember]
        public Int32 DATATYPE
        {
            //0表示矢量底图，1表示卫片，-1表示航片。2表示卫片和航片上叠加的矢量图。
            get { return GetInt32("DATATYPE"); }
            set { SetInt32("DATATYPE", value); }
        }
        [IgnoreDataMember]
        public String SERVICEURL
        {
            get { return GetString("SERVICEURL"); }
            set { SetString("SERVICEURL", value); }
        }
        [IgnoreDataMember]
        public Int32 SERVICEINDEX
        {
            get { return GetInt32("SERVICEINDEX"); }
            set { SetInt32("SERVICEINDEX", value); }
        }

        [IsDecimalExpression]
        [NotNullExpression]
        [IgnoreDataMember]
        public String XMIN
        {
            get { return GetString("XMIN"); }
            set { SetString("XMIN", value); }
        }
        [IsDecimalExpression]
        [NotNullExpression]
        [IgnoreDataMember]
        public String YMIN
        {
            get { return GetString("YMIN"); }
            set { SetString("YMIN", value); }
        }
        [IsDecimalExpression]
        [NotNullExpression]
        [IgnoreDataMember]
        public String XMAX
        {
            get { return GetString("XMAX"); }
            set { SetString("XMAX", value); }
        }
        [IsDecimalExpression]
        [NotNullExpression]
        [IgnoreDataMember]
        public String YMAX
        {
            get { return GetString("YMAX"); }
            set { SetString("YMAX", value); }
        }
        [Display(Name = "底图年份")]
        [IsNumberExpression]
        [IgnoreDataMember]
        public int YEAR
        {
            get { return GetInt32("YEAR"); }
            set { SetInt32("YEAR", value); }
        }
        [Display(Name = "坐标系")]
        [NotNullExpression]
        [IgnoreDataMember]
        public String COORSYS
        {
            get { return GetString("COORSYS"); }
            set { SetString("COORSYS", value); }
        }

        public int CompareTo(GIS_BASEMAP other)
        {
            //DATATYPE = 0表示矢量底图, 1表示卫片底图, 2表示混合底图, 3表示航片底图
            if (this.DATATYPE == other.DATATYPE)
            {
                return 0;
            }
            else if (this.DATATYPE > other.DATATYPE)
            {
                return 1;
            }
            else 
            {
                return -1;
            }
        }
    }



    public partial class gis_BStolayers : EntityBase//<gis_BStolayers>
    {
        [IgnoreDataMember]
        public Int32 ID
        {
            get { return GetInt32("ID"); }
            set { SetInt32("ID", value); }
        }
        [IgnoreDataMember]
        public String BSCODE
        {
            get { return GetString("BSCODE"); }
            set { SetString("BSCODE", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String BSNAME
        {
            get { return GetString("BSNAME"); }
            set { SetString("BSNAME", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERCLASSID
        {
            get { return GetInt32("LAYERCLASSID"); }
            set { SetInt32("LAYERCLASSID", value); }
        }
        [IgnoreDataMember]
        public Int32 MAINLAYERID
        {
            get { return GetInt32("MAINLAYERID"); }
            set { SetInt32("MAINLAYERID", value); }
        }
    }


    public partial class gis_Common : EntityBase//<gis_Common>
    {
        [IgnoreDataMember]
        public String OBJCODE
        {
            get { return GetString("OBJCODE"); }
            set { SetString("OBJCODE", value); }
        }
        [IgnoreDataMember]
        public Int32 OBJNAME
        {
            get { return GetInt32("OBJNAME"); }
            set { SetInt32("OBJNAME", value); }
        }
        [IgnoreDataMember]
        public Int32 STATE
        {
            get { return GetInt32("STATE"); }
            set { SetInt32("STATE", value); }
        }
        [IgnoreDataMember]
        public String SSQX
        {
            get { return GetString("SSQX"); }
            set { SetString("SSQX", value); }
        }
        [IgnoreDataMember]
        public String SSXZ
        {
            get { return GetString("SSXZ"); }
            set { SetString("SSXZ", value); }
        }
        [IgnoreDataMember]
        public String SSJD
        {
            get { return GetString("SSJD"); }
            set { SetString("SSJD", value); }
        }
        [IgnoreDataMember]
        public Decimal OBJAREA
        {
            get { return GetDecimal("OBJAREA"); }
            set { SetDecimal("OBJAREA", value); }
        }
        [IgnoreDataMember]
        public Int32 OBJYEAR
        {
            get { return GetInt32("OBJYEAR"); }
            set { SetInt32("OBJYEAR", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
    }



    public partial class gis_Hislayer : EntityBase//<gis_Hislayer>
    {
        [IgnoreDataMember]
        public Int32 ID
        {
            get { return GetInt32("ID"); }
            set { SetInt32("ID", value); }
        }
        [IgnoreDataMember]
        public String LAYERYEAR
        {
            get { return GetString("LAYERYEAR"); }
            set { SetString("LAYERYEAR", value); }
        }
        [IgnoreDataMember]
        public Int32 DATACOUNT
        {
            get { return GetInt32("DATACOUNT"); }
            set { SetInt32("DATACOUNT", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERID
        {
            get { return GetInt32("LAYERID"); }
            set { SetInt32("LAYERID", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERIDHIS
        {
            get { return GetInt32("LAYERIDHIS"); }
            set { SetInt32("LAYERIDHIS", value); }
        }
    }



    public partial class gis_hotword : EntityBase//<gis_hotword>
    {

        [IgnoreDataMember]
        public String QWORD
        {
            get { return GetString("QWORD"); }
            set { SetString("QWORD", value); }
        }
        [IgnoreDataMember]
        public Int32 QCOUNT
        {
            get { return GetInt32("QCOUNT"); }
            set { SetInt32("QCOUNT", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }

        [IgnoreDataMember]
        public Int32 QSUCCESS
        {
            get { return GetInt32("QSUCCESS"); }
            set { SetInt32("QSUCCESS", value); }
        }
    }

    public partial class EAP_VIDEOS : EntityBase//<gis_layerclass>
    {
        [IgnoreDataMember]
        public Int32 ID
        {
            get { return GetInt32("ID"); }
            set { SetInt32("ID", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERID
        {
            get { return GetInt32("LAYERID"); }
            set { SetInt32("LAYERID", value); }
        }
        [IgnoreDataMember]
        public Int32 OBJID
        {
            get { return GetInt32("OBJID"); }
            set { SetInt32("OBJID", value); }
        }
        [IgnoreDataMember]
        public String VIDEOURL
        {
            get { return GetString("VIDEOURL"); }
            set { SetString("VIDEOURL", value); }
        }
        [IgnoreDataMember]
        public String VIDEODES
        {
            get { return GetString("VIDEODES"); }
            set { SetString("VIDEODES", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }
    }

    public partial class EAP_PICS : EntityBase//<gis_layerclass>
    {
        [IgnoreDataMember]
        public Int32 ID
        {
            get { return GetInt32("ID"); }
            set { SetInt32("ID", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERID
        {
            get { return GetInt32("LAYERID"); }
            set { SetInt32("LAYERID", value); }
        }
        [IgnoreDataMember]
        public Int32 OBJID
        {
            get { return GetInt32("OBJID"); }
            set { SetInt32("OBJID", value); }
        }
        [IgnoreDataMember]
        public String PICURL
        {
            get { return GetString("PICURL"); }
            set { SetString("PICURL", value); }
        }
        [IgnoreDataMember]
        public String PICDES
        {
            get { return GetString("PICDES"); }
            set { SetString("PICDES", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }
    }

    public partial class gis_layerclass : EntityBase//<gis_layerclass>
    {
        [Display(Name = "分类代码")]
        [NotNullExpression]
        [IgnoreDataMember]
        public String CLASSCODE
        {
            get { return GetString("CLASSCODE"); }
            set { SetString("CLASSCODE", value); }
        }
        [Display(Name = "分类名称")]
        [NotNullExpression]
        [IgnoreDataMember]
        public String CLASSNAME
        {
            get { return GetString("CLASSNAME"); }
            set { SetString("CLASSNAME", value); }
        }
        [Display(Name = "显示图片")]
        [NotNullExpression]
        [IgnoreDataMember]
        public String CLASSIMG
        {
            get { return GetString("CLASSIMG"); }
            set { SetString("CLASSIMG", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [Display(Name = "是否有效")]
        [IgnoreDataMember]
        public Boolean FLAGDELETE
        {
            get { return GetBoolean("FLAGDELETE"); }
            set { SetBoolean("FLAGDELETE", value); }
        }
        [Display(Name = "序号")]
        [IsNumberExpression()]
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }

        [IgnoreDataMember]
        public Boolean IsClassVaild_G
        {
            get { return GetBoolean("IsClassVaild_G"); }
            set
            {
                SetBoolean("IsClassVaild_G", value);
            }
        }
    }


    public class gis_layerclassMoudle
    {
        public int ID { get; set; }
        public string NAME { get; set; }
        public string ENAME { get; set; }
        public int TYPE { get; set; }
        public string IMG { get; set; }
        public string HREF { get; set; }
        /// <summary>
        /// 记录基数，ID是用数据库中的id+基数得到，如果需要返回数据库查数据，则需要减掉baseID
        /// </summary>
        public int baseID { get; set; }
        
        public List<servicClassD> LAYERS { get; set; }
    }

    public partial class gis_LayerClassD : EntityBase//<gis_LayerClassD>
    {
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [Display(Name = "序号")]
        [IsNumberExpression()]
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }
        [IgnoreDataMember]
        public Int32 CLASSID
        {
            get { return GetInt32("CLASSID"); }
            set { SetInt32("CLASSID", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERID
        {
            get { return GetInt32("LAYERID"); }
            set { SetInt32("LAYERID", value); }
        }

        [Display(Name = "图层中文名称")]
        [IgnoreDataMember]
        public String LayerName_G
        {
            get { return GetString("LayerName_G"); }
            set { SetString("LayerName_G", value); }
        }
        [Display(Name = "图层英文名称")]
        [IgnoreDataMember]
        public String LayerCode_G
        {
            get { return GetString("LayerCode_G"); }
            set { SetString("LayerCode_G", value); }
        }

        [IgnoreDataMember]
        public Boolean LayerFlagDelete_G
        {
            get { return GetBoolean("LayerFlagDelete_G"); }
            set { SetBoolean("LayerFlagDelete_G", value); }
        }
        [IgnoreDataMember]
        public String serviceurl_G
        {
            get { return GetString("serviceurl_G"); }
            set { SetString("serviceurl_G", value); }
        }

        [IgnoreDataMember]
        public int serviceindex_G
        {
            get { return GetInt32("serviceindex_G"); }
            set { SetInt32("serviceindex_G", value); }
        }


        [IgnoreDataMember]
        public String featureserviceurl_G
        {
            get { return GetString("featureserviceurl_G"); }
            set { SetString("featureserviceurl_G", value); }
        }

        [IgnoreDataMember]
        public int featureserviceindex_G
        {
            get { return GetInt32("featureserviceindex_G"); }
            set { SetInt32("featureserviceindex_G", value); }
        }

        [IgnoreDataMember]
        public Boolean ishistory_G
        {
            get { return GetBoolean("ishistory_G"); }
            set { SetBoolean("ishistory_G", value); }
        }


        /// <summary>
        ///   0：基础图层1：业务图层2;  专题图层
        /// </summary>
        [IgnoreDataMember]
        public Int32 LayerTypeint_G
        {
            get { return GetInt32("LayerTypeint_G"); }
            set { SetInt32("LayerTypeint_G", value); }
        }

        /// <summary>
        ///  0:面1：线2：点
        /// </summary>
        [IgnoreDataMember]
        public Int32 SHPTYPEint_G
        {
            get { return GetInt32("SHPTYPEint_G"); }
            set { SetInt32("SHPTYPEint_G", value); }
        }

        [IgnoreDataMember]
        public String LayerYears_G
        {
            get { return GetString("LayerYears_G"); }
            set { SetString("LayerYears_G", value); }
        }

        [IgnoreDataMember]
        public String GisDataFields_G
        {
            get { return GetString("GisDataFields_G"); }
            set { SetString("GisDataFields_G", value); }
        }

        [IgnoreDataMember]
        public Int32 VLEVEL_G
        {
            get { return GetInt32("VLEVEL_G"); }
            set { SetInt32("VLEVEL_G", value); }
        }

        [Display(Name = "专题图数据源名")]
        [IgnoreDataMember]
        public String SpecDS_G
        {
            get { return GetString("SpecDS_G"); }
            set { SetString("SpecDS_G", value); }
        }
        /// <summary>
        /// 0:直接显示1：柱状图2：饼图3：聚类图
        /// </summary>
        [IsNumberExpression]//
        [Display(Name = "专题图显示模式")]
        [IgnoreDataMember]
        public Int32 SpecShowMode_G
        {
            get { return GetInt32("SpecShowMode_G"); }
            set { SetInt32("SpecShowMode_G", value); }
        }

        [Display(Name = "专题图分组字段")]
        [IgnoreDataMember]
        public String SpecGroupFN_G
        {
            get { return GetString("SpecGroupFN_G"); }
            set { SetString("SpecGroupFN_G", value); }
        }

        /// <summary>
        /// 0：计数1：求和
        /// </summary>
        [IsNumberExpression]//
        [Display(Name = "专题图分组字段")]
        [IgnoreDataMember]
        public Int32 SpecStatMode_G
        {
            get { return GetInt32("SpecStatMode_G"); }
            set { SetInt32("SpecStatMode_G", value); }
        }

        [Display(Name = "专题图求和字段")]
        [IgnoreDataMember]
        public String SpecStatFN_G
        {
            get { return GetString("SpecStatFN_G"); }
            set { SetString("SpecStatFN_G", value); }
        }
    }
    public class servicClassD
    {

        public int id { get; set; }
        public string enName { get; set; }
        public string cnName { get; set; }
        public string mapServerUrl { get; set; }
        public int serverindex { get; set; }
        public string featureServerUrl { get; set; }
        public int featureServerIndex { get; set; }

        public bool isTimeData { get; set; }

        public string datayears { get; set; }
        public string gisdatafields { get; set; }
        public int vlevel { get; set; }

        public string actclassid { get; set; }

        public string layerid { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string layertype { get; set; }
        public int shptype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string specshowmode { get; set; }

    }

    public class ReflayerID
    {
        public string _reference { get; set; }
    }
    public class servicClassD2 : servicClassD
    {
        public string type { get; set; }
    }

    public class classbase
    {
        public classbase()
        {
            visibleIndexs = new List<int>();
        }
        public int classid { get; set; }
        public string mapServerUrl { get; set; }

        public List<int> visibleIndexs { get; set; }
    }

    public class servicClassDM : servicClassD2
    {
        public List<ReflayerID> children { get; set; }
    }

    public partial class gis_layerDefine : EntityBase//<gis_layerDefine>
    {
        [Display(Name = "英文名称")]
        [NotNullExpression]
        [IgnoreDataMember]
        public String LAYERCODE
        {
            get { return GetString("LAYERCODE"); }
            set { SetString("LAYERCODE", value); }
        }
        [Display(Name = "中文名称")]
        [NotNullExpression]
        [IgnoreDataMember]
        public String LAYERNAME
        {
            get { return GetString("LAYERNAME"); }
            set { SetString("LAYERNAME", value); }
        }
        [Display(Name = "数据来源")]
        [IgnoreDataMember]
        public String DSOURCE
        {
            get { return GetString("DSOURCE"); }
            set { SetString("DSOURCE", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Boolean FLAGDELETE
        {
            get { return GetBoolean("FLAGDELETE"); }
            set { SetBoolean("FLAGDELETE", value); }
        }
        [Display(Name = "是否历史图层")]
        [IgnoreDataMember]
        public bool ISHISTORY
        {
            get { return GetBoolean("ISHISTORY"); }
            set { SetBoolean("ISHISTORY", value); }
        }

        [Display(Name = "图层类别")]
        [IgnoreDataMember]
        public Int32 LAYERTYPE
        {
            get { return GetInt32("LAYERTYPE"); }
            set { SetInt32("LAYERTYPE", value); }
        }
        [Display(Name = "是否影像")]
        [IgnoreDataMember]
        public Int32 DATATYPE
        {
            get { return GetInt32("DATATYPE"); }
            set { SetInt32("DATATYPE", value); }
        }
        [Display(Name = "MapServer地址")]
        [NotNullExpression]
        [IgnoreDataMember]
        public String SERVICEURL
        {
            get { return GetString("SERVICEURL"); }
            set { SetString("SERVICEURL", value); }
        }

        [IsNumberExpression]//如果填写判断是否是数字
        [Display(Name = "MapServer服务序号")]
        [IgnoreDataMember]
        public Int32 SERVICEINDEX
        {
            get { return GetInt32("SERVICEINDEX"); }
            set { SetInt32("SERVICEINDEX", value); }
        }
        [Display(Name = "元素类型")]
        [IgnoreDataMember]
        public Int32 SHPTYPE
        {
            get { return GetInt32("SHPTYPE"); }
            set { SetInt32("SHPTYPE", value); }
        }
        [Display(Name = "数据年份")]
        [IgnoreDataMember]
        public String DataYears
        {
            get { return GetString("DataYears"); }
            set { SetString("DataYears", value); }
        }

        [Display(Name = "显示字段")]
        [IgnoreDataMember]
        public String GisDataFields
        {
            get { return GetString("GisDataFields"); }
            set { SetString("GisDataFields", value); }
        }

        [IsNumberExpression]//如果填写判断是否是数字
        [Display(Name = "最小可见层级")]
        [IgnoreDataMember]
        public Int32 VLEVEL
        {
            get { return GetInt32("VLEVEL"); }
            set { SetInt32("VLEVEL", value); }
        }
        /// <summary>
        /// 0：基础图层1：业务图层2;  专题图层
        /// </summary>
        [IgnoreDataMember]
        public String LAYERTYPE_G
        {
            get { return GetString("LAYERTYPE_G"); }
            set { SetString("LAYERTYPE_G", value); }
        }

        /// <summary>
        /// 0:矢量1：影像
        /// </summary>
        [IgnoreDataMember]
        public String DATATYPE_G
        {
            get { return GetString("DATATYPE_G"); }
            set { SetString("DATATYPE_G", value); }
        }

        /// <summary>
        ///  0:面1：线2：点
        /// </summary>
        [IgnoreDataMember]
        public String SHPTYPE_G
        {
            get { return GetString("SHPTYPE_G"); }
            set { SetString("SHPTYPE_G", value); }
        }


        [Display(Name = "专题图数据源名")]
        [IgnoreDataMember]
        public String SpecDS
        {
            get { return GetString("SpecDS"); }
            set { SetString("SpecDS", value); }
        }
        /// <summary>
        /// 0:直接显示1：柱状图2：饼图3：聚类图
        /// </summary>
        [IsNumberExpression]//
        [Display(Name = "专题图显示模式")]
        [IgnoreDataMember]
        public Int32 SpecShowMode
        {
            get { return GetInt32("SpecShowMode"); }
            set { SetInt32("SpecShowMode", value); }
        }

        [Display(Name = "专题图分组字段")]
        [IgnoreDataMember]
        public String SpecGroupFN
        {
            get { return GetString("SpecGroupFN"); }
            set { SetString("SpecGroupFN", value); }
        }

        /// <summary>
        /// 0：计数1：求和
        /// </summary>
        [IsNumberExpression]//
        [Display(Name = "专题图分组字段")]
        [IgnoreDataMember]
        public Int32 SpecStatMode
        {
            get { return GetInt32("SpecStatMode"); }
            set { SetInt32("SpecStatMode", value); }
        }

        [Display(Name = "专题图求和字段")]
        [IgnoreDataMember]
        public String SpecStatFN
        {
            get { return GetString("SpecStatFN"); }
            set { SetString("SpecStatFN", value); }
        }

        [Display(Name = "FeatureServer地址")]
        [IgnoreDataMember]
        public String FEATURESERVICEURL
        {
            get { return GetString("FEATURESERVICEURL"); }
            set { SetString("FEATURESERVICEURL", value); }
        }

        [IsNumberExpression]//如果填写判断是否是数字
        [Display(Name = "FeatureServer服务序号")]
        [IgnoreDataMember]
        public Int32 FEATURESERVICEINDEX
        {
            get { return GetInt32("FEATURESERVICEINDEX"); }
            set { SetInt32("FEATURESERVICEINDEX", value); }
        }

        [IgnoreDataMember]
        public Boolean IsCheck_G
        {
            get { return GetBoolean("IsCheck_G"); }
            set { SetBoolean("IsCheck_G", value); }
        }


        [IgnoreDataMember]
        public String XMIN_G
        {
            get { return GetString("XMIN_G"); }
            set { SetString("XMIN_G", value); }
        }

        [IgnoreDataMember]
        public String YMIN_G
        {
            get { return GetString("YMIN_G"); }
            set { SetString("YMIN_G", value); }
        }

        [IgnoreDataMember]
        public String XMAX_G
        {
            get { return GetString("XMAX_G"); }
            set { SetString("XMAX_G", value); }
        }

        [IgnoreDataMember]
        public String YMAX_G
        {
            get { return GetString("YMAX_G"); }
            set { SetString("YMAX_G", value); }
        }

        [IgnoreDataMember]
        public String COORSYS_G
        {
            get { return GetString("COORSYS_G"); }
            set { SetString("COORSYS_G", value); }
        }
    }

    public partial class gis_layerVersion : EntityBase//<gis_layerVersion>
    {
        [IgnoreDataMember]
        public Int32 ID
        {
            get { return GetInt32("ID"); }
            set { SetInt32("ID", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERID
        {
            get { return GetInt32("LAYERID"); }
            set { SetInt32("LAYERID", value); }
        }
        [IgnoreDataMember]
        public String VERSIONCODE
        {
            get { return GetString("VERSIONCODE"); }
            set { SetString("VERSIONCODE", value); }
        }
        [IgnoreDataMember]
        public String VERSIONNAME
        {
            get { return GetString("VERSIONNAME"); }
            set { SetString("VERSIONNAME", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }
        [IgnoreDataMember]
        public Int32 ISDEFAULT
        {
            get { return GetInt32("ISDEFAULT"); }
            set { SetInt32("ISDEFAULT", value); }
        }
    }

    /// <summary>
    /// 一个临时对象，获取从客户端的变动数据使用的对象
    /// </summary>
    public class gis_layerVersionColpo
    {
        public int ID { get; set; }
        public int LayerID { get; set; }

        public String COLCODE { get; set; }

        public String COLNAME { get; set; }

        public Int32 ISVISIBLE { get; set; }

        public Int32 ODERNUM { get; set; }

        public Int32 ISGROUP { get; set; }

    }

    public partial class gis_layerVersionCol : EntityBase//<gis_layerVersionCol>
    {
        [IgnoreDataMember]
        public Int32 LayerID
        {
            get { return GetInt32("LayerID"); }
            set { SetInt32("LayerID", value); }
        }
        [IgnoreDataMember]
        public Int32 LAYERVID
        {
            get { return GetInt32("LAYERVID"); }
            set { SetInt32("LAYERVID", value); }
        }
        [IgnoreDataMember]
        public String COLCODE
        {
            get { return GetString("COLCODE"); }
            set { SetString("COLCODE", value); }
        }
        [IgnoreDataMember]
        public String COLNAME
        {
            get { return GetString("COLNAME"); }
            set { SetString("COLNAME", value); }
        }
        [IgnoreDataMember]
        public String COLTYPE
        {
            get { return GetString("COLTYPE"); }
            set { SetString("COLTYPE", value); }
        }
        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set { SetDateTime("ADDON", value); }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }
        [IgnoreDataMember]
        public String DELETEBY
        {
            get { return GetString("DELETEBY"); }
            set { SetString("DELETEBY", value); }
        }
        [IgnoreDataMember]
        public DateTime DELETEON
        {
            get { return GetDateTime("DELETEON"); }
            set { SetDateTime("DELETEON", value); }
        }
        [IgnoreDataMember]
        public Int32 FLAGDELETE
        {
            get { return GetInt32("FLAGDELETE"); }
            set { SetInt32("FLAGDELETE", value); }
        }

        [IgnoreDataMember]
        public Int32 ColLen
        {
            get { return GetInt32("ColLen"); }
            set { SetInt32("ColLen", value); }
        }

        [IgnoreDataMember]
        public Int32 ColXSLen
        {
            get { return GetInt32("ColXSLen"); }
            set { SetInt32("ColXSLen", value); }
        }

        [IgnoreDataMember]
        public Int32 ISVISIBLE
        {
            get { return GetInt32("ISVISIBLE"); }
            set { SetInt32("ISVISIBLE", value); }
        }
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }
        [IgnoreDataMember]
        public Int32 ISMAIN
        {
            get { return GetInt32("ISMAIN"); }
            set { SetInt32("ISMAIN", value); }
        }

        [IgnoreDataMember]
        public String COLTYPE_G
        {
            get { return GetString("COLTYPE_G"); }
            set { SetString("COLTYPE_G", value); }
        }

        [IgnoreDataMember]
        public Int32 ISGROUP
        {
            get { return GetInt32("ISGROUP"); }
            set { SetInt32("ISGROUP", value); }
        }
    }

    public class gis_layerdefine_subpoco
    {
        public int ID { get; set; }
        public int LayerID { get; set; }

        public String PROPNAME { get; set; }

        public String PROPVALUE { get; set; }

        public Int32 ISVISIBLE { get; set; }

        public Int32 ODERNUM { get; set; }

    }

    public partial class gis_layerdefine_sub : EntityBase//<gis_layerVersionCol>
    {
        [IgnoreDataMember]
        public int PropCode
        {
            get { return GetInt32("PropCode"); }
            set { SetInt32("PropCode", value); }
        }

        [IgnoreDataMember]
        public int ParentCode
        {
            get { return GetInt32("ParentCode"); }
            set { SetInt32("ParentCode", value); }
        }


        [IgnoreDataMember]
        public String PROPNAME
        {
            get { return GetString("PROPNAME"); }
            set { SetString("PROPNAME", value); }
        }
        [IgnoreDataMember]
        public String PROPVALUE
        {
            get { return GetString("PROPVALUE"); }
            set { SetString("PROPVALUE", value); }
        }

        [IgnoreDataMember]
        public String LAYERID
        {
            get { return GetString("LAYERID"); }
            set { SetString("LAYERID", value); }
        }

        [IgnoreDataMember]
        public Int32 ISVISIBLE
        {
            get { return GetInt32("ISVISIBLE"); }
            set { SetInt32("ISVISIBLE", value); }
        }
        [IgnoreDataMember]
        public Int32 ODERNUM
        {
            get { return GetInt32("ODERNUM"); }
            set { SetInt32("ODERNUM", value); }
        }

    }

    public class statuse : EntityBase
    {
        [IgnoreDataMember]
        public String objqx
        {
            get { return GetString("objqx"); }
            set { SetString("objqx", value); }
        }
        [IgnoreDataMember]
        public String groupname
        {
            get { return GetString("groupname"); }
            set { SetString("groupname", value); }
        }
        [IgnoreDataMember]
        public decimal total
        {
            get { return GetDecimal("total"); }
            set { SetDecimal("total", value); }
        }
    }
    public class statparam
    {
        public string k { get; set; }
        public string v { get; set; }
    }
    public class statx
    {
        public int value { get; set; }
        public string text { get; set; }
    }
    public class statpie
    {
        public int y { get; set; }
        public string text { get; set; }
        public string stroke { get; set; }
        public string tooltip { get; set; }

    }

    public class specstat
    {
        public specstat()
        {
            children = new List<statpie>();
        }
        public string name { get; set; }
        public string code { get; set; }

        public List<statpie> children { get; set; }
    }
    public class statResult
    {
        public statResult()
        {
            x = new List<statx>();
            y = new List<int>();
        }
        public List<statx> x;
        public List<int> y;
    }


    public class Gis_Error : EntityBase
    {
        [IgnoreDataMember]
        public string LayerID
        {
            get { return GetString("LayerID"); }
            set { SetString("LayerID", value); }
        }
        [IgnoreDataMember]
        public string LayerName
        {
            get { return GetString("LayerName"); }
            set { SetString("LayerName", value); }
        }
        [IgnoreDataMember]
        public string LayerCode
        {
            get { return GetString("LayerCode"); }
            set { SetString("LayerCode", value); }
        }
        [IgnoreDataMember]
        public string EleID
        {
            get { return GetString("EleID"); }
            set { SetString("EleID", value); }
        }
        [IgnoreDataMember]
        public string EleName
        {
            get { return GetString("EleName"); }
            set { SetString("EleName", value); }
        }

        [IgnoreDataMember]
        public string ErrorDesc
        {
            get { return GetString("ErrorDesc"); }
            set { SetString("ErrorDesc", value); }
        }

        [IgnoreDataMember]
        public String ADDBY
        {
            get { return GetString("ADDBY"); }
            set { SetString("ADDBY", value); }
        }
        [IgnoreDataMember]
        public DateTime ADDON
        {
            get { return GetDateTime("ADDON"); }
            set
            {
                SetDateTime("ADDON", value);
                SetString("AddOn_G", value.ToString("yyyy-MM-dd"));
            }
        }
        [IgnoreDataMember]
        public String EDITBY
        {
            get { return GetString("EDITBY"); }
            set { SetString("EDITBY", value); }
        }
        [IgnoreDataMember]
        public DateTime EDITON
        {
            get { return GetDateTime("EDITON"); }
            set { SetDateTime("EDITON", value); }
        }

        [IgnoreDataMember]
        public Int32 Status
        {
            get { return GetInt32("Status"); }
            set { SetInt32("Status", value); }
        }

        public string AddOn_G
        {
            get { return GetString("AddOn_G"); }
        }
    }
}
