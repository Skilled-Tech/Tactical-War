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
    [CreateAssetMenu(menuName = MenuPath + "Player")]
	public class PlayerCore : Core.Module
	{
		[SerializeField]
        protected Funds funds;
        public Funds Funds { get { return funds; } }

        public override void Configure()
        {
            base.Configure();

            Funds.Configure();

            funds.Gold.Value = 99999;
            funds.XP.Value = 99999;
        }
    }
}