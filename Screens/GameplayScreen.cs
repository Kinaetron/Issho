using System;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using PolyOne.ScreenManager;
using PolyOne.Input;
using PolyOne.Utility;
using PolyOne.Engine;

namespace Issho.Screens
{
    class GameplayScreen : GameScreen
    {
        private float pauseAlpha;
        private Level level = new Level();
        private string levelName;
        private bool nextLevel;

        private string saveFileName = @"Issho.sav";

        SaveData saveData; 
        SaveData preSaveData;
        SaveSystem<SaveData> saveSystem = new SaveSystem<SaveData>();

        private Song inGameTrack;

        public GameplayScreen(string levelName = "Introduction", bool nextLevel = false)
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);


            this.levelName = levelName;
            this.nextLevel = nextLevel;
        }

        public override void LoadContent()
        {
            ScreenManager.Game.ResetElapsedTime();

            if(saveSystem.Exists(saveFileName) && nextLevel == false)
            {
                saveData = saveSystem.Load(saveFileName);

                levelName = saveData.LevelName;
                level.CurrentPlayerPosition = saveData.LevelPosition;
            }
            else {
                saveSystem.Save(saveFileName, new SaveData(levelName, Vector2.Zero));
            }

            level.LoadLevel(levelName);

            inGameTrack = Engine.Instance.Content.Load<Song>("Sounds/ingame");
            MediaPlayer.Play(inGameTrack);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (coveredByOtherScreen) {
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            }
            else {
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);
            }
               

            if (IsActive == true)
            {
                if(MediaPlayer.State == MediaState.Paused) {
                    MediaPlayer.Resume();
                }

                level.Update();
            }
            else {
                MediaPlayer.Pause();
            }

           if(level.Player.IsDead == true) {

                LoadingScreen.Load(ScreenManager, true, ControllingPlayer,
                            new GameplayScreen(levelName));
            }

            foreach (Checkpoint checkpoint in level.Checkpoints)
            {
                if(checkpoint.Reached == true)
                {
                    saveData = new SaveData(levelName, new Vector2(checkpoint.Position.X, checkpoint.Bottom - 20));
                    saveSystem.Save(saveFileName, saveData);
                    preSaveData = saveData;
                }
            }

           if(level.Exit.Reached == true || PolyInput.Keyboard.Pressed(Keys.N) == true) {
                level.Exit.Reached = false;

                if(level.NextLevel == null) {
                    saveSystem.Delete(saveFileName);

                    Engine.Instance.Content.Unload();

                    LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                                   new MainMenuScreen());
                }
                else {

                    LoadingScreen.Load(ScreenManager, true, ControllingPlayer,
                        new GameplayScreen(level.NextLevel, true));
                }
            }
        }

        public override void HandleInput(InputMenuState input)
        {
            if (input.IsPauseGame(ControllingPlayer)) {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            level.Draw();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);
                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
    }
}
