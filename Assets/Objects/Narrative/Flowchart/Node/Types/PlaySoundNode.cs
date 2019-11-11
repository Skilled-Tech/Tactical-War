using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class PlaySFXNode : Node
	{
        [SerializeField]
        protected SFXProperty _SFX;
        public SFXProperty SFX { get { return _SFX; } }

        public override void Execute()
        {
            base.Execute();

            Core.Instance.Audio.SFX.PlayOneShot(SFX);

            End();
        }
    }
}