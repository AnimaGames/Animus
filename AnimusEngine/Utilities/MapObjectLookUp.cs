using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Entities;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;


namespace AnimusEngine
{
    public static class MapObjectLookUp
    {
        static private GameObject mapObj;

        public static GameObject MapObjLUT(string inputName, Vector2 initPosition)
        {
            switch (inputName)
            {
                case "HorPlatformSmall":
                    mapObj = new PatrolEnemy(initPosition);
                    break;
                case "HorPlatformMed":
                    mapObj = new Enemy(initPosition);
                    break;
                case "HorPlatformLarge":
                    mapObj = new Enemy(initPosition);
                    break;

                case "VertPlatformSmall":
                    mapObj = new PatrolEnemy(initPosition);
                    break;
                case "VertPlatformMed":
                    mapObj = new Enemy(initPosition);
                    break;
                case "VertPlatformLarge":
                    mapObj = new Enemy(initPosition);
                    break;
                default:
                    Console.WriteLine("got nuthin, stupid");
                    break;
            }
            return mapObj;
        }
    }
}
