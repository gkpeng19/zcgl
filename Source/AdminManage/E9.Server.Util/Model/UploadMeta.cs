using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Util;

namespace NM.Model
{
    public class UploadMeta : TJson
    {
        public UploadMeta()
        {
        }
        public string FileName { get; set; }

        public int PackCount { get; set; }

        public int Index { get; set; }

        public string Content { get; set; }

        public bool IsBinery { get; set; }

        public void SetBinary(byte[] bin)
        {
            Content = Encoding.UTF8.GetString(bin, 0, bin.Length);
        }

        public byte[] GetBinary()
        {
            return Encoding.UTF8.GetBytes(Content);
        }
    }
}
