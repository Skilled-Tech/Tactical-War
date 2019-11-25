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

namespace Game
{
    [Serializable]
	public class LocalizationCore : Core.Property
	{
        public const string Key = "Localization";

        [SerializeField]
        protected LocalizationType target = LocalizationType.English;
        public LocalizationType Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;

                Core.Prefs.Set(Key, target);

                if (OnTargetChange != null) OnTargetChange(target);
            }
        }
        public delegate void TargetChangeDelegate(LocalizationType target);
        public event TargetChangeDelegate OnTargetChange;

        public virtual void Progress()
        {
            IList values = Enum.GetValues(typeof(LocalizationType));

            var index = values.IndexOf(target);

            if (index + 1 < values.Count)
                Target = (LocalizationType)values[index + 1];
            else
                Target = (LocalizationType)values[0];
        }

        public class Property : Core.Property
        {
            public LocalizationCore Localization => Core.Localization;
        }

        [SerializeField]
        protected LocalizationPhrasesCore phrases;
        public LocalizationPhrasesCore Phrases { get { return phrases; } }

        [SerializeField]
        protected LocalizationPresetsCore presets;
        public LocalizationPresetsCore Presets { get { return presets; } }

        public override void Configure()
        {
            base.Configure();

            Target = Core.Prefs.GetEnum(Key, target);

            Register(phrases);
            Register(presets);
        }
    }

    public static class LocalizationCode
    {
        static readonly ElementData[] list = new ElementData[]
        {
            new ElementData(LocalizationType.English, "en"),
            new ElementData(LocalizationType.Arabic, "ar"),
        };
        [Serializable]
        public struct ElementData
        {
            public LocalizationType Type { get; private set; }
            public string Code { get; private set; }

            public ElementData(LocalizationType type, string code)
            {
                this.Type = type;
                this.Code = code;
            }
        }

        public static string From(LocalizationType type)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].Type == type)
                    return list[i].Code;

            throw new NotImplementedException();
        }

        public static LocalizationType To(string code)
        {
            for (int i = 0; i < list.Length; i++)
                if (list[i].Code == code)
                    return list[i].Type;

            throw new NotImplementedException();
        }
    }

    public enum LocalizationType
    {
        English, Arabic
    }
}