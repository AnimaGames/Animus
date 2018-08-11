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
    public class GameObject
    {
        protected Texture2D objectTexture;
        public TextureAtlas objectAtlas;
        public SpriteSheetAnimationFactory animationFactory;
        public AnimatedSprite objectAnimated;
        public Sprite objectSprite;

        public int spriteHeight, spriteWidth;
        public Vector2 position;
        public Vector2 previousPosition;
        public Vector2 drawPosition;

        public Color drawColor = Color.White;
        public float scale = 1f, rotation = 0f;
        public float layerDepth = 0.5f;
        public bool active = true;
        public Vector2 center;
        public string objectType;

        public bool solid = true;
        public int health = 500;
        public bool enemyInvincible;
        public bool bouncing;
        public Vector2 Knockback;

        public bool invincible;
        public int invincibleTimer;
        public int invincibleTimerMax = 100;
        public bool canMove = true;
        public static int damageAmount = 1;

        protected int boundingBoxWidth, boundingBoxHeight;
        protected Vector2 boundingBoxOffset;
        Texture2D boundingBoxTexture;
        const bool drawBoundingBoxes = true;   //change for visible bounding boxes

        protected Vector2 direction = new Vector2(1, 0);

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)(position.X + boundingBoxOffset.X), 
                                     (int)(position.Y + boundingBoxOffset.Y), 
                                     boundingBoxWidth, 
                                     boundingBoxHeight);
            }
        }

        public GameObject()
        { }

        public virtual void Initialize()
        { }

        public virtual void Load(ContentManager content)
        {
            boundingBoxTexture = content.Load<Texture2D>("Sprites/pixel");
            CalculateCenter();
            if (objectTexture != null)
            {
                boundingBoxHeight = spriteHeight;
                boundingBoxWidth = spriteWidth;
            }
        }

        public virtual void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            drawPosition = new Vector2(position.X + (spriteWidth / 2), position.Y + (spriteHeight / 2));
        }

        public virtual bool CheckCollision(Rectangle init)
        {
            return BoundingBox.Intersects(init);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (boundingBoxTexture != null && drawBoundingBoxes == true && active == true)
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
            if (objectSprite != null && active == true)
            {
                spriteBatch.Draw(objectSprite, drawPosition, 0f);
            }
        }

        private void CalculateCenter()
        {
            if (spriteWidth == 0) { return; }

            center.X = spriteWidth / 2;
            center.Y = spriteHeight / 2;
        }
    }
}
