#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
#endregion

namespace GameStateManagement
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

        MenuEntry SoundMenuEntry;

        enum SoundOption
        {
            Off,
            On,
        }

        static SoundOption currentOption = SoundOption.On;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen()
            : base("Options")
        {
            // Create our menu entries.
            SoundMenuEntry = new MenuEntry(string.Empty);
            SetMenuEntryText();

            if (currentOption == SoundOption.On)
            {
                ScreenManager.IsSound = true;
            }
            else 
            {
                ScreenManager.IsSound = false;
            }

            MenuEntry back = new MenuEntry("Back");

            // Hook up menu event handlers.
            SoundMenuEntry.Selected += SoundMenuEntrySelected;
            back.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(SoundMenuEntry);
            MenuEntries.Add(back);
        }


        /// <summary>
        /// Fills in the latest values for the options screen menu text.
        /// </summary>
        void SetMenuEntryText()
        {
            SoundMenuEntry.Text =  "Sound : " + currentOption;
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void SoundMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            currentOption++;

            if (currentOption == SoundOption.On) 
            {
                
            }
            else
            {
                ScreenManager.IsSound = false;
            }

            if (currentOption > SoundOption.On)
                currentOption = 0;

            SetMenuEntryText();
        }

 


        #endregion
    }
}
