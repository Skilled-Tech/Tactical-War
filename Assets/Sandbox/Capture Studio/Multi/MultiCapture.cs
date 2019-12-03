#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

using UnityEditor;
using UnityEditorInternal;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

using Game;

namespace CaptureStudio
{
    public class MultiCapture : CaptureStudio.Behaviour
    {
        public GameObject[] subjects;

        protected override IEnumerator Procedure()
        {
            yield return base.Procedure();

            foreach (var subject in subjects)
            {
                subject.SetActive(false);
            }

            foreach (var subject in subjects)
            {
                var path = CaptureStudio.FormatPath("Multi/", subject.name);

                subject.SetActive(true);

                Capture(path);

                subject.SetActive(false);

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
#endif