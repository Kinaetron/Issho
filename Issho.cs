using Microsoft.Xna.Framework;

using PolyOne.Engine;
using PolyOne.Utility;

using Issho.Screens;

namespace Issho
{
    public class Issho : Engine
    {
        static readonly string[] preloadAssets =
        {
            "MenuAssets/gradient",
        };

        public Issho()
            : base(640, 360, "Issho", 2.0f, false)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            TileInformation.TileDiemensions(16, 16);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            foreach (string asset in preloadAssets)
            {
                Engine.Instance.Content.Load<object>(asset);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Issho game = new Issho())
            {
                game.Run();
            }
        }
    }
}
