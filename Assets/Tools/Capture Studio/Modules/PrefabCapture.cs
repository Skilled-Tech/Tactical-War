﻿#if UNITY_EDITOR
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

namespace CaptureStudio
{
    public abstract class PrefabCapture<TSubject> : CaptureStudio.Behaviour
    {
        public List<TSubject> subjects;

        public abstract GameObject GetPrefab(TSubject subject);
        public virtual GameObject GetInstance(TSubject subject) => Instantiate(GetPrefab(subject));

        protected override IEnumerator Procedure()
        {
            yield return base.Procedure();

            foreach (var subject in subjects)
            {
                var path = GetPath(subject);

                var instance = GetInstance(subject);

                Capture(path);

                Destroy(instance);

                yield return new WaitForEndOfFrame();
            }
        }

        protected virtual string GetPath(TSubject subject)
        {
            var prefab = GetPrefab(subject);

            var result = AssetDatabase.GetAssetPath(prefab);

            result = Path.ChangeExtension(result, "png");

            return result;
        }
    }

    public class PrefabCapture : PrefabCapture<GameObject>
    {
        public override GameObject GetPrefab(GameObject type) => type;
    }
}
#endif