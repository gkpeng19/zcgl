﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由T4模板自动生成
//	   生成时间 2013-04-23 17:24:50 by App
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.DataAnnotations;

namespace App.Models.MIS
{

    public class MIS_Article_CommentModel
    {

        [Display(Name = "ID")]
        public string Id { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "文章ID")]
        public string ArticleId { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "用户")]
        public string UserId { get; set; }

        [MaxWordsExpression(50)]
        [Display(Name = "昵称")]
        public string TrueName { get; set; }

        [MaxWordsExpression(255)]
        [Display(Name = "IP")]
        public string IP { get; set; }

        [MaxWordsExpression(4000)]
        [NotNullExpression]//不能为空
        [Display(Name = "内容")]
        public string BodyContent { get; set; }

        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "创建时间")]
        public DateTime? CreateTime { get; set; }

        [IsNumberExpression]//如果填写判断是否是数字
        [Display(Name = "是否回复")]
        public int? IsReply { get; set; }

        [MaxWordsExpression(4000)]
        [Display(Name = "回复内容")]
        public string ReplyContent { get; set; }

        [DateExpression]//如果填写判断是否是日期
        [Display(Name = "回复时间")]
        public DateTime? ReplyTime { get; set; }



    }
}

