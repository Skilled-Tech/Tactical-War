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
        protected GameScene loadingMenu;
        public GameScene LoadingMenu { get { return loadingMenu; } }

        [SerializeField]
        protected GameScene mainMenu;
        public GameScene MainMenu { get { return mainMenu; } }

        [SerializeField]
        protected GameScene level;
        public GameScene Level { get { return level; } }

        public FaderUI Fader => Core.UI.Fader;

        public virtual void Load(string name)
        {
            LoadAll(name);
        }

        public virtual void LoadAll(params GameScene[] scenes)
        {
            var names = new string[scenes.Length];

            for (int i = 0; i < scenes.Length; i++)
                names[i] = scenes[i];

            LoadAll(names);
        }
#pragma warning disable CS0618
        public virtual void LoadAll(params string[] names)
        {
            IEnumerator Procedure()
            {
                yield return Fader.To(1f);
                SceneManager.LoadScene(loadingMenu.name);
                yield return Fader.To(0f);

                var operations = new AsyncOperation[names.Length];
                for (int i = 0; i < names.Length; i++)
                {
                    operations[i] = SceneManager.LoadSceneAsync(names[i], LoadSceneMode.Additive);
                    operations[i].allowSceneActivation = false;

                    bool IsReady() => operations[i].progress == 0.9f;
                    
                    yield return new WaitUntil(IsReady);
                }

                yield return Fader.To(1f);

                void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
                {
                    SceneManager.sceneLoaded -= SceneLoadedCallback;

                    SceneManager.SetActiveScene(scene);

                    SceneManager.UnloadScene(loadingMenu.name);
                }
                SceneManager.sceneLoaded += SceneLoadedCallback;

                for (int i = 0; i < operations.Length; i++)
                    operations[i].allowSceneActivation = true;

                yield return new WaitForSeconds(0.5f);

                yield return Fader.To(0f);
            }

            Core.SceneAcessor.StartCoroutine(Procedure());
        }
#pragma warning restore CS0618 // Type or member is obsolete

        public virtual void Load(GameScene scene)
        {
            if (scene == null)
                throw new NullReferenceException("Trying to load null scene");

            Load(scene.name);
        }
    }
}