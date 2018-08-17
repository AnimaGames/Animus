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
            Left,
            Up,
            Down,
            RightWalk,
            LeftWalk,
            UpWalk,
            DownWalk
        }

        public PlayerOverworld(Vector2 initPosition)
        {
            position = initPosition;
        }

        public override void Initialize()
        {
            objectType = "player";
            PlayerState = State.Down;
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
            animationFactory.Add("down", new SpriteSheetAnimationData(new[] { 0 }));
            animationFactory.Add("downwalk", new SpriteSheetAnimationData(new[] { 1, 2 }, frameDuration: 0.2f, isLooping: true));

            animationFactory.Add("up", new SpriteSheetAnimationData(new[] { 3 }));
            animationFactory.Add("upwalk", new SpriteSheetAnimationData(new[] { 4, 5 }, frameDuration: 0.2f, isLooping: true));

            animationFactory.Add("right", new SpriteSheetAnimationData(new[] { 6 }));
            animationFactory.Add("rightwalk", new SpriteSheetAnimationData(new[] { 7, 8 }, frameDuration: 0.2f, isLooping: true));

            animationFactory.Add("left", new SpriteSheetAnimationData(new[] { 9 }));
            animationFactory.Add("leftwalk", new SpriteSheetAnimationData(new[] { 10, 11 }, frameDuration: 0.2f, isLooping: true));

            objectAnimated = new AnimatedSprite(animationFactory, "down");
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

            CheckInput();
            objectAnimated.Update(gameTime);
            

            switch (PlayerState)
            {
                case State.Right:
                    objectAnimated.Play("right");
                    break;
                case State.Left:
                    objectAnimated.Play("left");
                    break;
                case State.Down:
                    objectAnimated.Play("down");
                    break;
                case State.Up:
                    objectAnimated.Play("up");
                    break;

                case State.RightWalk:
                    objectAnimated.Play("rightwalk");
                    break;
                case State.LeftWalk:
                    objectAnimated.Play("leftwalk");
                    break;
                case State.DownWalk:
                    objectAnimated.Play("downwalk");
                    break;
                case State.UpWalk:
                    objectAnimated.Play("upwalk");
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

        private void CheckInput()
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
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                MoveRight();
                objectAnimated.Effect = SpriteEffects.None;
                PlayerState = State.RightWalk;
            }
            else if (keyboardState.IsKeyDown(Keys.Up))
            {
                MoveUp();
                objectAnimated.Effect = SpriteEffects.None;
                PlayerState = State.UpWalk;
            } 
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                MoveDown();
                objectAnimated.Effect = SpriteEffects.None;
                PlayerState = State.DownWalk;
            } 
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                MoveLeft();
                PlayerState = State.LeftWalk;
            }

            if (velocity == Vector2.Zero)
            {
                switch (PlayerState)
                {
                    case State.DownWalk:
                        PlayerState = State.Down;
                        break;
                    case State.RightWalk:
                        PlayerState = State.Right;
                        break;
                    case State.LeftWalk:
                        PlayerState = State.Left;
                        break;
                    case State.UpWalk:
                        PlayerState = State.Up;
                        break;
                }
            }
        }
    }
}
