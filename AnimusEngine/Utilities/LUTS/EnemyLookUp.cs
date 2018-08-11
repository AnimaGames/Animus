using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Input;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Renderers;
using System;


namespace AnimusEngine
{
    public static class EnemyLookUp
    {
        static private GameObject enemyObj;

        public static GameObject EnemyLUT(string inputName, Vector2 initPosition)
        {
            switch(inputName){
                case "PatrolEnemy":
                    enemyObj = new PatrolEnemy(initPosition);
                    break;
                case "Enemy":
                    enemyObj = new Enemy(initPosition);
                    break;

                default:
                    Console.WriteLine("got nuthin, stupid");
                    break;
            }
            return enemyObj;
        }
    }
}
