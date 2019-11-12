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
    [RequireComponent(typeof(Slider))]
	public class LocalizedSliderDirection : LocalizationBehaviour.DataModifer<Slider, Slider.Direction, LocalizedSliderDirection.Element>
    {
        public override Slider.Direction Value
        {
            get => Component.direction;
            protected set => Component.direction = value;
        }

        [Serializable]
        public class Element : Element<Slider.Direction> { }
    }
}