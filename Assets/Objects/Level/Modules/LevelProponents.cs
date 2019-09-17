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
	public class LevelProponents : Level.Module
	{
        [SerializeField]
        protected PlayerProponent player;
        public PlayerProponent Player { get { return player; } }

        [SerializeField]
        protected AIProponent _AI;
        public AIProponent AI { get { return _AI; } }

        public virtual Proponent GetOther(Proponent proponent)
        {
            if (proponent == player) return _AI;

            if (proponent == _AI) return player;

            throw new NotImplementedException();
        }

        public override void Init()
        {
            base.Init();

            player.OnDefeat += ()=>ProponentDefeated(player);
            _AI.OnDefeat += ()=>ProponentDefeated(_AI);
        }

        public delegate void ProponentDefeatedDelegate(Proponent proponent);
        public event ProponentDefeatedDelegate OnDefeat;
        void ProponentDefeated(Proponent proponent)
        {
            if (OnDefeat != null) OnDefeat(proponent);
        }
    }
}