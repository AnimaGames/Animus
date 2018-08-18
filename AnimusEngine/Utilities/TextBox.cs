using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System.Collections.Generic;

namespace AnimusEngine
{
    public class TextBox : GameObject
    {
        //all the text boxes are drawn in the HUD class
        Texture2D textboxTexture;
        Rectangle textboxRect;

        private SpriteFont font;
        public static bool isInTextBox;
        public static List<string> textBoxText = new List<string>();
        private int itereator;
        private int textTimer;
        private int textTimerMax = 50;

        public TextBox()
        {
        }

        public override void Load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/megaman");
            textboxTexture = content.Load<Texture2D>("Sprites/pixel");
            textboxRect = new Rectangle(30, 40, 340, 80);

            //temp text
            textBoxText.Add("really");
            textBoxText.Add("really really");
            textBoxText.Add("really really really");
            textBoxText.Add("really really really really");
            textBoxText.Add("really really");
            textBoxText.Add("big");
            textBoxText.Add("penis");

            base.Load(content);
        }

        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
            var keyboardState = KeyboardExtended.GetState();

            if (keyboardState.WasKeyJustUp(Keys.V) && textTimer <= 0)
            {
                itereator += 1;
                if (itereator == textBoxText.Count)
                {
                    isInTextBox = false;
                    itereator = 0;
                } else {
                    textTimer = textTimerMax;
                }
            }

            if (textTimer > 0) 
            {
                textTimer--;
            }


            if (isInTextBox)
            {
                for (int i = 0; i < _objects.Count; i++)
                {
                    _objects[i].canMove = false;
                }
            }
            else 
            {
                for (int i = 0; i < _objects.Count; i++)
                {
                    _objects[i].canMove = true;
                }
            }
            base.Update(_objects, map, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isInTextBox)
            {
                spriteBatch.Draw(textboxTexture, textboxRect, Color.Black);

                spriteBatch.DrawString(font, textBoxText[itereator], new Vector2(35, 45), Color.White);
            }
            base.Draw(spriteBatch);
        }
    }
}
