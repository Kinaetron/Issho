using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne.Engine;
using PolyOne.ScreenManager;
using PolyOne.Utility;
using PolyOne.Animation;


namespace Issho.Screens
{
    class LoadingScreen : GameScreen
    {
        bool loadingIsSlow;
        bool otherScreensAreGone;

        protected static AnimationPlayer sprite;
        protected static AnimationData loadingAnimation;

        GameScreen[] screensToLoad;

        private LoadingScreen(ScreenManager screenManager, bool loadingIsSlow,
                             GameScreen[] screensToLoad)
        {
            this.loadingIsSlow = loadingIsSlow;
            this.screensToLoad = screensToLoad;

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
        }

        public static void Load(ScreenManager screenManager, bool loadingIsSlow,
                              PlayerIndex? controllingPlayer,
                              params GameScreen[] screensToLoad)
        {
            foreach (GameScreen screen in screenManager.GetScreens())
                screen.ExitScreen();

            LoadingScreen loadingScreen = new LoadingScreen(screenManager,
                                                            loadingIsSlow,
                                                            screensToLoad);

            screenManager.AddScreen(loadingScreen, controllingPlayer);

            sprite = new AnimationPlayer();
            loadingAnimation = new AnimationData(Engine.Instance.Content.Load<Texture2D>("LoadingAnimation"), 150, 32, true);
            sprite.PlayAnimation(loadingAnimation);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                    bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (otherScreensAreGone)
            {
                ScreenManager.RemoveScreen(this);

                foreach (GameScreen screen in screensToLoad)
                {
                    if (screen != null)
                    {
                        ScreenManager.AddScreen(screen, ControllingPlayer);
                    }
                }

                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            if ((ScreenState == ScreenState.Active) &&
                (ScreenManager.GetScreens().Length == 1))
            {
                otherScreensAreGone = true;
            }

            if (loadingIsSlow)
            {
                Vector2 loadingPosition = new Vector2(Engine.VirtualWidth - 16, Engine.VirtualHeight - 16);
                Color color = Color.White * TransitionAlpha;

                Engine.Begin(Resolution.GetScaleMatrix);
                sprite.Draw(loadingPosition, 0f, SpriteEffects.None, color);
                Engine.End();
            }
        }
    }
}
