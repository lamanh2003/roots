using TMPro;
using UnityEngine;

namespace Controller
{
    public class GamePlayController: MonoBehaviour
    {
        public static GamePlayController Singleton;

        public NodeController nodeController;
        public LevelController levelController;
        public GameResources gameResources;
        public int point;
        public TextMeshProUGUI pointTMP;
        
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
            point = 0;
            nodeController.Init();
            levelController.Init();
        }
    }
}