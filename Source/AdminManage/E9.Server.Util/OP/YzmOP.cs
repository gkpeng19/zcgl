using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NM.Model;
using System.Web.SessionState;
using System.Web;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace NM.OP
{
   public class YzmOP :IRequiresSessionState
   {
       string _rancode = string.Empty;
       /// <summary>
       /// 生成验证码，存入Session，索引为YZM。
       /// </summary>
       /// <returns></returns>
        public CommandResult GenerateYZM()
        {
            CommandResult result = new CommandResult();
            _rancode = GetRanCode();
            HttpContext.Current.Session["YZM"] = _rancode;
            result.Message = _rancode;
            return result;
        }

       /// <summary>
       /// 得到设定位数的随机数，未存入session
       /// </summary>
       /// <param name="length"></param>
       /// <returns></returns>
        public static string GetRanCode(int length = 4)
        {
            string YzmCode = string.Empty;
            string[] YZMResource = ConfigurationManager.AppSettings["YZM"].Split(',');
            Random ran = new Random();
            for (int i = 0; i < length; i++)
            {
                YzmCode += YZMResource[ran.Next(YZMResource.Length)];
            }
            return YzmCode;
        }

       /// <summary>
       /// 根据GenerateYZM方法生成的随机数，生成验证码图片
       /// </summary>
       /// <returns></returns>
        public Stream CreateYZM()
        {
            if (string.IsNullOrEmpty(_rancode))
            { 
                return null;
            }
            Matrix m = new Matrix();//定义几何变换
            Bitmap charbmp = new Bitmap(90, 30);//图片前景色，即生成背景透明的随机字符串图片

            //定义字体
            Font[] fonts = {
                                       new Font(new FontFamily("Times New Roman"), 17, FontStyle.Regular),
                                       new Font(new FontFamily("Georgia"), 17, FontStyle.Regular),
                                       new Font(new FontFamily("Arial"), 17, FontStyle.Regular),
                                       new Font(new FontFamily("Comic Sans MS"), 17, FontStyle.Regular)
                                    };


            //定义图片背景色
            System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((_rancode.Length * 22.5)), 30);

            //开始描绘
            Graphics g = Graphics.FromImage(image);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;

            //定义背景色为白色
            g.Clear(Color.White);
            try
            {
                Random random = new Random();       //生成随机生成器
                g.Clear(Color.White);               //清空图片背景色
                for (int i = 0; i < 2; i++)              //画图片的背景噪音线，i表示画多少条噪音线
                {
                    int x1 = random.Next(image.Width);
                    int x2 = random.Next(image.Width);
                    int y1 = random.Next(image.Height);
                    int y2 = random.Next(image.Height);
                    g.DrawLine(new Pen(Color.Black), x1, y1, x2, y2);
                }



                //开始描绘前景图
                Graphics charg = Graphics.FromImage(charbmp);

                SolidBrush drawBrush = new SolidBrush(Color.FromArgb(random.Next(101), random.Next(101), random.Next(101)));
                float charx = -18;

                //把随机字符串，逐个写入前景图
                for (int i = 0; i < _rancode.Length; i++)
                {
                    m.Reset();
                    m.RotateAt(random.Next(31) - 25, new PointF(random.Next(4) + 7, random.Next(4) + 7));

                    charg.Clear(Color.Transparent);//定义前景图为透明
                    charg.Transform = m;
                    //定义前景色为黑色
                    drawBrush.Color = Color.Black;

                    charx = charx + 20 + random.Next(3);
                    PointF drawPoint = new PointF(charx, 0.1F);
                    charg.DrawString(_rancode[i].ToString(), fonts[random.Next(fonts.Length)], drawBrush, new PointF(0, 0));//通过特定的几何变换，旋转或变形随机字符，写入前景图

                    charg.ResetTransform();

                    g.DrawImage(charbmp, drawPoint);
                }


                //画图片的前景噪音点
                for (int i = 0; i < 25; i++)
                {
                    int x = random.Next(image.Width);
                    int y = random.Next(image.Height);

                    image.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                //画图片的边框线
                g.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);

                //保存
                Stream stream = new MemoryStream();
                image.Save(stream, ImageFormat.Bmp);
                return stream;
            }
            finally
            {
                g.Dispose();
                image.Dispose();
            }
        }
    }
}
