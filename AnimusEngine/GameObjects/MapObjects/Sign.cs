using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class Sign : GameObject
    {
        Texture2D textboxTexture;

        public Sign(string inputName, Rectangle initPosition)
        {
            position = new Vector2(initPosition.X, initPosition.Y);
            boundingBoxWidth = initPosition.Width;
            boundingBoxHeight = initPosition.Height;
            objectType = "sign";
            solid = true;
        }

        public override void Load(ContentManager content)
        {
            textboxTexture = content.Load<Texture2D>("Sprites/pixel");
            textList.Add("this is a cool sign");
            base.Load(content);
            boundingBoxWidth = 16;
            boundingBoxHeight = 32;
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            base.Update(_objects, map, gameTime);
        }
    }
}
