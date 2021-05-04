using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Collision;

namespace Issho
{

    public class GravityBoxMediumCombo : GravityItemCombo
    {
        public GravityBoxMediumCombo(Vector2 position):
            base(position)
        {
            this.Tag((int)GameTags.GravityBoxMedium);
            texture = Engine.Instance.Content.Load<Texture2D>("GravityBoxLarge");
            arrow = Engine.Instance.Content.Load<Texture2D>("Arrow");
            this.Collider = new Hitbox((float)32.0f, (float)32.0f, 0.0f, 0.0f);
            this.Visible = true;

            gravity = 0.2f;
            fallspeed = 2.0f;
            climbspeed = 1.0f;
            GravityTime = 3000.0f;

            Direction = GravityDirection.Down;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            base.Update();
        }

        protected override void MovementHorizontal(float amount)
        {
            base.MovementHorizontal(amount);
        }

        protected override void MovementVerical(float amount)
        {
            base.MovementVerical(amount);
        }

        public override void Draw()
        {
            Engine.SpriteBatch.Draw(texture, Position, Color.White);

            if (GravitySwitch == true)
            {
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 10, Position.Y - 3), arrowUp);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 22, Position.Y + 33), null, arrowDown, MathHelper.Pi, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 33, Position.Y + 10), null, arrowRight, MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X - 1, Position.Y + 22), null, arrowLeft, -MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            base.Draw();
        }
    }
}
