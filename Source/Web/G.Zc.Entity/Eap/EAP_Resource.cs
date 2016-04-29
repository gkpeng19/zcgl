using GOMFrameWork.DataEntity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G.Zc.Entity.Eap
{
    public partial class EAP_Resource : EapBaseEntity
    {
        [JsonIgnore]
        public int SourceID
        {
            get { return GetInt32("SourceID"); }
            set
            {
                SetValue("SourceID", value);
            }
        }

        [JsonIgnore]
        public String Name
        {
            get { return GetString("Name"); }
            set
            {
                SetValue("Name", value);
            }
        }

        [JsonIgnore]
        public String Title
        {
            get { return GetString("Title"); }
            set
            {
                SetValue("Title", value);
            }
        }

        [JsonIgnore]
        public String Description
        {
            get { return GetString("Description"); }
            set
            {
                SetValue("Description", value);
            }
        }

        [JsonIgnore]
        public String Type
        {
            get { return GetString("Type"); }
            set
            {
                SetValue("Type", value);
            }
        }

        [JsonIgnore]
        public Int32 ParentId
        {
            get { return GetInt32("ParentId"); }
            set
            {
                SetValue("ParentId", value);
            }
        }

        [JsonIgnore]
        public Int32 Level
        {
            get { return GetInt32("Level"); }
            set
            {
                SetValue("Level", value);
            }
        }

        [JsonIgnore]
        public String PageId
        {
            get { return GetString("PageId"); }
            set
            {
                SetValue("PageId", value);
            }
        }

        [JsonIgnore]
        public Int32 VRow
        {
            get { return GetInt32("VRow"); }
            set
            {
                SetValue("VRow", value);
            }
        }

        [JsonIgnore]
        public Int32 VCol
        {
            get { return GetInt32("VCol"); }
            set
            {
                SetValue("VCol", value);
            }
        }

        [JsonIgnore]
        public Int32 SortBy
        {
            get { return GetInt32("SortBy"); }
            set
            {
                SetValue("SortBy", value);
            }
        }

        [JsonIgnore]
        public String Permission
        {
            get { return GetString("Permission"); }
            set
            {
                SetValue("Permission", value);
            }
        }

        [JsonIgnore]
        public String Image
        {
            get { return GetString("Image"); }
            set
            {
                SetValue("Image", value);
            }
        }

        [JsonIgnore]
        public String AddBy
        {
            get { return GetString("AddBy"); }
            set
            {
                SetValue("AddBy", value);
            }
        }

        [JsonIgnore]
        public DateTime AddOn
        {
            get { return GetDateTime("AddOn"); }
            set
            {
                SetValue("AddOn", value);
            }
        }

        [JsonIgnore]
        public String EditBy
        {
            get { return GetString("EditBy"); }
            set
            {
                SetValue("EditBy", value);
            }
        }


        [JsonIgnore]
        public Int32 HasChild_G
        {
            get { return GetInt32("HasChild_G"); }
            set
            {
                SetValue("HasChild_G", value);
            }
        }
        [JsonIgnore]
        public DateTime EditOn
        {
            get { return GetDateTime("EditOn"); }
            set
            {
                SetValue("EditOn", value);
            }
        }

        [JsonIgnore]
        public Boolean Flag_Delete
        {
            get { return GetBoolean("Flag_Delete"); }
            set
            {
                SetValue("Flag_Delete", value);
            }
        }

        /// <summary>
        /// 仅用在加载功能按钮使用
        /// </summary>
        [JsonIgnore]
        public int IsLoadChild_G
        {
            get { return GetInt32("IsLoadChild_G"); }
            set
            {
                SetValue("IsLoadChild_G", value);
            }
        }
        [JsonIgnore]
        public Boolean IsVaild_G
        {
            get { return GetBoolean("IsVaild_G"); }
            set
            {
                SetValue("IsVaild_G", value);
            }
        }

    }
}
