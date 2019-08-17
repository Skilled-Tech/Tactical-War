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
        protected Proponent proponent1;
        public Proponent Proponent1 { get { return proponent1; } }

        [SerializeField]
        protected Proponent proponent2;
        public Proponent Proponent2 { get { return proponent2; } }

        public virtual Proponent GetOther(Proponent proponent)
        {
            if (proponent == proponent1) return proponent2;

            if (proponent == proponent2) return proponent1;

            throw new NotImplementedException();
        }

        public override void Init()
        {
            base.Init();

            proponent1.OnDefeat += ()=>ProponentDefeated(proponent1);
            proponent2.OnDefeat += ()=>ProponentDefeated(proponent2);
        }

        public delegate void ProponentDefeatedDelegate(Proponent proponent);
        public event ProponentDefeatedDelegate OnDefeat;
        void ProponentDefeated(Proponent proponent)
        {
            if (OnDefeat != null) OnDefeat(proponent);
        }
    }
}