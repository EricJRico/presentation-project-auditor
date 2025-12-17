using UnityEngine;

namespace ProjectAuditorDemos._01_ExpensiveCode
{
    public class FindEnemyWithTagFixedManager : MonoBehaviour
    {
        void Update()
        {
            var enemies = EnemyManager.GetEnemies();
            if (enemies.Count > 0)
            {
                enemies[0].transform.Rotate(Vector3.up);
            }
        }
    }
}