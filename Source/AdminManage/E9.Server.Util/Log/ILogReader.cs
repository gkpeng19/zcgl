
/// <Author>  shuagnfu </Author>   
/// <CreateDate> 2007-6-22 16:56:59  </CreateDate>
/// <summary>  
///  IlogReader.cs
/// <summary>  
/// <Update>2007-2-26 9:52:02</Update> 
/// <remarks> </remarks>

using System;
using System.Collections.Generic;

namespace NM.Log
{
    public interface IlogReader
    {
        List<LogItem> Load(DateTime time);        
    }
}

