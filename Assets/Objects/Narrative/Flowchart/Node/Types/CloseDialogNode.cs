using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
	public class CloseDialogNode : Node
	{
        public DialogUI SayDialog => Level.Instance.SayDialog;

        public override void Execute()
        {
            base.Execute();

            SayDialog.Hide();

            End();
        }
    }
}