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

using RTLTMPro;

namespace Game
{
    [RequireComponent(typeof(RTLTextMeshPro))]
	public class TMPLocalizedText : MonoBehaviour
	{
        [SerializeField]
        string _ID;
        public string ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
            }
        }

        [SerializeField]
        protected FontData font;
        public FontData Font { get { return font; } }
        [Serializable]
        public class FontData
        {
            [SerializeField]
            protected TMP_FontAsset _default;
            public TMP_FontAsset Default { get { return _default; } }

            [SerializeField]
            protected ElementData[] elements;
            public ElementData[] Elements { get { return elements; } }

            public virtual TMP_FontAsset Get(LocalizationType localization)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].Localization == localization)
                        return elements[i].Asset;
                }

                return Default;
            }

            [Serializable]
            public class ElementData
            {
                [SerializeField]
                protected LocalizationType localization;
                public LocalizationType Localization { get { return localization; } }

                [SerializeField]
                protected TMP_FontAsset asset;
                public TMP_FontAsset Asset { get { return asset; } }
            }
        }

		public RTLTextMeshPro Label { get; protected set; }

        public LocalizedPhrase Phrase { get; protected set; }

        public Core Core => Core.Instance;
        public LocalizationCore Localization => Core.Localization;

        private void Reset()
        {
            Label = GetComponent<RTLTextMeshPro>();

            ID = Label.text.ToLower();
        }

        private void Start()
        {
            Label = GetComponent<RTLTextMeshPro>();

            Phrase = Localization.Phrases.Get(ID);

            UpdateState();

            Localization.OnTargetChange += LocalizationTargetChangeCallback;
        }

        private void LocalizationTargetChangeCallback(LocalizationType target)
        {
            UpdateState();
        }

        void UpdateState()
        {
            Label.text = Phrase[Localization.Target];

            Label.font = font.Get(Localization.Target);
        }
    }
}