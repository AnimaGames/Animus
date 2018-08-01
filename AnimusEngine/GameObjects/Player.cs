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
            objectTexture = content.Load<Texture2D>("player");
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

            //create animations from sprite sheet
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 1, 2, 3, 4 }, frameDuration: 0.1f, isLooping: true));
            animationFactory.Add("jump", new SpriteSheetAnimationData(new[] { 4 }));

            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;

            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 24;
            boundingBoxOffset = new Vector2(9, 7);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            drawPosition = new Vector2(position.X + (spriteWidth/2), position.Y + (spriteHeight/2));
            CheckInput(map);
            objectAnimated.Update(gameTime);
            if (velocity.X == 0) { objectAnimated.Play("idle"); }
            base.Update(_objects, map, gameTime);
        }

        private void CheckInput(Map map)
        {
            var keyboardState = KeyboardExtended.GetState();

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
    }
}
