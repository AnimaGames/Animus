using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;


namespace AnimusEngine
{
    public class RupeeFive : DestructibleObject
    {
        public RupeeFive()
        {
        }

        public RupeeFive(string inputName, Vector2 initPosition)
        {
            position = initPosition;
            destructibleName = inputName;
            active = true;
            solid = true;
            objectType = "destructible";
            drawColor = Color.CornflowerBlue;
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
                HUD.rupeeCount += 5;
                _objects.Remove(this);
            }
            base.Update(_objects, map, gameTime);
        }
    }
}
