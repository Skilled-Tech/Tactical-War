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

using PlayFab.ClientModels;

namespace Game
{
    public class ItemInstanceUITemplate : UIElement
    {
        [SerializeField]
        protected Image icon;
        public Image Icon { get { return icon; } }

        [SerializeField]
        protected TMP_Text uses;
        public TMP_Text Uses { get { return uses; } }

        public virtual void Init()
        {

        }

        public virtual void Set(PlayerInventoryCore.ItemData itemData)
        {
            icon.sprite = itemData.Template.Icon;

            if (itemData.Instance.RemainingUses.HasValue)
            {
                uses.gameObject.SetActive(true);

                uses.text = "x" + itemData.Instance.RemainingUses.ToString();
            }
            else
            {
                uses.gameObject.SetActive(false);
            }
        }
    }
}