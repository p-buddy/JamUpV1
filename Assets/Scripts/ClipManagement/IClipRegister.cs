using MonoBehaviours.Utility;
using UnityEngine;

namespace ClipManagement
{
    public interface IClipRegister : IRetrievableFromGameManager
    {
        bool LoadingInProgress();
        bool TryRegisterClip(string filename, out ClipAliasComponent clipAlias);
        bool TryGetClipAliasForState(in ClipAliasComponent baseClipAlias, in SampleState state, out ClipAliasComponent clipAlias);
        bool TryGetClip(in ClipAliasComponent alias, out AudioClip clip);
        void Clear();
    }
}