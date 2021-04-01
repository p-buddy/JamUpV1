using MonoBehaviours.Utility;

namespace Lua
{
    public interface ILuaInterpreter : IRetrievableFromGameManager
    {
        void Execute(string code);
    }
}