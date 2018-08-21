using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using static AnimusEngine.SaveLoad;

namespace AnimusEngine
{
    public class StateCheck
    {
        int deathTimerMax = 150;
        public int deathTimer = 150;
        static public bool playerDead;
        private List<string> clearList = new List<string>();

        public StateCheck()
        {
        }

        //IS PLAYER DEAD?
        public void CheckForDeath(List<GameObject> _objects,
                                 SceneCreator sceneCreator,
                                 GraphicsDeviceManager graphics,
                                 ContentManager content,
                                 string roomNumber)
        {
            if (playerDead)
            {
                if (SceneCreator.soundEffectInstance != null)
                {
                    SceneCreator.soundEffectInstance.Stop(true);
                    SceneCreator.soundEffectInstance.Dispose();
                }

                deathTimer--;

                if (deathTimer <= 0)
                {
                    HUD.playerHealth = HUD.playerMaxHealth;
                    _objects[0].invincible = false;

                    sceneCreator.UnloadObjects(true, _objects);

                    if (HUD.playerLives > 0)
                    {
                        HUD.playerLives--;
                        Game1.inMenu = false;

                        sceneCreator.LevelLoader(content,
                                                 graphics.GraphicsDevice,
                                                 _objects,
                                                 Game1.levelNumber,
                                                 roomNumber,
                                                 true);
                        
                        Camera.LookAt(_objects[0].position);
                    }

                    if (HUD.playerLives == 0)
                    {
                        HUD.playerLives = 3;
                        Game1.levelNumber = "GameOver";
                        Game1.checkPoint = roomNumber = "1";
                        Game1.inMenu = true;
                        HUD.rupeeCount = 0;

                        //clear respawn list 
                        foreach (var obj in Game1._destroyedObjects)
                        {
                            clearList.Add(obj);
                        }
                        foreach(string o in clearList)
                        {
                            Game1._destroyedObjects.Remove(o);
                        }

                        sceneCreator.LevelLoader(content, 
                                                 graphics.GraphicsDevice, 
                                                 _objects, 
                                                 Game1.levelNumber, 
                                                 roomNumber, 
                                                 true);

                        XmlSerialization.WriteToXmlFile("SaveFile0" + Game1.saveSlot + ".txt", Game1._destroyedPermanent);
                        XmlSerialization.WriteToXmlFile("HealthFile0" + Game1.saveSlot + ".txt", HUD.playerMaxHealth);
                    }
                    PauseMenu.active = false;
                    deathTimer = deathTimerMax;
                    playerDead = false;
                }
            }
        }
    }
}
