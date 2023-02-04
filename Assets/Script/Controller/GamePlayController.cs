using UnityEngine;

namespace Controller
{
    public class GamePlayController: MonoBehaviour
    {
        public static GamePlayController Singleton;

        public NodeController nodeController;
        public LevelController levelController;
        
        private void Awake()
        {
            if (Singleton != null)
            {
                DestroyImmediate(this);
            }
            Singleton = this;
            Init();

        }

        private void Start()
        {
        }

        private void Init()
        {
            nodeController.Init();
            levelController.Init();
        }
    }
}