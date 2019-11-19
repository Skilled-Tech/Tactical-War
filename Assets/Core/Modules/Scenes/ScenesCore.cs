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
            protected GameScene menu;
            public GameScene Menu { get { return menu; } }

            public ScenesCore Scenes => Core.Scenes;
            public FaderUI Fader => Core.UI.Fader;

            public List<AsyncOperation> Operations { get; protected set; }

            public float Progress { get; protected set; } = 0f;

            public virtual void One(string name) => All(name);
            public virtual void One(GameScene scene)
            {
                if (scene == null)
                    throw new NullReferenceException("Trying to load null scene");

                One(scene.name);
            }

            public Coroutine Coroutine { get; protected set; }
            public bool IsProcessing => Coroutine != null;

            public virtual void All(params GameScene[] scenes)
            {
                var names = new string[scenes.Length];

                for (int i = 0; i < scenes.Length; i++)
                    names[i] = scenes[i];

                All(names);
            }
            public virtual void All(params string[] names) => All(names as IList<string>);
            public virtual void All(IList<string> names)
            {
                if(IsProcessing)
                {
                    Debug.LogError("Trying to load scenes during a scene loading, please wait for current scenes to load");
                    return;
                }

                Coroutine = Core.SceneAcessor.StartCoroutine(Procedure());

                IEnumerator Procedure()
                {
                    Progress = 0f;

                    yield return Fader.To(1f);
                    SceneManager.LoadScene(menu.name);
                    yield return Fader.To(0f);

                    Operations = new List<AsyncOperation>();
                    for (int i = 0; i < names.Count; i++)
                    {
                        var operation = SceneManager.LoadSceneAsync(names[i], LoadSceneMode.Additive);
                        operation.allowSceneActivation = false;

                        Operations.Add(operation);

                        var ratio = 1f / names.Count;

                        while (true)
                        {
                            Progress = (Mathf.InverseLerp(0f, 0.9f, operation.progress) * ratio) + ((Operations.Count - 1) * ratio);

                            if (operation.progress == 0.9f) break;

                            yield return new WaitForEndOfFrame();
                        }
                    }

                    Progress = 1f;

                    yield return new WaitForSecondsRealtime(0.6f);

                    yield return Fader.To(1f);

                    yield return new WaitForSecondsRealtime(0.2f);

                    void SceneLoadedCallback(Scene scene, LoadSceneMode mode)
                    {
                        var index = names.IndexOf(scene.name);

                        Operations[index] = null;

                        if (scene.name == names.First())
                        {
                            SceneManager.SetActiveScene(scene);
                        }

                        if (scene.name == names.Last())
                        {
                            SceneManager.sceneLoaded -= SceneLoadedCallback;

                            Operations.Clear();

#pragma warning disable CS0618 // Type or member is obsolete
                            SceneManager.UnloadScene(menu.name);
#pragma warning restore CS0618 // Type or member is obsolete
                        }
                    }
                    SceneManager.sceneLoaded += SceneLoadedCallback;

                    for (int i = 0; i < Operations.Count; i++)
                        Operations[i].allowSceneActivation = true;

                    yield return new WaitForSeconds(0.25f);

                    yield return Fader.To(0f);

                    End();
                }
            }

            public event Action OnEnd;
            void End()
            {
                Coroutine = null;
                Operations = null;

                OnEnd?.Invoke();
            }
        }
    }
}