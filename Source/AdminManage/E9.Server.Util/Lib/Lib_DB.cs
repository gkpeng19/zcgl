
/// <Author>  shuagnfu </Author>   
/// <CreateDate> 2007-6-15 17:24:19  </CreateDate>
 /// <summary>  
///  Lib_DB.cs
 /// <summary>  
/// <Update>2007-6-15 17:30:48</Update> 
/// <remarks> </remarks>

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace NM.Util
{
	public class Lib_DB
	{
		public static object DefaultValue(System.Type sysType)
		{
			switch (sysType.Name)
			{
				case "Guid":
					return 0;
				case "String":
					return string.Empty;
				case "Integer":
				case "Int32":
				case "int":
				case "byte":
				case "short":
				case "Int16":
				case "long":
				case "Int64":
					return 0;
				case "double":
				case "Double":
				case "float":
				case "decimal":
				case "Decimal":
					return new Decimal(0.0);
				case "byte[]":
					return null;
				case "object":
					return null;
				case "bool":
					return false;
				case "DateTime":
				case "TimeSpan":
					return DateTime.Now;
				default:
					return "";

			}
		}

		public static object DefaultValue(DbType sysType)
		{
			object result;
			switch (sysType)
			{
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
				case DbType.Double:
				case DbType.Decimal:
				case DbType.VarNumeric:
					result = 0;
					break;
				case DbType.String:
				case DbType.StringFixedLength:
					result = "";
					break;
				case DbType.Date:
				case DbType.Time:
				case DbType.DateTime:
					result = DateTime.MinValue;
					break;
				default:
					result = "null";
					break;
			}
			return result;
		}

		public static string DefaultValue(DbType sysType, object objValue)
		{
			if (objValue == null || (objValue.GetType().Name == "String" && string.IsNullOrEmpty(objValue.ToString().Trim())))
				return DefaultValue(sysType).ToString();
			else
				return objValue.ToString();
		}

		public static SqlDbType GetSqlDBType(DbType dbType)
		{
			switch (dbType)
			{
				case DbType.AnsiString: return SqlDbType.VarChar;
				case DbType.AnsiStringFixedLength: return SqlDbType.Char;
				case DbType.Binary: return SqlDbType.VarBinary;
				case DbType.Boolean: return SqlDbType.Bit;
				case DbType.Byte: return SqlDbType.TinyInt;
				case DbType.Currency: return SqlDbType.Money;
				case DbType.Date: return SqlDbType.DateTime;
				case DbType.DateTime: return SqlDbType.DateTime;
				case DbType.Decimal: return SqlDbType.Decimal;
				case DbType.Double: return SqlDbType.Float;
				case DbType.Guid: return SqlDbType.UniqueIdentifier;
				case DbType.Int16: return SqlDbType.Int;
				case DbType.Int32: return SqlDbType.Int;
				case DbType.Int64: return SqlDbType.BigInt;
				case DbType.Object: return SqlDbType.Variant;
				case DbType.SByte: return SqlDbType.TinyInt;
				case DbType.Single: return SqlDbType.Real;
				case DbType.String: return SqlDbType.NVarChar;
				case DbType.StringFixedLength: return SqlDbType.NChar;
				case DbType.Time: return SqlDbType.DateTime;
				case DbType.UInt16: return SqlDbType.Int;
				case DbType.UInt32: return SqlDbType.Int;
				case DbType.UInt64: return SqlDbType.BigInt;
				case DbType.VarNumeric: return SqlDbType.Decimal;

				default:
					{
						return SqlDbType.VarChar;
					}
			}
		}

        public static DbType GetDBType(SqlDbType dbType)
        {
            switch (dbType)
            {
                case SqlDbType.BigInt: return DbType.Int64;
                case SqlDbType.Binary: return DbType.Binary;
                case SqlDbType.Bit: return DbType.Boolean;
                case SqlDbType.Char: return DbType.StringFixedLength;
                case SqlDbType.Date: return DbType.Date;
                case SqlDbType.DateTime: return DbType.DateTime;
                case SqlDbType.DateTime2: return DbType.DateTime2;
                case SqlDbType.DateTimeOffset: return DbType.DateTimeOffset;
                case SqlDbType.Decimal: return DbType.Decimal;
                case SqlDbType.Float: return DbType.Double;
                case SqlDbType.Image: return DbType.Binary;
                case SqlDbType.Int: return DbType.Int32;
                case SqlDbType.Money: return DbType.Decimal;
                case SqlDbType.NChar: return DbType.StringFixedLength;
                case SqlDbType.NText: return DbType.String;
                case SqlDbType.NVarChar: return DbType.String;
                case SqlDbType.Real: return DbType.Single;
                case SqlDbType.SmallDateTime: return DbType.DateTime;
                case SqlDbType.SmallInt: return DbType.Int16;
                case SqlDbType.SmallMoney: return DbType.Decimal;
                case SqlDbType.Structured: return DbType.Object;
                case SqlDbType.Text: return DbType.String;
                case SqlDbType.Time: return DbType.Time;
                case SqlDbType.Timestamp: return DbType.Binary;
                case SqlDbType.TinyInt: return DbType.Byte;
                case SqlDbType.Udt: return DbType.Object;
                case SqlDbType.UniqueIdentifier: return DbType.Guid;
                case SqlDbType.VarBinary: return DbType.Binary;
                case SqlDbType.VarChar: return DbType.String;
                case SqlDbType.Variant: return DbType.Object;
                case SqlDbType.Xml: return DbType.Xml;

                default:
                    return DbType.String;
            }
        }


		public static Type GetSystemType(DbType dbType)
		{
			switch (dbType)
			{
				case DbType.AnsiString: return typeof(string);
				case DbType.AnsiStringFixedLength: return typeof(string);
				case DbType.Binary: return typeof(Byte[]);
				case DbType.Boolean: return typeof(Boolean);
				case DbType.Byte: return typeof(Byte);
				case DbType.Currency: return typeof(Decimal);
				case DbType.Date: return typeof(DateTime);
				case DbType.DateTime: return typeof(DateTime);
				case DbType.Decimal: return typeof(Decimal);
				case DbType.Double: return typeof(Double);
				case DbType.Guid: return typeof(Guid);
				case DbType.Int16: return typeof(Int16);
				case DbType.Int32: return typeof(Int32);
				case DbType.Int64: return typeof(Int64);
				case DbType.Object: return typeof(Object);
				case DbType.SByte: return typeof(SByte);
				case DbType.Single: return typeof(Single);
				case DbType.String: return typeof(String);
				case DbType.StringFixedLength: return typeof(String);
				case DbType.Time: return typeof(TimeSpan);
				case DbType.UInt16: return typeof(UInt16);
				case DbType.UInt32: return typeof(UInt32);
				case DbType.UInt64: return typeof(UInt64);
				case DbType.VarNumeric: return typeof(Decimal);

				default:
					{
						return typeof(string);
					}
			}
		}
	}
}

