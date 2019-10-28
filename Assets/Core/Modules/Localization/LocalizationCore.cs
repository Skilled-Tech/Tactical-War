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

                if (OnTargetChange != null) OnTargetChange(target);
            }
        }
        public delegate void TargetChangeDelegate(LocalizationType target);
        public event TargetChangeDelegate OnTargetChange;

        public class Property : Core.Property
        {
            public LocalizationCore Localization => Core.Localization;
        }

        [SerializeField]
        protected LocalizationPhrasesCore phrases;
        public LocalizationPhrasesCore Phrases { get { return phrases; } }

        public override void Configure()
        {
            base.Configure();

            Register(phrases);
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