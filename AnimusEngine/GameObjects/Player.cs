using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;


namespace AnimusEngine
{
    public class Player : Entity
    {
        public static bool playerInvinsible;
        private int invincibleTimer;
        private int invincibleTimerMax = 100;

        public Player()
        { }

        public Player(Vector2 initPosition)
        {
            position = initPosition;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            // initiliaze sprite
            spriteWidth = spriteHeight = 32;
            objectTexture = content.Load<Texture2D>("Sprites/player");
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

            //create animations from sprite sheet
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 1, 2, 3, 4 }, frameDuration: 0.1f, isLooping: true));
            animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 4 }));

            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;
            objectSprite.Depth = 0.1f;
                        
            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 24;
            boundingBoxOffset = new Vector2(9, 7);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            drawPosition = new Vector2(position.X + (spriteWidth/2), position.Y + (spriteHeight/2));

            if (!Door.doorEnter && !Game1.playerDead)
            {
                CheckInput(map);
                objectAnimated.Update(gameTime);
            }
            Invincible();

            base.Update(_objects, map, gameTime);
        }

        private void CheckInput(Map map)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyJustUp(Keys.Enter))
            {
                Game1.inMenu = true;
            }

            if (keyboardState.IsKeyDown(Keys.Left)) {
                MoveLeft();
                objectAnimated.Play("walk");
                objectAnimated.Effect = SpriteEffects.FlipHorizontally;
            }

            if (keyboardState.IsKeyDown(Keys.Right)) {
                MoveRight();
                objectAnimated.Play("walk");
                objectAnimated.Effect = SpriteEffects.None;
            } 
            if (!keyboardState.IsKeyDown(Keys.Right) && (!keyboardState.IsKeyDown(Keys.Left)))
            {
                objectAnimated.Play("idle");
            }

            if (!applyGravity)
            {
                if (keyboardState.IsKeyDown(Keys.Up)) {
                    MoveUp();
                }
                if (keyboardState.IsKeyDown(Keys.Down)) {
                    MoveDown();
                }
            } 
            else 
            {
                if (keyboardState.WasKeyJustUp(Keys.Space)) {
                    Jump(map);
                }
                if (keyboardState.WasKeyJustDown(Keys.Space)) {
                    JumpCancel(map);
                }
                if (isJumping) 
                {
                    objectAnimated.Play("jump");
                }
            }
        }
        private void Invincible()
        {
            if (playerInvinsible && invincibleTimer <= 0)
            {
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
                    objectSprite.Color = new Color(0, 0, 0, 0);
                }

                invincibleTimer--;
                playerInvinsible &= invincibleTimer > 0;
            }
        }
    }
}
