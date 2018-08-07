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

namespace AnimusEngine
{
    public class DamageObject : GameObject
    {

        public int radius;
        public int deathTimer;
        public int damageAmount;
        public int deathTimerMax = 40;
        Entity owner;

        public DamageObject()
        {
            active = false;
        }

        public override void Initialize()
        {
            objectType = "damage";
            solid = true;
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
            drawPosition = position;
            deathTimer--;
            if (deathTimer <= 0)
            {
               // _objects.Remove(this);
            }
            base.Update(_objects, map, gameTime);
        }

        public void CheckCollisions(List<GameObject> objects, Map map)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].active && objects[i] != owner && objects[i].CheckCollision(BoundingBox))
                {
                    objects[i].health -= damageAmount;
                    Destroy(); 
                }
            }
        }

        public void Destroy()
        {
            active = false;
        }

        public void CreateDamageObj(Entity inputOwner, Vector2 initPosition, Vector2 initDirection)
        {
            owner = inputOwner;
            position = initPosition;
            direction = initDirection;
            active = true;
            deathTimer = deathTimerMax;
        }
    }
}
