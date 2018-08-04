using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;

namespace AnimusEngine
{
    public class Game1 : Game
    {
        // graphics
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        HUD playerHUD = new HUD();

        //map items
        Map map = new Map();
        TiledMap _map;
        TiledMapRenderer _renderer;
        public TiledMapObjectLayer _objectLayer { get; private set; } = null;
        public List<GameObject> _objects = new List<GameObject>();

        //level items
        static public string levelNumber;
        static public string roomPlaceHolder;
        public string roomNumber;
        static public string screenDir;
        static public int screenTimer;
        static public int deathTimer;
        static public bool playerDead;

        //menu items
        static public bool inMenu = true;
        static public bool hudOn;
        static public SpriteFont font;

        //cleanup items
        public List<GameObject> _killObjects = new List<GameObject>();
        public List<Door> _killDoors = new List<Door>();
        public List<Wall> _killWalls = new List<Wall>();
        int willKillPlayer;


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Resolution.Init(ref _graphics);
            Resolution.SetVirtualResolution(400, 240);
            Resolution.SetResolution(1920, 1080, true);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
        }

        protected override void Initialize()
        {
            roomNumber = "1";
            levelNumber = "StartScreen";
            screenTimer = 50;
            deathTimer = 100;
            Camera.Initialize();
            Camera.cameraOffset = new Vector2(Resolution.VirtualWidth / 2, Resolution.VirtualHeight / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Fonts/megaman");
            LevelLoader("Maps/Level_" + levelNumber);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Door.doorEnter) {
                UnloadContent(false);
                ScreenTransition(screenDir);
            }

            CheckForDeath();
            CheckForMenu(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Resolution.BeginDraw();
            //draw objects
            _spriteBatch.Begin(SpriteSortMode.BackToFront, 
                               BlendState.AlphaBlend,
                               SamplerState.PointClamp, 
                               null, 
                               null, 
                               null, 
                               Camera.GetTransformMatrix());

            if (_objects.Count == 0)
            {
                _renderer.Draw(Resolution.GetTransformationMatrix());
            } else {
                _renderer.Draw(Camera.GetTransformMatrix());
            }

            map.DrawWalls(_spriteBatch);
            DrawObjects();

            _spriteBatch.End();

            if (_objects.Count > 0)
            {
                playerHUD.Draw(_spriteBatch);
            }
            base.Draw(gameTime);
        }

        public void LevelLoader(string levelName)
        {
            map.Load(Content);
            _map = Content.Load<TiledMap>(levelName);
            _renderer = new TiledMapRenderer(GraphicsDevice, _map);

            foreach (var tileLayer in _map.TileLayers)
            {
                for (var x = 0; x < tileLayer.Width; x++)
                {
                    for (var y = 0; y < tileLayer.Height; y++)
                    {
                        var tile = tileLayer.GetTile((ushort)x, (ushort)y);

                        if (tile.GlobalIdentifier == 16)
                        {
                            var tileWidth = _map.TileWidth;
                            var tileHeight = _map.TileHeight;
                            map.walls.Add(new Wall(new Rectangle(x*tileWidth, y*tileHeight, tileWidth, tileHeight)));
                        }
                    }
                }
            }

            //parsing in objects from object layer
            _objectLayer = _map.GetLayer<TiledMapObjectLayer>("Room"+ roomNumber);

            for (int i = 0; i < _objectLayer.Objects.Length; i++)
            {
                if (_objectLayer.Objects[i].Type == "camera")
                {
                    //set camera max and min per room
                    if (_objectLayer.Objects[i].Name == "cMin" + roomNumber)
                    {
                        Camera.cameraMin = _objectLayer.Objects[i].Position + Camera.cameraOffset;
                    }
                    if (_objectLayer.Objects[i].Name == "cMax" + roomNumber)
                    {
                        Camera.cameraMax = _objectLayer.Objects[i].Position - Camera.cameraOffset;
                    }
                }
                if (_objectLayer.Objects[i].Type == "player" && _objects.Count == 0)
                {
                    //create player if none exists
                    if (_objectLayer.Objects[i].Name == "playerStart")
                    {
                        _objects.Add(new Player(_objectLayer.Objects[i].Position));
                        _objects[0].Initialize();
                        _objects[0].Load(Content);
                    }
                }
                //create doors
                if (_objectLayer.Objects[i].Type == "door")
                {
                    map.doors.Add(new Door(new Rectangle((int)_objectLayer.Objects[i].Position.X,
                                                         (int)_objectLayer.Objects[i].Position.Y,
                                                         (int)_objectLayer.Objects[i].Size.Width,
                                                         (int)_objectLayer.Objects[i].Size.Height),
                                           _objectLayer.Objects[i].Name));
                }

                //create enemies
                if (_objectLayer.Objects[i].Type == "enemy")
                {
                    _objects.Add(EnemyLookUp.EnemyLUT(_objectLayer.Objects[i].Name, _objectLayer.Objects[i].Position));
                }

                //create npcs
                if (_objectLayer.Objects[i].Type == "npc")
                {
                    _objects.Add(NPCLookUp.NpcLUT(_objectLayer.Objects[i].Name, _objectLayer.Objects[i].Position));
                }

                //create map objects
                if (_objectLayer.Objects[i].Type == "object")
                {
                    _objects.Add(MapObjectLookUp.MapObjLUT(_objectLayer.Objects[i].Name, _objectLayer.Objects[i].Position));
                }
            } 
            if (_objects.Count > 0) 
            {
                playerHUD = new HUD();
            } 
            playerHUD.Load(Content);
            LoadObjects();
        }

        protected void UnloadContent(bool killPlayer)
        {
            if (killPlayer == true)
            {
                willKillPlayer = 0;
            } else {
                willKillPlayer = 1;
            }

            for (int i = willKillPlayer; i < _objects.Count; i++)
            {
                _killObjects.Add(_objects[i]);
            }
            foreach (var doors in map.doors)
            {
                _killDoors.Add(doors);
            }
            foreach (var walls in map.walls)
            {
                _killWalls.Add(walls);
            }
            //detroy old objects --- there must be a smarter way to do this....
            foreach (GameObject o in _killObjects)
            {
                _objects.Remove(o);
            }
            foreach (Door d in _killDoors)
            {
                map.doors.Remove(d);
            }
            foreach (Wall w in _killWalls)
            {
                map.walls.Remove(w);
            }
        }

        public void RemovePlayer()
        {
            _objects.Remove(_objects[0]);
        }

        public void LoadObjects()
        {
            for (int i=1; i<_objects.Count; i++)
            {
                _objects[i].Initialize();
                _objects[i].Load(Content);
            }
        }

        public void UpdateObjects(GameTime gameTime)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].Update(_objects, map, gameTime);
            }
        }

        public void DrawObjects()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].Draw(_spriteBatch);
            }
        }

        public void UpdateCamera()
        {
            Camera.LookAt(new Vector2(0, 0));
            if (_objects.Count == 0) { return; }

            Camera.Update(_objects[0].position + new Vector2(16, 0));
        }

        public void ScreenTransition(string direction)
        {
            
            if (direction == "right")
            {
                Camera.cameraMin.X = Camera.position.X;
                _objects[0].position.Y = _objects[0].position.Y;
                _objects[0].position.X += 0.5f;
                Camera.cameraMin.X += 8;
                Camera.cameraMax.X += 8;
            }
            if (direction == "left")
            {
                Camera.cameraMax.X = Camera.position.X;
                _objects[0].position.Y = _objects[0].position.Y;
                _objects[0].position.X -= 0.5f;
                Camera.cameraMin.X -= 8;
                Camera.cameraMax.X -= 8;
            }
            screenTimer--;

            if (screenTimer < 0 ){
                roomNumber = roomPlaceHolder;
                Door.doorEnter = false;
                screenTimer = 50;
                LevelLoader("Maps/Level_" + levelNumber);
                Entity.applyGravity = true;
            }
        }

        //IS PLAYER DEAD?
        public void CheckForDeath()
        {
            if (HUD.playerHealth <= 0)
            {
                playerDead = true;
                deathTimer--;
                if (deathTimer <= 0)
                {
                    HUD.playerHealth = HUD.playerMaxHealth;
                    levelNumber = "StartScreen";
                    UnloadContent(true);
                    map.pauseScreenRec.active = false;
                    inMenu = true;
                    LevelLoader("Maps/Level_" + levelNumber);
                    deathTimer = 50;
                    playerDead = false;
                }
            }
        }

        //MENU UPDATE
        public void CheckForMenu(GameTime gameTime)
        {
            if (!inMenu)
            {
                UpdateCamera();
                UpdateObjects(gameTime);
            }
            else
            {
                UpdateMenu();
            }
        }

        public void UpdateMenu()
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyJustUp(Keys.Enter))
            {
                if (levelNumber != "StartScreen")
                {
                    //darken screen
                    map.pauseScreenRec.pauseScreen.X = (int)(Camera.position.X - Camera.cameraOffset.X);
                    map.pauseScreenRec.pauseScreen.Y = (int)(Camera.position.Y - Camera.cameraOffset.Y);
                    map.pauseScreenRec.active = true;

                    inMenu = false;
                }
                else
                {
                    levelNumber = "1";
                    UnloadContent();
                    inMenu = false;
                    hudOn = true;
                    LevelLoader("Maps/Level_" + levelNumber);
                }
            }
        }
    }
}
