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

        public Level Level { get { return Level.Instance; } }
        public LevelProponents Proponents { get { return Level.Proponents; } }
        public PlayerProponent Player { get { return Proponents.Player; } }

        void Start()
        {
            button.onClick.AddListener(OnClick);
        }

        public UnitData Data { get; protected set; }
        public virtual void Set(UnitData data)
        {
            this.Data = data;

            label.text = data.name;
            image.sprite = data.Sprite;
        }

        BaseUnitCreator.Procedure currentProcedure;

        void OnClick()
        {
            currentProcedure = Player.Base.UnitCreator.ReadyUp(Data);

            StartCoroutine(DeploymentProcedure());
        }

        IEnumerator DeploymentProcedure()
        {
            button.interactable = false;

            while(currentProcedure.Timer > 0f)
            {
                Progress.Value = currentProcedure.Rate;

                yield return new WaitForEndOfFrame();
            }

            Progress.Value = 1f;

            button.interactable = true;

            currentProcedure = null;
        }
    }
}