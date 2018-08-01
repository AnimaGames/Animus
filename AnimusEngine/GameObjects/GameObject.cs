﻿using System;
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
    public class GameObject
    {
        protected Texture2D objectTexture;
        public TextureAtlas objectAtlas;
        public SpriteSheetAnimationFactory animationFactory;
        public AnimatedSprite objectAnimated;

        public Vector2 position;
        public Color drawColor = Color.White;
        public float scale = 1f, rotation = 0f;
        public float layerDepth = 0.5f;
        public bool active = true;
        public Vector2 center;
        public int spriteHeight, spriteWidth;

        public bool solid = true;
        protected int boundingBoxWidth, boundingBoxHeight;
        protected Vector2 boundingBoxOffset;
        Texture2D boundingBoxTexture;
        const bool drawBoundingBoxes = true;   //change for visible bounding boxes

        protected Vector2 direction = new Vector2(1, 0);

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)(position.X + boundingBoxOffset.X), (int)(position.Y + boundingBoxOffset.Y), boundingBoxWidth, boundingBoxHeight);
            }
        }

        public GameObject()
        { }

        public virtual void Initialize()
        {
        }

        public virtual void Load(ContentManager content)
        {
            boundingBoxTexture = content.Load<Texture2D>("pixel");
            CalculateCenter();
            if (objectTexture != null)
            {
                boundingBoxHeight = objectTexture.Height;
                boundingBoxWidth = objectTexture.Width;
            }
        }

        public virtual void Update(List<GameObject> _objects, Map map)
        { }

        public virtual bool CheckCollision(Rectangle init)
        {
            return BoundingBox.Intersects(init);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (boundingBoxTexture != null && active == true)
            {
                spriteBatch.Draw(boundingBoxTexture, new Vector2(BoundingBox.X, BoundingBox.Y), BoundingBox, new Color(255, 0, 0, 128), rotation, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            }
            if (objectTexture != null && active == true)
            {
                spriteBatch.Draw(objectTexture, position, null, drawColor, rotation, Vector2.Zero, scale, SpriteEffects.None, layerDepth);
            }

        }

        private void CalculateCenter()
        {
            if (objectTexture == null) { return; }

            center.X = objectTexture.Width / 2;
            center.Y = objectTexture.Height / 2;
        }
    }
}
