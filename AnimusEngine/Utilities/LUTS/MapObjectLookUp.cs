using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
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
                    //rupee drops
                case "rupee":
                    mapObj = new Rupee(inputName, new Vector2(initPosition.X, initPosition.Y));
                    break;
                case "rupeefive":
                    mapObj = new RupeeFive(inputName, new Vector2(initPosition.X, initPosition.Y));
                    break;
                case "rupeeten":
                    mapObj = new RupeeTen(inputName, new Vector2(initPosition.X, initPosition.Y));
                    break;
                case "Sign":
                    mapObj = new Sign(inputName, initPosition);
                    break;
                case "heartupgrade":
                    mapObj = new HeartUpgrade(inputName, new Vector2(initPosition.X, initPosition.Y));
                    break;
                default:
#if DEBUG
                    Console.WriteLine("got nuthin, stupid");
#endif
                    break;
            }
            return mapObj;
        }
    }
}
