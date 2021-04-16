namespace MonoBehaviours.Utility
{
    public interface IObjectWrapper<T> where T : IRetrievableFromGameManager
    {
        void Set(T toSet);
        bool TryFetch(out T fill);
    }
}
