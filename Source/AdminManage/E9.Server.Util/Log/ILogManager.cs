/// <summary>
/// Purpose: Logging 
/// Created By : SHuangfu
/// Created On: 6/2/2007
/// 
/// Purpose :
/// Modified By :
/// Modified On :
/// </summary>

using System;

namespace NM.Log
{
  public  interface ILogManager
    {
        void ErrorIt(int SessionID, string message);
        void WarningIt(int SessionID, string message);
        void InforIt(int SessionID, string message);
        void LogException(int SessionID, Exception exp);
    }
}
