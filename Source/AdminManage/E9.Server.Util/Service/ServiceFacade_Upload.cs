using NM.Service;
using NM.Util;
using NM.Model;
using System;
using System.IO;

namespace NM.OP
{
    [ServiceFacadeAttribute("System")]
    public class ServiceFacade_Upload : ServiceFacadeBase
    {
        [Service("UploadTextFile")]
        public void UploadTextFile(ServiceContext context)
        {
            CommandResult returnCommand = new CommandResult();
            UploadMeta meta = TJson.Parse<UploadMeta>(context.E9_Request["P0"]);

            string dataPath = context.HttpContext.Server.MapPath("~/") + "data/";
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string fileName = dataPath + meta.FileName;
            if (meta.Index == 0)
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }

            using (FileStream stream = new FileStream(fileName, FileMode.Append))
            {
                StreamWriter writer = new StreamWriter(stream);
                if (meta.IsBinery)
                {
                    var bits = meta.GetBinary();
                    stream.Write(bits, 0, bits.Length);
                    //writer.Write(meta.GetBinary());
                }
                else
                    writer.Write(meta.Content);
                writer.Flush();
                stream.Flush();
                writer.Close();
                stream.Close();
            }
            // if (meta.Index == meta.PackCount - 1)
            {
                returnCommand.Message = fileName;
            }
            returnCommand.Result = true;

            context.E9_Response.Value = ToJson(returnCommand);

            /*
            CommandResult returnCommand = new CommandResult();
            UploadMeta meta = TJson.Parse<UploadMeta>(context.E9_Request["P0"]);

            string dataPath = context.HttpContext.Server.MapPath("~/") + "data/";
            if (!Directory.Exists(dataPath))
            {
                Directory.CreateDirectory(dataPath);
            }
            string fileName = dataPath + meta.FileName;
            if (meta.Index == 0)
            {
                if (File.Exists(fileName))
                    File.Delete(fileName);
            }
            
            using (FileStream stream = new FileStream(fileName, FileMode.Append))
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(meta.Content);
                writer.Flush();
                stream.Flush();
                writer.Close();
                stream.Close();
            }
            if (meta.Index == meta.PackCount - 1)
            {
                returnCommand.Message = fileName;              
            }
            returnCommand.Result = true;
           
            context.E9_Response.Value = ToJson(returnCommand);
            */
        }

        [Service("UploadFileAsync")]
        public void UploadFileAsync(DataRequest request, DataResponse response, DataProvider dataprovider)
        {
            FileChunk fc = TJson.Parse<FileChunk>(request["P0"]);
            response.Value = ToJson(new FileZillaOP(request.LogIn, dataprovider).UploadFileAsync(fc));
        }

        [Service("DownloadFileAsync")]
        public void DownloadFileAsync(DataRequest request, DataResponse response, DataProvider dataprovider)
        {
            FileChunk fc = TJson.Parse<FileChunk>(request["P0"]);
            response.Value = ToJson(new FileZillaOP(request.LogIn, dataprovider).DownloadFileAsync(fc));
        }
    }
}
