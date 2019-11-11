using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Game
{
	public class Page : MonoBehaviour
	{
        Book book;
        int index;
        public void Init(Book book)
        {
            this.book = book;

            index = book.Pages.IndexOf(this);
        }

        public GameObject scene;

        public bool Visible
        {
            get
            {
                return gameObject.activeSelf;
            }
            set
            {
                gameObject.SetActive(value);

                if (scene != null)
                    scene.SetActive(value);
            }
        }

        public UnityEvent OnBegin;
		public void Begin()
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;

            Visible = true;

            OnBegin.Invoke();
        }

        public UnityEvent OnEnd;
        public void End()
        {
            OnEnd.Invoke();
        }
	}
}