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
	public class PlayerHUD : MonoBehaviour
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

        public interface IReference : IReference<PlayerHUD> { }
        public abstract class Reference : Reference<PlayerHUD>
        {
            public PlayerHUD HUD { get { return Data; } }
        }

        public LevelProponents Proponents { get { return Level.Instance.Proponents; } }

        public PlayerProponent Player { get { return Proponents.Player; } }
        public Proponent Enemy { get { return Proponents.Enemy; } }

        PlayerProponent player;
        public virtual void Init(PlayerProponent player)
        {
            this.player = player;

            References.Init(this);

            Age = Dependancy.Get<PlayerHUDAge>(gameObject);
        }

        protected virtual void Start()
        {
            player.Funds.OnValueChanged += OnFundsChanged;
            funds.Set(player);

            health.Player.Value = Player.Base.Health.Rate;
            Player.Base.Health.OnValueChanged += OnBaseHealthChanged;

            health.Enemy.Value = Enemy.Base.Health.Rate;
            Enemy.Base.Health.OnValueChanged += OnBaseHealthChanged;

            Age.Set(player.Age);
        }

        void OnBaseHealthChanged(float value)
        {
            health.Player.Value = Player.Base.Health.Rate;

            health.Enemy.Value = Enemy.Base.Health.Rate;
        }

        void OnFundsChanged()
        {
            funds.Set(player);
        }

        public virtual void SetAge(Age age)
        {

        }
	}
}