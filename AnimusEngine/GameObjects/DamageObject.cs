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
    public class DamageObject : GameObject
    {
        public int radius;
        public int deathTimer;
        public int deathTimerMax = 15;
        Entity owner;
        public bool bounce;

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
            base.Load(content);
            boundingBoxWidth = 14;
            boundingBoxHeight = 21;
            boundingBoxOffset = new Vector2(9, 6);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            //CheckCollisions(_objects);

            if (deathTimer <= 0)
            {
                Destroy();
            } else {
                deathTimer--;
            }
           
            base.Update(_objects, map, gameTime);
        }

        //public void CheckCollisions(List<GameObject> objects)
        //{ if (active)
        //    {
        //        for (int i = 0; i < objects.Count; i++)
        //        {
        //            if (objects[i].active &&objects[i].objectType == "enemy" && objects[i] != owner && objects[i].CheckCollision(BoundingBox))
        //            {
        //                objects[i].Knockback = new Vector2(-(position.X - objects[i].position.X),
        //                                        (position.Y - objects[i].position.Y));
        //                if (bounce){
        //                    objects[0].bouncing = true;
        //                    objects[0].Knockback = new Vector2(0, -40);
        //                    bounce = false;
        //                }
        //                objects[i].enemyInvincible = true;
        //                objects[i].health -= 1;
        //                Destroy();
        //            }
        //        }
        //    }
        //}

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public void Destroy()
        {
            active = false;
        }

        public void CreateDamageObj(Entity inputOwner, Vector2 initPosition, Vector2 initDirection, bool isDown)
        {
            owner = inputOwner;
            position = initPosition;
            direction = initDirection;
            deathTimer = deathTimerMax;
            active = true;

            if (isDown)
            {
                bounce = true;
                position = new Vector2(initPosition.X+6, initPosition.Y + 50);

            } else
            {
                if (direction.X > 0) {
                    position = new Vector2(initPosition.X + 18, initPosition.Y + 6);
                } else {
                    position = new Vector2(initPosition.X - 5, initPosition.Y + 6);
                }
            }
        }
    }
}
