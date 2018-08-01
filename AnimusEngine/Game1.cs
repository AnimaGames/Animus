using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;

namespace AnimusEngine.Desktop
{
    public class Game1 : Game
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
        TiledMap _map;
        TiledMapRenderer _renderer;
        Map map = new Map();
        public List<GameObject> _objects = new List<GameObject>();


        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Resolution.Init(ref _graphics);
            Resolution.SetVirtualResolution(400, 240);
            Resolution.SetResolution(854, 480, false);
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Camera.Initialize();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            LevelLoader("Level_00_00");
            // TODO: use this.Content to load your game content here

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

 
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Input.Update();
            UpdateCamera();
            UpdateObjects();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            Resolution.BeginDraw();
            _spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Camera.GetTransformMatrix());
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

            _objects.Add(new Player(new Vector2(200, 100)));
            _objects.Add(new Enemy(new Vector2(300, 100)));
            LoadObjects();
        }

        public void LoadObjects()
        {
            for (int i=0; i<_objects.Count; i++)
            {
                _objects[i].Initialize();
                _objects[i].Load(Content);
            }
        }

        public void UpdateObjects()
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].Update(_objects, map);
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
            Camera.Update(_objects[0].position);
        }
    }
}
