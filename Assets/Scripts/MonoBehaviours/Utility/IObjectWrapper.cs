namespace MonoBehaviours.Utility
{
    public interface IObjectWrapper<T> where T : IRetrievableFromGameManager
    {
        bool TryFetch(out T fill);
    }
}
