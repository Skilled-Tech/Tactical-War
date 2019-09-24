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
	public class DestroyAfter : MonoBehaviour
	{
        [SerializeField]
        protected float duration = 1f;
        public float Duration { get { return duration; } }

        IEnumerator Start()
        {
            yield return new WaitForSeconds(duration);

            Destroy(gameObject);
        }
    }
}