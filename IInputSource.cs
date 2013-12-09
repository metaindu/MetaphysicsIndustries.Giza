using System;

namespace MetaphysicsIndustries.Giza
{
    public interface IInputSource<T>
        where T : IInputElement
    {
        // random access
        InputElementSet<T> GetInputAtLocation(int index);
        int Length { get; }
        InputPosition GetPosition(int index);
        void SetCurrentIndex(int index);

        // stream access
        InputPosition CurrentPosition { get; }
        InputChar Peek();
        InputChar GetNextValue();
        bool IsAtEnd { get; }
    }
}
