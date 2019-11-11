using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class SayNode : Node, IDialog
	{
        [SerializeField]
        Character character = null;
        public Character Character => character;

        [SerializeField]
        string text = "";
        public string Text => text;

        [SerializeField]
        bool waitForClick = true;

        public DialogUI SayDialog => Level.Instance.SayDialog;

        public override void Execute()
        {
            base.Execute();

            if(waitForClick)
                SayDialog.OnProgress += SayDialogProgressCallback;

            SayDialog.Show(this);

            if (waitForClick == false)
                End();
        }

        private void SayDialogProgressCallback()
        {
            SayDialog.OnProgress -= SayDialogProgressCallback;

            End();
        }
    }
}