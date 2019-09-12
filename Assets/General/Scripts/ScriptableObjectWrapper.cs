using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game {
    public abstract class ScriptableObjectWrapper : ScriptableObject {
#if UNITY_EDITOR
        [CustomEditor (typeof (ScriptableObjectWrapper), true)]
        public class Inspector : Editor {
            SerializedProperty data;

            private void OnEnable () {
                data = serializedObject.FindProperty ("data");
            }

            public override void OnInspectorGUI () {
                EditorGUILayout.PropertyField (data, true);

                serializedObject.ApplyModifiedProperties ();
            }
        }
#endif
    }

    public class ScriptableObjectWrapper<TData> : ScriptableObjectWrapper {
        [SerializeField]
        protected TData data;
        public TData Data { get { return data; } }
    }
}