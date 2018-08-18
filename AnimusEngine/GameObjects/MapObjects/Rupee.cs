using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace AnimusEngine
{
    public class Rupee : DestructibleObject
    {
        public Rupee()
        {
        }

        public Rupee(string inputName, Vector2 initPosition)
        {
            position = initPosition;
            destructibleName = inputName;
            active = true;
            solid = true;
            objectType = "destructible";
#if DEBUG
            drawBoundingBoxes = true;   //change for visible bounding boxes
#endif
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            objectTexture = content.Load<Texture2D>("Sprites/Destructibles/rupee");
            boundingBoxWidth = 16;
            boundingBoxHeight = 16;
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            if (health <= 0 && knockbackTimer <= 0)
            {
                HUD.rupeeCount++;
                Game1._destroyedObjects.Add(objectType + objectId + Game1.levelNumber + Game1.checkPoint);
                _objects.Remove(this);
            }
            base.Update(_objects, map, gameTime);
        }
    }
}
