using System;

using Microsoft.Xna.Framework;

using PolyOne.Utility;


namespace Issho
{
    public class PlayerCamera : Camera
    {
        private const float cameraLerpFactorSide = 0.12f;
        private const float cameraLerpFactorUp = 0.02f;
        private float multiplyBy = 0;
        private float newX;

        public Rectangle CameraTrap
        {
            get { return cameraTrap; }
            set { cameraTrap = value; }
        }
        private Rectangle cameraTrap;

        public void LockToTarget(Rectangle collider, int screenWidth, int screenHeight)
        {
            if (collider.Right > CameraTrap.Right)
            {
                multiplyBy = 0.3f;
                cameraTrap.X = collider.Right - CameraTrap.Width;
            }

            if (collider.Left < CameraTrap.Left)
            {
                multiplyBy = 0.6f;
                cameraTrap.X = collider.Left;
            }

            if (collider.Bottom > CameraTrap.Bottom) {
                cameraTrap.Y = collider.Bottom - CameraTrap.Height;
            }

            if (collider.Top < CameraTrap.Top) {
                cameraTrap.Y = collider.Top;
            }

            newX = cameraTrap.X + (cameraTrap.Width * multiplyBy) - (screenWidth * multiplyBy);
            Position.X = (int)Math.Round(MathHelper.Lerp(Position.X, newX, cameraLerpFactorSide));
            Position.Y = (int)Math.Round((double)cameraTrap.Y + (cameraTrap.Height / 2) - (screenHeight / 2));
        }

        public void MoveTrapUp(float target)
        {
            float moveCamera = target - cameraTrap.Height;
            cameraTrap.Y = (int)MathHelper.Lerp(CameraTrap.Y, moveCamera, cameraLerpFactorUp);
        }
    }
}
