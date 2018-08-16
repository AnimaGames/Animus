using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using System;

namespace AnimusEngine
{
    public class PlayerOverworld : Entity
    {

        //private bool canMove = true;
        public static bool isOnPlatform;
        public State PlayerState { get; set; }

        private Vector2 attackOffset = new Vector2(16, 0);
        private Vector2 positionOffset = new Vector2(7, 6);
        const float jumpSpeed = 8.0f;

        public SoundEffect jumpSFX;
        public SoundEffect attackSFX;
        public SoundEffect hurtSFX;
        public SoundEffect deadSFX;

        public PlayerOverworld()
        { }

        public enum State
        {
            Right,
            Up,
            Down
        }

        public PlayerOverworld(Vector2 initPosition)
        {
            position = initPosition;
        }

        public override void Initialize()
        {
            objectType = "player";
            health = 3;
            maxSpeed = 1;
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            // initiliaze sprite
            spriteWidth = 20;
            spriteHeight = 20;
            objectTexture = content.Load<Texture2D>("Sprites/playerOverworld");
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

            //create animations from sprite sheet
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            animationFactory.Add("down", new SpriteSheetAnimationData(new[] { 0 }, frameDuration: 0.1f, isLooping: true));
            animationFactory.Add("up", new SpriteSheetAnimationData(new[] { 1 }, frameDuration: 0.1f, isLooping: true));
            animationFactory.Add("right", new SpriteSheetAnimationData(new[] { 2 }));

            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;

            objectSprite.Depth = 0.1f;

            base.Load(content);
            boundingBoxWidth = 16;
            boundingBoxHeight = 16;
            boundingBoxOffset = new Vector2(2, 4);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            HUD.playerHealth = health;

            if (!Door.doorEnter && !StateCheck.playerDead && canMove && knockbackTimer <= 0)
            {
                CheckInput(map);
                objectAnimated.Update(gameTime);
            }

            switch (PlayerState)
            {
                case State.Right:
                    objectAnimated.Play("right");
                    break;
                case State.Down:
                    objectAnimated.Play("down");
                    break;
                case State.Up:
                    objectAnimated.Play("up");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Update(_objects, map, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        private void CheckInput(Map map)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyJustUp(Keys.Enter))
            {
                if (!Game1.inMenu)
                {
                    //darken screen
                    PauseMenu.pauseScreen.X = (int)(Camera.position.X - Camera.cameraOffset.X);
                    PauseMenu.pauseScreenRec.Y = (int)(Camera.position.Y - Camera.cameraOffset.Y);
                    PauseMenu.active = true;
                    Game1.inMenu = true;
                }
            }
            // move top down in overworld
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                MoveLeft();
                PlayerState = State.Right;
                objectAnimated.Effect = SpriteEffects.FlipHorizontally;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                MoveRight();
                PlayerState = State.Right;
                objectAnimated.Effect = SpriteEffects.None;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                PlayerState = State.Up;
                MoveUp();
            } 
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                PlayerState = State.Down;
                MoveDown();
            } 

            if (isOnPlatform && !keyboardState.IsKeyDown(Keys.Left) &&
                                !keyboardState.IsKeyDown(Keys.Right) &&
                                !keyboardState.IsKeyDown(Keys.Down) &&
                                !keyboardState.IsKeyDown(Keys.Up))
            {
                //stand still
            }

        }
    }
}
