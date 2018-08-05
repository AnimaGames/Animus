using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;

namespace AnimusEngine
{
    public class PatrolEnemy : Enemy
    {
        public float walkSpeed;
        private float previousX;

        public PatrolEnemy()
        {
        }

        public PatrolEnemy(Vector2 initPosition)
        {
            position = initPosition;
            solid = false;
            maxSpeed = 1;
            walkSpeed = 0.3f;
        }

        public override void Initialize()
        {
            previousX = position.X;
            base.Initialize();
        }

        public override void Load(ContentManager content)
        {
            // initiliaze sprite
            spriteWidth = spriteHeight = 32;
            objectTexture = content.Load<Texture2D>("Sprites/" + "tinyCaro");
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);

            //create animations from sprite sheet
            animationFactory = new SpriteSheetAnimationFactory(objectAtlas);
            objectAtlas = TextureAtlas.Create("objectAtlas", objectTexture, spriteWidth, spriteHeight);
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

            velocity.X = walkSpeed;

            if (position.X == previousX)
            {
                walkSpeed = -walkSpeed;
                position.X += walkSpeed;
                if(objectAnimated.Effect == SpriteEffects.None)
                {
                    objectAnimated.Effect = SpriteEffects.FlipHorizontally;
                } else {
                    objectAnimated.Effect = SpriteEffects.None;
                }
            }
            previousX = position.X;
        }
    }
}
