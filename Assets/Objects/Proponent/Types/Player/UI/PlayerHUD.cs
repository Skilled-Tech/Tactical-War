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

using TMPro;

namespace Game
{
	public class PlayerHUD : PlayerProponent.Module
	{
        public PlayerHUDUnits Units { get; protected set; }

        public abstract class Module : Module<PlayerHUD>
        {
            public PlayerHUD HUD { get { return Data; } }

            public PlayerProponent Player { get { return HUD.Player; } }
        }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }
        public Proponent Enemy { get { return Proponents.Enemy; } }

        public override void Configure(PlayerProponent data)
        {
            base.Configure(data);

            Units = Dependancy.Get<PlayerHUDUnits>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            SetAge(Level.Ages.List.First());

            Modules.Init(this);
        }

        public virtual void SetAge(AgeData age)
        {
            Units.SetAge(age);
        }
	}
}