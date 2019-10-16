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
	public class LevelBackground : Level.Module
	{
        public const string MenuPath = Level.MenuPath + "Background/";

        public List<LevelBackgroundTemplate> Templates { get; protected set; }

        public override void Init()
        {
            base.Init();

            Set(Data.Level.Background);
        }

        public virtual void Set(LevelBackgroundData data)
        {
            Templates = new List<LevelBackgroundTemplate>();

            for (int i = 0; i < data.Elements.Length; i++)
            {
                var instance = CreateTemplate(data, data.Elements[i], i);

                Templates.Add(instance);
            }
        }

        public virtual LevelBackgroundTemplate CreateTemplate(LevelBackgroundData data, LevelBackgroundData.ElementData element, int index)
        {
            var instance = new GameObject(element.Prefab.name);

            instance.transform.SetParent(transform);

            instance.transform.localPosition = Vector3.zero;
            instance.transform.localEulerAngles = Vector3.zero;
            instance.transform.localScale = Vector3.one;

            var script = instance.AddComponent<LevelBackgroundTemplate>();

            script.Init();
            script.Set(data, element, index);

            return script;
        }
    }
}