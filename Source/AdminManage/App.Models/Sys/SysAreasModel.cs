

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NM.Util;
namespace App.Models.Sys
{

    public class SysAreasModel
    {
        [NotNullExpression]
        [IsCharExpression] 
        [MaxWordsExpression(50)]
        [Display(Name = "编码")]
        public string Id { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "名称")]
        public string Name { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "上级编码")]
        public string ParentId { get; set; }

        [Display(Name = "排序")]
        public int Sort { get; set; }

        [Display(Name = "状态")]
        public bool Enable { get; set; }

        [Display(Name = "创建时间")]
        public DateTime CreateTime { get; set; }

        [Display(Name = "直辖市")]
        public bool IsMunicipality { get; set; }

        [Display(Name = "特别行政区")]
        public bool IsHKMT { get; set; }

        [Display(Name = "其他")]
        public bool IsOther { get; set; }

        public string state { get; set; }

        public List<SysAreasModel> clildren { get; set; }


    }
}

