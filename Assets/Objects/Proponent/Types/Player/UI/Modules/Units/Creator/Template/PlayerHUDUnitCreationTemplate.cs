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
	public class PlayerHUDUnitCreationTemplate : MonoBehaviour
	{
		[SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } } 

        [SerializeField]
        protected Image image;
        public Image Image { get { return image; } }

        [SerializeField]
        protected Button button;
        public Button Button { get { return button; } } 

        [SerializeField]
        protected ProgressBar progress;
        public ProgressBar Progress { get { return progress; } }


        public Proponent Player { get; protected set; }
        public UnitData Data { get; protected set; }

        public Level Level { get { return Level.Instance; } }

        void Start()
        {
            button.onClick.AddListener(OnClick);

            UpdateState();
        }
        
        public void UpdateState()
        {
            if (Deployment == null)
            {
                button.interactable = Player.Base.Units.Creator.CanDeploy(Data);
            }
        }

        
        public virtual void Set(PlayerProponent player, UnitData data)
        {
            this.Player = player;
            this.Data = data;

            label.text = data.name;
            image.sprite = data.Sprite;
        }

        void OnClick()
        {
            StartCoroutine(ClickProcedure());
        }

        public BaseUnitsCreator.Deployment Deployment { get; protected set; }
        IEnumerator ClickProcedure()
        {
            Deployment = Player.Base.Units.Creator.Deploy(Data);

            button.interactable = false;

            while(Deployment.Timer > 0f)
            {
                Progress.Value = Deployment.Rate;

                yield return new WaitForEndOfFrame();
            }

            Progress.Value = 1f;

            Deployment = null;

            UpdateState();
        }
    }
}