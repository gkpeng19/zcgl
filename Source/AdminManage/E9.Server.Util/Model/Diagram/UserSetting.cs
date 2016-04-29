using System.IO;
using System.IO.IsolatedStorage;
using NM.Util;


namespace NM.Diagram.Render
{
    /// <summary>
    /// Business entity used to model a GraphicSetting
    /// </summary>
    public class UserSetting : GraphicSetting
    {
        /// <summary>
        /// Default constructor with some specified initial values
        /// </summary>
        public UserSetting()
        {
            ReleaseDate = "27/12/2008";
        }

        public UserSetting(int staffId)
            : this()
        {
            StaffId = staffId;
        }

        public string ReleaseDate { get; set; }
        public int StaffId { get; set; }

        bool _IsDebug;
        public bool IsDebug
        {
            get { return _IsDebug; }
            set
            {
                _IsDebug = value;
                OnPropertyChanged("IsDebug");
            }
        }

        bool _UseCache;
        public bool UseCache
        {
            get { return _UseCache; }
            set
            {
                _UseCache = value;
                OnPropertyChanged("UseCache");
            }
        }


        bool _AllowHideToolTip;
        public bool AllowHideToolTip
        {
            get { return _AllowHideToolTip; }
            set
            {
                _AllowHideToolTip = value;
                OnPropertyChanged("AllowHideToolTip");
            }
        }

        bool _AllowHideCaption;
        public bool AllowHideCaption
        {
            get { return _AllowHideCaption; }
            set
            {
                _AllowHideCaption = value;
                OnPropertyChanged("AllowHideCaption");
            }
        }

        bool _AllowHideRelationship;
        public bool AllowHideRelationship
        {
            get { return _AllowHideRelationship; }
            set
            {
                _AllowHideRelationship = value;
                OnPropertyChanged("AllowHideRelationship");
            }
        }

        bool _AllowMoveChildren;
        public bool AllowMoveChildren
        {
            get { return _AllowMoveChildren; }
            set
            {
                _AllowMoveChildren = value;
                OnPropertyChanged("AllowMoveChildren");
                if (value)
                    AllowMultiSelect = false;
            }
        }

        bool _AllowMultiSelect;
        public bool AllowMultiSelect
        {
            get { return _AllowMultiSelect; }
            set
            {
                _AllowMultiSelect = value;
                OnPropertyChanged("AllowMultiSelect");
                if (value)
                    AllowMoveChildren = false;
            }
        }
        int _VisibleCount;
        public int VisilbeCount
        {
            get { return _VisibleCount; }
            set
            {
                _VisibleCount = value;
                OnPropertyChanged("VisibleCount");
            }
        }

        //bool _AllowMultiHighLight;
        //public bool AllowMultiHighLight
        //{
        //    get { return _AllowMultiHighLight; }
        //    set
        //    {
        //        _AllowMultiHighLight = value;
        //        OnPropertyChanged("AllowMultiHighLight");
        //    }
        //}

        bool _IsShowIndirectAssociate;

        public bool IsShowIndirectAssociate
        {
            get { return _IsShowIndirectAssociate; }
            set
            {
                _IsShowIndirectAssociate = value;
                OnPropertyChanged("IsShowIndirectAssociate");
            }
        } 

        public GraphicSetting DefaultGraphicSetting { get; set; }
        private static string GetFileName(int StaffId)
        {
            return string.Format("{0}.UserSetting", StaffId);
        }

#if SILVERLIGHT
        public static UserSetting Load(int stafferId)
        {
            UserSetting result = null;
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string FileName = GetFileName(stafferId);
                    if (file.FileExists(FileName))
                    {
                        IsolatedStorageFileStream filestream = file.OpenFile(FileName, FileMode.Open);
                        int fileSize = (int)filestream.Length;
                        byte[] context = new byte[fileSize];

                        filestream.Read(context, 0, fileSize);
                        MemoryStream stream = new MemoryStream(context);

                        StreamReader reader = new StreamReader(stream);
                        string josnString = reader.ReadToEnd();
                        reader.Close();

                        try
                        {
                            result = TJson.Parse<UserSetting>(josnString);
                        }
                        catch
                        {
                        }
                    }
                }
            }
            catch
            {
            }

            if (result == null)
                result = new UserSetting(stafferId);
            return result;
        }

        public void Flush()
        {
            try
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string FileName = GetFileName(StaffId);

                    using (IsolatedStorageFileStream fileStream = file.CreateFile(FileName))
                    {
                        MemoryStream stream = new MemoryStream();
                        StreamWriter writer = new StreamWriter(stream);
                        writer.Write(this.ToJson());
                        writer.Flush();
                        fileStream.Write(stream.ToArray(), 0, (int)stream.Length);
                        stream.Close();
                    }
                }
            }
            catch
            {
            }
        }
#endif
    }
}
