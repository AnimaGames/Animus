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
    public static class NPCLookUp
    {
        static private GameObject npcObj;

        public static GameObject NpcLUT(string npcName, Vector2 initPosition)
        {
            switch (npcName)
            {
                case "tinyCaro":
                    npcObj = new NPC(initPosition, npcName);
                    break;
                
                default:
                    Console.WriteLine("got nuthin, stupid");
                    break;
            }
            return npcObj;
        }
    }
}
