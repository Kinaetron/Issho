using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

using PolyOne.Engine;

namespace Issho.Screens
{
    class MainMenuScreen : MenuScreen
    {
        private Song titleTrack;

        public MainMenuScreen()
            : base("Main Menu")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("Play Game");
            MenuEntry optionsMenuEntry = new MenuEntry("Options");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            titleTrack = Engine.Instance.Content.Load<Song>("Sounds/title");
            MediaPlayer.Play(titleTrack);
        }

        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MediaPlayer.Stop();

            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex,
                               new GameplayScreen());
        }

        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        protected override void OnCancel(PlayerIndex playerIndex)
        {
            ScreenManager.Game.Exit();
        }
    }
}
