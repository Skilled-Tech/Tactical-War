using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

namespace Game
{
	public class Book : MonoBehaviour
	{
        [SerializeField]
        protected RegionCore region;
        public RegionCore Region { get { return region; } }

        public Page[] Pages { get; protected set; }

        public int Index { get; protected set; }

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

        private void Awake()
        {
            
        }

        private void Start()
        {
            Pages = GetComponentsInChildren<Page>(true);

            region.OnShowStory += Begin;

            for (int i = 0; i < Pages.Length; i++)
            {
                Pages[i].Init(this);

                Pages[i].Visible = false;
            }

            Visible = false;
        }

        public void Begin()
        {
            Visible = true;

            Begin(0);
        }

        public void Begin(Page page)
        {
            Index = Array.IndexOf(Pages, page);

            Begin(Index);
        }
        void Begin(int index)
        {
            this.Index = index;

            Pages[Index].OnClose.AddListener(PageEndCallback);

            Pages[Index].Show();
        }

        private void PageEndCallback()
        {
            Pages[Index].OnClose.RemoveListener(PageEndCallback);

            Next();
        }

        public void Next()
        {
            if(Index + 1 >= Pages.Length)
            {
                Close();

                Index = Pages.Length;
            }
            else
            {
                Begin(Index + 1);
            }
        }

        public event Action OnClose;
        void Close()
        {
            Visible = false;

            OnClose?.Invoke();
        }
    }
}