using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
    [Serializable]
	public class ScenesCore : Core.Property
	{
        [SerializeField]
        protected GameScene login;
        public GameScene Login { get { return login; } }

        [SerializeField]
        protected GameScene mainMenu;
        public GameScene MainMenu { get { return mainMenu; } }

        [SerializeField]
        protected GameScene level;
        public GameScene Level { get { return level; } }

        public virtual void Load(string name)
        {
            Load(name, LoadSceneMode.Single);
        }
        public virtual void Load(string name, LoadSceneMode mode)
        {
            SceneManager.LoadScene(name, mode);
        }

        public virtual void Load(GameScene scene)
        {
            Load(scene, LoadSceneMode.Single);
        }
        public virtual void Load(GameScene scene, LoadSceneMode mode)
        {
            if (scene == null)
                throw new NullReferenceException("Trying to load null scene");

            Load(scene.name, mode);
        }
    }
}