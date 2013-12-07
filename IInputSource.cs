using System;

namespace MetaphysicsIndustries.Giza
{
    public interface IInputSource<T>
        where T : IInputElement
    {
        InputElementSet<T> GetInputAtLocation(int index);
    }
}
