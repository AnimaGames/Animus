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
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 1, 2, 3, 4 }, isLooping: true));

            objectAnimated = new AnimatedSprite(animationFactory, "idle");

            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 24;
            boundingBoxOffset = new Vector2(9, 6);
        }

        public override void Update(List<GameObject> _objects, Map map)
        {
            CheckInput(map);
            base.Update(_objects, map);
        }

        private void CheckInput(Map map)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.IsKeyDown(Keys.Left)) {
                MoveLeft();
            }

            if (keyboardState.IsKeyDown(Keys.Right)) {
                MoveRight();
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
            }
        }
    }
}
