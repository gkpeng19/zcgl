
/// <Author>  shuagnfu </Author>   
/// <CreateDate> 2007-6-15 17:24:19  </CreateDate>
 /// <summary>  
///  Lib_String.cs
 /// <summary>  
/// <Update>2007-6-15 17:30:48</Update> 
/// <remarks> </remarks>

using System;
using System.Collections.Generic;
using System.Text;

namespace NM.Util
{
	public static class Lib_String
	{
        public static string Montage<T>(this IEnumerable<T> source, Func<T, string> toString, string splitter)
        {

            StringBuilder result = new StringBuilder();

            splitter = splitter ?? string.Empty;

            foreach (T item in source)
            { 
                result.Append(toString(item));  
                result.Append(splitter); 
            }

            string resultStr = result.ToString();

            if (resultStr.EndsWith(splitter))

                resultStr = resultStr.Remove(resultStr.Length - splitter.Length, splitter.Length);

            return resultStr; 
        } 


		public static string ConvertToChineseNum(double number)
		{
			string numList = "零壹贰叁肆伍陆柒捌玖";
			string rmbList = "分角元拾佰仟万拾佰仟亿拾佰仟万";
			string tempOutString = null;


			//     if (number > 9999999999999.99)           this.noteMessage = "超出范围的人民币值";

			//将小数转化为整数字符串 
			string tempNumberString = Convert.ToInt64(number * 100).ToString();
			int tempNmberLength = tempNumberString.Length;
			int i = 0;
			while (i < tempNmberLength)
			{
				int oneNumber = Int32.Parse(tempNumberString.Substring(i, 1));
				string oneNumberChar = numList.Substring(oneNumber, 1);
				string oneNumberUnit = rmbList.Substring(tempNmberLength - i - 1, 1);
				if (oneNumberChar != "零")
					tempOutString += oneNumberChar + oneNumberUnit;
				else
				{
					if (oneNumberUnit == "亿" || oneNumberUnit == "万" || oneNumberUnit == "元" || oneNumberUnit == "零")
					{
						while (tempOutString.EndsWith("零"))
						{
							tempOutString = tempOutString.Substring(0, tempOutString.Length - 1);
						}

					}
					if (oneNumberUnit == "亿" || (oneNumberUnit == "万" && !tempOutString.EndsWith("亿")) || oneNumberUnit == "元")
					{
						tempOutString += oneNumberUnit;
					}
					else
					{
						bool tempEnd = tempOutString.EndsWith("亿");
						bool zeroEnd = tempOutString.EndsWith("零");
						if (tempOutString.Length > 1)
						{
							bool zeroStart = tempOutString.Substring(tempOutString.Length - 2, 2).StartsWith("零");
							if (!zeroEnd && (zeroStart || !tempEnd))
								tempOutString += oneNumberChar;
						}
						else
						{
							if (!zeroEnd && !tempEnd)
								tempOutString += oneNumberChar;
						}
					}
				}
				i += 1;
			}

			while (tempOutString.EndsWith("零"))
			{
				tempOutString = tempOutString.Substring(0, tempOutString.Length - 1);
			}

			while (tempOutString.EndsWith("元"))
			{
				tempOutString = tempOutString + "整";
			}

			return tempOutString;
		}

		/// <summary>
		/// 对用户权限数组按照角色-操作-操作对象的顺序进行排序
		/// </summary>
		/// lbq add 06-03-20
		/// <param name="inArray">排序前的字符串数组</param>
		/// <returns>排序后的字符串数组</returns>
		public static string[] ArraySort(string[] inArray)
		{
			string hold = "";
			for (int pass = 1; pass <= inArray.Length - 1; pass++)
			{
				int i = 0;
				foreach (string value in inArray)
				{
					if (string.IsNullOrEmpty(value))
						continue;
					// string value = inArray[i];
					string compareValue = inArray[i + 1];
					string ValuerightType = value.Substring(0, 1);
					if (!string.IsNullOrEmpty(compareValue)
					   && ((compareValue.Substring(0, 1) == "R"
					   && (ValuerightType == "+"
					   || ValuerightType == "-"
					   || ValuerightType == "P"))
					   || ((compareValue.Substring(0, 1) == "+"
					   || compareValue.Substring(0, 1) == "-")
					   && ValuerightType == "P")))
					{
						hold = value;
						inArray[i] = inArray[i + 1];
						inArray[i + 1] = hold;
					}
					i++;
				}
			}
			return inArray;
		}

		private static string[] ones_numerals = new string[] { "", "Ⅰ", "Ⅱ", "Ⅲ", "Ⅳ", "Ⅴ", "Ⅵ", "Ⅶ", "Ⅷ", "Ⅸ" ,
															   "", "Ⅹ", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" ,
															   "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" , 
															   "", "M", "MM", "MM" };

		public static string ConvertToRomaNum(int number)
		{
			int n = number;
			string value = number.ToString();
			int index = value.Length-1;
			string result = "";
			foreach (char c in value)
			{
				result += ones_numerals[index-- * 10 + (c - '0')];
			}
			return result;
		}
	}
    /// <summary>
    /// ChsToSpellConverter 将汉字转化为拼音。
    /// 原理: 先将汉字转化成为内码，然后通过内码和拼音的对照来查找。
    /// 2005.11.30
    /// </summary>
    public class ChsToSpellConverter
    {
        #region constArray
        private static int[] pyvalue = new int[]{-20319,-20317,-20304,-20295,-20292,-20283,-20265,-20257,-20242,-20230,-20051,-20036,-20032,-20026,
												  -20002,-19990,-19986,-19982,-19976,-19805,-19784,-19775,-19774,-19763,-19756,-19751,-19746,-19741,-19739,-19728,
												  -19725,-19715,-19540,-19531,-19525,-19515,-19500,-19484,-19479,-19467,-19289,-19288,-19281,-19275,-19270,-19263,
												  -19261,-19249,-19243,-19242,-19238,-19235,-19227,-19224,-19218,-19212,-19038,-19023,-19018,-19006,-19003,-18996,
												  -18977,-18961,-18952,-18783,-18774,-18773,-18763,-18756,-18741,-18735,-18731,-18722,-18710,-18697,-18696,-18526,
												  -18518,-18501,-18490,-18478,-18463,-18448,-18447,-18446,-18239,-18237,-18231,-18220,-18211,-18201,-18184,-18183,
												  -18181,-18012,-17997,-17988,-17970,-17964,-17961,-17950,-17947,-17931,-17928,-17922,-17759,-17752,-17733,-17730,
												  -17721,-17703,-17701,-17697,-17692,-17683,-17676,-17496,-17487,-17482,-17468,-17454,-17433,-17427,-17417,-17202,
												  -17185,-16983,-16970,-16942,-16915,-16733,-16708,-16706,-16689,-16664,-16657,-16647,-16474,-16470,-16465,-16459,
												  -16452,-16448,-16433,-16429,-16427,-16423,-16419,-16412,-16407,-16403,-16401,-16393,-16220,-16216,-16212,-16205,
												  -16202,-16187,-16180,-16171,-16169,-16158,-16155,-15959,-15958,-15944,-15933,-15920,-15915,-15903,-15889,-15878,
												  -15707,-15701,-15681,-15667,-15661,-15659,-15652,-15640,-15631,-15625,-15454,-15448,-15436,-15435,-15419,-15416,
												  -15408,-15394,-15385,-15377,-15375,-15369,-15363,-15362,-15183,-15180,-15165,-15158,-15153,-15150,-15149,-15144,
												  -15143,-15141,-15140,-15139,-15128,-15121,-15119,-15117,-15110,-15109,-14941,-14937,-14933,-14930,-14929,-14928,
												  -14926,-14922,-14921,-14914,-14908,-14902,-14894,-14889,-14882,-14873,-14871,-14857,-14678,-14674,-14670,-14668,
												  -14663,-14654,-14645,-14630,-14594,-14429,-14407,-14399,-14384,-14379,-14368,-14355,-14353,-14345,-14170,-14159,
												  -14151,-14149,-14145,-14140,-14137,-14135,-14125,-14123,-14122,-14112,-14109,-14099,-14097,-14094,-14092,-14090,
												  -14087,-14083,-13917,-13914,-13910,-13907,-13906,-13905,-13896,-13894,-13878,-13870,-13859,-13847,-13831,-13658,
												  -13611,-13601,-13406,-13404,-13400,-13398,-13395,-13391,-13387,-13383,-13367,-13359,-13356,-13343,-13340,-13329,
												  -13326,-13318,-13147,-13138,-13120,-13107,-13096,-13095,-13091,-13076,-13068,-13063,-13060,-12888,-12875,-12871,
												  -12860,-12858,-12852,-12849,-12838,-12831,-12829,-12812,-12802,-12607,-12597,-12594,-12585,-12556,-12359,-12346,
												  -12320,-12300,-12120,-12099,-12089,-12074,-12067,-12058,-12039,-11867,-11861,-11847,-11831,-11798,-11781,-11604,
												  -11589,-11536,-11358,-11340,-11339,-11324,-11303,-11097,-11077,-11067,-11055,-11052,-11045,-11041,-11038,-11024,
												  -11020,-11019,-11018,-11014,-10838,-10832,-10815,-10800,-10790,-10780,-10764,-10587,-10544,-10533,-10519,-10331,
												  -10329,-10328,-10322,-10315,-10309,-10307,-10296,-10281,-10274,-10270,-10262,-10260,-10256,-10254};

        private static string[] pystr = new string[]{"a","ai","an","ang","ao","ba","bai","ban","bang","bao","bei","ben","beng","bi","bian","biao",
													  "bie","bin","bing","bo","bu","ca","cai","can","cang","cao","ce","ceng","cha","chai","chan","chang","chao","che","chen",
													  "cheng","chi","chong","chou","chu","chuai","chuan","chuang","chui","chun","chuo","ci","cong","cou","cu","cuan","cui",
													  "cun","cuo","da","dai","dan","dang","dao","de","deng","di","dian","diao","die","ding","diu","dong","dou","du","duan",
													  "dui","dun","duo","e","en","er","fa","fan","fang","fei","fen","feng","fo","fou","fu","ga","gai","gan","gang","gao",
													  "ge","gei","gen","geng","gong","gou","gu","gua","guai","guan","guang","gui","gun","guo","ha","hai","han","hang",
													  "hao","he","hei","hen","heng","hong","hou","hu","hua","huai","huan","huang","hui","hun","huo","ji","jia","jian",
													  "jiang","jiao","jie","jin","jing","jiong","jiu","ju","juan","jue","jun","ka","kai","kan","kang","kao","ke","ken",
													  "keng","kong","kou","ku","kua","kuai","kuan","kuang","kui","kun","kuo","la","lai","lan","lang","lao","le","lei",
													  "leng","li","lia","lian","liang","liao","lie","lin","ling","liu","long","lou","lu","lv","luan","lue","lun","luo",
													  "ma","mai","man","mang","mao","me","mei","men","meng","mi","mian","miao","mie","min","ming","miu","mo","mou","mu",
													  "na","nai","nan","nang","nao","ne","nei","nen","neng","ni","nian","niang","niao","nie","nin","ning","niu","nong",
													  "nu","nv","nuan","nue","nuo","o","ou","pa","pai","pan","pang","pao","pei","pen","peng","pi","pian","piao","pie",
													  "pin","ping","po","pu","qi","qia","qian","qiang","qiao","qie","qin","qing","qiong","qiu","qu","quan","que","qun",
													  "ran","rang","rao","re","ren","reng","ri","rong","rou","ru","ruan","rui","run","ruo","sa","sai","san","sang",
													  "sao","se","sen","seng","sha","shai","shan","shang","shao","she","shen","sheng","shi","shou","shu","shua",
													  "shuai","shuan","shuang","shui","shun","shuo","si","song","sou","su","suan","sui","sun","suo","ta","tai",
													  "tan","tang","tao","te","teng","ti","tian","tiao","tie","ting","tong","tou","tu","tuan","tui","tun","tuo",
													  "wa","wai","wan","wang","wei","wen","weng","wo","wu","xi","xia","xian","xiang","xiao","xie","xin","xing",
													  "xiong","xiu","xu","xuan","xue","xun","ya","yan","yang","yao","ye","yi","yin","ying","yo","yong","you",
													  "yu","yuan","yue","yun","za","zai","zan","zang","zao","ze","zei","zen","zeng","zha","zhai","zhan","zhang",
													  "zhao","zhe","zhen","zheng","zhi","zhong","zhou","zhu","zhua","zhuai","zhuan","zhuang","zhui","zhun","zhuo",
													  "zi","zong","zou","zu","zuan","zui","zun","zuo"};
        #endregion

        #region Convert
        public static string Convert(string chrstr)
        {
            byte[] array = new byte[2];
            string returnstr = "";
            int chrasc = 0;
            int i1 = 0;
            int i2 = 0;
            char[] nowchar = chrstr.ToCharArray();
            for (int j = 0; j < nowchar.Length; j++)
            {
                array = System.Text.Encoding.Default.GetBytes(nowchar[j].ToString());
                i1 = (short)(array[0]);
                i2 = (short)(array[1]);

                chrasc = i1 * 256 + i2 - 65536;
                if (chrasc > 0 && chrasc < 160)
                {
                    returnstr += nowchar[j];
                }
                else
                {
                    for (int i = (pyvalue.Length - 1); i >= 0; i--)
                    {
                        if (pyvalue[i] <= chrasc)
                        {
                            returnstr += pystr[i];
                            break;
                        }
                    }
                }
            }

            return returnstr;
        }
        #endregion

    }

}

