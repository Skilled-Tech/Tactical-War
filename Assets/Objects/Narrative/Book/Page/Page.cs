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
        public Book Book { get; protected set; }
        public void Init(Book book)
        {
            this.Book = book;

            Index = Array.IndexOf(book.Pages, this);
        }
        public int Index { get; protected set; }

        [SerializeField]
        protected Flowchart flowchart;
        public Flowchart Flowchart { get { return flowchart; } }

        private void Reset()
        {
            flowchart = Dependancy.Get<Flowchart>(gameObject);

            if(flowchart == null)
            {
                var gameObject = new GameObject(nameof(Flowchart));

                gameObject.transform.SetParent(transform);

                flowchart = gameObject.AddComponent<Flowchart>();
            }
        }

        public bool Visible
        {
            get
            {
                return gameObject.activeSelf;
            }
            set
            {
                gameObject.SetActive(value);
            }
        }

        public UnityEvent OnShow;
		public void Show()
        {
            Visible = true;

            flowchart.Execute();

            OnShow.Invoke();
        }

        public UnityEvent OnClose;
        public void Close()
        {
            Visible = false;

            OnClose.Invoke();
        }
	}
}