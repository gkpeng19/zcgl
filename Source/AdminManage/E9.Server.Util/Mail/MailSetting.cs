using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace NM.Mail
{
    [Serializable]
    public class MailSetting : ICloneable
    {
        public MailSetting()
        {
        }

        public static MailSetting _Default;
        public static MailSetting Default
        {
            get
            {
                if (null == _Default)
                {
                    // _Default= ConfigBase.Load<ConfigOper>();

                    if (File.Exists(SqlMappingFileName))
                    {
                        try
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(MailSetting));
                            Stream reader = new FileStream(SqlMappingFileName, FileMode.Open);
                            _Default = (MailSetting)serializer.Deserialize(reader);
                            _Default.Changed = false;
                            reader.Close();
                        }
                        catch
                        {
                        }
                    }
                    if (null == _Default)
                    {
                        _Default = new MailSetting();
                        _Default.Changed = true;
                    }
                }
                return _Default;
            }
        }

        public static void Reset()
        {
            _Default = null;
        }
        private static string SqlMappingFileName
        {
            get
            {
                return Path.Combine(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName, "ConfigOper.xml");
            }
        }

        public void Flush()
        {
            //    if (Changed)
            {
                try
                {
                    XmlSerializer Serializer = new XmlSerializer(typeof(MailSetting));
                    FileStream fs = new FileStream(SqlMappingFileName, FileMode.Create);
                    TextWriter writer = new StreamWriter(fs, new UTF8Encoding());
                    Serializer.Serialize(writer, this);
                    writer.Close();
                    _Changed = false;
                }
                catch
                {

                }
            }
        }

        public object Clone()
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formater = new BinaryFormatter();
            formater.Serialize(stream, this);
            object result = formater.Deserialize(stream);
            stream.Close();
            return result;
        }

        private bool _Changed = false;
        [XmlIgnore]
        [Browsable(false)]
        public bool Changed
        {
            get { return _Changed; }
            set
            { _Changed = value; }
        }

        #region cofiguration

        //todo  1 your configure

        private string _ConnectionString;
        //[Editor(typeof(ConnectionStringEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(String))]
        [Category("InfoShare")]
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set { _ConnectionString = value; }
        }

        private int _TimespanForUpdate = 60;
        [Browsable(false)]
        public int TimespanForUpdate
        {
            get { return _TimespanForUpdate; }
            set { _TimespanForUpdate = value; }
        }

        private string _FileLibPath;
        [Category("InfoShare")]
        [Description("InfoDocs share folder")]
        //[Editor(typeof(FolderEditer), typeof(UITypeEditor))]
        public string InfoDocPath
        {
            get { return _FileLibPath; }
            set { _FileLibPath = value; }
        }

        private MailServerInfo _SmtpServerConn;
        [Category("SMTP")]
        //[Editor(typeof(MailSmtpServerEditor), typeof(UITypeEditor))]
        public MailServerInfo SmtpServerConn
        {
            get
            {
                if (_SmtpServerConn == null)
                    _SmtpServerConn = new MailServerInfo();
                return _SmtpServerConn;
            }
            set { _SmtpServerConn = value; }
        }

        private string _DefaultSenderMail = "youhok@gmail.com";
        [Category("SMTP")]
        [Browsable(false)]
        public string DefaultSenderMailAddress
        {
            get { return _DefaultSenderMail; }
            set { _DefaultSenderMail = value; }
        }

        #endregion
        /*
                private string _SmtpServer;
                [Category("SMTP")]
                public string SmtpServer
                {
                    get { return _SmtpServer; }
                    set { _SmtpServer = value; }
                }

                private int _SmtpPort = 25;
                [Category("SMTP")]
                public int SmtpPort
                {
                    get { return _SmtpPort; }
                    set { _SmtpPort = value; }
                }

                private string _UserName;
                [Category("SMTP")]
                public string UserName
                {
                    get { return _UserName; }
                    set { _UserName = value; }
                }

                private string _Password;
                [Category("SMTP")]
                public string Password
                {
                    get { return _Password; }
                    set { _Password = value; }
                }

                private bool _NeedSSL = false;
                [Category("SMTP")]
                public bool NeedSSL
                {
                    get { return _NeedSSL; }
                    set { _NeedSSL = value; }
                } 
         */
    }
}
