using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static AnimusEngine.SaveLoad;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class HeartUpgrade : DestructibleObject
    {
        public HeartUpgrade(string inputName, Vector2 initPosition)
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
            objectTexture = content.Load<Texture2D>("Sprites/Destructibles/heartUpgrade");
            boundingBoxWidth = 16;
            boundingBoxHeight = 16;
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            if (health <= 0 && knockbackTimer <= 0)
            {
                HUD.playerMaxHealth++;
                Game1._destroyedPermanent.Add(objectType + objectId + Game1.levelNumber + Game1.checkPoint);
                _objects.Remove(this);
                _objects[0].health = HUD.playerMaxHealth;

                // save the game
                XmlSerialization.WriteToXmlFile("SaveFile0" + Game1.saveSlot + ".txt", Game1._destroyedPermanent);
                XmlSerialization.WriteToXmlFile("HealthFile0" + Game1.saveSlot + ".txt", HUD.playerMaxHealth);
            }
            base.Update(_objects, map, gameTime);
        }
    }
}
