using System.Collections;
using System.Collections.Generic;
using MonoBehaviours.Utility;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Networking;
using Utility;

namespace ClipManagement
{
    public class ClipRegister : IClipRegister
    {
        private readonly struct ClipVariant
        {
            public ClipAliasComponent ClipAlias { get; }
            public SampleState State { get; }

            public ClipVariant(in ClipAliasComponent alias, in SampleState state)
            {
                ClipAlias = alias;
                State = state;
            }
        }
        
        private int loading;
        private readonly List<AudioClip> clipRegister;
        private readonly Dictionary<ClipAliasComponent, List<ClipVariant>> variantsForBaseClip;
        private readonly List<ClipAliasComponent> failedClips;
        
        public ClipRegister()
        {
            loading = 0;
            clipRegister = new List<AudioClip>();
            variantsForBaseClip = new Dictionary<ClipAliasComponent, List<ClipVariant>>();
            failedClips = new List<ClipAliasComponent>();
            Clear();
        }
        
        private bool TryRegisterVariantClip(in ClipAliasComponent baseClipAlias,
            in SampleState variantState,
            out ClipAliasComponent variantClipAlias)
        {
            if (!TryGetClip(baseClipAlias, out AudioClip baseClip))
            {
                variantClipAlias = default;
                return false;
            }
            
            AudioClip variantClip = baseClip.MakeCopy();
            // process variantClip
            clipRegister.Add(variantClip);
            variantClipAlias = new ClipAliasComponent(clipRegister.Count - 1);
            return true;
        }

        #region IClipRegisterImplementation
        public bool LoadingInProgress() => loading > 0;

        public bool TryRegisterClip(string filename, out ClipAliasComponent clipAlias)
        {
            clipRegister.Add(null);
            ClipAliasComponent alias = new ClipAliasComponent(clipRegister.Count - 1);

            void OnComplete(AudioClip loadedClip)
            {
                clipRegister[alias.Index] = loadedClip;
                --loading;
            }

            bool failed = false;
            void OnFaulure(string error)
            {
                failed = true;
                failedClips.Add(alias);
                Debug.LogError($"Unable to load {filename}, due to: {error}");
                --loading;
            }
            CoroutineProcessor.Instance.EnqueCoroutine(LoadAudioHelper.Load(filename, OnComplete, OnFaulure));
            ++loading;

            clipAlias = alias;
            return !failed;
        }

        public bool TryGetClipAliasForState(in ClipAliasComponent baseClipAlias,
            in SampleState state,
            out ClipAliasComponent clipAlias)
        {
            if (state.Equals(default))
            {
                clipAlias = baseClipAlias;
                return true;
            }

            foreach (ClipVariant variant in variantsForBaseClip[baseClipAlias])
            {
                if (variant.State.StartOffset == state.StartOffset && variant.State.EndOffset == state.EndOffset)
                {
                    clipAlias = variant.ClipAlias;
                    return true;
                }
            }

            return TryRegisterVariantClip(baseClipAlias, state, out clipAlias);
        }

        public bool TryGetClip(in ClipAliasComponent alias, out AudioClip clip)
        {
            clip =  clipRegister[alias.Index];
            return (clip != null);
        }

        public void Clear()
        {
            clipRegister.Clear();
            failedClips.Clear();
            variantsForBaseClip.Clear();
            loading = 0;
        }

        #endregion IClipRegisterImplementation
    }
}