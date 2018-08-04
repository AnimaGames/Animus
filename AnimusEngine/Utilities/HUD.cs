using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Input;
using System.Collections.Generic;
using MonoGame.Extended.Animations.SpriteSheets;
using MonoGame.Extended.TextureAtlases;
using MonoGame.Extended.Animations;

namespace AnimusEngine
{
    public class HUD : GameObject
    {
        private int playerMaxHealth = 5;
        private int playerHealth = 3;
        private int playerLives = 3;
        private Texture2D healthFullTexture;
        private Texture2D healthEmptyTexture;
        private Texture2D livesTexture;

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        { }

        public override void Load(ContentManager content)
        {
            healthFullTexture = content.Load<Texture2D>("Sprites/HUD/playerHealthFull");
            healthEmptyTexture = content.Load<Texture2D>("Sprites/HUD/playerHealthEmpty");
            livesTexture = content.Load<Texture2D>("Sprites/HUD/playerLives");
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (healthFullTexture != null)
            {
                spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Resolution.GetTransformationMatrix());

                for (int i = 0; i < playerMaxHealth; i++)
                {
                    spriteBatch.Draw(healthFullTexture, new Vector2(12 + (i * 16), 12), Color.White);
                }
                for (int i = 0; i < playerHealth; i++)
                {
                    spriteBatch.Draw(healthFullTexture, new Vector2(12 + (i * 16),12), Color.White);
                }
                for (int i = 0; i < playerLives; i++)
                {
                    spriteBatch.Draw(livesTexture, new Vector2(12 + (i * 16), 24), Color.White);
                }

                spriteBatch.End();
            }
        }
    }
}
