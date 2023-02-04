using System;
using UnityEngine;

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
            Init();
            Singleton = this;
        }

        private void Start()
        {
            
        }

        private void Init()
        {
            
        }
    }
}