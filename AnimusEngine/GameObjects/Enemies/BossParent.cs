using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;
using System;
using static AnimusEngine.SaveLoad;

namespace AnimusEngine
{
    public class BossParent : Enemy
    {
        public BossParent()
        { }

        public BossParent(Vector2 initPosition, int id)
        {
            position = initPosition;
            objectId = id;
        }

        public override void Initialize()
        {
            health = 3;
            objectType = "enemy";
            solid = false;
            invincibleTimerMax = 30;
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
            objectSprite.Color = Color.MistyRose;
            objectSprite.Depth = 0.2F;

            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 21;
            boundingBoxOffset = new Vector2(9, 6);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            if (health <= 0 && knockbackTimer <= 0)
            {
                Game1._destroyedPermanent.Add(objectType + objectId + Game1.levelNumber + Game1.checkPoint);
                XmlSerialization.WriteToXmlFile("SaveFile0" + Game1.saveSlot + ".txt", Game1._destroyedPermanent);
                _objects.Remove(this);
            }

            if (knockbackTimer > 0)
            {
                knockbackMult = 4;
                velocity = NormalizeVector(knockback) * (maxSpeed * knockbackMult);
            } 
            else 
            {
                knockbackMult = 1;
            }

            base.Update(_objects, map, gameTime);
        }
    }
}
