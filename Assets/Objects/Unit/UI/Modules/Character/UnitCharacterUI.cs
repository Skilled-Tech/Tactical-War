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
    public class UnitCharacterUI : UnitsUI.Module
    {
        [SerializeField]
        protected TMP_Text label;
        public TMP_Text Label { get { return label; } }

        [SerializeField]
        protected TMP_Text type;
        public TMP_Text Type { get { return type; } }

        [SerializeField]
        protected Image illustration;
        public Image Illustration { get { return illustration; } }

        public UnitTemplate Data { get; protected set; }

        public UnitTemplate Template { get; protected set; }
        public virtual void Set(UnitTemplate data)
        {
            this.Template = data;

            if(data.Illustration.Sprite == null)
            {
                illustration.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                illustration.color = Color.white;

                illustration.sprite = data.Illustration.Sprite;
            }

            UpdateIllustration();

            UpdateState();
        }

        void UpdateIllustration()
        {
            if (Template != null)
            {
                illustration.rectTransform.localPosition = Template.Illustration.Offset;

                illustration.rectTransform.localScale = Vector3.one * Template.Illustration.Scale;
            }
        }

        void Update()
        {
            UpdateIllustration();
        }

        protected virtual void UpdateState()
        {
            label.text = Template.DisplayName.Text;

            type.text = Template.Type.DisplayName.Text;
        }
    }
}