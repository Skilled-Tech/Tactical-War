using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public class BookOpenButton : MonoBehaviour
	{
        public Book target;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(Action);
        }

        void Action()
        {
            target.Open();
        }
    }
}