using System;
using AudioPlayBack;
using ClipManagement;
using Lua;
using MonoBehaviours.Utility;
using UnityEngine;

namespace MonoBehaviours
{
    public class GameManager : Singleton<GameManager>,
        IObjectWrapper<IPlayBack>,
        IObjectWrapper<IAudioEventSpawner>,
        IObjectWrapper<IClipRegister>,
        IObjectWrapper<IAudioClipPlayer>,
        IObjectWrapper<ILuaInterpreter>
    {
        #region Set WrappedObjects
        public void Set(IPlayBack toSet) => playBack = toSet;
        public void Set(IAudioEventSpawner toSet) => entitySpawner = toSet;
        public void Set(IClipRegister toSet) => clipRegister = toSet;
        public void Set(IAudioClipPlayer toSet) => player = toSet;
        public void Set(ILuaInterpreter toSet) => luaInterpreter = toSet;
        #endregion
        
        #region WrappedObjects
        public bool TryFetch(out IPlayBack fill)
        {
            fill = playBack;
            return !(fill is null);
        }

        public bool TryFetch(out IAudioEventSpawner fill)
        {
            fill = entitySpawner;
            return !(fill is null);
        }

        public bool TryFetch(out IClipRegister fill)
        {
            fill = clipRegister;
            return !(fill is null);
        }

        public bool TryFetch(out IAudioClipPlayer fill)
        {
            fill = player;
            return !(fill is null);
        }
        

        public bool TryFetch(out ILuaInterpreter fill)
        {
            fill = luaInterpreter;
            return !(fill is null);
        }

        #endregion WrappedObjects

        private IAudioEventSpawner entitySpawner;
        private IAudioClipPlayer player;
        private IPlayBack playBack;
        private IClipRegister clipRegister;
        private ILuaInterpreter luaInterpreter;

        public void StartSong()
        {
            playBack = new PlayBack(player);
        }

        public void StopSong()
        {
            playBack = null;
        }

        public void Start()
        {
            player = player ?? new AudioSourcePool();
            clipRegister = clipRegister ?? new ClipRegister();
            entitySpawner = entitySpawner ?? new EntitySpawner();
            luaInterpreter = luaInterpreter ?? new LuaInterpreter();
        }

        public void OnDisable()
        {
            Type[] types = typeof(GameManager).GetInterfaces();
            foreach (var type in types)
            {
                if (type.GetGenericTypeDefinition() == typeof(IObjectWrapper<>))
                {
                    string methodName = nameof(IObjectWrapper<IRetrievableFromGameManager>.TryFetch);
                    object[] parameters = {null};
                    type.GetMethod(methodName)?.Invoke(this, parameters);
                    (parameters[0] as IDisposable)?.Dispose();
                }
            }
        }
    }
}
