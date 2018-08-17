using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Input;
using MonoGame.Extended.Tiled.Renderers;
using System;
using Microsoft.Xna.Framework.Content;

namespace AnimusEngine
{
    public class StateCheck
    {
        int deathTimerMax = 150;
        public int deathTimer = 150;
        static public bool playerDead;

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
                        sceneCreator.LevelLoader(content, 
                                                 graphics.GraphicsDevice, 
                                                 _objects, 
                                                 Game1.levelNumber, 
                                                 roomNumber, 
                                                 true);
                    }
                    PauseMenu.active = false;
                    deathTimer = deathTimerMax;
                    playerDead = false;
                }
            }
        }
    }
}
