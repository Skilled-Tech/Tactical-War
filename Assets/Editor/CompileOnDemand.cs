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
    [InitializeOnLoad]
    public class CompileOnDemand : EditorWindow
	{
        public const string Name = "Compile On Demand";

        public static string Key => "m_" + nameof(CompileOnDemand);

        public static bool IsOn
        {
            get
            {
                return EditorPrefs.GetBool(Key, false);
            }
            set
            {
                EditorPrefs.SetBool(Key, value);

                if(value)
                {
                    EditorApplication.LockReloadAssemblies();
                }
                else
                {
                    EditorApplication.UnlockReloadAssemblies();

                    AssetDatabase.Refresh();
                }
            }
        }
        
        public static class Menu
        {
            public const string Path = "Tools/" + Name + "/";

            [MenuItem(Path + "Window")]
            static void Window()
            {
                Instance = GetWindow<CompileOnDemand>("Compile On Demand");

                Instance.Show();
            }

            [MenuItem(Path + "Enable")]
            static void Enable()
            {
                CompileOnDemand.IsOn = true;
            }

            [MenuItem(Path + "Disable")]
            static void Disable()
            {
                CompileOnDemand.IsOn = false;
            }
        }

        public static CompileOnDemand Instance { get; protected set; }

        public GUIStyle Style { get; protected set; }

        private void OnEnable()
        {
            minSize = Vector2.zero;
        }

        private void OnGUI()
        {
            Style = new GUIStyle(GUI.skin.button);

            Style.stretchHeight = true;
            Style.fontSize = 20;
            Style.fontStyle = FontStyle.Bold;

            if (IsOn)
            {
                if (GUILayout.Button("Auto Compile Off", Style))
                {
                    IsOn = false;
                }
            }
            else
            {
                if (GUILayout.Button("Auto Compile On", Style))
                {
                    IsOn = true;
                }
            }
        }
    }
}