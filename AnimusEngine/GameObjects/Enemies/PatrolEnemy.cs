using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using System;

namespace AnimusEngine
{
    public class PatrolEnemy : Enemy
    {
        public float walkSpeed;
        private float previousX;
        Random randomNum;

        public PatrolEnemy()
        {
        }

        public PatrolEnemy(Vector2 initPosition, int id)
        {
            position = initPosition;
            maxSpeed = 1;
            objectId = id;
            walkSpeed = 0.3f;
        }

        public override void Initialize()
        {
            randomNum = new Random();
            previousX = position.X;
            walkSpeed = (walkSpeed * randomNum.Next(2, 5))/3;
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            // initiliaze sprite
            spriteWidth = spriteHeight = 32;
            objectTexture = content.Load<Texture2D>("Sprites/" + "enemy");
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

            //create animations from sprite sheet
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            animationFactory.Add("walk", new SpriteSheetAnimationData(new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, frameDuration: 0.1f, isLooping: true));

             objectAnimated = new AnimatedSprite(animationFactory, "walk");

            objectSprite = objectAnimated;
            objectSprite.Depth = 0.2F;

            boundingBoxWidth = 14;
            boundingBoxHeight = 21;
            boundingBoxOffset = new Vector2(9, 6);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            objectAnimated.Play("walk");
            objectAnimated.Update(gameTime);

            if (knockbackTimer <= 0)
            {
                velocity.X = walkSpeed;

                if (position.X == previousX)
                {
                    walkSpeed = -walkSpeed;
                    position.X += walkSpeed;
                    if (objectAnimated.Effect == SpriteEffects.None)
                    {
                        objectAnimated.Effect = SpriteEffects.FlipHorizontally;
                    }
                    else
                    {
                        objectAnimated.Effect = SpriteEffects.None;
                    }
                }
                previousX = position.X;
            }
            base.Update(_objects, map, gameTime);
        }
    }
}
