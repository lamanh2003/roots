using UnityEngine;

namespace Controller
{
    public class GamePlayController: MonoBehaviour
    {
        public static GamePlayController Singleton;

        public NodeController nodeController;

        private void Awake()
        {
            if (Singleton != null)
            {
                DestroyImmediate(this);
            }
            Init();
            Singleton = this;
        }

        private void Start()
        {
            
        }

        private void Init()
        {
            nodeController.Init();
        }
    }
}