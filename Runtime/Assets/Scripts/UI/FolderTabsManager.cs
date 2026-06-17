using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLAT
{
    public class FolderTabsManager : MonoBehaviour
    {

        // FolderTabsManager manages the folder tabs section of the UI


        #region VARIABLES


        [SerializeField] private HighlightableTab[] folderTabs = new HighlightableTab[4];
        [SerializeField] private int hidHeight;
        [SerializeField] private int activeHeight;
        public static float tabMoveTime = .25f;
        private int currentActiveTab;


        #endregion


        #region CONTROL


        // Initially sets look active
        //--------------------------------------//
        public void InitialStart()
        //--------------------------------------//
        {
            folderTabs[0].rectTrans.TweenMoveLocalY(activeHeight, tabMoveTime).SetEaseOutQuad();

        } // END InitialStart


        // Begins the tabbing process between folder tabs
        //--------------------------------------//
        public void BeginTabbing()
        //--------------------------------------//
        {
            folderTabs[currentActiveTab].highlightCanvGroup.alpha = .0f;
            folderTabs[currentActiveTab].highlightCanvGroup.TweenAlpha(1f, tabMoveTime).SetLoopPingPong();

        } // END BeginTabbing


        // Stops the tabbing process between folder tabs
        //--------------------------------------//
        public void EndTabbing()
        //--------------------------------------//
        {
            VLATTween.Cancel(folderTabs[currentActiveTab].highlightCanvGroup.gameObject);
            folderTabs[currentActiveTab].highlightCanvGroup.TweenAlpha(0f, tabMoveTime);

        } // END EndTabbing


        // Tabs the folders one to the left, returns the new active tab
        //--------------------------------------//
        public int TabLeft()
        //--------------------------------------//
        {
            folderTabs[currentActiveTab].rectTrans.TweenMoveLocalY(hidHeight, tabMoveTime).SetEaseOutQuad();
            VLATTween.Cancel(folderTabs[currentActiveTab].highlightCanvGroup.gameObject);
            folderTabs[currentActiveTab].highlightCanvGroup.TweenAlpha(0f, tabMoveTime);

            currentActiveTab--;
            if (currentActiveTab < 0)
            {
                currentActiveTab = 3;
            }

            folderTabs[currentActiveTab].rectTrans.TweenMoveLocalY(activeHeight, tabMoveTime).SetEaseOutQuad();
            BeginTabbing();

            return currentActiveTab;

        } // END TabLeft


        // Tabs the folders one to the left, returns the new active tab
        //--------------------------------------//
        public int TabRight()
        //--------------------------------------//
        {
            folderTabs[currentActiveTab].rectTrans.TweenMoveLocalY(hidHeight, tabMoveTime).SetEaseOutQuad();
            VLATTween.Cancel(folderTabs[currentActiveTab].highlightCanvGroup.gameObject);
            folderTabs[currentActiveTab].highlightCanvGroup.TweenAlpha(0f, tabMoveTime);

            currentActiveTab++;
            if (currentActiveTab > 3)
            {
                currentActiveTab = 0;
            }

            folderTabs[currentActiveTab].rectTrans.TweenMoveLocalY(activeHeight, tabMoveTime).SetEaseOutQuad();
            BeginTabbing();

            return currentActiveTab;

        } // END TabRight


        #endregion


    } // END FolderTabsManager.cs
}