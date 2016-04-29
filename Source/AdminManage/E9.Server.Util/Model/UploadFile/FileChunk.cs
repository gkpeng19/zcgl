using NM.Util;

namespace NM.Model
{
    public class FileChunk : TJson
    {
        public FileChunk() { }

        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] ChunkBuffer { get; set; }
        public int Length { get; set; }
        public string Params { get; set; }
        public bool FirstChunk { get; set; }
        public bool LastChunk { get; set; }
    }
}
