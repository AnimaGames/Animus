using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using System.Collections.Generic;

namespace AnimusEngine
{
    static public class PauseMenu
    {
        static public Rectangle pauseScreen;
        static public bool active;
        static public Texture2D pauseTexture;
        static public Rectangle pauseScreenRec;
        static private SpriteFont font;

        static public void Load(ContentManager content)
        {
            pauseTexture = content.Load<Texture2D>("Sprites/pixel");
            pauseScreenRec = new Rectangle(-20, -20, Resolution.VirtualWidth + 40, Resolution.VirtualHeight + 40);
            font = content.Load<SpriteFont>("Fonts/megaman");
        }

        static public void Draw(SpriteBatch _spriteBatch)
        {
            if (active)
            {
                _spriteBatch.Begin(SpriteSortMode.BackToFront,
                              BlendState.AlphaBlend,
                              SamplerState.PointClamp,
                              null,
                              null,
                              null,
                              Camera.GetTransformMatrix());
                
                _spriteBatch.Draw(pauseTexture,
                                  new Vector2(Camera.position.X - (Resolution.VirtualWidth / 2) - 20, 
                                              Camera.position.Y - (Resolution.VirtualHeight / 2) - 20),
                                  pauseScreenRec,
                                  new Color(0, 0, 0, 120),
                                  0f,
                                  Vector2.Zero,
                                  1f,
                                  SpriteEffects.None,
                                  0.01f);
                
                _spriteBatch.DrawString(font, "Pause",
                                        new Vector2((Camera.position.X - (font.MeasureString("Pause").X)/2),
                                                    Camera.position.Y), 
                                        Color.White);
                _spriteBatch.End();
            }
        }
    }
}
