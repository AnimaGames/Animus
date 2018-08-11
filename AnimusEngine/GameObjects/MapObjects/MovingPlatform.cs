using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using System.Collections.Generic;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Animations.SpriteSheets;

namespace AnimusEngine
{
    public class MovingPlatform : Entity
    {
        public int topRangeX;
        public int topRangeY;
        public int bottomRangeX;
        public int bottomRangeY;
        public string platformSize;
        public string typeOfPlatform;
        public bool isHorizontal;

        public float moveSpeed = 0.5f;

        public MovingPlatform(string platformName, Rectangle initPosition)
        {
            topRangeX = initPosition.Right;
            topRangeY = initPosition.Top;
            bottomRangeX = initPosition.Left;
            bottomRangeY = initPosition.Bottom;
            solid = true;
            active = true;

            typeOfPlatform = platformName;

            //set texture
            if (typeOfPlatform == "HorPlatformSmall" || typeOfPlatform == "VertPlatformSmall")
            {
                platformSize = "PlatformSmall";
            }
            if (typeOfPlatform == "HorPlatformMed" || typeOfPlatform == "VertPlatformMed")
            {
                platformSize = "PlatformMedium";
            }
            if (typeOfPlatform == "HorPlatformLarge" || typeOfPlatform == "VertPlatformLarge")
            {
                platformSize = "PlatformLarge";
            }

            //set direction
            if (typeOfPlatform == "HorPlatformSmall" || typeOfPlatform == "HorPlatformMed" || typeOfPlatform == "HorPlatformLarge")
            {
                isHorizontal = true;
            }
            else 
            {
                isHorizontal = false;
            }

            position = new Vector2 (initPosition.X, initPosition.Y);

        }

        public override void Initialize()
        {
            objectType = "platform"; 
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            objectTexture = content.Load<Texture2D>("Maps/MapObjects/" + platformSize);
            spriteWidth = objectTexture.Width;
            spriteHeight = objectTexture.Height;

            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);

            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;
            objectSprite.Depth = 0.5f;
            base.Load(content);
            boundingBoxWidth = objectTexture.Width;
            boundingBoxHeight = objectTexture.Height;
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            if (isHorizontal)
            {
                if (position.X >= topRangeX)
                {
                    moveSpeed = -moveSpeed;
                }
                else if (position.X <= bottomRangeX && moveSpeed < 0)
                {
                    moveSpeed = -moveSpeed;
                }
                velocity.X = moveSpeed;
            } else 
            {
                if (position.Y >= bottomRangeY)
                {
                    moveSpeed = -moveSpeed;
                }
                else if (position.Y <= topRangeY && moveSpeed < 0)
                {
                    moveSpeed = -moveSpeed;
                } 
                velocity.Y = moveSpeed;
            }

            //drawPosition = new Vector2(position.X + (spriteWidth / 2), position.Y + (spriteHeight / 2));
            base.Update(_objects, map, gameTime);
        }

    }

}
