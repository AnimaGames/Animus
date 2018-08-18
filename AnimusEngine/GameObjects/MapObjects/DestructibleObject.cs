using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class DestructibleObject : Entity
    {
        public string destructibleName;


        public DestructibleObject()
        {
            health = 1;
            drawColor = Color.White;
        }

        public DestructibleObject(string inputName, Vector2 initPosition)
        {
            position = initPosition;
            destructibleName = inputName;
            active = true;
            solid = true;
#if DEBUG
            drawBoundingBoxes = true;   //change for visible bounding boxes
#endif
        }

        public override void Load(ContentManager content)
        {
            objectTexture = content.Load<Texture2D>("Sprites/tinyCaro");
            base.Load(content);
            boundingBoxWidth = 16;
            boundingBoxHeight = 16;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(objectTexture, position, drawColor);
            base.Draw(spriteBatch);
        }
    }
}
