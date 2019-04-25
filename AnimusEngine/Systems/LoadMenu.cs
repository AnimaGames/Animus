using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using static AnimusEngine.SaveLoad;

namespace AnimusEngine
{
    public class LoadMenu
    {
        readonly List<string> emptyList = new List<string>();

        public LoadMenu()
        {}


        public void ChooseSaveFIle(List<GameObject> _objects,
                                   SceneCreator sceneCreator,
                                   ContentManager content,
                                   GraphicsDevice graphics,
                                   LoadScreen loadScreen)
        {
            if (loadScreen.menuIndex != 4)
            {
                Game1.saveSlot = loadScreen.menuIndex;
                if (loadScreen.deleteMode)
                {
                    XmlSerialization.WriteToXmlFile("SaveFile0" + Game1.saveSlot + ".txt", emptyList);
                    XmlSerialization.WriteToXmlFile("HealthFile0" + Game1.saveSlot + ".txt", 3);
                    loadScreen.menuIndex = 1;
                    loadScreen.deleteMode = false;
                    //Console.WriteLine("file deleted");
                }
                else
                {
                    Game1.inMenu = false;
                    Game1.levelNumber = "0";
                    Game1._destroyedPermanent = XmlSerialization.ReadFromXmlFile<List<string>>("SaveFile0" + Game1.saveSlot + ".txt");
                    HUD.playerMaxHealth = XmlSerialization.ReadFromXmlFile<int>("HealthFile0" + Game1.saveSlot + ".txt");
                    sceneCreator.UnloadObjects(true, _objects);
                    sceneCreator.LevelLoader(content, graphics, _objects, Game1.levelNumber, Game1.checkPoint, true);
                }
            }
            else
            {
                if (loadScreen.deleteMode)
                {
                    loadScreen.deleteMode = false;
                }
                else
                {
                    loadScreen.deleteMode = true;
                    loadScreen.menuIndex = 1;
                }
            }
        }
    }
}
