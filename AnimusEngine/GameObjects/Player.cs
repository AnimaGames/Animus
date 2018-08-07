using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
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
        public static bool playerInvinsible;
        private int invincibleTimer;
        private int invincibleTimerMax = 100;
        private bool canMove = true;
        public static bool isOnPlatform;
        public static int damageAmount = 1;
        public State PlayerState { get; set; }
        public DamageObject attackObj;

        public Player()
        { }

        public enum State
        {
            Idle,
            Attacking,
            Jumping,
            Falling,
            Walking,
            Ducking
        }

        public Player(Vector2 initPosition)
        {
            position = initPosition;
        }

        public override void Initialize()
        {
            attackObj = new DamageObject();
            objectType = "player";
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

            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;
            objectSprite.Depth = 0.1f;

            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 24;
            boundingBoxOffset = new Vector2(16, 7);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            drawPosition = new Vector2(position.X + (spriteWidth/2), position.Y + (spriteHeight/2));

            if (!Door.doorEnter && !Game1.playerDead && canMove)
            {
                CheckInput(_objects, map);
                objectAnimated.Update(gameTime);
            }
            Invincible();

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
                default:
                    throw new ArgumentOutOfRangeException();
            }

            base.Update(_objects, map, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //attackObj.Draw(spriteBatch);
            base.Draw(spriteBatch);
        }

        private void CheckInput(List<GameObject> _objects, Map map)
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

            if (!applyGravity)
            {
                if (keyboardState.IsKeyDown(Keys.Up)) {
                    MoveUp();
                    PlayerState = State.Walking;
                }
                if (keyboardState.IsKeyDown(Keys.Down)) {
                    MoveDown();
                    PlayerState = State.Walking;
                }
            } 
            else 
            {
                //jump
                if (keyboardState.WasKeyJustUp(Keys.Space)) {
                    Jump(map);
                }
                if (keyboardState.WasKeyJustDown(Keys.Space)) {
                    JumpCancel(map);
                }
                if (keyboardState.WasKeyJustUp(Keys.V))
                {
                    attackObj.CreateDamageObj(this, position, direction);
                    velocity.X = 0;
                    PlayerState = State.Attacking;
                }
            }

            if (PlayerState != State.Attacking)
            {
                if (keyboardState.IsKeyDown(Keys.Left)) {
                    MoveLeft();
                    objectAnimated.Effect = SpriteEffects.FlipHorizontally;
                }

                if (keyboardState.IsKeyDown(Keys.Right)) {
                    MoveRight();
                    objectAnimated.Effect = SpriteEffects.None;
                } 

                if (velocity.X > 0 || velocity.X < 0)
                { PlayerState = State.Walking; }

                if (velocity == Vector2.Zero || 
                    (isOnPlatform && !keyboardState.IsKeyDown(Keys.Left) && 
                     !keyboardState.IsKeyDown(Keys.Right)))
                { PlayerState = State.Idle; }

                if (isJumping)
                { PlayerState = State.Jumping; }
            }
        }

        private void Invincible()
        {
            if (playerInvinsible && invincibleTimer <= 0)
            {
                canMove = false;
                velocity += Knockback * new Vector2(0.55f, 0.5f);
                invincibleTimer = invincibleTimerMax;
            }

            if (invincibleTimer > 0)
            {
                if (invincibleTimer % 4 == 0)
                {
                    objectSprite.Color = Color.White;
                }
                if (invincibleTimer % 8 == 0)
                {
                    canMove = true; 
                    objectSprite.Color = new Color(0, 0, 0, 0);
                }
                invincibleTimer--;
                playerInvinsible &= invincibleTimer > 0;
            }
        }
    }
}
