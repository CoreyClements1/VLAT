using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VLAT
{
    public class HighlightableTab : MonoBehaviour
    {

        // FolderTab handles a single folder tab's components


        #region VARIABLES


        public RectTransform rectTrans;
        public CanvasGroup highlightCanvGroup;
        public TMP_Text buttonText;
        private bool highlighting = false;


        #endregion


        #region HIGHLIGHTING


        // Starts highlighting
        //--------------------------------------//
        public void StartHighlight()
        //--------------------------------------//
        {
            if (!highlighting)
            {
                highlighting = true;
                highlightCanvGroup.alpha = .0f;
                highlightCanvGroup.TweenAlpha(1f, 0.25f).SetLoopPingPong();
            }

        } // END StartHighlighting


        // Ends highlighting
        //--------------------------------------//
        public void EndHighlight()
        //--------------------------------------//
        {
            if (highlighting)
            {
                highlighting = false;
                VLATTween.Cancel(highlightCanvGroup.gameObject);
                highlightCanvGroup.TweenAlpha(0f, 0.25f);
            }

        } // END EndHighlight


        #endregion


    } // END FolderTab.cs
}