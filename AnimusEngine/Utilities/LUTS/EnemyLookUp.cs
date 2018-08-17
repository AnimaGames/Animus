using Microsoft.Xna.Framework;
using System;


namespace AnimusEngine
{
    public static class EnemyLookUp
    {
        static private GameObject enemyObj;

        public static GameObject EnemyLUT(string inputName, Vector2 initPosition)
        {
            switch (inputName)
            {
                case "Enemy":
                    enemyObj = new Enemy(initPosition);
                    break;
                case "PatrolEnemy":
                    enemyObj = new PatrolEnemy(initPosition);
                    break;

                default:
#if DEBUG
                    Console.WriteLine("got nuthin, stupid");
#endif
                    break;
            }
            return enemyObj;
        }
    }
}
