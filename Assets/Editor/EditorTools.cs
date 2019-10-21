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
#endif

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Game
{
	public static class EditorTools
	{
        public static Core Core { get { return Core.Instance; } }

		public static class Tools
        {
            public const string MenuPath = "Tools/";

            public static Core Core { get { return EditorTools.Core; } }

            public static class Scenes
            {
                public const string MenuPath = Tools.MenuPath + "Scenes/";

                public static ScenesCore Core { get { return Tools.Core.Scenes; } }

                [MenuItem(MenuPath + "Login")]
                public static void Login()
                {
                    Load(Core.Login);
                }

                [MenuItem(MenuPath + "Main Menu")]
                public static void MainMenu()
                {
                    Load(Core.MainMenu);
                }

                [MenuItem(MenuPath + "Level")]
                public static void Level()
                {
                    Load(Core.Level);
                }

                public static void Load(GameScene scene)
                {
                    EditorSceneManager.OpenScene(scene.Path);
                }
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
	}
}