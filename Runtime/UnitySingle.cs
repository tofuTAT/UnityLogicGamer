using UnityEngine;

namespace UnityLogicGamer
{
    public class UnitySingle<T>:MonoBehaviour where T:UnitySingle<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this as T;
        }
    }
}