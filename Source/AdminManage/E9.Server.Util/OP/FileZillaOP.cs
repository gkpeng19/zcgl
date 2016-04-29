using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Model;
using System.IO;
using System.Web.Hosting;
using NM.Util;

namespace NM.OP
{
    public class FileZillaOP : CertifiedProviderOP
    {
        public FileZillaOP(LoginInfo login, DataProvider dp)
            : base(login, dp)
        {
        }

        private string m_TempExtension = "_temp";
        private string m_UploadFolder = "ImageForPrint";

        #region ** Upload File

        public CommandResult UploadFileAsync(FileChunk fc)
        {
            CommandResult result = new CommandResult() { Result = false };
            string _FileName = fc.FileName;

            if (fc.FirstChunk)
                _FileName = this.GetFileName() + fc.FileExtension;

            //string _FileFullName = _FileName + fc.FileExtension;

            string _FileExtension = fc.FileExtension;
            byte[] _DataBytes = fc.ChunkBuffer;
            int _DataLength = fc.Length;
            //string _Parameters = fc.Params;
            bool _FirstChunk = fc.FirstChunk;
            bool _LastChunk = fc.LastChunk;

            string _UploadFolder = this.GetFileFolder();

            string _FilePath = _UploadFolder + "/" + _FileName;
            string _TempFilePath = _FilePath + m_TempExtension;

            try
            {
                if (_FirstChunk)
                {
                    // Delete temp file
                    if (File.Exists(_TempFilePath))
                        File.Delete(_TempFilePath);

                    // Delete target file
                    if (File.Exists(_FilePath))
                        File.Delete(_FilePath);
                }

                FileStream fs = File.Open(_TempFilePath, FileMode.Append);
                fs.Write(_DataBytes, 0, _DataLength);
                fs.Close();

                if (_LastChunk)
                {
                    //Rename file to original file
                    File.Move(_TempFilePath, _FilePath);

                    //Finish stuff....
                }

                result.Result = true;
                if (_FirstChunk)
                {
                    _FilePath = "/" + m_UploadFolder;
                    result.ReturnValue.Add(new LookupDataItem() { K = "ImageName", V = _FileName });
                    result.ReturnValue.Add(new LookupDataItem() { K = "ImagePath", V = _FilePath });
                }
            }
            catch
            {
                result.Message = "Save File Failed.";
            }
            return result;
        }

        #endregion

        #region ** Dowmload File

        //string _FilePath = HttpContext.Current.Server.MapPath(_FileName);
        public FileChunk DownloadFileAsync(FileChunk fc)
        {
            string _FileName = fc.FileName;
            string _UploadFolder = this.GetFileFolder();
            string _FilePath = _UploadFolder + "/" + _FileName;

            if (!File.Exists(_FilePath))
                return null;

            int _DataDownedLength = fc.Length;
            bool _LastChunk = false;


            FileStream fs = File.OpenRead(_FilePath);
            int iLength = 128 * 1024;

            byte[] _DataBytes = new byte[iLength];

            fs.Position = _DataDownedLength;
            int bytesRead = fs.Read(_DataBytes, 0, iLength);
            if ((_DataDownedLength + bytesRead) == fs.Length)
                _LastChunk = true;
            fs.Close();

            FileChunk result = new FileChunk()
            {
                FileName = _FileName,
                ChunkBuffer = _DataBytes,
                LastChunk = _LastChunk,
                Length = iLength
            };

            return result;
        }

        #endregion

        private string GetFileFolder()
        {
            string localPath = @HostingEnvironment.ApplicationPhysicalPath + "/" + m_UploadFolder;

            if (!Directory.Exists(localPath))
                Directory.CreateDirectory(localPath);

            return localPath;
        }

        private string GetFileName()
        {
            string sFileName = DESEncrypt.NewGuid();
            return sFileName;
        }
    }
}
