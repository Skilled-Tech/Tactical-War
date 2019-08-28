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
        public Color BackgroundOrigianlColor { get; protected set; }

        public UIGrayscaleController GrayscaleController { get; protected set; }

        [SerializeField]
        protected Button button;
        public Button Button { get { return button; } }

        [SerializeField]
        protected ProgressBar progress;
        public ProgressBar Progress { get { return progress; } }

        public Proponent Player { get; protected set; }
        public UnitTemplate Data { get; protected set; }

        public Level Level { get { return Level.Instance; } }

        void Awake()
        {
            BackgroundOrigianlColor = background.color;

            GrayscaleController = new UIGrayscaleController(this);
        }
        
        void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        public void UpdateState()
        {
            if (Deployment == null)
            {
                button.interactable = Player.Base.Units.Creator.CanDeploy(Data);

                GrayscaleController.Off = button.interactable;

                Progress.Value = 0f;
            }
            else
            {
                button.interactable = false;

                GrayscaleController.On = true;
            }

            Background.color = button.interactable ? BackgroundOrigianlColor : Color.Lerp(Color.grey, Color.black, 0.6f);
        }
        
        public virtual void Set(PlayerProponent player, UnitTemplate data)
        {
            this.Player = player;
            this.Data = data;

            icon.sprite = data.Icon;

            UpdateState();
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

            Progress.Value = 1f;
            UpdateState();

            while (Deployment.Timer > 0f)
            {
                Progress.Value = Deployment.Rate;

                yield return new WaitForEndOfFrame();
            }

            Deployment = null;

            Progress.Value = 0f;

            UpdateState();
        }
    }
}