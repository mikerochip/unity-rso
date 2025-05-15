using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build.DataBuilders;
#endif

namespace MikeSchweitzer.Rso
{
    // Override this in Project Settings to be lower than any other class.
    [DefaultExecutionOrder(-10000)]
    public class RuntimeScriptableObject : ScriptableObject
    {
        #region Private Fields
        private static int _signalCount;
        private static List<RuntimeScriptableObject> _objectsAwaitingSignal = new();

#if UNITY_EDITOR
        private static bool? _usingAssetDatabase;
#endif

        private RuntimeScriptableObjectProxy _proxy;
        #endregion

        #region Lifecycle Methods
        protected virtual void RuntimeAwake() { }
        protected virtual void RuntimeStart() { }
        protected virtual void RuntimeUpdate() { }
        protected virtual void RuntimeOnApplicationQuit() { }
        protected virtual void RuntimeOnApplicationFocus(bool hasFocus) { }
        protected virtual void RuntimeOnApplicationPause(bool pauseStatus) { }
        #endregion

        #region Coroutine Methods
        public Coroutine StartCoroutine(IEnumerator method) => _proxy.StartCoroutine(method);
        public void StopCoroutine(Coroutine coroutine) => _proxy.StopCoroutine(coroutine);
        public void StopAllCoroutines() => _proxy.StopAllCoroutines();
        #endregion

        #region Signal Methods
        internal static void SignalAwake()
        {
            ++_signalCount;
            if (_signalCount > 1)
                return;

            _objectsAwaitingSignal.ForEach(obj => obj.CreateProxy());
            _objectsAwaitingSignal.Clear();
        }

        internal static void SignalDestroy()
        {
            --_signalCount;
        }
        #endregion

        #region Proxy Methods
        internal void ProxyAwake() => RuntimeAwake();
        internal void ProxyStart() => RuntimeStart();
        internal void ProxyUpdate() => RuntimeUpdate();
        internal void ProxyApplicationQuit() => RuntimeOnApplicationQuit();
        internal void ProxyApplicationFocus(bool hasFocus) => RuntimeOnApplicationFocus(hasFocus);
        internal void ProxyApplicationPause(bool pauseStatus) => RuntimeOnApplicationPause(pauseStatus);
        #endregion

        #region Private Methods
        private void CreateProxy()
        {
            var go = new GameObject($"{name}.Proxy");
            DontDestroyOnLoad(go);
            _proxy = go.AddComponent<RuntimeScriptableObjectProxy>();
            _proxy.Initialize(this);
        }
        #endregion

        #region Unity Methods
        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            // We have to use a different guard depending on whether we're loading RSOs from
            // AssetDatabase vs AssetBundles.
            //
            // This is the scenario:
            //
            // 1. Change into play mode
            // 2. Unity sets EditorApplication.isPlayingOrWillChangePlaymode = true
            // 3. SO.OnEnable() is called for whatever SOs Unity happens to have in editor memory
            // 4. Unity sets EditorApplication.isPlaying = true
            // 5. SO.OnEnable() is called for any SOs that were loaded from AssetBundles
            // 6. Awake() is called on any active scene objects
            //
            // We have to check isPlayingOrWillChangePlaymode when using the AssetDatabase because
            // RSO initialization depends on what's loaded in editor.
            //
            // We have to check isPlaying when using AssetBundles (editor play mode OR builds),
            // because we _assume_ that all RSOs will come from AssetBundles.
            //
            // The result of this is that we want to ignore OnEnable() for RSOs in editor memory
            // when running with AssetBundles because they should not be considered part of the
            // runtime. And we can tell which ones are in editor memory because their OnEnable()
            // is called after isPlayingOrWillChangePlaymode but before isPlaying.
            if (_usingAssetDatabase == null)
            {
                // Addressables isn't loaded yet, probably this is a first-time startup of
                // Unity with a freshly built Library
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings == null)
                    return;

                var builder = settings.ActivePlayModeDataBuilder;
                _usingAssetDatabase = builder is BuildScriptFastMode;
            }

            if (_usingAssetDatabase.Value)
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                    return;
            }
            else
            {
                if (!EditorApplication.isPlaying)
                    return;
            }
#endif

            if (_signalCount > 0)
                CreateProxy();
            else
                _objectsAwaitingSignal.Add(this);
        }
        #endregion
    }
}
