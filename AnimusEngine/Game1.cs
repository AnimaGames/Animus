using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using static AnimusEngine.SaveLoad;

namespace AnimusEngine
{
    public class Game1 : Game
    {
        // graphics
        GraphicsDeviceManager graphics;
        SpriteBatch _spriteBatch;
        //list of all gameobjects
        readonly List<GameObject> _objects = new List<GameObject>();

        //lists for destoryed objects
        public static List<string> _destroyedObjects = new List<string>();
        public static List<string> _destroyedPermanent = new List<string>();
        readonly List<string> emptyList = new List<string>();
        readonly SceneCreator sceneCreator = new SceneCreator();

        static public SpriteFont font;
        Screens screens = new Screens();
        StateCheck stateCheck = new StateCheck();
        readonly LoadScreen loadScreen = new LoadScreen();
        readonly LoadMenu loadMenu = new LoadMenu(); 

        //level items
        static public string levelNumber;
        static public string previousLevel;
        static public string checkPoint;
      
        //menu items
        static public bool inMenu = true;
        static public int saveSlot;

        //debug
        SpriteFont debugFont;
        float frameRate;

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
            checkPoint = "1";
            HUD.playerHealth = HUD.playerMaxHealth;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            debugFont = Content.Load<SpriteFont>("Fonts/DebugFont");
            font = Content.Load<SpriteFont>("Fonts/megaman");
            screens.Load(Content);
            loadScreen.Load(Content);
            sceneCreator.LevelLoader(Content, graphics.GraphicsDevice, _objects, levelNumber, checkPoint, true);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Door.doorEnter) 
            {
                screens.ScreenTransition(_objects,
                             graphics, 
                             Content,
                             sceneCreator,
                             Map.screenDir,   
                             checkPoint);
                
            }
            if (inMenu && levelNumber == "Load")
            {
                loadScreen.Update(null, null, gameTime);
            }
            Console.WriteLine(saveSlot);
            stateCheck.CheckForDeath(_objects, sceneCreator, graphics, Content, checkPoint);
            CheckForMenu(gameTime);
            frameRate = 1 / (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            
#if DEBUG
            Vector2 cameraPos = Camera.position;
            int xpos = 50;
            int ypos = -115;
            _spriteBatch.DrawString(debugFont, "framerate: " + frameRate, cameraPos + new Vector2(xpos, ypos), Color.White);
            _spriteBatch.DrawString(debugFont, "level number: " + levelNumber, cameraPos +  new Vector2(xpos, ypos + 10), Color.White);
            _spriteBatch.DrawString(debugFont, "room number: " + checkPoint, cameraPos +  new Vector2(xpos, ypos + 20), Color.White);
            _spriteBatch.DrawString(debugFont, "previous level: " + previousLevel, cameraPos +  new Vector2(xpos, ypos + 30), Color.White);
            _spriteBatch.DrawString(debugFont, "object count: " + _objects.Count, cameraPos +  new Vector2(xpos, ypos + 40), Color.White);
            _spriteBatch.DrawString(debugFont, "dead count: " + _destroyedPermanent.Count, cameraPos + new Vector2(xpos, ypos + 50), Color.White);
#endif

            if (_objects.Count == 0)
            {
                sceneCreator._renderer.Draw(Resolution.GetTransformationMatrix());
            } else {
                sceneCreator._renderer.Draw(Camera.GetTransformMatrix());
            }

            sceneCreator.map.DrawWalls(_spriteBatch);

            DrawObjects();

            _spriteBatch.End();

            //draw HUD
            if (_objects.Count > 0)
            {
                _objects[_objects.Count - 1].Draw(_spriteBatch);
            }
            //draw fade transition
            PauseMenu.Draw(_spriteBatch);
            screens.Draw(_spriteBatch);
            loadScreen.Draw(_spriteBatch);
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
            for (int i = 0; i < _objects.Count-1; i++)
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

            if (keyboardState.WasKeyJustUp(Keys.Up))
            {
                loadScreen.menuIndex--;
            }
            else if (keyboardState.WasKeyJustUp(Keys.Down))
            {
                loadScreen.menuIndex++;
            }

            if (keyboardState.WasKeyJustUp(Keys.Enter))
                
            {
                switch (levelNumber)
                {
                    case "StartScreen":
                        levelNumber = "Load";
                        loadScreen.menuIndex = 1;
                        sceneCreator.UnloadObjects(true, _objects);
                        sceneCreator.LevelLoader(Content, graphics.GraphicsDevice, _objects, levelNumber, checkPoint, false);
                        break;

                    case "Load":
                        loadMenu.ChooseSaveFIle(_objects, sceneCreator, Content, GraphicsDevice, loadScreen);

                        break;

                    case "GameOver":
                        levelNumber = previousLevel;
                        sceneCreator.UnloadObjects(true, _objects);
                        inMenu = false;
                        sceneCreator.LevelLoader(Content, graphics.GraphicsDevice, _objects, levelNumber, checkPoint, true);
                        break;

                    default:
                        inMenu = false;
                        PauseMenu.active = false;
                        break;
                }
                if (keyboardState.IsKeyDown(Keys.RightShift) && 
                    keyboardState.IsKeyDown(Keys.V) &&
                    keyboardState.IsKeyDown(Keys.Space))
                {
                    levelNumber = "StartScreen";
                    checkPoint = "1";
                    inMenu = true;
                    Entity.applyGravity = true;
                    sceneCreator.UnloadObjects(true, _objects);
                    sceneCreator.LevelLoader(Content, graphics.GraphicsDevice, _objects, levelNumber, checkPoint, false); 
                }
            }
        }
    }
}