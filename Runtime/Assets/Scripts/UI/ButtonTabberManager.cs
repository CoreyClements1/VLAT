using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLAT
{
    public class ButtonTabberManager : MonoBehaviour
    {

        // FolderTabsManager manages the folder tabs section of the UI


        #region VARIABLES


        [SerializeField] private List<HighlightableTab> highlightableTabs = new List<HighlightableTab>();
        public static float tabMoveTime = .25f;
        private int currentActiveTab;


        #endregion


        #region CONTROL


        // Resets tabbing process
        //--------------------------------------//
        public void ResetTabbing()
        //--------------------------------------//
        {
            if (highlightableTabs.Count == 0)
                return;

            if (IsTabNavigable(currentActiveTab))
            {
                VLATTween.Cancel(highlightableTabs[currentActiveTab].highlightCanvGroup.gameObject);
                highlightableTabs[currentActiveTab].highlightCanvGroup.alpha = 0f;
            }

            currentActiveTab = FindFirstActiveTab();
            BeginTabbing();

        } // END ResetTabbing


        // Begins the tabbing process between folder tabs
        //--------------------------------------//
        public void BeginTabbing()
        //--------------------------------------//
        {
            if (!IsTabNavigable(currentActiveTab))
                currentActiveTab = FindFirstActiveTab();

            if (!IsTabNavigable(currentActiveTab))
                return;

            highlightableTabs[currentActiveTab].highlightCanvGroup.alpha = .0f;
            highlightableTabs[currentActiveTab].highlightCanvGroup.TweenAlpha(1f, tabMoveTime).SetLoopPingPong();

        } // END BeginTabbing


        // Stops the tabbing process between folder tabs
        //--------------------------------------//
        public void EndTabbing()
        //--------------------------------------//
        {
            if (!IsTabNavigable(currentActiveTab))
                return;

            VLATTween.Cancel(highlightableTabs[currentActiveTab].highlightCanvGroup.gameObject);
            highlightableTabs[currentActiveTab].highlightCanvGroup.TweenAlpha(0f, tabMoveTime);

        } // END EndTabbing


        private int FindFirstActiveTab()
        {
            for (int i = 0; i < highlightableTabs.Count; i++)
            {
                if (IsTabNavigable(i))
                    return i;
            }

            return 0;
        }


        // Tabs the folders one to the left, returns the new active tab
        //--------------------------------------//
        public int TabLeft()
        //--------------------------------------//
        {
            EndTabbing();
            currentActiveTab = FindNextActiveTab(-1);
            BeginTabbing();

            return currentActiveTab;

        } // END TabLeft


        // Tabs the folders one to the right, returns the new active tab
        //--------------------------------------//
        public int TabRight()
        //--------------------------------------//
        {
            EndTabbing();
            currentActiveTab = FindNextActiveTab(1);
            BeginTabbing();

            return currentActiveTab;

        } // END TabRight


        private int FindNextActiveTab(int direction)
        {
            if (highlightableTabs.Count == 0)
                return 0;

            int index = currentActiveTab;
            for (int i = 0; i < highlightableTabs.Count; i++)
            {
                index += direction;
                if (index < 0)
                    index = highlightableTabs.Count - 1;
                else if (index >= highlightableTabs.Count)
                    index = 0;

                if (IsTabNavigable(index))
                    return index;
            }

            return currentActiveTab;
        }


        private bool IsTabNavigable(int index)
        {
            if (index < 0 || index >= highlightableTabs.Count)
                return false;

            HighlightableTab tab = highlightableTabs[index];
            return tab != null && tab.gameObject.activeInHierarchy;
        }


        #endregion


        #region CLEARING AND ADDING TABS


        // Clears the list of tabs
        //--------------------------------------//
        public void ClearTabs()
        //--------------------------------------//
        {
            for (int i = 0; i < highlightableTabs.Count; i++)
            {
                VLATTween.Cancel(highlightableTabs[i].gameObject);
                Destroy(highlightableTabs[i].gameObject);
            }

            highlightableTabs.Clear();
            currentActiveTab = 0;

        } // END ClearTabs


        // Adds to the list of tabs
        //--------------------------------------//
        public void AddTab(HighlightableTab tab)
        //--------------------------------------//
        {
            highlightableTabs.Add(tab);

        } // END AddTab


        // Returns tab at index
        //--------------------------------------//
        public HighlightableTab GetTab(int index)
        //--------------------------------------//
        {
            return highlightableTabs[index];

        } // END GetTab


        #endregion


    } // END ButtonTabberManager.cs
}