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

using UnityEngine.Rendering;

namespace Game
{
	public class LevelBackgroundTemplate : MonoBehaviour
	{
        public GameObject[] Panels { get; protected set; }

        public Bounds Bounds { get; protected set; }

        public Vector3 StartPosition { get; protected set; }

        public SortingGroup SortingGroup { get; protected set; }

        public LevelBackgroundData.ElementData Element { get; protected set; }


        public Level Level { get { return Level.Instance; } }

        new public Camera camera { get { return Level.camera.component; } }

        public Transform Anchor { get { return Level.camera.transform; } }

		public virtual void Init()
        {
            StartPosition = transform.position;

            SortingGroup = gameObject.AddComponent<SortingGroup>();
        }

        public virtual void Set(LevelBackgroundData data, LevelBackgroundData.ElementData element, int index)
        {
            this.Element = element;

            SortingGroup.sortingLayerName = "Background"; //TODO, don't hard code
            SortingGroup.sortingOrder = index;

            Bounds = Tools.Bounds.FromRenderer(element.Prefab);

            Panels = new GameObject[data.Copies];
            
            var totalWidth = Bounds.size.x * (Panels.Length - 1);

            for (int i = 0; i < Panels.Length; i++)
            {
                var panel = Instantiate(element.Prefab, transform);

                panel.name = element.Prefab.name + " " + (i + 1).ToString();

                var rate = i / (Panels.Length - 1f);

                panel.transform.localPosition = new Vector3(Mathf.Lerp(-totalWidth / 2f, totalWidth / 2f, rate), 0f, 0f);
            }
        }

        void Update()
        {
            var position = transform.position;
            {
                position.y = ProcessAnchorHeight() + Element.Height;

                position.x = StartPosition.x + Anchor.position.x * Element.Parallax;

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

        float ProcessAnchorHeight()
        {
            switch (Element.Anchor)
            {
                case BackgroundAnchor.Bottom:
                    {
                        var screenPoint = new Vector3()
                        {
                            x = 0.5f * Screen.width,
                            y = 0f,
                            z = Mathf.Abs(transform.position.z - Anchor.position.z)
                        };

                        var worldPosition = camera.ScreenToWorldPoint(screenPoint);

                        return worldPosition.y + Bounds.extents.y;
                    }

                case BackgroundAnchor.Center:
                        return Anchor.position.y;

                case BackgroundAnchor.Top:
                    {
                        var screenPoint = new Vector3()
                        {
                            x = 0.5f * Screen.width,
                            y = Screen.height,
                            z = Mathf.Abs(transform.position.z - Anchor.position.z)
                        };

                        var worldPosition = camera.ScreenToWorldPoint(screenPoint);

                        return worldPosition.y - Bounds.extents.y;
                    }
            }

            throw new NotImplementedException();
        }
    }
}