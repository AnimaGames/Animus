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
    public class Entity : GameObject
    {
        public Vector2 velocity;
        public Vector2 parentPosition;
        //customize movement
        protected float friction = .8f;
        protected float accel = 0.75f;
        protected float maxSpeed = 2.0f;

        const float gravity = 1f;
        const float gravityAdjust = 0.5f;
        const float jumpSpeed = 8.0f;
        const float termVel = 8.0f;
        public Vector2 Knockback;

        protected bool isJumping;
        public static bool applyGravity = true;

        public Entity()
        { }

        public override void Initialize()
        {
            velocity = Vector2.Zero;
            previousPosition = position;
            parentPosition = Vector2.Zero;
            isJumping = false;
            base.Initialize();
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            UpdateMovement(_objects, map);
            base.Update(_objects, map, gameTime);
        }

        private void UpdateMovement(List<GameObject> _objects, Map map)
        {
            if ((velocity.X > 0 || velocity.X < 0) && CheckCollisions(map, _objects, true))
            {
                velocity.X = 0;
            }
            position.X += velocity.X;

            if ((velocity.Y > 0 || velocity.Y < 0) && CheckCollisions(map, _objects, false))
            {
                velocity.Y = 0;
            }
            position.Y += velocity.Y;

            if (applyGravity)
            {
                ApplyGravity(map);

                if (OnGround(map) != Rectangle.Empty && velocity.Y == 0)
                {
                    isJumping = false;
                }
                else if (OnGround(map) == Rectangle.Empty && velocity.Y == 0 && objectType != "platform")
                {
                    position.Y -= 1;
                }
            }

            velocity.X = TendToZero(velocity.X, friction);
            if (!applyGravity)
            {
                velocity.Y = TendToZero(velocity.Y, friction);
            }
        }

        private void ApplyGravity(Map map)
        {
            if ((isJumping || OnGround(map) == Rectangle.Empty) && objectType != "platform")
            {
                velocity.Y += (float)(gravity*gravityAdjust);
            }
            if (velocity.Y > termVel)
            {
                velocity.Y = termVel;
            }
        }

        protected void MoveRight()
        {
            velocity.X += accel + friction;

            if (velocity.X > maxSpeed)
            {
                velocity.X = maxSpeed;
            }
            direction.X = 1;
        }
        protected void MoveLeft()
        {
            velocity.X -= accel + friction;

            if (velocity.X < -maxSpeed)
            {
                velocity.X = -maxSpeed;
            }
            direction.X = -1;
        }
        protected void MoveDown()
        {
            velocity.Y += accel + friction;

            if (velocity.Y > maxSpeed)
            {
                velocity.Y = maxSpeed;
            }
            direction.Y = 1;
        }
        protected void MoveUp()
        {
            velocity.Y -= accel + friction;

            if (velocity.Y < -maxSpeed)
            {
                velocity.Y = -maxSpeed;
            }
            direction.Y = -1;
        }

        protected bool Jump(Map map)
        {
            if (isJumping) { return false; }

            if (((int)velocity.Y == 0 && OnGround(map) != Rectangle.Empty) || Player.isOnPlatform)
            {
                velocity.Y -= jumpSpeed;
                isJumping = true;
                return true;
            }
            return false;
        }
        protected bool JumpCancel(Map map)
        {
            if (!isJumping || velocity.Y > 0) { return false; }

            velocity.Y = 0;
            return true;
        }


        protected virtual bool CheckCollisions(Map map, List<GameObject> _objects, bool xAxis)
        {
            Rectangle futureBoundingBox = BoundingBox;

            int maxX = (int)maxSpeed;
            int maxY = (int)maxSpeed;

            if (applyGravity) { maxY = (int)jumpSpeed; }

            if (xAxis && (velocity.X > 0 || velocity.X < 0))
            {
                if (velocity.X > 0)
                {
                    futureBoundingBox.X += maxX;
                } else {
                    futureBoundingBox.X -= maxX;
                }
            } 
            else if (!applyGravity && !xAxis && (velocity.Y > 0 || velocity.Y < 0))
            {
                if (velocity.Y > 0)
                {
                    futureBoundingBox.Y += maxY;
                } else {
                    futureBoundingBox.Y -= maxY;
                }
            }
            else if (applyGravity && !xAxis && (velocity.Y > gravity || velocity.Y < gravity))
            {
                if (velocity.Y > 0)
                {
                    futureBoundingBox.Y += maxY;
                }
                else
                {
                    futureBoundingBox.Y -= maxY;
                }
            }


            Rectangle wallCollision = map.CheckCollisions(futureBoundingBox);

            if (wallCollision != Rectangle.Empty)
            {
                if (applyGravity && 
                    (futureBoundingBox.Bottom >= wallCollision.Top - maxSpeed) && 
                    (futureBoundingBox.Bottom <= wallCollision.Top + 8) &&
                    velocity.Y > 0)
                {
                    LandResponse(wallCollision);
                    return true;
                } else {
                    return true;
                }
            }

            //check for object collisions
            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i] != this &&
                    objectType != "platform" &&
                    _objects[i].active &&
                    _objects[i].CheckCollision(futureBoundingBox) &&
                    (_objects[i].objectType == "player" || _objects[i].objectType == "enemy")
                   )
                {
                    //hurt player
                    if (!Player.playerInvinsible)
                    {
                        Knockback = new Vector2((position.X - _objects[i].position.X),
                                                (position.Y - _objects[i].position.Y));
                        HUD.playerHealth -= 1;
                        Player.playerInvinsible = true;
                    }
                }
                // moving platform
                if (_objects[i] != this &&
                    _objects[i].active &&
                    _objects[i].objectType == "platform" &&
                    _objects[i].CheckCollision(futureBoundingBox))
                {
                    parentPosition = _objects[i].position - _objects[i].previousPosition;

                    if ((applyGravity &&
                        (futureBoundingBox.Bottom >= _objects[i].BoundingBox.Top - maxSpeed) &&
                        (futureBoundingBox.Bottom <= _objects[i].BoundingBox.Top + 8)) || 
                        Player.isOnPlatform)

                    {
                        position += parentPosition;
                        LandOnPlatform(_objects[i].BoundingBox);
                        return true;
                    }
                    return true;
                }
                _objects[i].previousPosition = _objects[i].position;
            }
            return false;
        }

        public void LandResponse(Rectangle wallCollision)
        {
            position.Y = wallCollision.Top - (boundingBoxHeight + boundingBoxOffset.Y);
            velocity.Y = 0f;
            Player.isOnPlatform = false;
            isJumping = false;
        }
        public void LandOnPlatform(Rectangle wallCollision)
        {
            position.Y = wallCollision.Top - (boundingBoxHeight + boundingBoxOffset.Y+.4f);
            velocity.Y = 0f;
            Player.isOnPlatform = true;
            isJumping = false;
        }

        protected Rectangle OnGround(Map map)
        {
            Rectangle futureBoundingBox = new Rectangle((int)(position.X + boundingBoxOffset.X), (int)(position.Y + boundingBoxOffset.Y + (velocity.Y + gravity)), boundingBoxWidth, boundingBoxHeight);

            return map.CheckCollisions(futureBoundingBox);
        }

        protected float TendToZero(float val, float amount)
        {
            if (val > 0f && (val -= amount) < 0f) return 0f;
            if (val < 0f && (val += amount) > 0f) return 0f;
            return val;
        }
    }
}
