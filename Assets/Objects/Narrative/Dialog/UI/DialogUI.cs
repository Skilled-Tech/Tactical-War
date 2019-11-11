using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game
{
	public class DialogUI : UIElement, IPointerClickHandler
	{
        [SerializeField]
        TMP_Text text = null;

        public virtual void Show(IDialog dialog)
        {
            character.gameObject.SetActive(dialog.Character != null);

            if (dialog.Character == null)
            {
                
            }
            else
            {
                character.text = dialog.Character.DisplayName + ":";
            }

            text.text = dialog.Text;

            Show();
        }

        public event Action OnProgress;
        public void OnPointerClick(PointerEventData eventData)
        {
            OnProgress?.Invoke();
        }
    }
}