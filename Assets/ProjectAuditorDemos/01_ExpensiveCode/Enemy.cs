using UnityEngine;

namespace ProjectAuditorDemos._01_ExpensiveCode
{
    public class Enemy : MonoBehaviour
    {
        void Awake()
        {
            // Register self with manager when spawned
            EnemyManager.RegisterEnemy(this);
        }

        void OnDestroy()
        {
            // Deregister when destroyed
            EnemyManager.UnregisterEnemy(this);
        }
    }
}