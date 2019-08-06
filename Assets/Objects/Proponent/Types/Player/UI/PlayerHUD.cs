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
        [SerializeField]
        protected FundsData funds;
        public FundsData Funds { get { return funds; } } 
        [Serializable]
        public class FundsData
        {
            [SerializeField]
            protected TMP_Text gold;
            public TMP_Text Gold { get { return gold; } } 

            [SerializeField]
            protected TMP_Text xp;
            public TMP_Text XP { get { return xp; } } 

            public void Set(Proponent proponent)
            {
                gold.text = "Gold: " + proponent.Funds.Gold.Value;

                xp.text = "XP: " + proponent.Funds.XP.Value;
            }
        }

        [SerializeField]
        protected HealthData health;
        public HealthData Health { get { return health; } }
        [Serializable]
        public class HealthData
        {
            [SerializeField]
            protected ProgressBar player;
            public ProgressBar Player { get { return player; } } 

            [SerializeField]
            protected ProgressBar enemy;
            public ProgressBar Enemy { get { return enemy; } } 
        }

        public PlayerHUDAge Age { get; protected set; }

        public abstract class Reference : Module<PlayerHUD>
        {
            public PlayerHUD HUD { get { return Data; } }
        }

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }
        public Proponent Enemy { get { return Proponents.Enemy; } }

        public override void Configure(PlayerProponent data)
        {
            base.Configure(data);

            Age = Dependancy.Get<PlayerHUDAge>(gameObject);

            Modules.Configure(this);
        }

        public override void Init()
        {
            base.Init();

            funds.Set(Player);
            Player.Funds.OnValueChanged += OnFundsChanged;

            Age.Set(Player.Age);

            health.Player.Value = Player.Base.Health.Rate;
            Player.Base.Health.OnValueChanged += OnBaseHealthChanged;

            health.Enemy.Value = Enemy.Base.Health.Rate;
            Enemy.Base.Health.OnValueChanged += OnBaseHealthChanged;

            Modules.Init(this);
        }

        void OnBaseHealthChanged(float value)
        {
            health.Player.Value = Player.Base.Health.Rate;

            health.Enemy.Value = Enemy.Base.Health.Rate;
        }

        void OnFundsChanged()
        {
            funds.Set(Player);
        }

        public virtual void SetAge(Age age)
        {

        }
	}
}