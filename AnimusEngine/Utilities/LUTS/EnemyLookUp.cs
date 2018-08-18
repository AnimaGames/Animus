using Microsoft.Xna.Framework;
using System;


namespace AnimusEngine
{
    public static class EnemyLookUp
    {
        static private GameObject enemyObj;

        public static GameObject EnemyLUT(string inputName, Vector2 initPosition, int id)
        {
            switch (inputName)
            {
                case "Enemy":
                    enemyObj = new Enemy(initPosition, id);
                    break;
                case "PatrolEnemy":
                    enemyObj = new PatrolEnemy(initPosition, id);
                    break;
                case "Boss":
                    enemyObj = new BossParent(initPosition, id);
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
