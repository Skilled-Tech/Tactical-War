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
    [CreateAssetMenu(menuName = MenuPath + "Levels")]
	public class LevelsCore : Core.Module
	{
        [SerializeField]
        protected ElementData[] list;
        public ElementData[] List { get { return list; } }
        [Serializable]
        public class ElementData
        {
            [SerializeField]
            protected GameScene scene;
            public GameScene Scene { get { return scene; } }

            [SerializeField]
            protected Sprite icon;
            public Sprite Icon { get { return icon; } }

            [SerializeField]
            protected bool unclocked;
            public bool Unlocked { get { return unclocked; } }
        }

        public int Count { get { return list.Length; } }

        public ElementData this[int index] { get { return list[index]; } }

        public ScenesCore Scenes { get { return Core.Scenes; } }

        public override void Configure()
        {
            base.Configure();

            Current = null;
        }

        public ElementData Current { get; protected set; }
        public virtual void Load(ElementData element)
        {
            Current = element;

            SceneManager.LoadScene(Core.Scenes.Level.Name);

            SceneManager.LoadScene(element.Scene, LoadSceneMode.Additive);
        }
        
        public virtual void Reload()
        {
            if (Current == null)
                Scenes.Load(SceneManager.GetActiveScene().name);
            else
                Load(Current);
        }

        public virtual void ReturnToMainMenu()
        {
            Current = null;
            
            Scenes.Load(Scenes.MainMenu);
        }
    }
}