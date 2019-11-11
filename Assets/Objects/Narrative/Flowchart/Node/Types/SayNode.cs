using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class SayNode : Node, IDialog
	{
        [SerializeField]
        string text = "";
        public string Text => text;

        [SerializeField]
        bool waitForClick = true;

        public DialogUI DialogUI => Core.Instance.UI.Dialog;

        public override void Execute()
        {
            base.Execute();

            if(waitForClick)
                DialogUI.OnProgress += SayDialogProgressCallback;

            DialogUI.Show(this);

            if (waitForClick == false)
                End();
        }

        private void SayDialogProgressCallback()
        {
            DialogUI.OnProgress -= SayDialogProgressCallback;

            End();
        }
    }
}