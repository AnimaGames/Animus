using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using System;
using Microsoft.Xna.Framework.Content;
using static AnimusEngine.SaveLoad;

namespace AnimusEngine
{
    public class LoadScreen : GameObject
    {
        public int menuIndex;
        public SpriteFont font;
        public Color selectColor = new Color(255, 0, 255, 255);
        public Color nonSelectColor = new Color(255, 255, 255, 255);

        private Texture2D healthFullTexture;
        public Color textDrawColor1; 
        public Color textDrawColor2; 
        public Color textDrawColor3; 
        public Color textDrawColor4;

        public bool deleteMode;

        public LoadScreen()
        {
            menuIndex = 1;
        }

        public override void Load(ContentManager content)
        {
            font = content.Load<SpriteFont>("Fonts/megaman");
            healthFullTexture = content.Load<Texture2D>("Sprites/HUD/playerHealthFull");
            base.Load(content);
        }
      
        public override void Update(List<GameObject> _objects, Map map, GameTime gameTime)
        {
           
            if (menuIndex == 5) //make sure menu index stays within values
            { menuIndex = 1; }
            if (menuIndex == 0)
            { menuIndex = 4; }

            if (deleteMode) //change color for delete mode
            {
                selectColor = new Color(255, 0, 0, 255);
            }
            else
            {
                selectColor = new Color(0, 255, 255, 255);
            }

            switch (menuIndex) // really stupid alternative to an array
            {
                case 1:
                    textDrawColor1 = selectColor;
                    textDrawColor2 = nonSelectColor;
                    textDrawColor3 = nonSelectColor;
                    textDrawColor4 = nonSelectColor;
                    break;
                case 2:
                    textDrawColor1 = nonSelectColor;
                    textDrawColor2 = selectColor;
                    textDrawColor3 = nonSelectColor;
                    textDrawColor4 = nonSelectColor;
                    break;
                case 3:
                    textDrawColor1 = nonSelectColor;
                    textDrawColor2 = nonSelectColor;
                    textDrawColor3 = selectColor;
                    textDrawColor4 = nonSelectColor;
                    break;
                default:
                    textDrawColor1 = nonSelectColor;
                    textDrawColor2 = nonSelectColor;
                    textDrawColor3 = nonSelectColor;
                    textDrawColor4 = selectColor;
                    break;
            }

            base.Update(_objects, map, gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.levelNumber == "Load")
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront,
                              BlendState.AlphaBlend,
                              SamplerState.PointClamp,
                              null,
                              null,
                              null,
                              Resolution.GetTransformationMatrix());

                //file 1
                spriteBatch.DrawString(font, "Save File 01", new Vector2(100, 50), textDrawColor1);
                for (int i = 0; i < XmlSerialization.ReadFromXmlFile<int>("HealthFile01.txt"); i++)
                {
                    spriteBatch.Draw(healthFullTexture, new Vector2(100 + (i * 16), 66), Color.White);
                }

                //file 2
                spriteBatch.DrawString(font, "Save File 02", new Vector2(100, 100), textDrawColor2);

                for (int i = 0; i < XmlSerialization.ReadFromXmlFile<int>("HealthFile02.txt"); i++)
                {
                    spriteBatch.Draw(healthFullTexture, new Vector2(100 + (i * 16), 116), Color.White);
                }

                //file 3
                spriteBatch.DrawString(font, "Save File 03", new Vector2(100, 150), textDrawColor3);
                for (int i = 0; i < XmlSerialization.ReadFromXmlFile<int>("HealthFile03.txt"); i++)
                {
                    spriteBatch.Draw(healthFullTexture, new Vector2(100 + (i * 16), 166), Color.White);
                }

                //delete file
                spriteBatch.DrawString(font, "Delete File", new Vector2(100, 200), textDrawColor4);

                if (deleteMode)
                {
                    spriteBatch.DrawString(font, "Choose File to Delete", new Vector2(75, 25), nonSelectColor);
                }

                spriteBatch.End();
            }

            base.Draw(spriteBatch);
        }

    }
}
