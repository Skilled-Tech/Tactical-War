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
    public class ProponentUnitCreationUITemplate : MonoBehaviour
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
        public UnitTemplate Template { get; protected set; }

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
                button.interactable = Player.Base.Units.Creator.CanDeploy(Template);

                GrayscaleController.Off = button.interactable;

                Progress.Value = 0f;
            }
            else
            {
                button.interactable = false;

                GrayscaleController.On = true;
            }
        }

        public virtual void Set(PlayerProponent player, UnitTemplate template)
        {
            this.Player = player;
            this.Template = template;

            template.Icon.ApplyTo(icon);

            UpdateState();
        }

        void OnClick()
        {
            StartCoroutine(ClickProcedure());
        }

        public BaseUnitsCreator.Deployment Deployment { get; protected set; }
        IEnumerator ClickProcedure()
        {
            Deployment = Player.Base.Units.Creator.Deploy(Template);

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