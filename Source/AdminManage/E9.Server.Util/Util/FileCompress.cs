using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
namespace NM.Util
{
    public class FileCompress
    {
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="path">该帐套数据存放的文价夹的路径</param>
        public static string Compress(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new Exception("要压缩的文件夹路径不存在!");
            }
            string zipFileName=path+".kct";
            ZipDirectory(path, zipFileName);
            return zipFileName;
        }

        /// <summary>
        /// 只能解压压缩文件中的文件
        /// </summary>
        /// <param name="path">解压到文件路径</param>
        /// <param name="zipFilePath">要解压的压缩文件路径</param>
        public static void Decompress(string path, string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                throw new Exception("指定的压缩文件不存在!");
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            UnZipFile(path, zipFilePath);
        }

        /// <summary>
        /// 该文件夹下的子文件夹不会被压缩
        /// </summary>
        /// <param name="path">文件夹的路径</param>
        /// <param name="zipFilePath">压缩文件的路径</param>
        static void ZipDirectory(string path,string zipFilePath)
        {
            using (ZipOutputStream u = new ZipOutputStream(File.Create(zipFilePath)))
            {
                u.Password = "KCT_Soft";
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var item in di.GetFiles())
                {
                    ZipEntry ze = new ZipEntry(item.Name);
                    u.PutNextEntry(ze);
                    using (FileStream fs = File.OpenRead(item.FullName))
                    {
                        fs.CopyTo(u);
                    }
                }
                u.Finish();
                u.Close();
            }
        }

        /// <summary>
        /// 只能解压压缩文件中的文件
        /// </summary>
        /// <param name="path">解压到文件路径</param>
        /// <param name="zipFilePath">要解压的压缩文件路径</param>
        static void UnZipFile(string path, string zipFilePath)
        {
            using (ZipInputStream u = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                u.Password = "KCT_Soft";
                ZipEntry ze=null;
                while((ze= u.GetNextEntry())!=null)
                {
                    string filePath=path+"\\"+ze.Name;
                    if (ze.IsFile)
                    {
                        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
                        {
                            u.CopyTo(fs);
                        }
                    }
                }
            }
        }

        #region 参考

        //public void UnZipFile(string p)   //解压缩
        //{
        //    string[] un_tmp = p.Split(new char[]{'*'});
        //    int i2=0;  //防止名称冲突的参数
        //    for(int j=0;j<un_tmp.Length;j++)
        //    {
        //            if(un_tmp[j]!="")
        //            {
        //                    string un_time=System.DateTime.Now.ToShortDateString()+"-"+System.DateTime.Now.Hour.ToString()+"-"+System.DateTime.Now.Minute.ToString()+"-"+(System.DateTime.Now.Second+i2).ToString();
        //                    string un_dir =ServerDir+"Unzip-"+un_time;
        //                    Directory.CreateDirectory(un_dir);    //创建以解压时间为名称的文件夹
        //                    ZipInputStream f = new ZipInputStream(File.OpenRead(ServerDir+un_tmp[j])); //读取压缩文件，并用此文件流新建 “ZipInputStream”对象

        //                    A:  ZipEntry zp = f.GetNextEntry();   //获取解压文件流中的项目。 另注（我的理解）：在压缩包里每个文件都以“ZipEntry”形式存在，其中包括存放文件的目录信息。如果空目录被压缩，该目录下将出现一个名称为空、大小为 0 、“Crc”属性为 00000000 的“文件”。此文件只是个标记，不会被解压。

        //                    while(zp!=null)
        //                    {
        //                        string un_tmp2;
        //                        if(zp.Name.IndexOf("//")>=0) //获取文件的目录信息
        //                        {
        //                                    int tmp1 = zp.Name.LastIndexOf("//");
        //                                    un_tmp2 = zp.Name.Substring(0,tmp1);
        //                                    Directory.CreateDirectory(un_dir+"//"+un_tmp2+"//"); //必须先创建目录，否则解压失败 --- （A） 关系到下面的步骤（B）
        //                        }
        //                            if(!zp.IsDirectory&&zp.Crc!=00000000L) //此“ZipEntry”不是“标记文件”
        //                            {
        //                                    int i =2048;
        //                                    byte[] b = new byte[i];  //每次缓冲 2048 字节
        //                                    FileStream s= File.Create(un_dir+"//"+zp.Name); //（B)-新建文件流
        //                                    while(true) //持续读取字节，直到一个“ZipEntry”字节读完
        //                                    {
        //                                        i = f.Read(b,0,b.Length); //读取“ZipEntry”中的字节
        //                                            if(i>0)
        //                                            {
        //                                                    s.Write(b,0,i); //将字节写入新建的文件流
        //                                            }
        //                                            else
        //                                            {
        //                                                    break; //读取的字节为 0 ，跳出循环
        //                                            }
        //                                    }
        //                                    s.Close();
        //                            }
        //                            goto A; //进入下一个“ZipEntry”
        //                        }
        //                        f.Close();
        //                        i2++;
        //            }
        //    }
        //}

        #endregion
        #region 参考

        //添加压缩项目：p 为需压缩的文件或文件夹； u 为现有的源ZipOutputStream；  out j为已添加“ZipEntry”的“ZipOutputStream”
        //public void AddZipEntry(string p, ZipOutputStream u, out ZipOutputStream j)
        //{
        //    string s = ServerDir + p;

        //    if (Directory.Exists(s))  //文件夹的处理
        //    {
        //        DirectoryInfo di = new DirectoryInfo(s);

        //        //***********以下内容是修订后添加的***********

        //        if (di.GetDirectories().Length <= 0)   //没有子目录
        //        {
        //            ZipEntry z = new ZipEntry(p + "//"); //末尾“//”用于文件夹的标记
        //            u.PutNextEntry(z);
        //        }

        //        //***************以上内容是修订后添加的***************


        //        foreach (DirectoryInfo tem in di.GetDirectories())  //获取子目录
        //        {
        //            ZipEntry z = new ZipEntry(this.ShortDir(tem.FullName) + "//"); //末尾“//”用于文件夹的标记
        //            u.PutNextEntry(z);    //此句不可少，否则空目录不会被添加
        //            s = this.ShortDir(tem.FullName);
        //            this.AddZipEntry(s, u, out u);       //递归
        //        }
        //        foreach (FileInfo temp in di.GetFiles())  //获取此目录的文件
        //        {
        //            s = this.ShortDir(temp.FullName);
        //            this.AddZipEntry(s, u, out u);      //递归
        //        }
        //    }
        //    else if (File.Exists(s))  //文件的处理
        //    {
        //        u.SetLevel(9);      //压缩等级
        //        FileStream f = File.OpenRead(s);
        //        byte[] b = new byte[f.Length];
        //        f.Read(b, 0, b.Length);          //将文件流加入缓冲字节中
        //        ZipEntry z = new ZipEntry(this.ShortDir(s));
        //        u.PutNextEntry(z);             //为压缩文件流提供一个容器
        //        u.Write(b, 0, b.Length);  //写入字节
        //        f.Close();
        //    }
        //    j = u;    //返回已添加数据的“ZipOutputStream”
        //}

        #endregion
    }
}
