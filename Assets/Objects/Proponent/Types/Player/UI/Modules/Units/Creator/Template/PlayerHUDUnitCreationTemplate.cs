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
        protected Image icon;
        public Image Icon { get { return icon; } }

        [SerializeField]
        protected Image background;
        public Image Background { get { return background; } }

        public float Grayscale
        {
            set
            {
                icon.material.SetFloat("_EffectAmount", value);

                background.material.SetFloat("_EffectAmount", value);
            }
        }

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

            icon.material = Instantiate(icon.material);
            background.material = Instantiate(background.material);

            UpdateState();
        }

        public void UpdateState()
        {
            if (Deployment == null)
            {
                button.interactable = Player.Base.Units.Creator.CanDeploy(Data);

                Grayscale = button.interactable ? 0f : 1f;
            }
        }
        
        public virtual void Set(PlayerProponent player, UnitData data)
        {
            this.Player = player;
            this.Data = data;

            icon.sprite = data.Sprite;

            Progress.Value = 0f;
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

            Grayscale = 1f;

            while (Deployment.Timer > 0f)
            {
                Progress.Value = Deployment.Rate;

                yield return new WaitForEndOfFrame();
            }

            Grayscale = 0f;
            Progress.Value = 0f;

            Deployment = null;

            UpdateState();
        }
    }
}