namespace ECS.Systems.Jobs.DTO
{
    public interface IClipData<T> where T : unmanaged
    { 
        T Sum(in T other);
        T Difference(in T other);
    }
}