using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;


namespace AnimusEngine
{
    public class Enemy : Entity
    {
        public Enemy()
        { }

        public Enemy(Vector2 initPosition)
        {
            position = initPosition;
            solid = false;
        }

        public override void Initialize()
        {
            objectType = "enemy";
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            // initiliaze sprite
            spriteWidth = spriteHeight = 32;
            objectTexture = content.Load<Texture2D>("Sprites/" + "enemy");
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

            //create animations from sprite sheet
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            animationFactory.Add("idle", new SpriteSheetAnimationData(new[] { 0 }));

            objectAnimated = new AnimatedSprite(animationFactory, "idle");
            objectSprite = objectAnimated;
            objectSprite.Depth = 0.2F;

            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 21;
            boundingBoxOffset = new Vector2(9, 6);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            drawPosition = new Vector2(position.X + (spriteWidth / 2), position.Y + (spriteHeight / 2));
            base.Update(_objects, map, gameTime);
        }
    }
}
