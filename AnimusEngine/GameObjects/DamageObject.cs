﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class DamageObject : GameObject
    {
        public GameObject owner;
        public int deathTimer;
        public int deathTimerMax = 3;

        public bool bounce;

        public DamageObject()
        {
            objectType = "damage";
            solid = true;
            active = false;
#if DEBUG
            drawBoundingBoxes = true;   //change for visible bounding boxes
#endif
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            boundingBoxWidth = 16;
            boundingBoxHeight = 16;
            boundingBoxOffset = new Vector2(9, 6);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            if (active) { CheckCollisions(_objects); }

            if (deathTimer <= 0)
            {
                Destroy(_objects);
            } else {
                deathTimer--;
            }
           
            base.Update(_objects, map, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (active && drawBoundingBoxes)
            {
                spriteBatch.Draw(boundingBoxTexture,
                                     new Vector2(BoundingBox.X, BoundingBox.Y),
                                     BoundingBox,
                                     new Color(255, 0, 0, 128),
                                     rotation,
                                     Vector2.Zero,
                                     scale,
                                     SpriteEffects.None,
                                     layerDepth);
            }
            base.Draw(spriteBatch);
        }

        public void CheckCollisions(List<GameObject> _objects)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i].active && 
                    _objects[i] != owner && 
                    !_objects[i].invincible &&
                    (_objects[i].objectType == "player" || 
                     _objects[i].objectType == "enemy" || 
                     _objects[i].objectType == "destructible") &&
                    _objects[i].CheckCollision(BoundingBox))
                {
                    _objects[i].knockback = new Vector2(-(owner.position.X - _objects[i].position.X),
                                                        (_objects[i].position.Y - owner.position.Y));
                    
                    _objects[i].invincible = true;
                    Destroy(_objects);

                    // bouce off enemy
                    if (bounce)
                    {
                        _objects[0].bouncing = true;
                        _objects[0].knockback = new Vector2(0, -40);
                        bounce = false;
                    }
                }
            }
        }

        public void Damage(GameObject inputOwner, Vector2 initPosition)
        {
            owner = inputOwner;
            position = initPosition;
            active = true;
            deathTimer = deathTimerMax;
        }

        public void Destroy(List<GameObject> _objects)
        {
                _objects.Remove(this);
                active = false;
        }
    }
}
