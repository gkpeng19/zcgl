/// <summary>
/// Purpose: Logging 
/// Created By : SHuangfu
/// Created On: 6/2/2007
/// 
/// Purpose :
/// Modified By :
/// Modified On :
/// </summary>                  

namespace NM.Log
{        
	/// <summary>
	/// ��ֵ��¼��
	/// </summary>
	public interface ILogWriter
	{
        bool LogIt(LogItem log);	
        bool Flush();
	}   
}
