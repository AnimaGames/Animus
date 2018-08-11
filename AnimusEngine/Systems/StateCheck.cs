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
        static public int deathTimer = 100;
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
            if (HUD.playerHealth <= 0)
            {
                playerDead = true;
                deathTimer--;

                if (deathTimer <= 0 && HUD.playerLives > 0)
                {
                    HUD.playerLives--;
                    HUD.playerHealth = HUD.playerMaxHealth;
                    sceneCreator.UnloadObjects(true, _objects);
                    PauseMenu.active = false;
                    Game1.inMenu = false;
                    sceneCreator.LevelLoader(content, graphics.GraphicsDevice, _objects, Game1.levelNumber, roomNumber);
                    deathTimer = 50;
                    _objects[0].invincible = false;
                    playerDead = false;
                    Camera.LookAt(_objects[0].position);
                }

                if (deathTimer <= 0 && HUD.playerLives == 0)
                {
                    HUD.playerHealth = HUD.playerMaxHealth;
                    HUD.playerLives = 3;
                    Game1.levelNumber = "StartScreen";
                    sceneCreator.UnloadObjects(true, _objects);
                    PauseMenu.active = false;
                    Game1.inMenu = true;
                    sceneCreator.LevelLoader(content, graphics.GraphicsDevice, _objects, Game1.levelNumber, roomNumber);
                    deathTimer = 50;
                    _objects[0].invincible = false;
                    playerDead = false;
                }
            }
        }
    }
}
