using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Scenes;
using PolyOne.Components;

namespace Issho
{
    public class GravityItemLeftRight : GravityItem
    {
        public GravityItemLeftRight(Vector2 position) :
            base(position)
        {
            this.Add(counters);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            if (base.Scene is Level)
            {
                this.level = (base.Scene as Level);
            }
        }

        protected override void GravityPull()
        {
            arrowRight = Color.White;
            arrowLeft = Color.White;


            if (GravitySet == true && counterStart == false)
            {
                counters["gravityTime"] = GravityTime;
                counterStart = true;
                velocity = Vector2.Zero;
            }

            if (counters.Check("gravityTime") == false && counterStart == true)
            {
                Direction = GravityDirection.Down;
                velocity = Vector2.Zero;
                counterStart = false;
                stop = false;
            }

           if (Direction == GravityDirection.Right && counterStart == true)
            {
                arrowRight = Color.Red;
                Done = true;

                if (stop == false)
                    velocity.X += gravity;
            }
            else if (Direction == GravityDirection.Left && counterStart == true)
            {
                arrowLeft = Color.Red;
                Done = true;

                if (stop == false)
                    velocity.X -= gravity;
            }
            else if(isOnGround == false) {
                velocity.Y += gravity;
            }

            velocity.X = MathHelper.Clamp(velocity.X, -climbspeed, climbspeed);
            velocity.Y = MathHelper.Clamp(velocity.Y, -climbspeed, fallspeed);
        }

        protected override void MovementHorizontal(float amount)
        {
            base.MovementHorizontal(amount);
        }

        protected override void MovementVerical(float amount)
        {
            base.MovementVerical(amount);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
