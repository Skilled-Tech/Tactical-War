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
	public class EntityCustomBounds : Entity.Module
	{
		[SerializeField]
        protected Vector3 center;
        public Vector3 Center { get { return center; } }

        [SerializeField]
        protected Vector3 size;
        public Vector3 Size { get { return size; } }

        public Bounds Value
        {
            get
            {
                return new Bounds()
                {
                    center = Vector3.Scale(center, transform.localScale) + transform.localPosition,
                    size = Vector3.Scale(size, transform.localScale)
                };
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            Gizmos.matrix = transform.localToWorldMatrix;

            Gizmos.DrawWireCube(center, size);
        }
    }
}