using System;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Controller
{
    public class GameController: MonoBehaviour
    {
        public static GameController Singleton;

        private void Awake()
        {
            if (Singleton != null)
            {
                DestroyImmediate(this);
            }
            Singleton = this;
            Init();
        }

        public SoundController soundController;

        private void Start()
        {
            
        }

        private void Init()
        {
            DontDestroyOnLoad(this);

            SceneManager.LoadSceneAsync("MainMenu");
        }
    }
}