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
    public class Screens
    {
        public int screenTimer = 50;
        static public string roomPlaceHolder;

        public Screens()
        {
        }

        public void ScreenTransition(List<GameObject> _objects,
                                     SceneCreator sceneCreator,
                                     GraphicsDeviceManager graphics,
                                     ContentManager content,
                                     string direction,
                                     string roomNumber)
        {
            
            if (direction == "right")
            {
                Camera.cameraMin.X = Camera.position.X;
                _objects[0].position.Y = _objects[0].position.Y;
                _objects[0].position.X += 0.5f;
                Camera.cameraMin.X += 8;
                Camera.cameraMax.X += 8;
            }
            if (direction == "left")
            {
                Camera.cameraMax.X = Camera.position.X;
                _objects[0].position.Y = _objects[0].position.Y;
                _objects[0].position.X -= 0.5f;
                Camera.cameraMin.X -= 8;
                Camera.cameraMax.X -= 8;
            }
            screenTimer--;

            if (screenTimer < 0)
            {
                roomNumber = roomPlaceHolder;
                Door.doorEnter = false;
                screenTimer = 50;
                sceneCreator.LevelLoader(content, graphics.GraphicsDevice, _objects, Game1.levelNumber, roomNumber);
                Entity.applyGravity = true;
            }
        }
    }
}
