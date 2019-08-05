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
    [DefaultExecutionOrder(ExecutionOrder)]
	public class Level : MonoBehaviour
	{
        public const int ExecutionOrder = -200;

        public static Level Instance { get; protected set; }

        public LevelPause Pause { get; protected set; }

        public LevelAges Ages { get; protected set; }

        public LevelProponents Proponents { get; protected set; }

        new public GameCamera camera { get; protected set; }

        public interface IReference : IReference<Level> { }
        public abstract class Reference : Reference<Level>
        {
            public Level Level { get { return Data; } }
        }

        protected virtual void Awake()
        {
            Instance = this;

            References.Init(this);

            Pause = Dependancy.Get<LevelPause>(gameObject);

            Ages = Dependancy.Get<LevelAges>(gameObject);

            Proponents = FindObjectOfType<LevelProponents>();

            camera = FindObjectOfType<GameCamera>();
        }

		protected virtual void Start()
        {
            
        }

        public virtual void Quit()
        {
            SceneManager.LoadScene(0);
        }

        protected virtual void OnDestroy()
        {
            Instance = null;
        }
	}
}