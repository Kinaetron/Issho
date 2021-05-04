using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Collision;
using PolyOne.Scenes;
using PolyOne.Utility;
using PolyOne.Animation;
using PolyOne.Engine;

namespace Issho
{
    public class Exit : Entity
    {
        public bool Reached { get; set; }


        private bool intialReach;
        private Level level;

        private AnimationPlayer sprite;
        private Texture2D doorTexture;
        private AnimationData doorAnimation;

        public Exit(Vector2 position) :
            base(position)
        {
            this.Tag((int)GameTags.Exit);
            this.Collider = new Hitbox((float)8.0f, (float)16.0f, 0.0f, 0.0f);
            this.Visible = true;

            doorTexture = Engine.Instance.Content.Load<Texture2D>("Door");
            doorAnimation = new AnimationData(doorTexture, 100, 20, false);

             sprite = new AnimationPlayer();
             sprite.PlayAnimation(doorAnimation);
             sprite.Stop = true;
        }


        public override void Added(Scene scene)
        {
            base.Added(scene);

            if (base.Scene is Level) {
                this.level = (base.Scene as Level);
            }
        }

        public override void Update()
        {
            intialReach = base.CollideCheck((int)GameTags.Player, Position);
            base.Update();
        }

        public override void Draw()
        {
           
            if (intialReach == true)
            {
                sprite.Stop = false;

                if (doorAnimation.AnimationFinished == true) {
                    level.Player.StopRendering = true;
                    Reached = true;
                }
            }

            sprite.Draw(new Vector2(Position.X + 10, Position.Y + 3), 0.0f, SpriteEffects.None);
            base.Draw();
        }
    }
}
