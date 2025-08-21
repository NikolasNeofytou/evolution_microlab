namespace Microlab.Core
{
    public interface ICpu8085
    {
        CpuState State { get; }
        void Reset();
        int Step(IBus bus);
    }
}
