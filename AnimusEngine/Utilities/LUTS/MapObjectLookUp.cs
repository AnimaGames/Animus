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
using MovingPlatform = AnimusEngine.MovingPlatform;
using System;


namespace AnimusEngine
{
    public static class MapObjectLookUp
    {
        static private GameObject mapObj;

        public static GameObject MapObjLUT(string inputName, Rectangle initPosition)
        {
            switch (inputName)
            {
                case "HorPlatformSmall":
                    mapObj = new MovingPlatform(inputName, initPosition);
                    break;
                case "HorPlatformMed":
                    mapObj = new MovingPlatform(inputName, initPosition);
                    break;
                case "HorPlatformLarge":
                    mapObj = new MovingPlatform(inputName, initPosition);
                    break;

                case "VertPlatformSmall":
                    mapObj = new MovingPlatform(inputName, initPosition);
                    break;
                case "VertPlatformMed":
                    mapObj = new MovingPlatform(inputName, initPosition);
                    break;
                case "VertPlatformLarge":
                    mapObj = new MovingPlatform(inputName, initPosition);
                    break;
                default:
                    Console.WriteLine("got nuthin, stupid");
                    break;
            }
            return mapObj;
        }
    }
}
