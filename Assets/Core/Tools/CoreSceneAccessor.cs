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
	public class CoreSceneAccessor : MonoBehaviour
	{
        public Core Core => Core.Instance;

        protected virtual void Configure()
        {
            Core.OnInit += Init;
        }

        protected virtual void Init()
        {

        }

		public static CoreSceneAccessor Create()
        {
            var gameObject = new GameObject("Scene Accessor");

            DontDestroyOnLoad(gameObject);

            var component = gameObject.AddComponent<CoreSceneAccessor>();

            return component;
        }
	}
}