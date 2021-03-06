﻿using Microsoft.Xna.Framework;
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
    public class Player : Entity
    {
        
        //private bool canMove = true;
        public static bool isOnPlatform;
        public State PlayerState { get; set; }

        private Vector2 attackOffset = new Vector2(13, 0);
        private Vector2 positionOffset = new Vector2(17, 9);
        const float jumpSpeed = 8.0f;

        private int attackTimer;
        private int attackTimerMax = 6;

        private SoundEffect jumpSFX;
        private SoundEffect attackSFX;
        private SoundEffect hurtSFX;
        private SoundEffect deadSFX;

        public Player()
        { }

        public enum State
        {
            Idle,
            Attacking,
            AttackingDown,
            JumpAttack,
            Jumping,
            Falling,
            Walking,
            Ducking,
            Hurt, 
            Dead
        }

        public Player(Vector2 initPosition)
        {
            position = initPosition;
        }

        public override void Initialize()
        {
            objectType = "player";
            health = HUD.playerMaxHealth;
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
            animationFactory.Add("attackDown", new SpriteSheetAnimationData(new[] { 6, 7, 8, 9 }, frameDuration: 0.1f, isLooping: true));
            animationFactory.Add("hurt", new SpriteSheetAnimationData(new[] { 3 }));
            animationFactory.Add("duck", new SpriteSheetAnimationData(new[] { 10 }, frameDuration: 0.1f, isLooping: true));

            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;

            objectSprite.Depth = 0.1f;

            // load sound effects
            jumpSFX = content.Load<SoundEffect>("Audio/Sound Effects/Jump");
            attackSFX = content.Load<SoundEffect>("Audio/Sound Effects/playerAttack");
            hurtSFX = content.Load<SoundEffect>("Audio/Sound Effects/playerHurt");
            deadSFX = content.Load<SoundEffect>("Audio/Sound Effects/playerDeath");

            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 24;
            boundingBoxOffset = new Vector2(17, 7);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            HUD.playerHealth = health;

            if (!Door.doorEnter && !StateCheck.playerDead && canMove && knockbackTimer <=0)
            {
                CheckInput(map);
                if (isHurt) 
                { 
                    PlayerState = State.Hurt; 
                }
                knockbackMult = 1;
                objectAnimated.Update(gameTime);
            } 
            else if (knockbackTimer > 0)
            {
                knockbackMult = 2;
                velocity = NormalizeVector(knockback) * knockbackMult * maxSpeed;
            }

            if (attackTimer < 3 && attackTimer > 0)
            {
                Damage((attackOffset * direction + positionOffset + velocity));
                Damage((this));
                attackTimer--;
            } else if (attackTimer > 0) {
                attackTimer--;
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
                case State.JumpAttack:
                    objectAnimated.Play("attack", () => PlayerState = State.Idle);
                    break;
                case State.AttackingDown:
                    objectAnimated.Play("attackDown", () => PlayerState = State.Idle);
                    break;
                case State.Ducking:
                    objectAnimated.Play("duck");
                    break;
                case State.Dead:
                    boundingBoxHeight = 16;
                    boundingBoxOffset = new Vector2(17, 0);
                    objectAnimated.Effect = SpriteEffects.FlipVertically;
                    break;
                case State.Hurt:
                    objectAnimated.Play("hurt", () => PlayerState = State.Idle);
                    hurtSFX.Play();
                    isHurt = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (health == 0)
            {
                PlayerState = State.Dead;
                deadSFX.Play();
                hurtSFX.Play();
                health--;
                StateCheck.playerDead = true;
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

            if (PlayerState != State.Attacking)
            {
                if (keyboardState.WasKeyJustUp(Keys.Down))
                {
                    PlayerState = State.Ducking;
                    boundingBoxHeight = 16;
                    boundingBoxOffset = new Vector2(17, 15);
                }
                if (keyboardState.WasKeyJustDown(Keys.Down))
                {
                    PlayerState = State.Idle;
                    boundingBoxHeight = 22;
                    boundingBoxOffset = new Vector2(17, 9);
                }

                //move left right
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    MoveLeft();
                    objectAnimated.Effect = SpriteEffects.FlipHorizontally;
                }

                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    MoveRight();
                    objectAnimated.Effect = SpriteEffects.None;
                }

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
                    //if (keyboardState.IsKeyDown(Keys.Down) && isJumping){
                    //    Damage((new Vector2(0, 32)+ positionOffset), true);
                    //    PlayerState = State.AttackingDown;
                    //}  else

                    if (isJumping) 
                    { 
                        //attack in air
                        PlayerState = State.JumpAttack;
                        attackTimer = attackTimerMax;
                        attackSFX.Play();
                    } else {
                        // attack on ground
                        if (!isBySign)
                        {
                            PlayerState = State.Attacking;
                            attackSFX.Play();
                        }
                        attackTimer = attackTimerMax;
                    }
                    if (!isJumping) { velocity.X = 0; }
                }
            }

            if (PlayerState != State.Attacking && 
                PlayerState != State.JumpAttack && 
                PlayerState != State.AttackingDown)
            {
                if ((int)velocity.X != 0)
                { PlayerState = State.Walking; }
                else if (PlayerState != State.Ducking)
                { PlayerState = State.Idle; }
               
                if (isOnPlatform && !keyboardState.IsKeyDown(Keys.Left) && 
                    !keyboardState.IsKeyDown(Keys.Right) && !keyboardState.IsKeyDown(Keys.Down))
                { PlayerState = State.Idle; }

                if (isJumping)
                { PlayerState = State.Jumping; }
            }
        }
    }
}
