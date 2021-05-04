using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

using PolyOne.LevelProcessor;
using PolyOne.Engine;
using PolyOne.Scenes;
using PolyOne.Utility;

using Issho.Platforms;


namespace Issho
{
    public enum GameTags
    {
        None = 0,
        Player = 1,
        Solid = 2,
        Empty = 3,
        GravityBoxSmall = 4,
        GravityBoxSmallLeftRight = 5,
        GravityBoxSmallUpDown = 6,
        GravityBoxMedium = 7,
        GravityBoxMediumLeftRight = 8,
        GravityBoxMediumUpDown = 9,
        Enemy = 10,
        EnemyLeftRight = 11,
        EnemyUpDown = 12,
        Checkpoint = 13,
        Exit = 14
    }

    public class Level : Scene, IDisposable
    {
        LevelTilesSolid tilesSolid;
        LevelTilesEmpty tilesEmpty;
        LevelData levelData = new LevelData();
        public LevelTiler tile = new LevelTiler();

        public string NextLevel { get; private set; }

        public Vector2 CurrentPlayerPosition;


        public Player Player
        {
            get { return player; }
        }
        Player player;

        public List<GravityItem> GravityItemList = new List<GravityItem>();

        public List<Checkpoint> Checkpoints = new List<Checkpoint>();

        public EnemyCombo Enemy;
        public EnemyUpDown EnemyUpDown;
        public EnemyLeftRight EnemyLeftRight;

        public GravityBoxMediumCombo GravityBox;
        public GravityBoxMediumUpDown GravityBoxUpDown;
        public GravityBoxMediumLeftRight GravityBoxLeftRight;

        public GravityBoxSmallCombo GravityBoxSmall;
        public GravityBoxSmallUpDown GravityBoxSmallUpDown;
        public GravityBoxSmallLeftRight GravityBoxSmallLeftRight;

        public Exit Exit;
        Texture2D testCube;

        bool[,] collisionInfoSolid;
        bool[,] collisionInfoEmpty;

        private Point minDrawDistance = new Point(0, 0);
        private Point maxDrawDistance = new Point(45, 30);

        private int drawXOffSet = 40;
        private int drawYOffSet = 30;
        private Point playerPoint;

        private int cameraCountRight;
        private int cameraCountLeft;

        private Vector2 previousCamPosition;

        public Level()
        {
        }

        public void LoadLevel(string levelName)
        {
            LoadContent();

            levelData = Engine.Instance.Content.Load<LevelData>(levelName);
            tile.LoadContent(levelData);

            collisionInfoSolid = LevelTiler.TileConverison(tile.CollisionLayer, 73);
            tilesSolid = new LevelTilesSolid(collisionInfoSolid);
            this.Add(tilesSolid);

            collisionInfoEmpty = LevelTiler.TileConverison(tile.CollisionLayer, 0);
            tilesEmpty = new LevelTilesEmpty(collisionInfoEmpty);
            this.Add(tilesEmpty);

            foreach (var entity in tile.Entites)
            {

                if (entity.Type == "GravityBox")
                {
                    GravityBox = new GravityBoxMediumCombo(entity.Position);
                    GravityItemList.Add(GravityBox);
                    this.Add(GravityBox);
                    GravityBox.Added(this);
                }

                if (entity.Type == "GravityBoxLeftRight")
                {
                    GravityBoxLeftRight = new GravityBoxMediumLeftRight(entity.Position);
                    GravityItemList.Add(GravityBoxLeftRight);
                    this.Add(GravityBoxLeftRight);
                    GravityBoxLeftRight.Added(this);
                }

                if (entity.Type == "GravityBoxUpDown")
                {
                    GravityBoxUpDown = new GravityBoxMediumUpDown(entity.Position);
                    GravityItemList.Add(GravityBoxUpDown);
                    this.Add(GravityBoxUpDown);
                    GravityBoxUpDown.Added(this);
                }

                if (entity.Type == "GravityBoxSmall")
                {
                    GravityBoxSmall = new GravityBoxSmallCombo(entity.Position);
                    GravityItemList.Add(GravityBoxSmall);
                    this.Add(GravityBoxSmall);
                    GravityBoxSmall.Added(this);
                }

                if (entity.Type == "GravityBoxSmallLeftRight")
                {
                    GravityBoxSmallLeftRight = new GravityBoxSmallLeftRight(entity.Position);
                    GravityItemList.Add(GravityBoxSmallLeftRight);
                    this.Add(GravityBoxSmallLeftRight);
                    GravityBoxSmallLeftRight.Added(this);
                }

                if (entity.Type == "GravityBoxSmallUpDown")
                {
                    GravityBoxSmallUpDown = new GravityBoxSmallUpDown(entity.Position);
                    GravityItemList.Add(GravityBoxSmallUpDown);
                    this.Add(GravityBoxSmallUpDown);
                    GravityBoxSmallUpDown.Added(this);
                }

                if (entity.Type == "Enemy")
                {
                    if(entity.Properties.Count > 0)
                    {
                        EnemyMoveDirection direction = EnemyMoveDirection.Right;
                        int distance = 100;

                        foreach (var item in entity.Properties) {
                            if(item.Key == "Direction")
                            {
                                if(item.Value == "Right") {
                                    direction = EnemyMoveDirection.Right;
                                }
                                else if(item.Value == "Left") {
                                    direction = EnemyMoveDirection.Left;
                                }
                            }
                            else if(item.Key == "Distance") {
                                distance = int.Parse(item.Value);
                            }
                        }

                        Enemy = new EnemyCombo(entity.Position, distance, direction);
                        GravityItemList.Add(Enemy);
                        this.Add(Enemy);
                        Enemy.Added(this);
                    }
                    else
                    {
                        Enemy = new EnemyCombo(entity.Position);
                        GravityItemList.Add(Enemy);
                        this.Add(Enemy);
                        Enemy.Added(this);
                    }
                }

                if (entity.Type == "EnemyUpDown")
                {
                    if (entity.Properties.Count > 0)
                    {
                        EnemyMoveDirection direction = EnemyMoveDirection.Right;
                        int distance = 100;

                        foreach (var item in entity.Properties)
                        {
                            if (item.Key == "Direction")
                            {
                                if (item.Value == "Right") {
                                    direction = EnemyMoveDirection.Right;
                                }
                                else if (item.Value == "Left") {
                                    direction = EnemyMoveDirection.Left;
                                }
                            }
                            else if (item.Key == "Distance") {
                                distance = int.Parse(item.Value);
                            }
                        }

                        EnemyUpDown = new EnemyUpDown(entity.Position, distance, direction);
                        GravityItemList.Add(EnemyUpDown);
                        this.Add(EnemyUpDown);
                        EnemyUpDown.Added(this);
                    }
                    else
                    {
                        EnemyUpDown = new EnemyUpDown(entity.Position);
                        GravityItemList.Add(EnemyUpDown);
                        this.Add(EnemyUpDown);
                        EnemyUpDown.Added(this);
                    }
                }

                if (entity.Type == "EnemyLeftRight")
                {
                    if (entity.Properties.Count > 0)
                    {
                        EnemyMoveDirection direction = EnemyMoveDirection.Right;
                        int distance = 100;

                        foreach (var item in entity.Properties)
                        {
                            if (item.Key == "Direction")
                            {
                                if (item.Value == "Right") {
                                    direction = EnemyMoveDirection.Right;
                                }
                                else if (item.Value == "Left") {
                                    direction = EnemyMoveDirection.Left;
                                }
                            }
                            else if (item.Key == "Distance") {
                                distance = int.Parse(item.Value);
                            }
                        }

                        EnemyLeftRight = new EnemyLeftRight(entity.Position, distance, direction);
                        GravityItemList.Add(EnemyLeftRight);
                        this.Add(EnemyLeftRight);
                        EnemyLeftRight.Added(this);
                    }
                    else
                    {
                        EnemyLeftRight = new EnemyLeftRight(entity.Position);
                        GravityItemList.Add(EnemyLeftRight);
                        this.Add(EnemyLeftRight);
                        EnemyLeftRight.Added(this);
                    }
                }

                if(entity.Type == "Checkpoint")
                {
                    Checkpoint checkpoint = new Checkpoint(entity.Position);
                    this.Add(checkpoint);
                    checkpoint.Added(this);

                    Checkpoints.Add(checkpoint);
                }

                if (entity.Type == "Exit")
                {
                    Exit = new Exit(entity.Position);
                    this.Add(Exit);
                    Exit.Added(this);
                }

                if (entity.Type == "NextLevel") {
                    NextLevel = entity.Name;
                }

            }

            if (CurrentPlayerPosition == Vector2.Zero) {
                player = new Player(tile.PlayerPosition[0]);
            }
            else {
                player = new Player(CurrentPlayerPosition); 
            }

            this.Add(player);
            player.Added(this);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            testCube = Engine.Instance.Content.Load<Texture2D>("TestCube");
        }

        public override void UnloadContent()
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw()
        {
            playerPoint = new Point((int)player.Position.X / 16, (int)player.Position.Y / 16);

            if(player.Camera.Position.X != previousCamPosition.X) {
                minDrawDistance.X = MathHelper.Clamp(playerPoint.X - drawXOffSet, 0, levelData.Width);
                maxDrawDistance.X = MathHelper.Clamp(playerPoint.X + drawXOffSet, playerPoint.X + drawXOffSet, levelData.Width);
            }
           
            if(player.Camera.Position.Y != previousCamPosition.Y) {
                minDrawDistance.Y = MathHelper.Clamp(playerPoint.Y - drawYOffSet, 0, levelData.Height);
                maxDrawDistance.Y = MathHelper.Clamp(playerPoint.Y + drawYOffSet, playerPoint.Y + drawYOffSet, levelData.Height);
            }

            Engine.BeginParallax(player.Camera.TransformMatrix);
            tile.DrawImageBackground(player.Camera.Position, 0.2f);
            tile.DrawImageForeground(player.Camera.Position);
            Engine.End();

            Engine.Begin(player.Camera.TransformMatrix);
            tile.DrawBackground(minDrawDistance, maxDrawDistance);
            base.Draw();
            Engine.End();

            previousCamPosition = player.Camera.Position;
        }

        public void Dispose()
        {
        }
    }
}
