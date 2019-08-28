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
	public class UIGrayscaleController
	{
        public MonoBehaviour Behaviour { get; protected set; }

        public List<Graphic> Targets { get; protected set; }

        public float Ammount
        {
            set
            {
                value = Mathf.Clamp01(value);

                for (int i = 0; i < Targets.Count; i++)
                {
                    Targets[i].material.SetFloat("_EffectAmount", value);

                    for (int x = 0; x < 2; x++)
                        Targets[i].gameObject.SetActive(!Targets[i].gameObject.activeSelf);
                }
            }
        }

        public bool On { set { Ammount = value ? 1f : 0f; } }
        public bool Off { set { On = !value; } }

        public UIGrayscaleController(MonoBehaviour behaviour)
        {
            this.Behaviour = behaviour;

            Targets = new List<Graphic>();

            var graphics = Dependancy.GetAll<Graphic>(behaviour.gameObject);

            for (int i = 0; i < graphics.Count; i++)
            {
                if (graphics[i].material.shader.name.Contains("Grayscale"))
                {
                    graphics[i].material = Object.Instantiate(graphics[i].material);

                    Targets.Add(graphics[i]);
                }
            }
        }
	}
}