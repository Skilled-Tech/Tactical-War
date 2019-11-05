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
	public class PlaySFXOperation : Operation
	{
        [SerializeField]
        protected SFXProperty _SFX;
        public SFXProperty SFX { get { return _SFX; } }

        public AudioCore Audio => Core.Instance.Audio;

        public override void Execute()
        {
            Audio.SFX.PlayOneShot(SFX);
        }
    }
}