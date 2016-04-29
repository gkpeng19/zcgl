using System;
namespace NM.Diagram.Render
{
    /// <summary>
    /// The enum variable used to keep the state of one NodeMeta
    /// For the Model class Nodemeta
    /// </summary>
    [Flags]
    public enum GraphicState
    {
        Normal = 0,
        Loaded = 1,
        New = 2,
        Modified = 4,        
        ToSave=6,
        ToDelete = 8,
        Published = 16 
    }
}
