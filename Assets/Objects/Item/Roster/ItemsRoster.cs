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
    [CreateAssetMenu(menuName = ItemTemplate.MenuPath + "Roster")]
	public class ItemsRoster : ScriptableObject
	{
        [SerializeField]
        protected List<ItemTemplate> list;
        public List<ItemTemplate> List { get { return list; } }

#if UNITY_EDITOR
        protected virtual void Refresh()
        {
            list.Clear();

            var GUIDs = AssetDatabase.FindAssets("t:" + typeof(ItemTemplate));

            for (int i = 0; i < GUIDs.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(GUIDs[i]);

                var instance = AssetDatabase.LoadAssetAtPath<ItemTemplate>(path);

                if(instance.Visiblity.Rotster)
                    list.Add(instance);
            }

            EditorUtility.SetDirty(this);
        }
#endif

#if UNITY_EDITOR
        [CustomEditor(typeof(ItemsRoster))]
        public class Inspector : Editor
        {
            new ItemsRoster target;

            private void OnEnable()
            {
                target = base.target as ItemsRoster;
            }

            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                if (GUILayout.Button("Refresh"))
                {
                    target.Refresh();
                }
            }
        }
#endif
    }
}