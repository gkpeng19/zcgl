using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using NM.Util;
 
namespace NM.Mail
{
    [Serializable]
    public class MailInfo : TJson
    {
        int event_notify_id;

        public int Event_notify_id
        {
            get { return event_notify_id; }
            set { event_notify_id = value; }
        }

        int notify_property = 0;

        public int Notify_property
        {
            get { return notify_property; }
            set { notify_property = value; }
        }

        public MailStatus Status
        {
            get
            {
                return (MailStatus)Notify_property;
            }
            set
            {
                Notify_property = (int)value;
            }
        }


        string notify_title;

        public string Notify_title
        {
            get { return notify_title; }
            set { notify_title = value; }
        }
        string notify_content;

        public string Notify_content
        {
            get { return notify_content; }
            set { notify_content = value; }
        }
        DateTime notify_datetime;

        public DateTime Notify_datetime
        {
            get { return notify_datetime; }
            set { notify_datetime = value; }
        }
        string from_email;

        public string From_email
        {
            get { return from_email; }
            set { from_email = value; }
        }
        string email;

        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        string attach_file_path;

        public string Attach_file_path
        {
            get { return attach_file_path; }
            set
            {
                attach_file_path = value;
                _Attachments.Clear();
                SetAttachments(attach_file_path);
            }
        }

        int document_id;

        public int Document_id
        {
            get { return document_id; }
            set { document_id = value; }
        }
        int addby;

        public int Addby
        {
            get { return addby; }
            set { addby = value; }
        }

        DateTime addon;

        public DateTime Addon
        {
            get { return addon; }
            set { addon = value; }
        }
        int flag_delete;

        public int Flag_delete
        {
            get { return flag_delete; }
            set { flag_delete = value; }
        }


        private List<string> _Attachments;

        [CategoryAttribute("Context")]
        public List<string> Attachments
        {
            get { return _Attachments; }
            set { _Attachments = value; }
        }

        public MailInfo()
        {
            _Attachments = new List<string>();
        }

        #region 原始方法 无用

        //public void AddAttachment(string attachmentPath)
        //{
        //    if (!string.IsNullOrEmpty(attachmentPath))
        //    {
        //        if (!string.IsNullOrEmpty(MailSetting.Default.InfoDocPath) && attachmentPath.IndexOf(":") <= 0)
        //        {
        //            attachmentPath = attachmentPath.Replace('/', '\\');
        //            while (attachmentPath.StartsWith("\\"))
        //            {
        //                attachmentPath = attachmentPath.Remove(0, 1);
        //            }
        //            // here infodocs is alias of share folder,so repace it with phisic path                    
        //            if (attachmentPath.StartsWith("infodocs\\", StringComparison.OrdinalIgnoreCase))
        //                attachmentPath = attachmentPath.Remove(0, 9);

        //            attachmentPath = Path.Combine(MailSetting.Default.InfoDocPath, attachmentPath);
        //        }
        //        if (File.Exists(attachmentPath))
        //        {
        //            _Attachments.Add(attachmentPath);
        //        }
        //        else
        //        {
        //            ReturnMessage = attachmentPath + " not exists";
        //            //FileLogManager.Default.ErrorIt(ReturnMessage);
        //            Status = MailStatus.AttachmentMissing;
        //        }
        //    }
        //}

        #endregion

        /// <summary>
        /// 添加附件
        /// </summary>
        /// <param name="absolutePath">文件的绝对路径</param>
        public void AddAttachment(string absolutePath)
        {
            if (!string.IsNullOrEmpty(absolutePath))
            {
                if (absolutePath.IndexOf(":") > 0)
                {
                    if (File.Exists(absolutePath))
                    {
                        _Attachments.Add(absolutePath);
                    }
                    else
                    {
                        ReturnMessage = absolutePath + " not exists";
                        //FileLogManager.Default.ErrorIt(ReturnMessage);
                        Status = MailStatus.AttachmentMissing;
                    }
                }
            }
        }
        
        public void SetAttachments(string absolutePaths)
        {
            string[] mailAttachments = absolutePaths.Split(';');
            foreach (string file in mailAttachments)
            {
                AddAttachment(file);
                if (Status == MailStatus.AttachmentMissing)
                    break;
            }
        }

        private string _ReturnMessage = "Mail not been send";

        public string ReturnMessage
        {
            get { return _ReturnMessage; }
            set { _ReturnMessage = value; }
        }
    }
}
