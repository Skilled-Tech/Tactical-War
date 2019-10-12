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
	public class LevelBackgroundTemplate : MonoBehaviour
	{
        [SerializeField]
        protected float _parallax;
        public float Parallax
        {
            get
            {
                return _parallax;
            }
            set
            {
                value = Mathf.Clamp01(value);

                _parallax = value;
            }
        }

        public GameObject[] Panels { get; protected set; }

        public Bounds Bounds { get; protected set; }

        public Level Level { get { return Level.Instance; } }

        public Transform Anchor { get { return Level.camera.transform; } }

        public Vector3 StartPosition { get; protected set; }

		public virtual void Init()
        {
            StartPosition = transform.position;
        }

        public LevelBackgroundData.ElementData Data { get; protected set; }

        public virtual void Set(LevelBackgroundData.ElementData data, int index)
        {
            this.Data = data;

            Parallax = data.Parallax;

            Bounds = Tools.Bounds.FromRenderer(data.Prefab);

            Panels = new GameObject[data.Copies];
            
            var totalWidth = Bounds.size.x * (Panels.Length - 1);

            for (int i = 0; i < Panels.Length; i++)
            {
                var panel = Instantiate(data.Prefab, transform);

                panel.name = data.Prefab.name + " " + (i + 1).ToString();

                var rate = i / (Panels.Length - 1f);

                panel.transform.localPosition = new Vector3(Mathf.Lerp(-totalWidth / 2f, totalWidth / 2f, rate), 0f, 0f);
            }
        }

        void Update()
        {
            var position = transform.position;
            {
                position.y = Anchor.position.y;

                position.x = StartPosition.x + Anchor.position.x * Parallax;

                var difference = Anchor.position.x - transform.position.x;
                var distance = Mathf.Abs(difference);
                var direction = Mathf.Sign(difference);

                if (distance > Bounds.extents.x)
                {
                    position.x += direction * Bounds.size.x;

                    StartPosition += Vector3.right * direction * (Bounds.size.x);
                }
            }
            transform.position = position;
        }
    }
}