using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Collision;

namespace Issho
{
    public enum EnemyMoveDirection
    {
        Left = 0,
        Right = 1,
        Follow = 2
    }

    public class EnemyCombo : GravityItemCombo
    {
        private float startPoint;
        private float endPoint;
        private EnemyMoveDirection enemyDirection;
        private EnemyMoveDirection prevEnemyDirection;
        private SpriteEffects direction;

        private const float travelSpeed = 1.0f;
        private const float distancetoAggro = 120.0f;

        private int previousDirection;
        private bool isOnGround;
        private bool hitsObject;
        private bool isAtEdge;
        private bool aggroSwitch;
        private int distance;

        public EnemyCombo(Vector2 position, int distance = 100, 
                          EnemyMoveDirection direction = EnemyMoveDirection.Right) :
            base(position)
        {
            this.distance = distance;
            enemyDirection = direction;

            if (enemyDirection == EnemyMoveDirection.Left) {
                startPoint = Position.X - distance;
                endPoint = Position.X;
            }
            else {
                startPoint = Position.X;
                endPoint = Position.X + distance;
            }

            startPoint = Position.X;
            endPoint = Position.X + distance;

            this.Tag((int)GameTags.Enemy);
            texture = Engine.Instance.Content.Load<Texture2D>("Enemy");
            arrow = Engine.Instance.Content.Load<Texture2D>("Arrow");
            this.Collider = new Hitbox((float)20.0f, (float)10.0f, 0.0f, 0.0f);
            this.Visible = true;

            gravity = 0.3f;
            fallspeed = 1.0f;
            climbspeed = 2.5f;
            GravityTime = 3000.0f;

            Direction = GravityDirection.Down;
            enemyDirection = EnemyMoveDirection.Left;
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
        }

        public override void Update()
        {
            ConditionCheck();

            EnemyAI();

            if (Direction == GravityDirection.Left || Direction == GravityDirection.Right) {
                ResetPoints();
            }

            base.Update();
        }

        protected override void MovementHorizontal(float amount)
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

                    Position.X += sign;
                    move -= sign;
                }
            }
        }

        protected override void MovementVerical(float amount)
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

                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        }

        private void ConditionCheck()
        {
            isOnGround = base.CollideCheck((int)GameTags.Solid, this.Position + Vector2.UnitY);
            isAtEdge = base.CollideCheck((int)GameTags.Empty, new Vector2(this.Left, this.Bottom));
            hitsObject = base.CollideCheck((int)GameTags.GravityBoxSmall, this.Position + Vector2.UnitX);

            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxSmall, this.Position - Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxSmallUpDown, this.Position + Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxSmallUpDown, this.Position - Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxSmallLeftRight, this.Position + Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxSmallLeftRight, this.Position - Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxMedium, this.Position + Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxMedium, this.Position - Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxMediumUpDown, this.Position + Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxMediumUpDown, this.Position - Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxMediumLeftRight, this.Position + Vector2.UnitX);
            }
            if (hitsObject == false) {
                hitsObject = base.CollideCheck((int)GameTags.GravityBoxMediumLeftRight, this.Position - Vector2.UnitX);
            }
        }

        private void EnemyAI()
        {
            if (startPoint == endPoint) {
                return;
            }

            if (Direction == GravityDirection.Left || Direction == GravityDirection.Right) {
                return;
            }

            if (GravitySwitch == true || isOnGround == false)
            {
                velocity.X = 0f;
                return;
            }

            if (startPoint == Position.X && enemyDirection != EnemyMoveDirection.Follow)
            {
                velocity.X = 0;
                aggroSwitch = false;
                enemyDirection = EnemyMoveDirection.Right;
            }
            if (Position.X == endPoint && enemyDirection != EnemyMoveDirection.Follow)
            {
                velocity.X = 0;
                enemyDirection = EnemyMoveDirection.Left;
            }

            Vector2 playerDistance = level.Player.Position - Position;

            float distance = 0.0f;

            if (aggroSwitch == false) {
                distance = playerDistance.Length();
            }
            else {
                distance = 2000.0f;
            }

            if (distance < distancetoAggro)
            {
                if (enemyDirection != EnemyMoveDirection.Follow) {
                    prevEnemyDirection = enemyDirection;
                }

                enemyDirection = EnemyMoveDirection.Follow;
            }
            else if (distance > distancetoAggro && enemyDirection == EnemyMoveDirection.Follow) {
                enemyDirection = prevEnemyDirection;
            }


            if ((isAtEdge == true || hitsObject == true) == true && isOnGround == true &&
               (enemyDirection == EnemyMoveDirection.Right) && counters.Check("switchTimer") == false)
            {
                aggroSwitch = true;

                enemyDirection = EnemyMoveDirection.Left;
                counters["switchTimer"] = 50.0f;
            }
            else if ((isAtEdge == true || hitsObject == true) && isOnGround == true &&
                     (enemyDirection == EnemyMoveDirection.Left) && counters.Check("switchTimer") == false)
            {
                aggroSwitch = true;

                enemyDirection = EnemyMoveDirection.Right;
                counters["switchTimer"] = 50.0f;
            }
            else if (enemyDirection != EnemyMoveDirection.Follow)
            {
                if (Position.X > endPoint) {
                    enemyDirection = EnemyMoveDirection.Left;
                }
                else if (Position.X < startPoint) {
                    enemyDirection = EnemyMoveDirection.Right;
                }
            }


            if (enemyDirection == EnemyMoveDirection.Right)
            {
                velocity.X = travelSpeed;
            }
            if (enemyDirection == EnemyMoveDirection.Left)
            {
                velocity.X = -travelSpeed;
            }

            if (enemyDirection == EnemyMoveDirection.Follow && isAtEdge == true)
            {

                if (velocity.X != 0)
                {
                    previousDirection = Math.Sign(velocity.X);
                    velocity.X = 0.0f;
                }

                if (Math.Sign(playerDistance.X) != previousDirection) {
                    velocity.X = 1.0f * Math.Sign(playerDistance.X);
                }
            }
            if (enemyDirection == EnemyMoveDirection.Follow && isAtEdge == false)
            {
                playerDistance.Normalize();
                velocity.X = (playerDistance.X * travelSpeed);
            }
        }

        private void ResetPoints()
        {
            aggroSwitch = false;
            if (enemyDirection == EnemyMoveDirection.Left) {
                startPoint = Position.X - distance;
                endPoint = Position.X;
            }
            else {
                startPoint = Position.X;
                endPoint = Position.X + distance;
            }
        }

        public override void Draw()
        {
            if (velocity.X > 0) {
                direction = SpriteEffects.None;
            }
            else if (velocity.X < 0) {
                direction = SpriteEffects.FlipHorizontally;
            }

            Engine.SpriteBatch.Draw(texture, Position, null, Color.White, 0.0f, Vector2.Zero, 1.0f, direction, 0.0f);

            if (GravitySwitch == true)
            {
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 5, Position.Y - 10), arrowUp);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 17, Position.Y + 20), null, arrowDown, MathHelper.Pi, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X + 29, Position.Y), null, arrowRight, MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                Engine.SpriteBatch.Draw(arrow, new Vector2(Position.X - 7, Position.Y + 12), null, arrowLeft, -MathHelper.PiOver2, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }
            base.Draw();
        }
    }
}
