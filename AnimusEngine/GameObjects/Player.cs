using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using System;
using MonoGame.Extended;

namespace AnimusEngine
{
    public class Player : Entity
    {
        
        //private bool canMove = true;
        public static bool isOnPlatform;
        public State PlayerState { get; set; }
        public DamageObject attackObj;

        public SoundEffect jumpSFX;
        public SoundEffect attackSFX;
        public SoundEffect hurtSFX;
        const float jumpSpeed = 8.0f;

        public Player()
        { }

        public enum State
        {
            Idle,
            Attacking,
            Jumping,
            Falling,
            Walking,
            Ducking,
            Hurt
        }

        public Player(Vector2 initPosition)
        {
            position = initPosition;
        }

        public override void Initialize()
        {
            attackObj = new DamageObject();
            objectType = "player";
            attackObj.Initialize();
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            // initiliaze sprite
            spriteWidth = 48;
            spriteHeight = 32;
            objectTexture = content.Load<Texture2D>("Sprites/player");
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

            //create animations from sprite sheet
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 1, 2, 3, 4 }, frameDuration: 0.1f, isLooping: true));
            animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 3 }));
            animationFactory.Add("attack", new SpriteSheetAnimationData(new[] { 6, 7, 8, 9 }, frameDuration: 0.1f, isLooping: true));
            animationFactory.Add("hurt", new SpriteSheetAnimationData(new[] { 3 }));

            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;

            objectSprite.Depth = 0.1f;

            attackObj.Load(content);

            // load sound effects
            jumpSFX = content.Load<SoundEffect>("Audio/Sound Effects/Jump");
            attackSFX = content.Load<SoundEffect>("Audio/Sound Effects/playerAttack");
            hurtSFX = content.Load<SoundEffect>("Audio/Sound Effects/playerHurt");

            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 24;
            boundingBoxOffset = new Vector2(17, 7);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            attackObj.Update(_objects, map, gameTime);

            if (!Door.doorEnter && !StateCheck.playerDead && canMove)
            {
                CheckInput(map);
                if (isHurt) { PlayerState = State.Hurt; }
                objectAnimated.Update(gameTime);
            }


            switch (PlayerState)
            {
                case State.Jumping:
                    objectAnimated.Play("jump");
                    break;
                case State.Walking:
                    objectAnimated.Play("walk");
                    break;
                case State.Falling:
                    objectAnimated.Play("fall");
                    break;
                case State.Idle:
                    objectAnimated.Play("idle");
                    break;
                case State.Attacking:
                    objectAnimated.Play("attack", () => PlayerState = State.Idle);
                    break;
                case State.Ducking:
                    objectAnimated.Play("duck");
                    break;
                case State.Hurt:
                    objectAnimated.Play("hurt", () => PlayerState = State.Idle);
                    hurtSFX.Play();
                    isHurt = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Update(_objects, map, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            attackObj.Draw(spriteBatch);
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
            //move left right
            if (keyboardState.IsKeyDown(Keys.Left)) {
                MoveLeft();
                objectAnimated.Effect = SpriteEffects.FlipHorizontally;
            }

            if (keyboardState.IsKeyDown(Keys.Right)) {
                MoveRight();
                objectAnimated.Effect = SpriteEffects.None;
            } 

            if (!applyGravity)
            {
                // move top down in overworld
                if (keyboardState.IsKeyDown(Keys.Up)) {
                    MoveUp();
                }
                if (keyboardState.IsKeyDown(Keys.Down)) {
                    MoveDown();
                }
            } 
            else 
            {
                //jump and attack in levels
                if (keyboardState.WasKeyJustUp(Keys.Space) && !isJumping) {
                    jumpSFX.Play();
                    Jump(map);
                }
                if (keyboardState.WasKeyJustDown(Keys.Space)) {
                    JumpCancel(map);
                }
                if (keyboardState.WasKeyJustUp(Keys.V) && PlayerState != State.Attacking)
                {
                    if (keyboardState.IsKeyDown(Keys.Down) && isJumping){
                        attackObj.CreateDamageObj(this, position, direction, true);
                    }else {
                        attackObj.CreateDamageObj(this, position, direction, false);
                    }
                    if (!isJumping) { velocity.X = 0; }
                    attackSFX.Play();
                    PlayerState = State.Attacking;
                }
            }

            if (PlayerState != State.Attacking)
            {
                if ((int)velocity.X != 0)
                { PlayerState = State.Walking; }
                else
                { PlayerState = State.Idle; }
               
                if (isOnPlatform && !keyboardState.IsKeyDown(Keys.Left) && 
                     !keyboardState.IsKeyDown(Keys.Right))
                { PlayerState = State.Idle; }

                if (isJumping)
                { PlayerState = State.Jumping; }
            }
        }
    }
}
