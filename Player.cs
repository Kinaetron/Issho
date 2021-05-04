using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using PolyOne;
using PolyOne.Scenes;
using PolyOne.Collision;
using PolyOne.Engine;
using PolyOne.Input;
using PolyOne.Components;
using PolyOne.Animation;


namespace Issho
{
    enum PlayerAction
    {
        Idle = 0, 
        Walking = 1,
        Jumping = 2,
        Falling = 3,
        Dieing = 4
    }

    public class Player : Entity
    {
        Texture2D PlayerImage;
        Texture2D gravityImage;

        private float sign;

        private const float runAccel = 1.0f;
        private const float turnMul = 0.75f;
        private const float normMaxHorizSpeed = 3.0f;

        private const float gravityUp = 0.19f;
        private const float gravityDown = 0.1f;

        private const float fallspeed = 0.5f;
        private const float initialJumpHeight = -5.0f;
        private const float halfJumpHeight = -2.5f;
        private const float airFriction = 0.8f;
        private const float airInteria = 0.29f;

        private const float graceTime = 66.9f;
        private const float graceTimePush = 100.3f;

        private const float distanceToUse = 150.0f;

        private const float gravitySetTime = 1000.0f;

        private float graceTimer;
        private float graceTimerPush;

        private Vector2 remainder;
        private Vector2 velocity;
        private Vector2 prevVelocity;

        private bool isOnGround;
        private bool isOnObject;
        private bool touchingSideBox;
        private bool touchingSideTile;
        private bool touchingTopTile;
        private bool prevIsOnObject;
        private bool buttonPushed;
        private bool pushedUp;

        private bool gravitySetMode;

        private Level level;
        private List<GravityItem> gravityItemList = new List<GravityItem>();
        private GravityItem gravityItem;

        private CounterSet<string> counters = new CounterSet<string>();

        public bool IsDead { get; private set; }
        public bool StopRendering { get; set; }

        public PlayerCamera Camera = new PlayerCamera();

        private Texture2D red;

        private bool controllerMode;
        private bool keyboardMode;
        private List<Keys> keyList = new List<Keys>(new Keys[] { Keys.W, Keys.A, Keys.S, Keys.D, Keys.Up,
                                                                 Keys.Down, Keys.Left, Keys.Right ,Keys.C });
        private AnimationPlayer sprite;

        private Texture2D idleTexture;
        private Texture2D walkTexture;
        private Texture2D jumpTexture;
        private Texture2D fallingTexture;
        private Texture2D dieTexture;

        private AnimationData idleAnimation;
        private AnimationData walkAnimation;
        private AnimationData jumpAnimation;
        private AnimationData fallingAnimation;
        private AnimationData dieAnimation;

        private SpriteEffects direction;

        private PlayerAction action;


        private Random randGen = new Random();
        private const int maxEffects = 2;

        private SoundEffect[] dieEffect = new SoundEffect[maxEffects];
        private SoundEffect[] stepEffect = new SoundEffect[maxEffects];

        private SoundEffect jumpEffect;
        private SoundEffect openGravEffect;
        private SoundEffect fireGravEffect;
        private SoundEffect landEffect;

        private const float stepPlay = 400.0f;

        public Player(Vector2 position)
           : base(position)
        {
            sprite = new AnimationPlayer();

            idleTexture = Engine.Instance.Content.Load<Texture2D>("PlayerAnimations/PlayerIdle");
            idleAnimation = new AnimationData(idleTexture, 200, 16, true);

            walkTexture = Engine.Instance.Content.Load<Texture2D>("PlayerAnimations/PlayerWalk");
            walkAnimation = new AnimationData(walkTexture, 200, 20, true);

            fallingTexture = Engine.Instance.Content.Load<Texture2D>("PlayerAnimations/PlayerFalling");
            fallingAnimation = new AnimationData(fallingTexture, 80, 20, false);

            jumpTexture = Engine.Instance.Content.Load<Texture2D>("PlayerAnimations/PlayerJump");
            jumpAnimation = new AnimationData(jumpTexture, 80, 20, false);

            dieTexture = Engine.Instance.Content.Load<Texture2D>("PlayerAnimations/PlayerDied");
            dieAnimation = new AnimationData(dieTexture, 80, 30, false);

            dieEffect[0] = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/die0");
            dieEffect[1] = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/die1");
            stepEffect[0] = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/step0");
            stepEffect[1] = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/step1");


            jumpEffect = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/jump0");
            openGravEffect = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/openGrav0");
            fireGravEffect = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/fireGrav0");
            landEffect = Engine.Instance.Content.Load<SoundEffect>("Sounds/Effects/land0");

            sprite.PlayAnimation(idleAnimation);

            this.Tag((int)GameTags.Player);
            this.Collider = new Hitbox((float)16.0f, (float)7.0f, 0.0f, 0.0f);
            PlayerImage = Engine.Instance.Content.Load<Texture2D>("PlayerImage");
            gravityImage = Engine.Instance.Content.Load<Texture2D>("GravityCircle");
            red = Engine.Instance.Content.Load<Texture2D>("Tiles/Red");
            Camera.CameraTrap = new Rectangle((int)this.Right, (int)this.Bottom - 100, 70, 100);

            this.Visible = true;
            gravitySetMode = false;

            this.Add(counters);
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);

            if(base.Scene is Level) {
                this.level = (base.Scene as Level);
                gravityItemList = level.GravityItemList;
            }
        }

        private void ClosestBox()
        {
            if (gravityItem != null)
            {
                if (gravityItem.GravitySwitch == true) {
                    return;
                }
            }

            float minScore = float.MaxValue;

            foreach (GravityItem item in gravityItemList)
            {
                if (item.Distance < minScore) {
                    gravityItem = item;
                    minScore = item.Distance;
                }
            }
        }

        private void InputMode()
        {
            foreach (Keys key in keyList)
            {
                if (PolyInput.Keyboard.Check(key) == true)
                {
                    controllerMode = false;
                    keyboardMode = true;
                }
            }
            if (PolyInput.GamePads[0].ButtonCheck() == true)
            {
                controllerMode = true;
                keyboardMode = false;
            }

            if (controllerMode == false && keyboardMode == false) {
                keyboardMode = true;
            }
        }

        private void GravityInput()
        {
            if(gravityItem == null) {
                return;
            }

            if (gravityItem.Done == true) {
                gravitySetMode = false;
            }

            if (counters.Check("gravitySet") == true) {
                return;
            }

            if (keyboardMode == true)
            {
                if (PolyInput.Keyboard.Pressed(Keys.C))
                {
                    if (gravityItem.Distance <= distanceToUse && gravitySetMode == false && (isOnObject || isOnGround)) {
                        gravitySetMode = true;
                        openGravEffect.Play();
                    }
                    else if (gravitySetMode == true && (isOnObject || isOnGround)) {
                        gravitySetMode = false;
                    }

                    if (gravityItem.GravitySet == false) {
                        gravityItem.GravitySwitch = gravitySetMode;
                    }
                }
            }
            else if(controllerMode == true)
            {

                if (PolyInput.GamePads[0].Pressed(Buttons.X) || 
                    PolyInput.GamePads[0].LeftTriggerPressed(0.5f))
                {

                    if (gravityItem.Distance <= distanceToUse && gravitySetMode == false && (isOnObject || isOnGround)) {
                        gravitySetMode = true;
                        openGravEffect.Play();
                    }
                    else if (gravitySetMode == true && (isOnObject || isOnGround)) {
                        gravitySetMode = false;
                    }

                    if (gravityItem.GravitySet == false) {
                        gravityItem.GravitySwitch = gravitySetMode;
                    }
                }

            }

            if (gravitySetMode == true)
            {
                if (keyboardMode == true)
                {
                    if (PolyInput.Keyboard.Pressed(Keys.Down) == true ||
                        PolyInput.Keyboard.Pressed(Keys.S) == true)
                    {
                        if (gravityItem.GetType() == typeof(GravityBoxSmallUpDown) ||
                            gravityItem.GetType() == typeof(GravityBoxSmallCombo)  ||
                            gravityItem.GetType() == typeof(GravityBoxMediumCombo) || 
                            gravityItem.GetType() == typeof(GravityBoxMediumUpDown) ||
                            gravityItem.GetType() == typeof(EnemyCombo) || 
                            gravityItem.GetType() == typeof(EnemyUpDown))
                        {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Down;
                            gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                        }
                    }
                    else if (PolyInput.Keyboard.Pressed(Keys.Up) == true ||
                             PolyInput.Keyboard.Pressed(Keys.W) == true)
                    {
                        if (gravityItem.GetType() == typeof(GravityBoxSmallUpDown) ||
                             gravityItem.GetType() == typeof(GravityBoxSmallCombo) ||
                             gravityItem.GetType() == typeof(GravityBoxMediumCombo) ||
                             gravityItem.GetType() == typeof(GravityBoxMediumUpDown) ||
                             gravityItem.GetType() == typeof(EnemyCombo) ||
                             gravityItem.GetType() == typeof(EnemyUpDown))
                        {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Up;
                            gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                        }
                    }
                    else if (PolyInput.Keyboard.Pressed(Keys.Right) == true ||
                             PolyInput.Keyboard.Pressed(Keys.D) == true)
                    {
                        if (gravityItem.GetType() == typeof(GravityBoxSmallLeftRight) ||
                            gravityItem.GetType() == typeof(GravityBoxSmallCombo) ||
                            gravityItem.GetType() == typeof(GravityBoxMediumCombo) ||
                            gravityItem.GetType() == typeof(GravityBoxMediumLeftRight) ||
                            gravityItem.GetType() == typeof(EnemyCombo) ||
                            gravityItem.GetType() == typeof(EnemyLeftRight))
                        {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Right;
                             gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                        }
                    }
                    else if (PolyInput.Keyboard.Pressed(Keys.Left) == true ||
                             PolyInput.Keyboard.Pressed(Keys.A) == true)
                    {
                        if (gravityItem.GetType() == typeof(GravityBoxSmallLeftRight) ||
                           gravityItem.GetType() == typeof(GravityBoxSmallCombo) ||
                           gravityItem.GetType() == typeof(GravityBoxMediumLeftRight) ||
                           gravityItem.GetType() == typeof(GravityBoxMediumCombo) ||
                           gravityItem.GetType() == typeof(EnemyCombo) ||
                           gravityItem.GetType() == typeof(EnemyLeftRight))
                        {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Left;
                            gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                        }
                    }
                }
                else if(controllerMode == true)
                {
                    if (PolyInput.GamePads[0].RightStickDownCheck(0.3f) == true)
                    {
                        if (gravityItem.GetType() == typeof(GravityBoxSmallUpDown) ||
                           gravityItem.GetType() == typeof(GravityBoxSmallCombo) ||
                           gravityItem.GetType() == typeof(GravityBoxMediumCombo) ||
                           gravityItem.GetType() == typeof(GravityBoxMediumUpDown) ||
                           gravityItem.GetType() == typeof(EnemyCombo) ||
                           gravityItem.GetType() == typeof(EnemyUpDown))
                          {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Down;
                            gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                          }
                    }
                    else if (PolyInput.GamePads[0].RightStickUpCheck(0.3f) == true)
                    {
                        if (gravityItem.GetType() == typeof(GravityBoxSmallUpDown) ||
                           gravityItem.GetType() == typeof(GravityBoxSmallCombo) ||
                           gravityItem.GetType() == typeof(GravityBoxMediumCombo) ||
                           gravityItem.GetType() == typeof(GravityBoxMediumUpDown) ||
                           gravityItem.GetType() == typeof(EnemyCombo) ||
                           gravityItem.GetType() == typeof(EnemyUpDown))
                          {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Up;
                            gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                          }
                    }
                    else if (PolyInput.GamePads[0].RightStickRightCheck(0.3f) == true)
                    {
                        if (gravityItem.GetType() == typeof(GravityBoxSmallLeftRight) ||
                            gravityItem.GetType() == typeof(GravityBoxSmallCombo) ||
                            gravityItem.GetType() == typeof(GravityBoxMediumCombo) ||
                            gravityItem.GetType() == typeof(GravityBoxMediumLeftRight) ||
                            gravityItem.GetType() == typeof(EnemyCombo) ||
                            gravityItem.GetType() == typeof(EnemyLeftRight))
                          {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Right;
                            gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                          }
                    }
                    else if (PolyInput.GamePads[0].RightStickLeftCheck(0.3f) == true)
                        {

                        if (gravityItem.GetType() == typeof(GravityBoxSmallLeftRight) ||
                            gravityItem.GetType() == typeof(GravityBoxSmallCombo) ||
                            gravityItem.GetType() == typeof(GravityBoxMediumCombo) ||
                            gravityItem.GetType() == typeof(GravityBoxMediumLeftRight) ||
                            gravityItem.GetType() == typeof(EnemyCombo) ||
                            gravityItem.GetType() == typeof(EnemyLeftRight))
                        {
                            counters["gravitySet"] = gravitySetTime;
                            gravityItem.Direction = GravityDirection.Left;
                            gravityItem.GravitySet = true;
                            fireGravEffect.Play();
                        }
                    }
                }
            }
        }

        private void HorizontalInput()
        {
            if (gravitySetMode == false)
            {
                if (isOnGround == true || isOnObject == true) {
                    sign = 0;
                }

                if(keyboardMode == true)
                {
                    if (PolyInput.Keyboard.Check(Keys.D) == true ||
                        PolyInput.Keyboard.Check(Keys.Right) == true)
                    {
                        sign = 1;
                    }
                    else if (PolyInput.Keyboard.Check(Keys.A) == true || 
                             PolyInput.Keyboard.Check(Keys.Left) == true)
                    {
                        sign = -1;
                    }
                    else if (isOnGround == true || isOnObject == true) {
                        velocity.X = 0;
                    }
                    else {
                        velocity.X *= airInteria;
                    }
                }

                else if(controllerMode == true)
                {
                    if (PolyInput.GamePads[0].LeftStickHorizontal(0.3f) > 0.1f ||
                        PolyInput.GamePads[0].DPadRightCheck == true) {
                        sign = 1;
                    }
                    else if (PolyInput.GamePads[0].LeftStickHorizontal(0.3f) < -0.1f || 
                             PolyInput.GamePads[0].DPadLeftCheck == true) {
                        sign = -1;
                    }
                    else if (isOnGround == true || isOnObject == true) { 
                        velocity.X = 0;
                    }
                    else {
                        velocity.X *= airInteria;
                    }
                }
            }

            if(gravityItem != null)
            {
                if (isOnObject == true && sign == 0 && gravityItem.GetType() != typeof(EnemyCombo)) {
                    velocity.X = gravityItem.Velocity.X;
                }
                else if (gravityItem.PlayerIsOnX == true && gravityItem.GetType() != typeof(EnemyCombo)) {
                    velocity.X = gravityItem.Velocity.X;
                }
            }

            velocity.X += runAccel * sign;
            velocity.X = MathHelper.Clamp(velocity.X, -normMaxHorizSpeed, normMaxHorizSpeed);

            float currentSign = Math.Sign(velocity.X);

            if(gravityItem == null)
            {
              if (currentSign != 0 && currentSign != sign) {
                    velocity.X *= turnMul;
                }

                if (isOnGround == false && velocity.Y > halfJumpHeight && velocity.X > 1.0f) {
                    velocity.X *= airFriction;
                }
            }
            else
            {
                if (currentSign != 0 && currentSign != sign && gravityItem.PlayerIsOnX == false &&
                    isOnObject == false)
                {
                    velocity.X *= turnMul;
                }

                if (isOnGround == false && velocity.Y > halfJumpHeight && velocity.X > 1.0f &&
                   gravityItem.PlayerIsOnX == false && isOnObject == false)
                {
                    velocity.X *= airFriction;
                }

                if (gravitySetMode == true && gravityItem.PlayerIsOnX == false &&
                    isOnObject == false)
                {
                    velocity = Vector2.Zero;
                }
            }


            if(isOnObject == true && gravitySetMode == true && gravityItem.Direction == GravityDirection.Down) {
                velocity.X = 0f;
            }

            MovementHorizontal(velocity.X);
        }

        private void JumpPhysics()
        {
            if (isOnGround == true || isOnObject == true) {
                buttonPushed = false;
            }

            if (gravitySetMode == false)
            {
                if(keyboardMode == true)
                {
                    if (graceTimerPush > 0)
                    {
                        if (isOnGround == true || isOnObject == true || graceTimer > 0)
                        {
                            buttonPushed = true;
                            graceTimerPush = 0.0f;

                            jumpEffect.Play();

                            if (velocity.Y < -1.0f) {
                                velocity.Y = initialJumpHeight - 2;
                                action = PlayerAction.Jumping;
                            }
                            else {
                                velocity.Y = initialJumpHeight;
                                action = PlayerAction.Jumping;
                            }
                        }
                    }
                    else if (PolyInput.Keyboard.Released(Keys.Space) == true && velocity.Y < 0.0f &&
                             velocity.Y < halfJumpHeight)
                    {
                        graceTimerPush = 0.0f;

                        if (velocity.Y < initialJumpHeight) {
                            velocity.Y = halfJumpHeight - 2;
                        }
                        else {
                            velocity.Y = halfJumpHeight;
                        }
                    }

                    if(velocity.Y > 0 && (isOnGround == true || isOnObject == true ))
                    {
                        velocity.Y = 0.0f;
                    }
                }
                else if(controllerMode == true)
                {
                    if (graceTimerPush > 0)
                    {
                        if (isOnGround == true || isOnObject == true || graceTimer > 0)
                        {
                            
                            buttonPushed = true;
                            graceTimerPush = 0.0f;

                            jumpEffect.Play();

                            if (velocity.Y < -1.0f) {
                                velocity.Y = initialJumpHeight - 2;
                                action = PlayerAction.Jumping;
                            }
                            else {
                                velocity.Y = initialJumpHeight;
                                action = PlayerAction.Jumping;
                            }
                        }
                    }
                    else if (PolyInput.GamePads[0].Released(Buttons.A) == true && velocity.Y < 0.0f &&
                             velocity.Y < halfJumpHeight)
                    {
                        graceTimerPush = 0.0f;

                        if (velocity.Y < initialJumpHeight) {
                            velocity.Y = halfJumpHeight - 2;
                        }
                        else {
                            velocity.Y = halfJumpHeight;
                        }
                    }

                    if (velocity.Y > 0 && (isOnGround == true || isOnObject == true)) {
                        velocity.Y = 0.0f;
                    }
                }
            }

            if (gravityItem != null)
            {
                if (gravityItem.PlayerIsOnY == true && buttonPushed == false &&
                    gravityItem.GetType() != typeof(EnemyCombo) && (gravityItem.Direction == GravityDirection.Up))
                {
                    pushedUp = true;
                    velocity.Y = gravityItem.Velocity.Y;
                    Bottom = Bottom;
                }
            }


            if (graceTimePush > 0 && velocity.Y < 0 && isOnObject == false) {
                velocity.Y += gravityUp;
            }
            else if(isOnObject == false) {
                velocity.Y += gravityDown;
            }


            if(isOnObject == false && prevIsOnObject == true && buttonPushed == false){
                velocity.Y = 0.0f;
            }

            velocity.Y = MathHelper.Clamp(velocity.Y, initialJumpHeight - 2, fallspeed);

          
            MovementVerical(velocity.Y);
        }

        private void AnimationState()
        {

            if(action == PlayerAction.Falling && (isOnGround == true || isOnObject == true)) {
                landEffect.Play();
            }
           
            if(IsDead == true) {
                action = PlayerAction.Dieing;
            }
            else if((isOnGround == false && isOnObject == false && graceTimer <= 0) && Math.Abs(velocity.Y) > Math.Abs(prevVelocity.Y)) {
                 action = PlayerAction.Falling;
            }
            else if (sign != 0 && (isOnGround == true || isOnObject == true) && action != PlayerAction.Jumping) {
                action = PlayerAction.Walking;
            }
            else if (sign == 0 && (isOnGround == true || isOnObject == true) && action != PlayerAction.Jumping) {
                action = PlayerAction.Idle;
            }

            if (action == PlayerAction.Walking) {
                sprite.PlayAnimation(walkAnimation);

                if(counters["stepEffect"] <= 0)
                {
                    counters["stepEffect"] = stepPlay;
                    stepEffect[randGen.Next(0, maxEffects)].Play();
                }
               
            }

            if(action == PlayerAction.Dieing) {
                sprite.PlayAnimation(dieAnimation);
                dieEffect[randGen.Next(0, maxEffects)].Play();
            }

            if(action == PlayerAction.Jumping) {
                sprite.PlayAnimation(jumpAnimation);
            }

            if (action == PlayerAction.Falling) {
                sprite.PlayAnimation(fallingAnimation);
            }

            if (action == PlayerAction.Idle) {
                sprite.PlayAnimation(idleAnimation);
            }
        }

        private void Counters()
        {
            if(isOnGround == true || isOnObject == true) {
                graceTimer = graceTime;
            }

            if(keyboardMode == true) {
                if (PolyInput.Keyboard.Pressed(Keys.Space) == true) {
                    graceTimerPush = graceTimePush;
                }
            }
            else if(controllerMode == true) {
                if (PolyInput.GamePads[0].Pressed(Buttons.A) == true) {
                    graceTimerPush = graceTimePush;
                }
            }

            if (graceTimerPush > 0) {
                graceTimerPush -= Engine.DeltaTime;
            }

            if(graceTimer > 0) {
                graceTimer -= Engine.DeltaTime;
            }

            if(graceTimerPush < 0) {
                graceTimerPush = 0;
            }

            if(graceTimer < 0) {
               graceTimer = 0;
            }
        }

        private void AreYouDead()
        {
            touchingSideTile = base.CollideCheck((int)GameTags.Solid, this.Position + Vector2.UnitX);

            if(touchingSideTile == false) {
                touchingSideTile = base.CollideCheck((int)GameTags.Solid, this.Position - Vector2.UnitX);
            }

            touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxSmall, this.Position + Vector2.UnitX);

            if(touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxSmall, this.Position - Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxSmallLeftRight, this.Position + Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxSmallLeftRight, this.Position - Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxSmallUpDown, this.Position + Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxSmallUpDown, this.Position - Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxMedium, this.Position + Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxMedium, this.Position - Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxMediumLeftRight, this.Position + Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxMediumLeftRight, this.Position - Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxMediumUpDown, this.Position + Vector2.UnitX);
            }

            if (touchingSideBox == false) {
                touchingSideBox = base.CollideCheck((int)GameTags.GravityBoxMediumUpDown, this.Position - Vector2.UnitX);
            }


            touchingTopTile = base.CollideCheck((int)GameTags.Solid, this.Position - Vector2.UnitY);

            IsDead = base.CollideCheck((int)GameTags.Enemy, this.Position + Vector2.UnitX);


            if (IsDead == false) {
                IsDead = base.CollideCheck((int)GameTags.EnemyLeftRight, this.Position + Vector2.UnitX);
            }

            if (IsDead == false) {
                IsDead = base.CollideCheck((int)GameTags.EnemyUpDown, this.Position + Vector2.UnitX);
            }

            if (isOnObject == true && touchingTopTile == true && IsDead == false) {
                //IsDead = true;
            }

            if (touchingSideBox == true && touchingSideTile == true && IsDead == false) {
                IsDead = true;
            }

            if (IsDead == false) {
                IsDead = base.CollideCheck((int)GameTags.Enemy, this.Position);
            }

            if (IsDead == false) {
                IsDead = base.CollideCheck((int)GameTags.Enemy, this.Position - Vector2.UnitY);
            }

            if (IsDead == false && isOnGround == true) {
                IsDead = base.CollideCheck((int)GameTags.GravityBoxMedium, this.Position - Vector2.UnitY);
            }

            if (IsDead == false && isOnGround == true) {
                IsDead = base.CollideCheck((int)GameTags.GravityBoxMediumLeftRight, this.Position - Vector2.UnitY);
            }

            if (IsDead == false && isOnGround == true) {
                IsDead = base.CollideCheck((int)GameTags.GravityBoxMediumUpDown, this.Position - Vector2.UnitY);
            }

            if (IsDead == false && isOnGround == true) {
                IsDead = base.CollideCheck((int)GameTags.GravityBoxSmall, this.Position - Vector2.UnitY);
            }

            if (IsDead == false && isOnGround == true) {
                IsDead = base.CollideCheck((int)GameTags.GravityBoxSmallLeftRight, this.Position - Vector2.UnitY);
            }

            if (IsDead == false && isOnGround == true) {
                IsDead = base.CollideCheck((int)GameTags.GravityBoxSmallUpDown, this.Position - Vector2.UnitY);
            }

            if (Position.Y > level.tile.MapHeightInPixels + 20.0f && IsDead == false) {
                IsDead = true;
            }

            for (int i = 0; i < gravityItemList.Count; i++) {
                for (int j = 0; j < gravityItemList.Count; j++) {
                    if(i == j) {
                        continue;
                    }

                   if(gravityItemList[i].PlayerIsOnX == true && gravityItemList[j].PlayerIsOnX == true)
                   {
                        if(IsDead == false) {
                          IsDead = true;
                        }
                    }
                }
            }
        }

        private void KeepOnLevel()
        {
            if(Position.X < 0) {
                Position.X = 0;
            }
            if(Position.X > level.tile.MapWidthInPixels - 16) {
                Position.X = level.tile.MapWidthInPixels - 16; 
            }
        }

        public override void Update()
        {
            isOnGround = base.CollideCheck((int)GameTags.Solid, this.Position + Vector2.UnitY);


            isOnObject = base.CollideCheck((int)GameTags.GravityBoxSmall, this.Position + Vector2.UnitY);

            if (isOnObject == false) {
                isOnObject = base.CollideCheck((int)GameTags.GravityBoxMedium, this.Position + Vector2.UnitY);
            }

            if (isOnObject == false) {
                isOnObject = base.CollideCheck((int)GameTags.GravityBoxMediumLeftRight, this.Position + Vector2.UnitY);
            }

            if (isOnObject == false) {
                isOnObject = base.CollideCheck((int)GameTags.GravityBoxMediumUpDown, this.Position + Vector2.UnitY);
            }

            if (isOnObject == false) {
                isOnObject = base.CollideCheck((int)GameTags.GravityBoxSmallLeftRight, this.Position + Vector2.UnitY);
            }

            if (isOnObject == false) {
                isOnObject = base.CollideCheck((int)GameTags.GravityBoxSmallUpDown, this.Position + Vector2.UnitY);
            }

            pushedUp = false;

            AreYouDead();
            ClosestBox();
            Counters();
            HorizontalInput();
            JumpPhysics();
            GravityInput();
            InputMode();
            KeepOnLevel();
            AnimationState();

            base.Update();

            Camera.LockToTarget(this.Rectangle, Engine.VirtualWidth, Engine.VirtualHeight);
            Camera.ClampToArea((int)level.tile.MapWidthInPixels - Engine.VirtualWidth, (int)level.tile.MapHeightInPixels - Engine.VirtualHeight);


            if(isOnGround == true || isOnObject == true) {
                Camera.MoveTrapUp(Position.Y + 16);
            }

            prevIsOnObject = isOnObject;
            prevVelocity = velocity;
        }

        private void MovementHorizontal(float amount)
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
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmall, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallUpDown, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallLeftRight, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMedium, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumUpDown, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumLeftRight, newPosition) != null)
                    {
                        remainder.X = 0;
                        break;
                    }

                    Position.X += sign;
                    move -= sign;
                }
            }
        }

        private void MovementVerical(float amount)
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
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmall, newPosition) != null)
                    {
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallLeftRight, newPosition) != null)
                    {
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallUpDown, newPosition) != null)
                    {
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMedium, newPosition) != null)
                    {
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumUpDown, newPosition) != null)
                    {
                        velocity.Y = 0;
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumLeftRight, newPosition) != null)
                    {
                        velocity.Y = 0;
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
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmall, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxSmallLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMedium, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumUpDown, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    if (this.CollideFirst((int)GameTags.GravityBoxMediumLeftRight, newPosition) != null)
                    {
                        remainder.Y = 0;
                        break;
                    }

                    Position.Y += 1.0f;
                    move -= 1;
                }
            }
        
        }

        public override void Draw()
        {
            base.Draw();

            if (sign > 0) {
                direction = SpriteEffects.None;
            }
            else if (sign < 0) {
                direction = SpriteEffects.FlipHorizontally;
            }


            if (StopRendering == false)
            {
                if (pushedUp == true)
                {
                    if(gravityItem.GetType() == typeof(GravityBoxSmallCombo) || 
                       gravityItem.GetType() == typeof(GravityBoxSmallUpDown))
                    {
                        sprite.Draw(new Vector2(Position.X + 10, Position.Y + 2), 0.0f, direction);
                    }
                    else {
                        sprite.Draw(new Vector2(Position.X + 10, Position.Y + 1), 0.0f, direction);
                    }
                }
                else {
                    sprite.Draw(new Vector2(Position.X + 10, Position.Y), 0.0f, direction);
                }
            }

            if (gravitySetMode == true) {
                Engine.SpriteBatch.Draw(gravityImage, new Vector2(Position.X, Position.Y - 6), Color.White);
            }
        } 
    }
}