using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Scenes;
using PolyOne.Components;

namespace Issho
{
    public class GravityItemUpDown : GravityItem
    {
        public GravityItemUpDown(Vector2 position) :
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
            arrowDown = Color.White;
            arrowUp = Color.White;


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

            if (Direction == GravityDirection.Up && counterStart == true) {
                arrowRight = Color.Red;
                Done = true;

                velocity.Y -= gravity;
            }
            else if (Direction == GravityDirection.Down && counterStart == true)
            {
                arrowLeft = Color.Red;
                Done = true;
                velocity.Y += gravity;
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