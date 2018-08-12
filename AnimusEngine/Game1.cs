using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tiled.Renderers;
using System;

namespace AnimusEngine
{
    public class Game1 : Game
    {
        // graphics
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;
        List<GameObject> _objects = new List<GameObject>();
        SceneCreator sceneCreator = new SceneCreator();

        static public SpriteFont font;
        Screens screens = new Screens();
        StateCheck stateCheck = new StateCheck();

        //level items
        static public string levelNumber;
        public string roomNumber;

        //screen trans
        static public string screenDir;
      
        //menu items
        static public bool inMenu = true;
        static public bool hudOn;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Resolution.Init(ref graphics);
            Resolution.SetVirtualResolution(400, 240);
            Resolution.SetResolution(1920, 1080, true);

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);
        }

        protected override void Initialize()
        {
            Camera.Initialize();
            Camera.cameraOffset = new Vector2(Resolution.VirtualWidth / 2, Resolution.VirtualHeight / 2);
            levelNumber = "StartScreen";
            roomNumber = "1";
            HUD.playerHealth = HUD.playerMaxHealth;
         
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("Fonts/megaman");
            sceneCreator.LevelLoader(Content, graphics.GraphicsDevice, _objects, levelNumber, roomNumber);

        }

        protected override void Update(GameTime gameTime)
        {
            if (Door.doorEnter) {
                sceneCreator.UnloadObjects(false, _objects);
                screens.ScreenTransition(_objects, 
                                         sceneCreator, 
                                         graphics, 
                                         Content, 
                                         screenDir, 
                                         roomNumber);
            }
            stateCheck.CheckForDeath(_objects, sceneCreator, graphics, Content, roomNumber);
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
                sceneCreator._renderer.Draw(Resolution.GetTransformationMatrix());
            } else {
                sceneCreator._renderer.Draw(Camera.GetTransformMatrix());
            }

            sceneCreator.map.DrawWalls(_spriteBatch);
            DrawObjects();
            _spriteBatch.End();

            if (_objects.Count > 0)
            {
                sceneCreator.playerHUD.Draw(_spriteBatch);
            }
            base.Draw(gameTime);
        }

        public void UpdateObjects(GameTime gameTime)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                _objects[i].Update(_objects, sceneCreator.map, gameTime);
            }
        }

        public void DrawObjects()
        {
            PauseMenu.Draw(_spriteBatch);
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
                    inMenu = false;
                    PauseMenu.active = false;
                }
                else
                {
                    levelNumber = "1";
                    UnloadContent();
                    inMenu = false;

                    hudOn = true;
                    sceneCreator.LevelLoader(Content, graphics.GraphicsDevice, _objects, levelNumber, roomNumber);
                }
            }
        }
    }
}