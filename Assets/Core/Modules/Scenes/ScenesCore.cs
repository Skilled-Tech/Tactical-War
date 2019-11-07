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

        [SerializeField]
        protected LoadCore load;
        public LoadCore Load { get { return load; } }
        [Serializable]
        public class LoadCore : Core.Property
        {
            [SerializeField]
            protected GameScene scene;
            public GameScene Scene { get { return scene; } }

            public AsyncOperation[] Operations { get; protected set; }
            public virtual float Progress
            {
                get
                {
                    if (Operations == null) return 1;

                    var value = 0f;
                    var count = 0;

                    for (int i = 0; i < Operations.Length; i++)
                    {
                        if (Operations[i] == null) continue;

                        count++;
                        value += Operations[i].progress;
                    }

                    return Mathf.InverseLerp(0f, 0.9f, value / count);
                }
            }

            public ScenesCore Scenes => Core.Scenes;
            public FaderUI Fader => Core.UI.Fader;

            public virtual void One(string name)
            {
                All(name);
            }
            public virtual void One(GameScene scene)
            {
                if (scene == null)
                    throw new NullReferenceException("Trying to load null scene");

                One(scene.name);
            }

            public virtual void All(params string[] names)
            {
                IEnumerator Procedure()
                {
                    yield return Fader.To(1f);
                    SceneManager.LoadScene(scene.name);
                    yield return Fader.To(0f);

                    Operations = new AsyncOperation[names.Length];
                    for (int i = 0; i < names.Length; i++)
                    {
                        Operations[i] = SceneManager.LoadSceneAsync(names[i], LoadSceneMode.Additive);
                        Operations[i].allowSceneActivation = false;

                        bool IsReady() => Operations[i].progress == 0.9f;

                        yield return new WaitUntil(IsReady);
                    }

                    yield return new WaitForSecondsRealtime(0.6f);

                    yield return Fader.To(1f);

                    void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
                    {
                        if (scene.name == names.First())
                        {
                            SceneManager.SetActiveScene(scene);
                        }

                        if (scene.name == names.Last())
                        {
#pragma warning disable CS0618 // Type or member is obsolete
                            SceneManager.UnloadScene(this.scene.name);
#pragma warning restore CS0618 // Type or member is obsolete

                            SceneManager.sceneLoaded -= SceneLoadedCallback;
                        }
                    }
                    SceneManager.sceneLoaded += SceneLoadedCallback;

                    for (int i = 0; i < Operations.Length; i++)
                        Operations[i].allowSceneActivation = true;

                    Operations = null;

                    yield return Fader.To(0f);
                }

                Core.SceneAcessor.StartCoroutine(Procedure());
            }
            public virtual void All(params GameScene[] scenes)
            {
                var names = new string[scenes.Length];

                for (int i = 0; i < scenes.Length; i++)
                    names[i] = scenes[i];

                All(names);
            }
        }
    }
}