using System.Collections.Generic;

namespace ProjectAuditorDemos._01_ExpensiveCode
{
    public static class EnemyManager
    {
        private static readonly List<Enemy> Enemies = new List<Enemy>();

        public static void RegisterEnemy(Enemy enemy)
        {
            if (!Enemies.Contains(enemy))
                Enemies.Add(enemy);
        }

        public static void UnregisterEnemy(Enemy enemy)
        {
            Enemies.Remove(enemy);
        }

        public static List<Enemy> GetEnemies()
        {
            return Enemies;
        }
    }
}