using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne;
using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Components;

namespace Issho
{
    public enum GravityDirection
    {
        Down = 0,
        Up = 1,
        Right = 2,
        Left = 3
    }

    public class GravityItem : Entity
    {
        protected Texture2D arrow;
        protected Color arrowDown = Color.White;
        protected Color arrowUp = Color.White;
        protected Color arrowLeft = Color.White;
        protected Color arrowRight = Color.White;

        protected Texture2D texture;
        protected Vector2 remainder;

        public Vector2 Velocity
        {
            get { return velocity; }
        }
        protected Vector2 velocity;

        protected float gravity;
        protected float fallspeed;
        protected float climbspeed;

        protected bool counterStart;
        protected CounterSet<string> counters = new CounterSet<string>();


        public float GravityTime { get; protected set; }
        public GravityDirection Direction { get; set; }
        public bool GravitySwitch { get; set; }
        public bool GravitySet { get; set; }
        public bool Done { get; set; }
        public bool PlayerIsOnX { get; protected set; }
        public bool PlayerIsOnY { get; protected set; }

        public float Distance { get; private set; }
        protected Level level;
 
        protected bool stop;
        protected bool isOnGround;

        private int offSet = 20;

        public GravityItem(Vector2 position) :
            base(position)
        {
            this.Add(counters);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            if (base.Scene is Level) {
                this.level = (base.Scene as Level);
            }
        }

        protected virtual void GravityPull()
        {

        }

        public override void Update()
        {
            CheckGravityDone();
            DistanceCalculation();
            GravityPull();
            MovementHorizontal(velocity.X);
            MovementVerical(velocity.Y);
            CheckCollisionOnPlayer();
            IsOffLevel();
            base.Update();
        }

        private void CheckGravityDone()
        {
            if(Done == true)
            {
                GravitySwitch = false;
                GravitySet = false;
                Done = false;
            }
        }

        private void DistanceCalculation()
        {
            Vector2 distanceVector = level.Player.Position - Position;
            Distance = distanceVector.Length();
        }

        protected virtual void MovementHorizontal(float amount)
        {
            remainder.X += amount;
            int move = (int)Math.Round((double)remainder.X);

            if (move != 0)
            {
                remainder.X -= move;
                int sign = Math.Sign(move);

                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(sign, 0);

                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmall, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallUpDown, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallLeftRight, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMedium, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumUpDown, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumLeftRight, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.Enemy, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.EnemyUpDown, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.EnemyLeftRight, newPosition) != null)
                    {
                        remainder.X = 0;
                        velocity.X = 0;
                        stop = true;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.Player, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }

                    Position.X += sign;
                    move -= sign;
                }
            }
        }

        protected virtual void MovementVerical(float amount)
        {
            remainder.Y += amount;
            int move = (int)Math.Round((double)remainder.Y);

            if (move < 0)
            {
                remainder.Y -= move;
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, -1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmall, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMedium, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.Enemy, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.EnemyUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.EnemyLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.Player, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    Position.Y += -1.0f;
                    move -= -1;
                }
            }
            else if (move > 0)
            {
                while (move != 0)
                {
                    Vector2 newPosition = Position + new Vector2(0, 1.0f);
                    if (this.CollideFirst((int)GameTags.Solid, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmall, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }


                    if (this.CollideFirst((int)GameTags.GravityBoxMedium, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.Enemy, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.EnemyUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.EnemyLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        velocity.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.Player, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }
                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        private void CheckCollisionOnPlayer()
        {
            isOnGround = base.CollideCheck((int)GameTags.Solid, this.Position + Vector2.UnitY);

            PlayerIsOnX = base.CollideCheck((int)GameTags.Player, this.Position + Vector2.UnitX);

            if (PlayerIsOnX == false) {
                PlayerIsOnX = base.CollideCheck((int)GameTags.Player, this.Position - Vector2.UnitX);
            }

            PlayerIsOnY = base.CollideCheck((int)GameTags.Player, this.Position + Vector2.UnitY);

            if (PlayerIsOnY == false) {
                PlayerIsOnY = base.CollideCheck((int)GameTags.Player, this.Position - Vector2.UnitY);
            }
        }

        private void IsOffLevel()
        {
            if (Position.Y > level.tile.MapHeightInPixels + offSet)
            {
                this.Visible = false;
                this.Active = false;
                this.RemoveSelf();
            }
        }

        public override void Draw()
        {
            base.Draw();
        }
    }
}
