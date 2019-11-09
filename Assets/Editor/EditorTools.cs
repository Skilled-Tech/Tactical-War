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
using UnityEditor.SceneManagement;

using System.Reflection;
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using TMPro;
using RTLTMPro;

namespace Game
{
	public static class EditorTools
	{
        public const string MenuPath = "Tools/";

        public static Core Core { get { return Core.Instance; } }

        public static class Scenes
        {
            public const string MenuPath = EditorTools.MenuPath + "Scenes/";

            public static ScenesCore ScenesCore { get { return Core.Scenes; } }

            [MenuItem(MenuPath + "Login")]
            public static void Login()
            {
                Load(ScenesCore.Login);
            }

            [MenuItem(MenuPath + "Main Menu")]
            public static void MainMenu()
            {
                Load(ScenesCore.MainMenu);
            }

            [MenuItem(MenuPath + "Loading Menu")]
            public static void LoadingMenu()
            {
                Load(ScenesCore.Load.Scene);
            }

            [MenuItem(MenuPath + "Level")]
            public static void Level()
            {
                Load(ScenesCore.Level);
            }

            public static void Load(GameScene scene)
            {
                EditorSceneManager.OpenScene(scene.Path);
            }
        }

        public static class SetDirty
        {
            public const string MenuName = "Assets/Set Dirty";

            [MenuItem(MenuName)]
            public static void Execute()
            {
                for (int i = 0; i < Selection.objects.Length; i++)
                {
                    EditorUtility.SetDirty(Selection.objects[i]);
                }
            }

            [MenuItem(MenuName, true)]
            public static bool Validate()
            {
                if (Selection.objects.Length == 0) return false;

                for (int i = 0; i < Selection.objects.Length; i++)
                {

                }

                return true;
            }
        }

        public static class TMP
        {
            [MenuItem("CONTEXT/" + nameof(TMP_Text) + "/To RTL")]
            static void ToRTL(MenuCommand command)
            {
                var label = command.context as TMP_Text;

                var text = label.text;

                var serializedObject = new SerializedObject(label);

                var script = MonoScript.Get<RTLTextMeshPro>();

                MonoScript.Set(serializedObject, script);

                var rtl = serializedObject.targetObject as RTLTextMeshPro;

                rtl.text = text;
            }
        }

        public static class MonoScript
        {
            public static UnityEditor.MonoScript Get<TMonoBehaviour>()
            where TMonoBehaviour : MonoBehaviour
            {
                var instance = new GameObject();

                var behaviour = instance.AddComponent<TMonoBehaviour>();

                var script = UnityEditor.MonoScript.FromMonoBehaviour(behaviour);

                Object.DestroyImmediate(instance);

                return script;
            }

            public static void Set(Object target, UnityEditor.MonoScript script)
            {
                var serializedObject = new SerializedObject(target);

                Set(serializedObject, script);
            }
            public static void Set(SerializedObject target, UnityEditor.MonoScript script)
            {
                var property = target.FindProperty("m_Script");

                property.objectReferenceValue = script;

                target.ApplyModifiedProperties();
            }
        }
	}
}