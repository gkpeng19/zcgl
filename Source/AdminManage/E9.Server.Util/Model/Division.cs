using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using NM.Util;
using System.Linq;
using System.Collections.ObjectModel;
using NM.OP;

#if SILVERLIGHT
using System.Windows.Controls;
using System.Windows.Media;
using System.Text;
#endif

namespace NM.Model
{
#if SILVERLIGHT
    public class SYSConfigs:TJson
    {
        public SYSConfigs()
        {
            RoutineConfigs = new EntityList<EAP_SYSConfig>();
            ColumnConfigs = new EntityList<XX_Column>();
        }

        List<string> _defaultList = new List<string>() //必须小写
        {
            "isgroupmanage", "isnoorderrecItem", "isalertwhenlower","iskctmanage","iscrmmanage"
        };

        /// <summary>
        /// 自定义配置和基础配置(用xsysconfig字段标识 0: 基础配置  1:自定义配置 [数据库中默认值为0])
        /// </summary>
        public EntityList<EAP_SYSConfig> RoutineConfigs { get; private set; }

        /// <summary>
        /// 列配置
        /// </summary>
        public EntityList<XX_Column> ColumnConfigs { get; private set; }

        #region 初始化配置列表

        public void InitConfigs(EntityList<EAP_SYSConfig> list)
        {
            if (list != null)
            {
                this.RoutineConfigs = list;
            }
        }

        public void InitConfigs(EntityList<XX_Column> list)
        {
            if (list != null)
            {
                ColumnConfigs = list;
            }
        }

        #endregion

        #region Get;Set

        private Int32 GetInt32(string configCode)
        {
            foreach (var e in RoutineConfigs)
            {
                if (configCode.Equals(e.ConfgCode,StringComparison.CurrentCultureIgnoreCase))
                {
                    return Convert.ToInt32(e.ConfgValue);
                }
            }
            if (_defaultList.Contains(configCode.ToLower()))
            {
                SetValue(configCode, 1);
                return 1;
            }
            return 0;
        }

        private Decimal GetDecimal(string configCode)
        {
            foreach (var e in RoutineConfigs)
            {
                if (configCode.Equals(e.ConfgCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    return Convert.ToDecimal(e.ConfgValue);
                }
            }
            return 0;
        }

        private String GetString(string configCode)
        {
            foreach (var e in RoutineConfigs)
            {
                if (configCode.Equals(e.ConfgCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    return e.ConfgValue;
                }
            }
            return string.Empty;
        }

        private void SetValue<T>(string configCode,T value)
        {
            bool isHave = false;
            foreach (var e in RoutineConfigs)
            {
                if (configCode.Equals(e.ConfgCode, StringComparison.CurrentCultureIgnoreCase))
                {
                    e.ConfgValue = value.ToString();
                    isHave = true;
                    break;
                }
            }
            if (!isHave)
            {
                EAP_SYSConfig config = new EAP_SYSConfig()
                {
                    ConfgCode = configCode, 
                    ConfgValue = value.ToString()
                };
                RoutineConfigs.Add(config);
            }
        }

        #endregion

        /// <summary>
        /// 自定义配置  第二个选项卡
        /// </summary>
        [IgnoreDataMember]
        public EntityList<EAP_SYSConfig> CustomConfigList
        {
            get
            {
                EntityList<EAP_SYSConfig> arr = new EntityList<EAP_SYSConfig>();
                foreach (var c in RoutineConfigs)
                {
                    if (c.XSysConfig == "1")
                    {
                        arr.Add(c);
                    }
                }
                return arr;
            }
        }

        /// <summary>
        /// 获取自定义配置
        /// </summary>
        /// <param name="ConfigCode">配置编号</param>
        /// <returns></returns>
        public EAP_SYSConfig GetCustomConfig(string ConfigCode)
        {
            EAP_SYSConfig config = null;
            CustomConfigList.ForEach(e =>
            {
                if (e.ConfgCode == ConfigCode)
                {
                    config = e;
                }
            });
            return config;
        }

        [IgnoreDataMember]
        public IEnumerable<XX_Column> this[string IdentifyName, bool IsEntityName = true]
        {
            get
            {
                return from config in ColumnConfigs
                       where string.Compare(IsEntityName ? config.Entity_Name : config.Resource_Name, IdentifyName, StringComparison.CurrentCultureIgnoreCase) == 0
                       select config;
            }
        }

        [IgnoreDataMember]
        public XX_Column this[string IdentifyName, string ColumnName, bool IsEntityName = true]
        {
            get
            {
                foreach (var column in this[IdentifyName, IsEntityName])
                {
                    if (column.Column_Name.Equals(ColumnName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        return column;
                    }
                }
                return null;
            }
        }

        #region 公共基础配置属性

        /// <summary>
        /// 启用KCT管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsKCTManage
        {
            get { return 1; }
        }

        /// <summary>
        /// 启用CRM管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsCRMManage
        {
            get { return 1; }
        }

        /// <summary>
        /// 启用一商品多供应商管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsMoreSupManage
        {
            get { return GetInt32("IsMoreSupManage"); }
            set { SetValue("IsMoreSupManage", value); }
        }

        /// <summary>
        /// 启用合计管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsTotalManage
        {
            get { return GetInt32("IsTotalManage"); }
            set { SetValue("IsTotalManage", value); OnPropertyChanged("IsTotalManage"); }
        }
        
        /// <summary>
        /// 启用单匹管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsPieceManage
        {
            get { return GetInt32("IsPieceManage"); }
            set { SetValue("IsPieceManage", value); }
        }

        /// <summary>
        /// 启用单件管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsSingleManage
        {
            get { return GetInt32("IsSingleManage"); }
            set { SetValue("IsSingleManage", value); }
        }
        /// <summary>
        /// 启用颜色管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsColorManage
        {
            get { return GetInt32("IsColorManage"); }
            set { SetValue("IsColorManage", value); }
        }

        /// <summary>
        /// 启用批次管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsGroupManage
        {
            get { return GetInt32("IsGroupManage"); }
            set { SetValue("IsGroupManage", value); }
        }

        /// <summary>
        /// 启用货位管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsItemPositionManage
        {
            get { return GetInt32("IsItemPositionManage"); }
            set { SetValue("IsItemPositionManage", value); }
        }

        /// <summary>
        /// 启用不预览打印
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsPrintManage
        {
            get { return GetInt32("IsPrintManage"); }
            set { SetValue("IsPrintManage", value); }
        }
        /// <summary>
        /// 启用最小库存管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsMinStoreManage
        {
            get { return GetInt32("IsMinStoreManage"); }
            set { SetValue("IsMinStoreManage", value); }
        }

        /// <summary>
        /// 是否可无订单收货
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsNoOrderRecItem
        {
            get { return GetInt32("IsNoOrderRecItem"); }
            set { SetValue("IsNoOrderRecItem", value); }
        }
        /// <summary>
        /// 是否可超数量收货
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsMoreCountRecItem
        {
            get { return GetInt32("IsMoreCountRecItem"); }
            set { SetValue("IsMoreCountRecItem", value); }
        }
        /// <summary>
        /// 超数量收货百分比
        /// </summary>
        [IgnoreDataMember]
        public Decimal MoreCount
        {
            get { return GetDecimal("MoreCount"); }
            set { SetValue("MoreCount", value); OnPropertyChanged("MoreCount"); }
        }
        /// <summary>
        /// 是否当售价大于买价时提醒
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsAlertWhenLower
        {
            get { return GetInt32("IsAlertWhenLower"); }
            set { SetValue("IsAlertWhenLower", value); }
        }
        /// <summary>
        /// 是否启用bom管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsBomManage
        {
            get { return GetInt32("IsBomManage"); }
            set { SetValue("IsBomManage", value); }
        }

        /// <summary>
        /// 是否启用生产管理
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsProductManager
        {
            get { return GetInt32("IsProductManager"); }
            set { SetValue("IsProductManager", value); }
        }
        
             /// <summary>
        /// 一个销售订单允许多次出库
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsMoreTimeOutItem
        {
            get { return GetInt32("IsMoreTimeOutItem"); }
            set { SetValue("IsMoreTimeOutItem", value); }
        }
        /// <summary>
        /// 一个采购订单允许多次收获
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsMoreTimeRecItem
        {
            get { return GetInt32("IsMoreTimeRecItem"); }
            set { SetValue("IsMoreTimeRecItem", value); }
        }
        /// <summary>
        /// 是否允许手工结单
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsEndOrderHand
        {
            get { return GetInt32("IsEndOrderHand"); }
            set { SetValue("IsEndOrderHand", value); }
        }
        /// <summary>
        /// 采购订单明细是否显示收获数量
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsShowRecCount
        {
            get { return GetInt32("IsShowRecCount"); }
            set { SetValue("IsShowRecCount", value); }
        }
        /// <summary>
        /// 是否启用1-10录入
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsTenInput
        {
            get { return GetInt32("IsTenInput"); }
            set
            {
                SetValue("IsTenInput", value);
                OnPropertyChanged("IsTenInput");
            }
        }

        [IgnoreDataMember]
        public Int32 IsTaxable
        {
            get { return GetInt32("IsTaxable"); }
            set { SetValue("IsTaxable", value); }
        }
        [IgnoreDataMember]
        public Int32 IsEndSellOrderHand
        {
            get { return GetInt32("IsEndSellOrderHand"); }
            set { SetValue("IsEndSellOrderHand", value); }
        }

        #region 关于短信发送的配置

        [IgnoreDataMember]
        public Int32 IsSendMsgAuto
        {
            get { return GetInt32("IsSendMsgAuto"); }
            set { SetValue("IsSendMsgAuto", value); }
        }

        [IgnoreDataMember]
        public String MsgReceiveTels
        {
            get { return GetString("MsgReceiveTels"); }
            set { SetValue("MsgReceiveTels", value); }
        }

        #region 短信发送类型
        
        [IgnoreDataMember]
        public Int32 IsSendMsg_Rec
        {
            get { return GetInt32("IsSendMsg_Rec"); }
            set { SetValue("IsSendMsg_Rec", value); }
        }
        [IgnoreDataMember]
        public Int32 IsSendMsg_Sell
        {
            get { return GetInt32("IsSendMsg_Sell"); }
            set { SetValue("IsSendMsg_Sell", value); }
        }
        [IgnoreDataMember]
        public Int32 IsSendMsg_DaySum
        {
            get { return GetInt32("IsSendMsg_DaySum"); }
            set { SetValue("IsSendMsg_DaySum", value); }
        }

        #endregion



        #endregion


        #endregion

        #region 公共列配置属性

        public String Column_ItemCode
        {
            get { return GetString("Column_ItemCode"); }
            set { SetValue("Column_ItemCode", value); }
        }

        public String Column_CylNum
        {
            get { return GetString("Column_CylNum"); }
            set { SetValue("Column_CylNum", value); }
        }

        #endregion

    }
#endif

    public class XX_Column : EntityBase
    {
        [IgnoreDataMember]
        public Int32 OrgID
        {
            get { return GetInt32("OrgID"); }
            set { SetInt32("OrgID", value); }
        }
        [IgnoreDataMember]
        public String Resource_Name
        {
            get { return GetString("Resource_Name"); }
            set { SetString("Resource_Name", value); }
        }
        [IgnoreDataMember]
        public String Entity_Name
        {
            get { return GetString("Entity_Name"); }
            set { SetString("Entity_Name", value); }
        }
        [IgnoreDataMember]
        public String Field_Name
        {
            get { return GetString("Field_Name"); }
            set { SetString("Field_Name", value); }
        }
        [IgnoreDataMember]
        public String Column_Name
        {
            get { return GetString("Column_Name"); }
            set { SetString("Column_Name", value); }
        }
        [IgnoreDataMember]
        public String Column_Name_New
        {
            get { return GetString("Column_Name_New"); }
            set { SetString("Column_Name_New", value); }
        }
        [IgnoreDataMember]
        public String Data_Type
        {
            get { return GetString("Data_Type"); }
            set { SetString("Data_Type", value); }
        }
        [IgnoreDataMember]
        public String Memo
        {
            get { return GetString("Memo"); }
            set { SetString("Memo", value); }
        }
        [IgnoreDataMember]
        public Int32 IsVisibility
        {
            get { return GetInt32("IsVisibility"); }
            set { SetInt32("IsVisibility", value); OnPropertyChanged("IsVisibility"); }
        }
    }

    public class EAP_SYSConfig : EntityBase
    {
        [IgnoreDataMember]
        public Int32 OrgID
        {
            get { return GetInt32("OrgID");}
            set { SetInt32("OrgID", value); }
        }
        [IgnoreDataMember]
        [Required( ErrorMessage="1.配置编号必须填写!")]
        public String ConfgCode
        {
            get { return GetString("ConfgCode"); }
            set { SetString("ConfgCode", value); }
        }
        [IgnoreDataMember]
        public String XSysConfig
        {
            get { return GetString("XSysConfig"); }
            set { SetString("XSysConfig", value); }
        }
        [IgnoreDataMember]
        public String ConfgName
        {
            get { return GetString("ConfgName"); }
            set { SetString("ConfgName", value); }
        }
        [IgnoreDataMember]
        [Required(ErrorMessage="2.值必须填写!")]
        public String ConfgValue
        {
            get { return GetString("ConfgValue"); }
            set { SetString("ConfgValue", value); OnPropertyChanged("ConfgValue"); }
        }
    }

    #region 帐套
    public partial class KCT_ACCOUNTSET : EntityBase
    {

        [IgnoreDataMember]
        public Int32 ORGID
        {
            get { return GetInt32("ORGID"); }
            set { SetInt32("ORGID", value); }
        }
        [IgnoreDataMember]
        public String REMARK
        {
            get { return GetString("REMARK"); }
            set { SetString("REMARK", value); }
        }
        [IgnoreDataMember]
#if SILVERLIGHT
        [Required(ErrorMessage = "帐套时间为必填")]
#endif
        public String ACCOUNTYEAR
        {
            get { return GetString("ACCOUNTYEAR"); }
            set { SetString("ACCOUNTYEAR", value); }
        }

        [IgnoreDataMember]
        public Int32 ISCURRACCOUNT
        {
            get { return GetInt32("ISCURRACCOUNT"); }
            set { SetInt32("ISCURRACCOUNT", value); }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set { SetString("AddBy", value); }
        }
        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set { SetDateTime("AddOn", value); }
        }
        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set { SetString("EditBy", value); }
        }
        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set { SetDateTime("EditOn", value); }
        }
        [IgnoreDataMember]
        public string BakFileName
        {
            get { return GetString("BakFileName"); }
            set { SetString("BakFileName", value); }
        }
        [IgnoreDataMember]
        public string BakFileName_G
        {
            get 
            {
                if (!string.IsNullOrEmpty(BakFileName))
                {
                    return "下载";
                }
                else
                {
                    return "";
                }
            }
        }
        
        [IgnoreDataMember]
        public String ISCURRACCOUNT_G
        {
            get { return GetString("ISCURRACCOUNT_G"); }
            set { SetString("ISCURRACCOUNT_G", value); }
        }

    }
    #endregion

    #region 设置当前帐套服务时的返回结果
    public class WMS_ACCOUNTSET : EntityBase
    {
        /// <summary>
        /// 操作人
        /// </summary>
        [IgnoreDataMember]
        public String BusinessBy
        {
            get { return GetString("BusinessBy"); }
            set
            {
                SetString("BusinessBy", value);
            }
        }

        /// <summary>
        /// 业务操作结果
        /// 0:失败，1：成功 
        /// </summary>
        [IgnoreDataMember]
        public Int32 ResultID
        {
            get { return GetInt32("ResultID"); }
            set
            {
                SetInt32("ResultID", value);
            }
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        [IgnoreDataMember]
        public String ErrorMsg
        {
            get { return GetString("ErrorMsg"); }
            set
            {
                SetString("ErrorMsg", value);
            }
        }

        /// <summary>
        /// 当前的登录人ORGID 
        /// </summary>
        [IgnoreDataMember]
        public Int32 ORGID
        {
            get { return GetInt32("ORGID"); }
            set
            {
                SetInt32("ORGID", value);
            }
        }
    }
    #endregion
    //public class EAP_Resource : MetaBase
    //{
    //    public EAP_Resource()
    //    {
    //        Children = new List<EAP_Resource>();
    //    }

    //    // public int ID { get; set; }
    //    public string Name { get; set; }
    //    public string Title { get; set; }
    //    public string Description { get; set; }
    //    public int ParentId { get; set; }
    //    public int Level { get; set; }
    //    public int Row { get; set; }
    //    public int Col { get; set; }
    //    public string PageId { get; set; }
    //    public Boolean Flag_Delete { get; set; }
    //    public List<EAP_Resource> Children { get; set; }
    //}


    /*EAP_Resource
*
* exec ESP_ClassCodeGenerator  EAP_Resource,1
*/
    public partial class EAP_Resource : EntityBase//<EAP_Resource>
    {
        [IgnoreDataMember]
        public int SourceID
        {
            get { return GetInt32("SourceID"); }
            set
            {
                SetInt32("SourceID", value);
            }
        }

        [IgnoreDataMember]
        public String Name
        {
            get { return GetString("Name"); }
            set
            {
                SetString("Name", value);
            }
        }

        [IgnoreDataMember]
        public String Title
        {
            get { return GetString("Title"); }
            set
            {
                SetString("Title", value);
            }
        }

        [IgnoreDataMember]
        public String Description
        {
            get { return GetString("Description"); }
            set
            {
                SetString("Description", value);
            }
        }

        [IgnoreDataMember]
        public String Type
        {
            get { return GetString("Type"); }
            set
            {
                SetString("Type", value);
            }
        }

        [IgnoreDataMember]
        public Int32 ParentId
        {
            get { return GetInt32("ParentId"); }
            set
            {
                SetInt32("ParentId", value);
            }
        }

        [IgnoreDataMember]
        public Int32 Level
        {
            get { return GetInt32("Level"); }
            set
            {
                SetInt32("Level", value);
            }
        }

        [IgnoreDataMember]
        public String PageId
        {
            get { return GetString("PageId"); }
            set
            {
                SetString("PageId", value);
            }
        }

        [IgnoreDataMember]
        public Int32 VRow
        {
            get { return GetInt32("VRow"); }
            set
            {
                SetInt32("VRow", value);
            }
        }

        [IgnoreDataMember]
        public Int32 VCol
        {
            get { return GetInt32("VCol"); }
            set
            {
                SetInt32("VCol", value);
            }
        }

        [IgnoreDataMember]
        public Int32 SortBy
        {
            get { return GetInt32("SortBy"); }
            set
            {
                SetInt32("SortBy", value);
            }
        }

        [IgnoreDataMember]
        public String Permission
        {
            get { return GetString("Permission"); }
            set
            {
                SetString("Permission", value);
            }
        }

        [IgnoreDataMember]
        public String Image
        {
            get { return GetString("Image"); }
            set
            {
                SetString("Image", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetString("EditBy", value);
            }
        }

        
        [IgnoreDataMember]
        public Int32 HasChild_G
        {
            get { return GetInt32("HasChild_G"); }
            set
            {
                SetInt32("HasChild_G", value);
            }
        }
        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public Boolean Flag_Delete
        {
            get { return GetBoolean("Flag_Delete"); }
            set
            {
                SetBoolean("Flag_Delete", value);
            }
        }


        public void AddChild(EAP_Resource child)
        {
            Children.Add(child);
            child.Parent = this;
            IsLoadChild_G = 1;
        }

        public EAP_Resource Parent { get; set; }

        private TJsonList<EAP_Resource> _Children;
        public TJsonList<EAP_Resource> Children
        {
            get
            {
                if (null == _Children)
                {
                    _Children = new TJsonList<EAP_Resource>();
                }

                return _Children;
            }
            set 
            { 
                _Children = value;
                if ((value != null) && (value.Count > 0))
                {
                    IsLoadChild_G = 1;
                }
            }
        }
        

        public EAP_Resource FindChildByPageID(string pageID)
        {
            foreach (var c in Children)
            {
                if (c.PageId == pageID)
                    return c;
                EAP_Resource r = c.FindChildByPageID(pageID);
                if (r != null)
                    return r;
            }
            return null;
        }
        /// <summary>
        /// 仅用在加载功能按钮使用
        /// </summary>
        [IgnoreDataMember]
        public int IsLoadChild_G
        {
            get { return GetInt32("IsLoadChild_G"); }
            set
            {
                SetInt32("IsLoadChild_G", value);
            }
        }
                [IgnoreDataMember]
        public Boolean IsVaild_G
        {
            get { return GetBoolean("IsVaild_G"); }
            set
            {
                SetBoolean("IsVaild_G", value);
            }
        }
        
    }

    /*EAP_User
 *
 * exec ESP_ClassCodeGenerator  EAP_User,1
 */
    public partial class EAP_User : EntityBase//<EAP_User>
    {
        public EAP_User()
        {
            OrgList = new List<EAP_Org>();
          //  UserItemCategory = new EntityList<WMS_USERITEMCATEGORY>();
        }

        [XmlIgnoreAttribute]
        public List<EAP_Org> OrgList { get; set; }//借用此类


        [IgnoreDataMember]
        public int UserID
        {
            get { return GetInt32("UserID"); }
            set
            {
                SetInt32("UserID", value);
            }
        }

        [IgnoreDataMember]
        public Int32 AccountSetID
        {
            get { return GetInt32("AccountSetID"); }
            set
            {
                SetInt32("AccountSetID", value);
            }
        }
        [Display(Name="所属组织")]
        [IgnoreDataMember]
        public Int32 OrgId
        {
            get { return GetInt32("OrgId"); }
            set
            {
                SetInt32("OrgId", value);
            }
        }
        public String Org_Name_G
        {
            get { return GetString("Org_Name_G"); }
            set
            {
                SetString("Org_Name_G", value);
            }
        }
        /// <summary>
        /// 组织类型 0：分店 1：总公司 2:供应商
        /// </summary>
        [IgnoreDataMember]
        public Int32 ORGTYPE_G
        {
            get { return GetInt32("ORGTYPE_G"); }
            set
            {
                SetInt32("ORGTYPE_G", value);
            }
        }
        /// <summary>
        /// 当前组织是否启用了需要使用ukey才能登录，默认为0，不使用；
        /// </summary>
        [IgnoreDataMember]
        public Int32 isUseKey_G
        {
            get { return GetInt32("isUseKey_G"); }
            set
            {
                SetInt32("isUseKey_G", value);
            }
        }
        [NotNullExpression]
        [IsCharExpression]
        [Display(Name = "用户名")]
        [IgnoreDataMember]
#if SILVERLIGHT
        [Required(ErrorMessage = "请输入用户名。")]
#endif
        public String UserName
        {
            get { return GetString("UserName"); }
            set
            {
                SetString("UserName", value);
            }
        }
        [NotNullExpression]
        [Display(Name = "真实名称")]
        [IgnoreDataMember]
#if SILVERLIGHT
        [Required(ErrorMessage = "请输入真实名。")]
#endif
        public String TrueName
        {
            get { return GetString("TrueName"); }
            set
            {
                SetString("TrueName", value);
            }
        }
        //[NotNullExpression]
        //[StringLength(50, MinimumLength = 5)]
        //[System.Web.Mvc.Compare("ComparePassword_G", ErrorMessage = "两次密码不一致")]
        //[Display(Name = "密码")]
        [IgnoreDataMember]
#if SILVERLIGHT
        // [Required(ErrorMessage = "请输入密码。")]
#endif
        public String Password
        {
            get { return GetString("Password"); }
            set
            {
                SetString("Password", value);
            }
        }
        //[System.Web.Mvc.Compare("Password", ErrorMessage = "两次密码不一致")]
        //[Display(Name = "确认密码")]
        public String ComparePassword_G
        {
            get { return GetString("ComparePassword_G"); }
            set
            {
                SetString("ComparePassword_G", value);
            }
        }
        
        [IgnoreDataMember]
        public String BarCode
        {
            get { return GetString("BarCode"); }
            set
            {
                SetString("BarCode", value);
            }
        }

        /// <summary>
        /// 买价是否可见
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsBuyPrice
        {
            get { return GetInt32("IsBuyPrice"); }
            set
            {
                SetInt32("IsBuyPrice", value);
            }
        }
        /// <summary>
        /// 卖价是否可见
        /// </summary>
        [IgnoreDataMember]
        public Int32 IsSellPrice
        {
            get { return GetInt32("IsSellPrice"); }
            set
            {
                SetInt32("IsSellPrice", value);
            }
        }


        [IgnoreDataMember]
        public String Phone1
        {
            get { return GetString("Phone1"); }
            set
            {
                SetString("Phone1", value);
            }
        }

        [IgnoreDataMember]
        public String Phone2
        {
            get { return GetString("Phone2"); }
            set
            {
                SetString("Phone2", value);
            }
        }

        [IgnoreDataMember]
        public String Phone3
        {
            get { return GetString("Phone3"); }
            set
            {
                SetString("Phone3", value);
            }
        }

        [IgnoreDataMember]
        public String Email
        {
            get { return GetString("Email"); }
            set
            {
                SetString("Email", value);
            }
        }

        [IgnoreDataMember]
        public Boolean IsLock
        {
            get { return GetBoolean("IsLock"); }
            set
            {
                SetBoolean("IsLock", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetString("EditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public String DeleteBy
        {
            get { return GetString("DeleteBy"); }
            set
            {
                SetString("DeleteBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime DeleteOn
        {
            get { return GetDateTime("DeleteOn"); }
            set
            {
                SetDateTime("DeleteOn", value);
            }
        }

        [IgnoreDataMember]
        public Boolean Flag_Delete
        {
            get { return GetBoolean("Flag_Delete"); }
            set
            {
                SetBoolean("Flag_Delete", value);
            }
        }


        [Display(Name = "身份证")]
        [IgnoreDataMember]
        public String Card
        {
            get { return GetString("Card"); }
            set { SetString("Card", value); }
        }
      
        [Display(Name = "手机号码")]
        [IgnoreDataMember]
        public String MobileNumber
        {
            get { return GetString("MobileNumber"); }
            set { SetString("MobileNumber", value); }
        }
          [Display(Name = "电话号码")]
        [IgnoreDataMember]
        public String PhoneNumber
        {
            get { return GetString("PhoneNumber"); }
            set { SetString("PhoneNumber", value); }
        }
         [Display(Name = "QQ")]
        [IgnoreDataMember]
        public String QQ
        {
            get { return GetString("QQ"); }
            set { SetString("QQ", value); }
        }
         [Display(Name = "Email")]
        [IgnoreDataMember]
        public String EmailAddress
        {
            get { return GetString("EmailAddress"); }
            set { SetString("EmailAddress", value); }
        }
         [Display(Name = "其他联系方式")]
        [IgnoreDataMember]
        public String OtherContact
        {
            get { return GetString("OtherContact"); }
            set { SetString("OtherContact", value); }
        }
        [IgnoreDataMember]
        public String Province
        {
            get { return GetString("Province"); }
            set { SetString("Province", value); }
        }
        [IgnoreDataMember]
        public String City
        {
            get { return GetString("City"); }
            set { SetString("City", value); }
        }
        [IgnoreDataMember]
        public String Village
        {
            get { return GetString("Village"); }
            set { SetString("Village", value); }
        }
          [Display(Name = "详细地址")]
        [IgnoreDataMember]
        public String Address
        {
            get { return GetString("Address"); }
            set { SetString("Address", value); }
        }
        [Display(Name = "是否启用")]
        [IgnoreDataMember]
        public Int32 State
        {
            get { return GetInt32("State"); }
            set { SetInt32("State", value); }
        }

        public String State_G
        {
            get { return GetString("State_G"); }
            set { SetString("State_G", value); }
        }

         [Display(Name = "性别")]
        [IgnoreDataMember]
        public String Sex
        {
            get { return GetString("Sex"); }
            set { SetString("Sex", value); }
        }
         [DateExpression]//如果填写判断是否是日期
         [Display(Name = "生日")]
        [IgnoreDataMember]
        public String Birthday
        {
            get { return GetString("Birthday"); }
            set { SetString("Birthday", value); }
        }
         [DateExpression]//如果填写判断是否是日期
         [Display(Name = "加入时间")]
        [IgnoreDataMember]
        public String JoinDate
        {
            get { return GetString("JoinDate"); }
            set { SetString("JoinDate", value); }
        }
          [Display(Name = "婚姻状况")]
        [IgnoreDataMember]
        public String Marital
        {
            get { return GetString("Marital"); }
            set { SetString("Marital", value); }
        }
        [IgnoreDataMember]
        public String Political
        {
            get { return GetString("Political"); }
            set { SetString("Political", value); }
        }
        [IgnoreDataMember]
        public String Nationality
        {
            get { return GetString("Nationality"); }
            set { SetString("Nationality", value); }
        }
        [IgnoreDataMember]
        public String Native
        {
            get { return GetString("Native"); }
            set { SetString("Native", value); }
        }
        [IgnoreDataMember]
        public String SCHOOL
        {
            get { return GetString("SCHOOL"); }
            set { SetString("SCHOOL", value); }
        }
        [IgnoreDataMember]
        public String Professional
        {
            get { return GetString("Professional"); }
            set { SetString("Professional", value); }
        }
        [IgnoreDataMember]
        public String Degree
        {
            get { return GetString("Degree"); }
            set { SetString("Degree", value); }
        }
        [IgnoreDataMember]
        public String PosId
        {
            get { return GetString("PosId"); }
            set { SetString("PosId", value); }
        }
        [IgnoreDataMember]
        public String Expertise
        {
            get { return GetString("Expertise"); }
            set { SetString("Expertise", value); }
        }

        [IgnoreDataMember]
        public String Photo
        {
            get { return GetString("Photo"); }
            set { SetString("Photo", value); }
        }
        [IgnoreDataMember]
        public String Attach
        {
            get { return GetString("Attach"); }
            set { SetString("Attach", value); }
        }

        [IgnoreDataMember]
        public String CAID
        {
            get { return GetString("CAID"); }
            set { SetString("CAID", value); }
        }

        [IgnoreDataMember]
        public Boolean IsSave_G
        {
            get { return GetBoolean("IsSave_G"); }
            set
            {
                SetBoolean("IsSave_G", value);
            }
        }
        

        [IgnoreDataMember]
        public String RoleName_G
        {
            get { return GetString("RoleName_G"); }
            set { SetString("RoleName_G", value); }
        }
        private TJsonList<EAP_Role> _Roles;
        public TJsonList<EAP_Role> Roles
        {
            get
            {
                if (null == _Roles)
                {
                    _Roles = new TJsonList<EAP_Role>();
                }

                return _Roles;
            }
            set
            {
                _Roles = value;
            }
        }

        [IgnoreDataMember]
        public Int32 ALLITEMSRIGHT
        {
            get { return GetInt32("ALLITEMSRIGHT"); }
            set
            {
                SetInt32("ALLITEMSRIGHT", value);
            }
        }

        [IgnoreDataMember]
        public Int32 DEPARTMENTID
        {
            get { return GetInt32("DEPARTMENTID"); }
            set
            {
                SetInt32("DEPARTMENTID", value);
            }
        }
       [IgnoreDataMember]
        public Int32 SECTIONID
        {
            get { return GetInt32("SECTIONID"); }
            set
            {
                SetInt32("SECTIONID", value);
            }
        }

       [IgnoreDataMember]
       public String DEPARTMENTName_G
       {
           get { return GetString("DEPARTMENTName_G"); }
           set
           {
               SetString("DEPARTMENTName_G", value);
           }
       }
       [IgnoreDataMember]
       public String SECTIONName_G
       {
           get { return GetString("SECTIONName_G"); }
           set
           {
               SetString("SECTIONName_G", value);
           }
       }
        //[ForeignKey("USERID", "ID")]
        //public EntityList<WMS_USERITEMCATEGORY> UserItemCategory { get; set; }

    }


    /// <summary>
    /// 课别
    /// </summary>
    public partial class WMS_SECTION : EntityBase//<WMS_SECTION>
    {
        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set { SetString("AddBy", value); }
        }
        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set { SetDateTime("AddOn", value); }
        }
        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set { SetString("EditBy", value); }
        }
        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set { SetDateTime("EditOn", value); }
        }
        [IgnoreDataMember]
#if SILVERLIGHT
        [Required(ErrorMessage = "编号不能为空。")]
#endif
        public String CODE
        {
            get { return GetString("CODE"); }
            set { SetString("CODE", value); }
        }
        [IgnoreDataMember]
#if SILVERLIGHT
        [Required(ErrorMessage = "名称不能为空。")]
#endif
        public String NAME
        {
            get { return GetString("NAME"); }
            set { SetString("NAME", value); }
        }
    }
    /*wms_useritemcategory
*
* exec ESP_ClassCodeGenerator  wms_useritemcategory,1
*/
    public partial class WMS_USERITEMCATEGORY : EntityBase//<wms_useritemcategory>
    {
        [IgnoreDataMember]
        public Int32 USERID
        {
            get { return GetInt32("USERID"); }
            set { SetInt32("USERID", value); }
        }
        [IgnoreDataMember]
        public String CODE_G
        {
            get { return GetString("CODE_G"); }
            set { SetString("CODE_G", value); }
        }
        [IgnoreDataMember]
        public String NAME_G
        {
            get { return GetString("NAME_G"); }
            set { SetString("NAME_G", value); }
        }
        [IgnoreDataMember]
        public Int32 CATEGORYLEVEL1
        {
            get { return GetInt32("CATEGORYLEVEL1"); }
            set { SetInt32("CATEGORYLEVEL1", value); }
        }
        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set { SetString("AddBy", value); }
        }
        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set { SetDateTime("AddOn", value); }
        }
        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set { SetString("EditBy", value); }
        }
        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set { SetDateTime("EditOn", value); }
        }
    }

    public class UserItemsCategorySearch : SearchCriteria
    {
        public UserItemsCategorySearch()
        {
            SearchID = "WMS_USERITEMCATEGORY";
        }
        [IgnoreDataMember]
        public Int32 USERID
        {
            get { return GetInt32("USERID"); }
            set { SetInt32("USERID", value); }
        }
    }
    /*wms_itemscategory
*
* exec ESP_ClassCodeGenerator  wms_itemscategory,1
*/
    public partial class WMS_ITEMSCATEGORY : EntityBase//<wms_itemscategory>
    {
        [IgnoreDataMember]
        public Int32 PARENTID
        {
            get { return GetInt32("PARENTID"); }
            set { SetInt32("PARENTID", value); }
        }
        [IgnoreDataMember]
        public Int32 LEVELT
        {
            get { return GetInt32("LEVELT"); }
            set { SetInt32("LEVELT", value); }
        }
        [IgnoreDataMember]
        public String CODE
        {
            get { return GetString("CODE"); }
            set { SetString("CODE", value); }
        }
        [IgnoreDataMember]
        public String NAME
        {
            get { return GetString("NAME"); }
            set { SetString("NAME", value); }
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
        public Int32 DEPID
        {
            get { return GetInt32("DEPID"); }
            set { SetInt32("DEPID", value); }
        }
        [IgnoreDataMember]
        public Int32 SECID
        {
            get { return GetInt32("SECID"); }
            set { SetInt32("SECID", value); }
        }
    }

    public class ItemsCategorySearch : SearchCriteria
    {
        public ItemsCategorySearch()
        {
            SearchID = "WMS_ITEMSCATEGORY";
        }
        [IgnoreDataMember]
        public Int32 PARENTID
        {
            get { return GetInt32("PARENTID"); }
            set { SetInt32("PARENTID", value); }
        }
        [IgnoreDataMember]
        public Int32 LEVELT
        {
            get { return GetInt32("LEVELT"); }
            set { SetInt32("LEVELT", value); }
        }
    }

    /*wms_department
    *
    * exec ESP_ClassCodeGenerator  wms_department,1
    */
    public partial class WMS_DEPARTMENT : EntityBase//<wms_department>
    {
        [IgnoreDataMember]
        public Int32 PARENTID
        {
            get { return GetInt32("PARENTID"); }
            set { SetInt32("PARENTID", value); }
        }
        [IgnoreDataMember]
        public String CODE
        {
            get { return GetString("CODE"); }
            set { SetString("CODE", value); }
        }
        [IgnoreDataMember]
        public String NAME
        {
            get { return GetString("NAME"); }
            set { SetString("NAME", value); }
        }
        [IgnoreDataMember]
        public Int32 DEPLEVEL
        {
            get { return GetInt32("DEPLEVEL"); }
            set { SetInt32("DEPLEVEL", value); }
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
    }




    /*EAP_Role
 *
 * exec ESP_ClassCodeGenerator  EAP_Role,1
 */
    public partial class EAP_Role : EntityBase//<EAP_Role>
    {
        [IgnoreDataMember]
        public int OrgID
        {
            get { return GetInt32("OrgID"); }
            set
            {
                SetInt32("OrgID", value);
            }
        }
        [IgnoreDataMember]
        public int RoleID
        {
            get { return GetInt32("RoleID"); }
            set
            {
                SetInt32("RoleID", value);
            }
        }

        [NotNullExpression]
        [Display(Name = "角色名称")]
        [IgnoreDataMember]
#if SILVERLIGHT
        [Required(ErrorMessage = "请输入角色名。")]
#endif
        public String RoleName
        {
            get { return GetString("RoleName"); }
            set
            {
                SetString("RoleName", value);
            }
        }


        [IgnoreDataMember]
#if SILVERLIGHT
        [Required(ErrorMessage = "请输入角色描述信息。")]
#endif
        public String Description
        {
            get { return GetString("Description"); }
            set
            {
                SetString("Description", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetString("EditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public String DeleteBy
        {
            get { return GetString("DeleteBy"); }
            set
            {
                SetString("DeleteBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime DeleteOn
        {
            get { return GetDateTime("DeleteOn"); }
            set
            {
                SetDateTime("DeleteOn", value);
            }
        }

        [IgnoreDataMember]
        public Boolean Flag_Delete
        {
            get { return GetBoolean("Flag_Delete"); }
            set
            {
                SetBoolean("Flag_Delete", value);
            }
        }

        [IgnoreDataMember]
        public int UserCount
        {
            get { return GetInt32("UserCount"); }
            set
            {
                SetInt32("UserCount", value);
            }
        }

        private TJsonList<EAP_User> _Users;
        public TJsonList<EAP_User> Users
        {
            get
            {
                if (null == _Users)
                {
                    _Users = new TJsonList<EAP_User>();
                }

                return _Users;
            }
            set
            {
                _Users = value;
            }
        }

        private TJsonList<EAP_Resource> _Modules;
        public TJsonList<EAP_Resource> Modules
        {
            get
            {
                if (null == _Modules)
                {
                    _Modules = new TJsonList<EAP_Resource>();
                }

                return _Modules;
            }
            set
            {
                _Modules = value;
            }
        }
    }


    /*EAP_Org
*
* exec ESP_ClassCodeGenerator  EAP_Org,1
*/
    public partial class EAP_Org : EntityBase//<EAP_Org>
    {
        public EAP_Org()
        {
            Childs = new List<EAP_Org>();
        }
        public List<EAP_Org> Childs { get; set; }
        [IgnoreDataMember]
        public int ID
        {
            get { return GetInt32("ID"); }
            set
            {
                SetInt32("ID", value);
            }
        }

        [IgnoreDataMember]
        public String SourceID
        {
            get { return GetString("SourceID"); }
            set
            {
                SetString("SourceID", value);
            }
        }

        [IgnoreDataMember]
        public Int32 ParentID
        {
            get { return GetInt32("ParentID"); }
            set
            {
                SetInt32("ParentID", value);
            }
        }

        [IgnoreDataMember]
        public String HierarchyNo
        {
            get { return GetString("HierarchyNo"); }
            set
            {
                SetString("HierarchyNo", value);
            }
        }

        [IgnoreDataMember]
        public String Code
        {
            get { return GetString("Code"); }
            set
            {
                SetString("Code", value);
            }
        }

        [IgnoreDataMember]
        public String Name
        {
            get { return GetString("Name"); }
            set
            {
                SetString("Name", value);
            }
        }

        [IgnoreDataMember]
        public String ShortName
        {
            get { return GetString("ShortName"); }
            set
            {
                SetString("ShortName", value);
            }
        }

        [IgnoreDataMember]
        public String Address
        {
            get { return GetString("Address"); }
            set { SetString("Address", value); }
        }

        [IgnoreDataMember]
        public String SellContact
        {
            get { return GetString("SellContact"); }
            set { SetString("SellContact", value); }
        }

        [IgnoreDataMember]
        public Int32 Type
        {
            get { return GetInt32("Type"); }
            set
            {
                SetInt32("Type", value);
            }
        }

        [IgnoreDataMember]
        public Int32 Status
        {
            get { return GetInt32("Status"); }
            set
            {
                SetInt32("Status", value);
            }
        }

        [IgnoreDataMember]
        public Int32 IndustoryType
        {
            get { return GetInt32("IndustoryType"); }
            set
            {
                SetInt32("IndustoryType", value);
            }
        }

        [IgnoreDataMember]
        public string IndustoryType_G
        {
            get { return GetString("IndustoryType_G"); }
            set
            {
                SetString("IndustoryType_G", value);
            }
        }

        [IgnoreDataMember]
        public String Remark
        {
            get { return GetString("Remark"); }
            set
            {
                SetString("Remark", value);
            }
        }

        [IgnoreDataMember]
        public String Memonic
        {
            get { return GetString("Memonic"); }
            set
            {
                SetString("Memonic", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetString("EditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public String DeleteBy
        {
            get { return GetString("DeleteBy"); }
            set
            {
                SetString("DeleteBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime DeleteOn
        {
            get { return GetDateTime("DeleteOn"); }
            set
            {
                SetDateTime("DeleteOn", value);
            }
        }

        [IgnoreDataMember]
        public Int32 FlagDelete
        {
            get { return GetInt32("FlagDelete"); }
            set
            {
                SetInt32("FlagDelete", value);
            }
        }

        [IgnoreDataMember]
        public Int32 SortBy
        {
            get { return GetInt32("SortBy"); }
            set
            {
                SetInt32("SortBy", value);
            }
        }
        [IgnoreDataMember]
        public Int32 HasChild_G
        {
            get { return GetInt32("HasChild_G"); }
            set
            {
                SetInt32("HasChild_G", value);
            }
        }

    }

    /*App_Log
*
* exec ESP_ClassCodeGenerator  App_Log,1
*/
    public partial class APP_Log : EntityBase//<APP_Log>
    {
        [IgnoreDataMember]
        public Int32 LogID
        {
            get { return GetInt32("LogID"); }
            set
            {
                SetInt32("LogID", value);
            }
        }

        [IgnoreDataMember]
        public Int32 SessionID
        {
            get { return GetInt32("SessionID"); }
            set
            {
                SetInt32("SessionID", value);
            }
        }

        [IgnoreDataMember]
        public DateTime LogTime
        {
            get { return GetDateTime("LogTime"); }
            set
            {
                SetDateTime("LogTime", value);
            }
        }

        [IgnoreDataMember]
        public String Message
        {
            get { return GetString("Message"); }
            set
            {
                SetString("Message", value);
            }
        }

        [IgnoreDataMember]
        public String ModuleName
        {
            get { return GetString("ModuleName"); }
            set
            {
                SetString("ModuleName", value);
            }
        }

        [IgnoreDataMember]
        public Int32 LogLevel
        {
            get { return GetInt32("LogLevel"); }
            set
            {
                SetInt32("LogLevel", value);
            }
        }
        [IgnoreDataMember]
        public String LOGRESULT
        {
            get { return GetString("LOGRESULT"); }
            set
            {
                SetString("LOGRESULT", value);
            }
        }

        [IgnoreDataMember]
        public String LOGTYPE
        {
            get { return GetString("LOGTYPE"); }
            set
            {
                SetString("LOGTYPE", value);
            }
        }
        [IgnoreDataMember]
        public String OPERATOR
        {
            get { return GetString("OPERATOR"); }
            set
            {
                SetString("OPERATOR", value);
            }
        }
        
        
    }

    /*App_Login
    *
    * exec ESP_ClassCodeGenerator  App_Login,1
    */
    public partial class APP_Login : EntityBase//<APP_Login>
    {

        [IgnoreDataMember]
        public String UserName
        {
            get { return GetString("UserName"); }
            set
            {
                SetString("UserName", value);
            }
        }

        [IgnoreDataMember]
        public String ServerIP
        {
            get { return GetString("ServerIP"); }
            set
            {
                SetString("ServerIP", value);
            }
        }

        [IgnoreDataMember]
        public String ServerName
        {
            get { return GetString("ServerName"); }
            set
            {
                SetString("ServerName", value);
            }
        }

        [IgnoreDataMember]
        public String ClientIP
        {
            get { return GetString("ClientIP"); }
            set
            {
                SetString("ClientIP", value);
            }
        }

        [IgnoreDataMember]
        public String ClientName
        {
            get { return GetString("ClientName"); }
            set
            {
                SetString("ClientName", value);
            }
        }

        [IgnoreDataMember]
        public String LoginPort
        {
            get { return GetString("LoginPort"); }
            set
            {
                SetString("LoginPort", value);
            }
        }

        [IgnoreDataMember]
        public String CPUType
        {
            get { return GetString("CPUType"); }
            set
            {
                SetString("CPUType", value);
            }
        }

        [IgnoreDataMember]
        public String FilePath
        {
            get { return GetString("FilePath"); }
            set
            {
                SetString("FilePath", value);
            }
        }

        [IgnoreDataMember]
        public DateTime LoginTime
        {
            get { return GetDateTime("LoginTime"); }
            set
            {
                SetDateTime("LoginTime", value);
            }
        }

        [IgnoreDataMember]
        public DateTime LogoutTime
        {
            get { return GetDateTime("LogoutTime"); }
            set
            {
                SetDateTime("LogoutTime", value);
            }
        }

        [IgnoreDataMember]
        public String LogMessage
        {
            get { return GetString("LogMessage"); }
            set
            {
                SetString("LogMessage", value);
            }
        }

        [IgnoreDataMember]
        public Int32 Status
        {
            get { return GetInt32("Status"); }
            set
            {
                SetInt32("Status", value);
            }
        }

        private TJsonList<APP_Log> _Logs;
        public TJsonList<APP_Log> Logs
        {
            get
            {
                if (null == _Logs)
                {
                    _Logs = new TJsonList<APP_Log>();
                }

                return _Logs;
            }
            set
            {
                _Logs = value;
            }
        }
    }



    /*EAP_Navigator_Node
   *
   * exec ESP_ClassCodeGenerator  EAP_Navigator_Node,2
   */
    public partial class EAP_Navigator_Node : TJson
    {
        public Int32 ID { get; set; }

        public Int32 SourceID { get; set; }

        public Int32 Row { get; set; }

        public Int32 Col { get; set; }

        public String Name { get; set; }

        public String Text { get; set; }

        public bool Access { get; set; }

        public String PageID { get; set; }

        public String AddBy { get; set; }

        public DateTime AddOn { get; set; }

        public Boolean Flag_Delete { get; set; }

    }

    /*EAP_Navigator_Asso
    *
    * exec ESP_ClassCodeGenerator  EAP_Navigator_Asso,2
    */
    public partial class EAP_Navigator_Asso : TJson
    {

        public Int32 FromID { get; set; }

        public Int32 ToID { get; set; }

        public String Text { get; set; }

        public String AddBy { get; set; }

        public DateTime AddOn { get; set; }

        public Boolean Flag_Delete { get; set; }

    }


    public class NavigatorMeta : TJson
    {
        public NavigatorMeta()
        {
            Asso = new List<EAP_Navigator_Asso>();
            Nodes = new List<EAP_Navigator_Node>();
        }

        public List<EAP_Navigator_Asso> Asso { get; set; }
        public List<EAP_Navigator_Node> Nodes { get; set; }
    }



    /*DiagnoseResult
*
* exec ESP_ClassCodeGenerator  DiagnoseResult,1
*/
    public partial class DiagnoseResult : EntityBase//<DiagnoseResult>
    {
        [IgnoreDataMember]
        public String Name
        {
            get { return GetString("Name"); }
            set
            {
                SetString("Name", value);
            }
        }

        [IgnoreDataMember]
        public Int32 Result
        {
            get { return GetInt32("Result"); }
            set
            {
                SetInt32("Result", value);
            }
        }

        [IgnoreDataMember]
        public String Message
        {
            get { return GetString("Message"); }
            set
            {
                SetString("Message", value);
            }
        }

        [IgnoreDataMember]
        public DateTime BeginTime
        {
            get { return GetDateTime("BeginTime"); }
            set
            {
                SetDateTime("BeginTime", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EndTime
        {
            get { return GetDateTime("EndTime"); }
            set
            {
                SetDateTime("EndTime", value);
            }
        }

        public string Output()
        {
            return string.Format("{0}#{1}#{2}#{3}#{4}#{5}", ID, Name, Result, Message, BeginTime, EndTime);
        }
    }
    /*EAP_Bug
    *
    * exec ESP_ClassCodeGenerator  EAP_Bug,1
    */
    public partial class EAP_Bug : EntityBase//<EAP_Bug>
    {
        public EAP_Bug()
        {
            Commands = new EntityList<EAP_BugCommand>();
        }

        [IgnoreDataMember]
        public Int32 ResourceID
        {
            get { return GetInt32("ResourceID"); }
            set
            {
                SetInt32("ResourceID", value);
            }
        }

        [IgnoreDataMember]
        public Int32 LoginID
        {
            get { return GetInt32("LoginID"); }
            set
            {
                SetInt32("LoginID", value);
            }
        }

        public String ResourceName_G { get; set; }

        [IgnoreDataMember]
        public String Memo
        {
            get { return GetString("Memo"); }
            set
            {
                SetString("Memo", value);
            }
        }

        [IgnoreDataMember]
        public String Category
        {
            get { return GetString("Category"); }
            set
            {
                SetString("Category", value);
            }
        }

        [IgnoreDataMember]
        public String Priority
        {
            get { return GetString("Priority"); }
            set
            {
                SetString("Priority", value);
            }
        }

        [IgnoreDataMember]
        public String Status
        {
            get { return GetString("Status"); }
            set
            {
                SetString("Status", value);
            }
        }

        [IgnoreDataMember]
        public String Priority_G
        {
            get { return GetString("Priority_G"); }
            set
            {
                SetString("Priority_G", value);
            }
        }
        [IgnoreDataMember]
        public String Status_G
        {
            get { return GetString("Status_G"); }
            set
            {
                SetString("Status_G", value);
            }
        }
        [IgnoreDataMember]
        public String Category_G
        {
            get { return GetString("Category_G"); }
            set
            {
                SetString("Category_G", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetString("EditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public String ClientIP
        {
            get { return GetString("ClientIP"); }
            set
            {
                SetString("ClientIP", value);
            }
        }

        [ForeignKeyAttribute("BugID", "ID")]
        public EntityList<EAP_BugCommand> Commands { get; set; }
    }

    /*EAP_BugCommand
    *
    * exec ESP_ClassCodeGenerator  EAP_BugCommand,1
    */
    public partial class EAP_BugCommand : EntityBase//<EAP_BugCommand>
    {
        [IgnoreDataMember]
        public Int32 BugID
        {
            get { return GetInt32("BugID"); }
            set
            {
                SetInt32("BugID", value);
            }
        }

        [IgnoreDataMember]
        public String Memo
        {
            get { return GetString("Memo"); }
            set
            {
                SetString("Memo", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        public string ImageUir { get; set; }

#if SILVERLIGHT
        public bool HasImage { get { return ImageSource != null; } }
        ImageSource _ImageSource;
        [IgnoreDataMember]
        public ImageSource ImageSource
        {
            get { return _ImageSource; }
            set
            {
                _ImageSource = value;
                OnPropertyChanged("ImageSource");
                OnPropertyChanged("HasImage");
            }
        }
#endif

    }


    /*EAP_Requirement
*
* exec ESP_ClassCodeGenerator  EAP_Requirement,1
*/
    public partial class EAP_Requirement : EntityBase//<EAP_Requirement>
    {

        [IgnoreDataMember]
        public Int32 ResourceID
        {
            get { return GetInt32("ResourceID"); }
            set
            {
                SetInt32("ResourceID", value);
            }
        }

        [IgnoreDataMember]
        public String Title
        {
            get { return GetString("Title"); }
            set
            {
                SetString("Title", value);
            }
        }

        [IgnoreDataMember]
        public String Memo
        {
            get { return GetString("Memo"); }
            set
            {
                SetString("Memo", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public String AuditBy
        {
            get { return GetString("AuditBy"); }
            set
            {
                SetString("AuditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AuditOn
        {
            get { return GetDateTime("AuditOn"); }
            set
            {
                SetDateTime("AuditOn", value);
            }
        }

        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetString("EditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public String DeleteBy
        {
            get { return GetString("DeleteBy"); }
            set
            {
                SetString("DeleteBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime DeleteOn
        {
            get { return GetDateTime("DeleteOn"); }
            set
            {
                SetDateTime("DeleteOn", value);
            }
        }

        [IgnoreDataMember]
        public Int32 FlagDelete
        {
            get { return GetInt32("FlagDelete"); }
            set
            {
                SetInt32("FlagDelete", value);
            }
        }

    }

    /*EAP_Search
*
* exec ESP_ClassCodeGenerator  EAP_Search,1
*/
    public partial class EAP_Search : EntityBase//<EAP_Search>
    {
        public EAP_Search()
        {
            Sql_Select = "*";
            Sql_OrderBy = "ID desc";
            KeyField = "ID";
            PageSize = 20;

        }

        [IgnoreDataMember]
        public String SearchID
        {
            get { return GetString("SearchID"); }
            set
            {
                SetString("SearchID", value);
            }
        }

        [IgnoreDataMember]
        public String Sql_Select
        {
            get { return GetString("Sql_Select"); }
            set
            {
                SetString("Sql_Select", value);
            }
        }

        [IgnoreDataMember]
        public String Sql_From
        {
            get { return GetString("Sql_From"); }
            set
            {
                SetString("Sql_From", value);
            }
        }

        [IgnoreDataMember]
        public String Sql_OrderBy
        {
            get { return GetString("Sql_OrderBy"); }
            set
            {
                SetString("Sql_OrderBy", value);
            }
        }

        [IgnoreDataMember]
        public String SQL_Pro
        {
            get { return GetString("SQL_Pro"); }
            set
            {
                SetString("SQL_Pro", value);
            }
        }

        [IgnoreDataMember]
        public String KeyField
        {
            get { return GetString("KeyField"); }
            set
            {
                SetString("KeyField", value);
            }
        }

        [IgnoreDataMember]
        public String SubSearch
        {
            get { return GetString("SubSearch"); }
            set
            {
                SetString("SubSearch", value);
            }
        }

        [IgnoreDataMember]
        public Int32 PageSize
        {
            get { return GetInt32("PageSize"); }
            set
            {
                SetInt32("PageSize", value);
            }
        }

        [IgnoreDataMember]
        public String Note
        {
            get { return GetString("Note"); }
            set
            {
                SetString("Note", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public Int32 FlagDelete
        {
            get { return GetInt32("FlagDelete"); }
            set
            {
                SetInt32("FlagDelete", value);
            }
        }

    }

    /*EAP_SearchField
    *
    * exec ESP_ClassCodeGenerator  EAP_SearchField,1
    */
    public partial class EAP_SearchField : EntityBase//<EAP_SearchField>
    {
        public EAP_SearchField()
        {
            DataType = "String";
            Operation = "=";
            SqlFormat = "{0}={1}";
            Leading = "and (";
            Trailing = ")";
            Sort = 0;
        }

        [IgnoreDataMember]
        public String SearchID
        {
            get { return GetString("SearchID"); }
            set
            {
                SetString("SearchID", value);
            }
        }

        [IgnoreDataMember]
        public String FieldName
        {
            get { return GetString("FieldName"); }
            set
            {
                SetString("FieldName", value);
            }
        }

        [IgnoreDataMember]
        public String DataMember
        {
            get { return GetString("DataMember"); }
            set
            {
                SetString("DataMember", value);
            }
        }

        [IgnoreDataMember]
        public String DataType
        {
            get { return GetString("DataType"); }
            set
            {
                SetString("DataType", value);
            }
        }

        [IgnoreDataMember]
        public String Operation
        {
            get { return GetString("Operation"); }
            set
            {
                SetString("Operation", value);
            }
        }

        [IgnoreDataMember]
        public String SqlFormat
        {
            get { return GetString("SqlFormat"); }
            set
            {
                SetString("SqlFormat", value);
            }
        }

        [IgnoreDataMember]
        public Int32 Sort
        {
            get { return GetInt32("Sort"); }
            set
            {
                SetInt32("Sort", value);
            }
        }

        [IgnoreDataMember]
        public String Leading
        {
            get { return GetString("Leading"); }
            set
            {
                SetString("Leading", value);
            }
        }

        [IgnoreDataMember]
        public String Trailing
        {
            get { return GetString("Trailing"); }
            set
            {
                SetString("Trailing", value);
            }
        }

        [IgnoreDataMember]
        public String Note
        {
            get { return GetString("Note"); }
            set
            {
                SetString("Note", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public Boolean FlagDelete
        {
            get { return GetBoolean("FlagDelete"); }
            set
            {
                SetBoolean("FlagDelete", value);
            }
        }
    }

    /*App_Lookup
*
* exec ESP_ClassCodeGenerator  App_Lookup,1
*/
    public partial class App_Lookup : EntityBase//<App_Lookup>
    {

        [IgnoreDataMember]
        public String LookupID
        {
            get { return GetString("LookupID"); }
            set
            {
                SetString("LookupID", value);
            }
        }

        [IgnoreDataMember]
        public String Code
        {
            get { return GetString("Code"); }
            set
            {
                SetString("Code", value);
            }
        }

        [IgnoreDataMember]
        public String Value
        {
            get { return GetString("Value"); }
            set
            {
                SetString("Value", value);
            }
        }

        [IgnoreDataMember]
        public String ParentCode
        {
            get { return GetString("ParentCode"); }
            set
            {
                SetString("ParentCode", value);
            }
        }

        [IgnoreDataMember]
        public String Par1
        {
            get { return GetString("Par1"); }
            set
            {
                SetString("Par1", value);
            }
        }

        [IgnoreDataMember]
        public String Par2
        {
            get { return GetString("Par2"); }
            set
            {
                SetString("Par2", value);
            }
        }

        [IgnoreDataMember]
        public String Par3
        {
            get { return GetString("Par3"); }
            set
            {
                SetString("Par3", value);
            }
        }

        [IgnoreDataMember]
        public Int32 SortBy
        {
            get { return GetInt32("SortBy"); }
            set
            {
                SetInt32("SortBy", value);
            }
        }

        [IgnoreDataMember]
        public String Permission
        {
            get { return GetString("Permission"); }
            set
            {
                SetString("Permission", value);
            }
        }

        [IgnoreDataMember]
        public Int32 AddBy
        {
            get { return GetInt32("AddBy"); }
            set
            {
                SetInt32("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public Int32 EditBy
        {
            get { return GetInt32("EditBy"); }
            set
            {
                SetInt32("EditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public Boolean DeleteFlag
        {
            get { return GetBoolean("DeleteFlag"); }
            set
            {
                SetBoolean("DeleteFlag", value);
            }
        }

        [IgnoreDataMember]
        public Int32 DeleteBy
        {
            get { return GetInt32("DeleteBy"); }
            set
            {
                SetInt32("DeleteBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime DeleteOn
        {
            get { return GetDateTime("DeleteOn"); }
            set
            {
                SetDateTime("DeleteOn", value);
            }
        }

    }

    /*App_Config
    *
    * exec ESP_ClassCodeGenerator  App_Config,1
    */
    public partial class App_Config : EntityBase//<App_Config>
    {

        [IgnoreDataMember]
        public Int32 OrgID
        {
            get { return GetInt32("OrgID"); }
            set
            {
                SetInt32("OrgID", value);
            }
        }

        [IgnoreDataMember]
        public String GroupID
        {
            get { return GetString("GroupID"); }
            set
            {
                SetString("GroupID", value);
            }
        }

        [IgnoreDataMember]
        public String Name
        {
            get { return GetString("Name"); }
            set
            {
                SetString("Name", value);
            }
        }

        [IgnoreDataMember]
        public String Value
        {
            get { return GetString("Value"); }
            set
            {
                SetString("Value", value);
            }
        }

        [IgnoreDataMember]
        public String Description
        {
            get { return GetString("Description"); }
            set
            {
                SetString("Description", value);
            }
        }

        [IgnoreDataMember]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetString("AddBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

        [IgnoreDataMember]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetString("EditBy", value);
            }
        }

        [IgnoreDataMember]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetDateTime("EditOn", value);
            }
        }

        [IgnoreDataMember]
        public Boolean LoadOnStart
        {
            get { return GetBoolean("LoadOnStart"); }
            set
            {
                SetBoolean("LoadOnStart", value);
            }
        }

        [IgnoreDataMember]
        public Boolean Flag_Delete
        {
            get { return GetBoolean("Flag_Delete"); }
            set
            {
                SetBoolean("Flag_Delete", value);
            }
        }
    }


    /*EAP_RunServer
*
* exec ESP_ClassCodeGenerator  EAP_RunServer,1
*/
    public partial class EAP_RunServer : EntityBase//<EAP_RunServer>
    {
        public EAP_RunServer()
        {

        }

        public EAP_RunServer(LoginInfo login, AppSetting setting)
        {
            ClientIP = login.ClientIP;
            ClientName = login.ClientName;
            ServerIP = login.ServerIP;
            ServerName = login.ServerName;
            App_Title = setting.App_Title;
            App_Version = setting.App_Version;
            App_Customer = setting.Customer;
        }

        [IgnoreDataMember]
        public String ClientIP
        {
            get { return GetString("ClientIP"); }
            set
            {
                SetString("ClientIP", value);
            }
        }

        [IgnoreDataMember]
        public String ClientName
        {
            get { return GetString("ClientName"); }
            set
            {
                SetString("ClientName", value);
            }
        }

        [IgnoreDataMember]
        public String ServerIP
        {
            get { return GetString("ServerIP"); }
            set
            {
                SetString("ServerIP", value);
            }
        }

        [IgnoreDataMember]
        public String ServerName
        {
            get { return GetString("ServerName"); }
            set
            {
                SetString("ServerName", value);
            }
        }

        [IgnoreDataMember]
        public String App_Title
        {
            get { return GetString("App_Title"); }
            set
            {
                SetString("App_Title", value);
            }
        }

        [IgnoreDataMember]
        public String App_Version
        {
            get { return GetString("App_Version"); }
            set
            {
                SetString("App_Version", value);
            }
        }

        [IgnoreDataMember]
        public String App_Customer
        {
            get { return GetString("App_Customer"); }
            set
            {
                SetString("App_Customer", value);
            }
        }

        [IgnoreDataMember]
        public String EAPVersion
        {
            get { return GetString("EAPVersion"); }
            set
            {
                SetString("EAPVersion", value);
            }
        }


        [IgnoreDataMember]
        public String Variables
        {
            get { return GetString("Variables"); }
            set
            {
                SetString("Variables", value);
            }
        }

        [IgnoreDataMember]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetDateTime("AddOn", value);
            }
        }

    }



}
