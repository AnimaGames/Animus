using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
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

        public MovingPlatform(string platformName, Rectangle initPosition)
        {
            topRangeX = initPosition.Right;
            topRangeY = initPosition.Top;
            bottomRangeX = initPosition.Left;
            bottomRangeY = initPosition.Bottom;
            solid = true;
            active = true;

            typeOfPlatform = platformName;

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

            drawPosition = new Vector2(Camera.position.X, Camera.position.Y);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            objectTexture = content.Load<Texture2D>("Maps/MapObjects/" + platformSize);
            spriteWidth = objectTexture.Width;
            spriteHeight = objectTexture.Height;

            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            // initiliaze sprite
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));
            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;

            base.Load(content);
            boundingBoxWidth = objectTexture.Width;
            boundingBoxHeight = objectTexture.Height;
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            drawPosition = new Vector2(200, 120);
            base.Update(_objects, map, gameTime);
        }

    }

}
