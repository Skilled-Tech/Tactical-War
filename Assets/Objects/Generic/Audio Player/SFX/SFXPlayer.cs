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
	public class SFXPlayer : AudioPlayer
    {
        public virtual Coroutine PlayOneShot(SFXProperty property)
        {
            IEnumerator Procedure()
            {
                yield return new WaitForSeconds(property.Delay);

                Source.PlayOneShot(property.Clip, property.Volume);
            }

            return StartCoroutine(Procedure());
        }
        public virtual void PlayOneShot(IList<SFXProperty> list)
        {
            for (int i = 0; i < list.Count; i++)
                PlayOneShot(list[i]);
        }
	}

    [Serializable]
    public class SFXProperty
    {
        [SerializeField]
        protected AudioClip clip;
        public AudioClip Clip { get { return clip; } }

        [SerializeField]
        [Range(0f, 1f)]
        protected float volume = 1f;
        public float Volume { get { return volume; } }

        [SerializeField]
        protected float delay;
        public float Delay { get { return delay; } }

        public SFXProperty()
        {
            volume = 1f;
        }
    }
}