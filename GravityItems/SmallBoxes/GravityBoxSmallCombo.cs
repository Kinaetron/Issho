using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Collision;

namespace Issho
{

    public class GravityBoxSmallCombo : GravityItemCombo
    {

        public GravityBoxSmallCombo(Vector2 position) :
            base(position)
        {
            this.Tag((int)GameTags.GravityBoxSmall);
            texture = Engine.Instance.Content.Load<Texture2D>("GravityBoxSmall");
            arrow = Engine.Instance.Content.Load<Texture2D>("Arrow");
            this.Collider = new Hitbox((float)20.0f, (float)16.0f, 0.0f, 0.0f);
            this.Visible = true;

            gravity = 0.3f;
            climbspeed = 2.0f;
            fallspeed = 1.5f;
            GravityTime = 2000.0f;

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
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 4, Position.Y - 6), arrowUp);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 16, Position.Y + 23), null, arrowDown, MathHelper.Pi, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 27, Position.Y + 3), null, arrowRight, MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X - 7, Position.Y + 15), null, arrowLeft, -MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            base.Draw();
        }
    }
}
