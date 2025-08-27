using UnityEngine;


namespace MonoBehaviours
{
    public class GameAssets : MonoBehaviour
    {
        public const uint UNITY_LAYER = 1 << 7;
        public static GameAssets Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }
    }
}