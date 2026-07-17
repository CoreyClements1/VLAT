using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VLAT
{
    public class GlobalColorManager : MonoBehaviour
    {

        // GlobalColorManager provides a centralized area for obtaining color themes


        #region VARIABLES


        public enum ColorType
        {
            Look,
            Move,
            Interact,
            Settings,
            Highlight
        }

        public Color lookColorLight;
        public Color lookColorDark;
        public Color moveColorLight;
        public Color moveColorDark;
        public Color interactColorLight;
        public Color interactColorDark;
        public Color settingsColorLight;
        public Color settingsColorDark;
        public Color highlightColor;

        public static GlobalColorManager Instance;


        #endregion


        #region MONOBEHAVIOUR


        // Awake so Instance is ready before other components' Start() calls
        //--------------------------------------//
        void Awake()
        //--------------------------------------//
        {
            InitSingleton();

        } // END Awake


        // Inits singleton
        //--------------------------------------//
        private void InitSingleton()
        //--------------------------------------//
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

        } // END InitSingleton


        #endregion


    } // END GlobalColorManager.cs
}