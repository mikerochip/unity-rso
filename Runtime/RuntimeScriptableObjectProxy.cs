using UnityEngine;

namespace MikeSchweitzer.Rso
{
    internal class RuntimeScriptableObjectProxy : MonoBehaviour
    {
        #region Private Fields
        private RuntimeScriptableObject _object;
        #endregion

        #region Public Methods
        internal void Initialize(RuntimeScriptableObject obj)
        {
            _object = obj;
            _object.ProxyAwake();
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            _object.ProxyStart();
        }

        private void Update()
        {
            _object.ProxyUpdate();
        }

        private void OnApplicationQuit()
        {
            _object.ProxyApplicationQuit();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            _object.ProxyApplicationFocus(hasFocus);
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _object.ProxyApplicationPause(pauseStatus);
        }
        #endregion
    }
}
