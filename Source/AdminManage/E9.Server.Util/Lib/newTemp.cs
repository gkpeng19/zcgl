using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Runtime.InteropServices;
//using Regdll;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NM.Model;

namespace NM.Lib
{

    public class CheckResult
    {
        public bool isok { get; set; }
        public string Message { get; set; }
    }

    [Serializable]
    public struct HardDiskInfo
    {
        /// <summary> 
        /// 型号 
        /// </summary> 
        public string ModuleNumber;
        /// <summary> 
        /// 固件版本 
        /// </summary> 
        public string Firmware;
        /// <summary> 
        /// 序列号 
        /// </summary> 
        public string SerialNumber;
        /// <summary> 
        /// 容量，以M为单位 
        /// </summary> 
        public uint Capacity;
    }

    #region Internal Structs

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct GetVersionOutParams
    {
        public byte bVersion;
        public byte bRevision;
        public byte bReserved;
        public byte bIDEDeviceMap;
        public uint fCapabilities;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] dwReserved; // For future use. 
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IdeRegs
    {
        public byte bFeaturesReg;
        public byte bSectorCountReg;
        public byte bSectorNumberReg;
        public byte bCylLowReg;
        public byte bCylHighReg;
        public byte bDriveHeadReg;
        public byte bCommandReg;
        public byte bReserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SendCmdInParams
    {
        public uint cBufferSize;
        public IdeRegs irDriveRegs;
        public byte bDriveNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] bReserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] dwReserved;
        public byte bBuffer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DriverStatus
    {
        public byte bDriverError;
        public byte bIDEStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] dwReserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SendCmdOutParams
    {
        public uint cBufferSize;
        public DriverStatus DriverStatus;
        public IdSector bBuffer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
    internal struct IdSector
    {
        public ushort wGenConfig;
        public ushort wNumCyls;
        public ushort wReserved;
        public ushort wNumHeads;
        public ushort wBytesPerTrack;
        public ushort wBytesPerSector;
        public ushort wSectorsPerTrack;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public ushort[] wVendorUnique;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] sSerialNumber;
        public ushort wBufferType;
        public ushort wBufferSize;
        public ushort wECCSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] sFirmwareRev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] sModelNumber;
        public ushort wMoreVendorUnique;
        public ushort wDoubleWordIO;
        public ushort wCapabilities;
        public ushort wReserved1;
        public ushort wPIOTiming;
        public ushort wDMATiming;
        public ushort wBS;
        public ushort wNumCurrentCyls;
        public ushort wNumCurrentHeads;
        public ushort wNumCurrentSectorsPerTrack;
        public uint ulCurrentSectorCapacity;
        public ushort wMultSectorStuff;
        public uint ulTotalAddressableSectors;
        public ushort wSingleWordDMA;
        public ushort wMultiWordDMA;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] bReserved;
    }

    #endregion

    /// <summary> 
    /// ATAPI驱动器相关 
    /// </summary> 
    public class AtapiDevice
    {

        #region DllImport

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr lpSecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            IntPtr lpInBuffer,
            uint nInBufferSize,
            ref GetVersionOutParams lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            [Out] IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            ref SendCmdInParams lpInBuffer,
            uint nInBufferSize,
            ref SendCmdOutParams lpOutBuffer,
            uint nOutBufferSize,
            ref uint lpBytesReturned,
            [Out] IntPtr lpOverlapped);

        const uint DFP_GET_VERSION = 0x00074080;
        const uint DFP_SEND_DRIVE_COMMAND = 0x0007c084;
        const uint DFP_RECEIVE_DRIVE_DATA = 0x0007c088;

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint CREATE_NEW = 1;
        const uint OPEN_EXISTING = 3;

        #endregion

        #region GetHddInfo

        /// <summary> 
        /// 获得硬盘信息 
        /// </summary> 
        /// <param name="driveIndex">硬盘序号</param> 
        /// <returns>硬盘信息</returns> 
        /// <remarks> 
        /// 参考lu0的文章：http://lu0s1.3322.org/App/2k1103.html 
        /// by sunmast for everyone 
        /// thanks lu0 for his great works 
        /// 在Windows 98/ME中，S.M.A.R.T并不缺省安装，请将SMARTVSD.VXD拷贝到%SYSTEM%\IOSUBSYS目录下。 
        /// 在Windows 2000/2003下，需要Administrators组的权限。 
        /// </remarks> 
        /// <example> 
        /// AtapiDevice.GetHddInfo() 
        /// </example> 
        public static HardDiskInfo GetHddInfo(byte driveIndex)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                    return GetHddInfo9x(driveIndex);
                case PlatformID.Win32NT:
                    return GetHddInfoNT(driveIndex);
                case PlatformID.Win32S:
                    throw new NotSupportedException("Win32s is not supported.");
                case PlatformID.WinCE:
                    throw new NotSupportedException("WinCE is not supported.");
                default:
                    throw new NotSupportedException("Unknown Platform.");
            }
        }

        #region GetHddInfo9x

        private static HardDiskInfo GetHddInfo9x(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;

            IntPtr hDevice = CreateFile(
                @"\\.\Smartvsd",
                0,
                0,
                IntPtr.Zero,
                CREATE_NEW,
                0,
                IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
            {
                throw new Exception("Open smartvsd.vxd failed.");
            }
            if (0 == DeviceIoControl(
                hDevice,
                DFP_GET_VERSION,
                IntPtr.Zero,
                0,
                ref vers,
                (uint)Marshal.SizeOf(vers),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed:DFP_GET_VERSION");
            }
            // If IDE identify command not supported, fails 
            if (0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new Exception("Error: IDE identify command not supported.");
            }
            if (0 != (driveIndex & 1))
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            }
            else
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            }
            if (0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                // We don't detect a ATAPI device. 
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don't detect it", driveIndex + 1));
            }
            else
            {
                inParam.irDriveRegs.bCommandReg = 0xec;
            }
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;
            if (0 == DeviceIoControl(
                hDevice,
                DFP_RECEIVE_DRIVE_DATA,
                ref inParam,
                (uint)Marshal.SizeOf(inParam),
                ref outParam,
                (uint)Marshal.SizeOf(outParam),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            CloseHandle(hDevice);

            return GetHardDiskInfo(outParam.bBuffer);
        }

        #endregion

        #region GetHddInfoNT

        private static HardDiskInfo GetHddInfoNT(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;

            // We start in NT/Win2000 
            IntPtr hDevice = CreateFile(
                string.Format(@"\\.\PhysicalDrive{0}", driveIndex),
                GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
            {
                throw new Exception("CreateFile faild.");
            }
            if (0 == DeviceIoControl(
                hDevice,
                DFP_GET_VERSION,
                IntPtr.Zero,
                0,
                ref vers,
                (uint)Marshal.SizeOf(vers),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} may not exists.", driveIndex + 1));
            }
            // If IDE identify command not supported, fails 
            if (0 == (vers.fCapabilities & 1))
            {
                CloseHandle(hDevice);
                throw new Exception("Error: IDE identify command not supported.");
            }
            // Identify the IDE drives 
            if (0 != (driveIndex & 1))
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            }
            else
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            }
            if (0 != (vers.fCapabilities & (16 >> driveIndex)))
            {
                // We don't detect a ATAPI device. 
                CloseHandle(hDevice);
                throw new Exception(string.Format("Drive {0} is a ATAPI device, we don't detect it.", driveIndex + 1));
            }
            else
            {
                inParam.irDriveRegs.bCommandReg = 0xec;
            }
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;

            if (0 == DeviceIoControl(
                hDevice,
                DFP_RECEIVE_DRIVE_DATA,
                ref inParam,
                (uint)Marshal.SizeOf(inParam),
                ref outParam,
                (uint)Marshal.SizeOf(outParam),
                ref bytesReturned,
                IntPtr.Zero))
            {
                CloseHandle(hDevice);
                throw new Exception("DeviceIoControl failed: DFP_RECEIVE_DRIVE_DATA");
            }
            CloseHandle(hDevice);

            return GetHardDiskInfo(outParam.bBuffer);
        }

        #endregion

        private static HardDiskInfo GetHardDiskInfo(IdSector phdinfo)
        {
            HardDiskInfo hddInfo = new HardDiskInfo();

            ChangeByteOrder(phdinfo.sModelNumber);
            hddInfo.ModuleNumber = Encoding.ASCII.GetString(phdinfo.sModelNumber).Trim();

            ChangeByteOrder(phdinfo.sFirmwareRev);
            hddInfo.Firmware = Encoding.ASCII.GetString(phdinfo.sFirmwareRev).Trim();

            ChangeByteOrder(phdinfo.sSerialNumber);
            hddInfo.SerialNumber = Encoding.ASCII.GetString(phdinfo.sSerialNumber).Trim();

            hddInfo.Capacity = phdinfo.ulTotalAddressableSectors / 2 / 1024;

            return hddInfo;
        }

        private static void ChangeByteOrder(byte[] charArray)
        {
            byte temp;
            for (int i = 0; i < charArray.Length; i += 2)
            {
                temp = charArray[i];
                charArray[i] = charArray[i + 1];
                charArray[i + 1] = temp;
            }
        }

        #endregion
    }


    /// <summary>
    /// 序列化对象使用
    /// </summary>
    public class SerializeObjManager
    {
        public SerializeObjManager()
        { }

        /// <summary>
        /// 序列化 对象到字符串
        /// </summary>
        /// <param name="obj">泛型对象</param>
        /// <returns>序列化后的字符串</returns>
        public static string Serialize<T>(T obj)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, obj);
                stream.Position = 0;
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                stream.Flush();
                stream.Close();
                return Convert.ToBase64String(buffer);
            }
            catch (Exception ex)
            {
                throw new Exception("序列化失败,原因:" + ex.Message);
            }
        }

        /// <summary>
        /// 反序列化 字符串到对象
        /// </summary>
        /// <param name="obj">泛型对象</param>
        /// <param name="str">要转换为对象的字符串</param>
        /// <returns>反序列化出来的对象</returns>
        public static T Desrialize<T>(T obj, string str)
        {
            try
            {
                obj = default(T);
                IFormatter formatter = new BinaryFormatter();
                byte[] buffer = Convert.FromBase64String(str);
                MemoryStream stream = new MemoryStream(buffer);
                obj = (T)formatter.Deserialize(stream);
                stream.Flush();
                stream.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("反序列化失败,原因:" + ex.Message);
            }
            return obj;
        }
    }


    public class astatic
    {
        public static Dictionary<int, bool> DicOrgReg = new Dictionary<int, bool>();
        /// <summary>
        /// </summary>
        /// <param name="name">注册用户名称</param>
        /// <param name="p">过期时间</param>
        /// <returns></returns>
        public static string GenFile(string name, string p)
        {
            //HardDiskInfo h = AtapiDevice.GetHddInfo((byte)0);

            //c1 obj = new c1();
            //obj.n = name;
            //obj.x = "netwms";// sys;
            //obj.d = "v1"; //newTemp.EncryptString(id);
            //obj.e = p;

            string pp = "";// SerializeObjManager.Serialize<c1>(obj);

            string jimi = newTemp.EncryptString(pp);

            return jimi;
            //string sfilename = AppDomain.CurrentDomain.BaseDirectory + "reg.nmg";
            //StreamWriter sw = new StreamWriter(sfilename);
            //try
            //{

            //    sw.Write(jimi);
            //}
            //catch { }
            //finally
            //{
            //    sw.Close();
            //}
        }

        //static c1 GetInfo(string sfile)
        //{
        //    c1 c = null;
        //    try
        //    {
        //        // if (File.Exists(sfile))
        //        // {
        //        string mm = sfile;// File.ReadAllText(sfile);
        //        if (string.IsNullOrEmpty(mm))
        //        {
        //            return c;
        //        }
        //        string mmjiemi = newTemp.DecryptString(mm);
        //        c = SerializeObjManager.Desrialize<c1>(c, mmjiemi);

        //        return c;

        //        // }
        //    }
        //    catch
        //    { }



        //    return c;
        //}

        public static CheckResult checkreg(string RegInfo, string Customer)
        {
            CheckResult r = new CheckResult();
            r.isok = false;
            r.Message = "";
            if (string.IsNullOrEmpty(RegInfo))
            {
                r.Message = "软件尚未注册，请先注册。";
            }

            // HardDiskInfo h = AtapiDevice.GetHddInfo((byte)0);
            string ser = "";// h.SerialNumber;
            //label1.Text = "型号" + h.ModuleNumber;
            //label2.Text = "固件版本" + h.Firmware;
            //label3.Text = "序列号" + 
            //label4.Text = "容量" + h.Capacity.ToString();
            /*
            c1 c = GetInfo(RegInfo);

            if (c == null)
            {
                r.Message = "软件尚未注册，请先注册。";
            }
            else
            {
                #region
                // if ((newTemp.EncryptString(h.SerialNumber) == c.d) && (Customer == c.n))
                if (Customer == c.n)
                {
                    r.isok = true;
                    if (c.e == "forever")
                    {
                        r.Message = "正式版";
                    }
                    else
                    {
                        r.Message = "许可到期时间：" + c.e;
                        DateTime dt = DateTime.Now;
                        bool bl = DateTime.TryParse(c.e, out dt);
                        if (bl)
                        {
                            if (dt < DateTime.Now)
                            {
                                r.isok = false;
                                r.Message = "许可已到期。到期时间：" + c.e;
                            }
                        }
                        else
                        {
                            r.isok = false;
                            r.Message = "许可错误，请联系系统供应商。";
                        }
                    }

                }
                else
                {
                    r.Message = "注册信息非法。";

                }
                #endregion

            }
            */
            return r;

        }

    }
    public class newTemp
    {
        static string _StringKey = "abcdefgh"; //默认的加密key


        public newTemp()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 使用默认的key加密字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncryptString(string str)
        {
            return Encrypt(str, _StringKey);
        }

        /// <summary>
        /// 使用默认的key解密字符
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DecryptString(string str)
        {
 
             return Decrypt(str, _StringKey);
        }

        /// <summary>
        /// DEC 加密过程,根据指定的key加密字符
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>
        public static string Encrypt(string pToEncrypt, string sKey)
        {
            
            DESCryptoServiceProvider des = new DESCryptoServiceProvider(); //把字符串放到byte数组中 

            byte[] inputByteArray = Encoding.Unicode.GetBytes(pToEncrypt);
            //byte[] inputByteArray=Encoding.Unicode.GetBytes(pToEncrypt); 

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey); //建立加密对象的密钥和偏移量
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey); //原文使用ASCIIEncoding.ASCII方法的GetBytes方法 
            MemoryStream ms = new MemoryStream(); //使得输入密码必须输入英文文本
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }

        /// <summary>
        /// DEC 解密过程,根据指定的key解密字符
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <param name="sKey"></param>
        /// <returns></returns>

        public static string Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();

            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }

            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey); //建立加密对象的密钥和偏移量，此值重要，不能修改 
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder(); //建立StringBuild对象，CreateDecrypt使用的是流对象，必须把解密后的文本变成流对象 

            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }


    }
}
