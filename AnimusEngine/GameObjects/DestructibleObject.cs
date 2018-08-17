using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class DestructibleObject : GameObject
    {
        private string destructibleName;

        public DestructibleObject()
        {
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
            objectTexture = content.Load<Texture2D>("Sprites/Destructibles/rupee");
            base.Load(content);
            boundingBoxWidth = 16;
            boundingBoxHeight = 16;
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            base.Update(_objects, map, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }
}
