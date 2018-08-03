using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        //
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        //map items
        Map map = new Map();
        TiledMap _map;
        TiledMapRenderer _renderer;
        public TiledMapObjectLayer _objectLayer { get; private set; } = null;
        public List<GameObject> _objects = new List<GameObject>();

        //level items
        static public string levelNumber;
        static public string roomNumber;
        static public string screenDir;

        //cleanup items
        public List<GameObject> _killObjects = new List<GameObject>();
        public List<Door> _killDoors = new List<Door>();
        public List<Wall> _killWalls = new List<Wall>();


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Resolution.Init(ref _graphics);
            Resolution.SetVirtualResolution(400, 240);
            Resolution.SetResolution(800, 480, false);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
        }

        protected override void Initialize()
        {
            roomNumber = "1";
            levelNumber = "0";
            Camera.Initialize();
            Camera.cameraOffset = new Vector2(Resolution.VirtualWidth / 2, Resolution.VirtualHeight / 2);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LevelLoader("Level_" + levelNumber);
        }

        protected override void UnloadContent()
        {
            LevelCleaner();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Door.doorEnter) {
                ScreenTransition(screenDir, 
                                 roomNumber, 
                                 Resolution.VirtualWidth* 10);
                UnloadContent();
                LoadContent();
                Door.doorEnter = false;
            }

            UpdateCamera();
            UpdateObjects(gameTime);
            base.Update(gameTime);
            Console.WriteLine(_objects.Count);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Resolution.BeginDraw();
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, 
                                 SamplerState.PointClamp, null, null, null, Camera.GetTransformMatrix());

            _renderer.Draw(Camera.GetTransformMatrix());
            map.DrawWalls(_spriteBatch);
            DrawObjects();

            _spriteBatch.End();
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
                    _objects.Add(new Enemy(_objectLayer.Objects[i].Position, _objectLayer.Objects[i].Name));
                }

                //create npcs
                if (_objectLayer.Objects[i].Type == "npc")
                {
                    //_objects.Add(new Enemy(_objectLayer.Objects[i].Position, _objectLayer.Objects[i].Name));
                }
                //create map objects
                if (_objectLayer.Objects[i].Type == "object")
                {
                    //_objects.Add(new Enemy(_objectLayer.Objects[i].Position, _objectLayer.Objects[i].Name));
                }

            }

            LoadObjects();
        }

        public void LevelCleaner()
        {
            for (int i = 1; i < _objects.Count; i++)
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

        public void LoadObjects()
        {
            for (int i=0; i<_objects.Count; i++)
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
            if (_objects.Count == 0) { return; }

            if (!Door.doorEnter) {
                Camera.Update(_objects[0].position + new Vector2(16, 0));
            }
        }

        public void ScreenTransition(string direction, string roomNumber, int timer)
        {
            Camera.cameraMax = Camera.cameraNoBoundsMax;
            Camera.cameraMin = Camera.cameraNoBoundsMin;

            if (direction == "right")
            {
                _objects[0].position.X += 20;
                Entity.applyGravity = true;
                while (timer > 0)
                {
                    Camera.position.X++;
                    Camera.LookAt(Camera.position);
                    timer--;
                }
            }
            if (direction == "left")
            {
                _objects[0].position.X -= 20;
                Entity.applyGravity = true;
                while (timer > 0)
                {
                    Camera.position.X--;
                    Camera.LookAt(Camera.position);
                    timer--;
                }
            }

        }
    }
}
