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
	public class MusicPlayer : AudioPlayer
	{
        [SerializeField]
        protected float fadeSpeed = 1f;
        public float FadeSpeed
        {
            get
            {
                return fadeSpeed;
            }
            set
            {
                if (value < 0f) value = 0f;

                fadeSpeed = value;
            }
        }

        protected override void Start()
        {
            base.Start();

            Priority = 200;

            Source.ignoreListenerPause = true;
        }

        public virtual void Play(MusicTrack track)
        {
            if(track.Size == 0)
            {
                Debug.LogError("Trying to play Musical Track: " + track.name + " but it has no clips in it");
                return;
            }

            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                IEnumerator Play(AudioClip clip)
                {
                    Source.Stop();

                    Source.clip = clip;

                    Source.Play();

                    yield return Fade(track.Volume);

                    yield return new WaitForSecondsRealtime(clip.length);
                }

                yield return Fade(0);

                for (int i = 0; i < track.Size; i++)
                    yield return Play(track[i]);

                while (true)
                    yield return Play(track.Last);
            }
        }

        Coroutine coroutine;

        public virtual void Stop()
        {
            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(Procedure());

            IEnumerator Procedure()
            {
                yield return Fade(0);

                Source.Stop();
                Source.clip = null;
            }
        }

        IEnumerator Fade(float target)
        {
            target = Mathf.Clamp01(target);

            while(Volume != target)
            {
                Volume = Mathf.MoveTowards(Volume, target, fadeSpeed * Time.deltaTime);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}