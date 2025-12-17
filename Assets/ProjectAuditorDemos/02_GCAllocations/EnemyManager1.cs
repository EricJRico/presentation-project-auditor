using System.Linq;
using UnityEngine;

namespace ProjectAuditorDemos._02_GCAllocations
{
    public class EnemyManager1 : MonoBehaviour
    {
        public Enemy1[] enemies;
        public Transform player; // centralized player reference

        void Update()
        {
            if (player == null) return;

            // LINQ: Filter active enemies (allocation occurs naturally when enemies die)
            var activeEnemies = 
                enemies.Where(e => e.gameObject.activeSelf).ToArray();

            foreach (var enemy in activeEnemies)
            {
                enemy.RotateTowards(player.position);

                Vector3 start = enemy.transform.position + Vector3.up;
                Vector3 end = enemy.transform.position + Vector3.up * 2;
                float radius = 0.5f;
                float distance = 1f;

                // BAD: allocates array per frame
                var raycastHits = Physics.CapsuleCastAll(
                    start, end, 
                    radius, enemy.transform.forward, distance);
                for (int h = 0; h < raycastHits.Length; h++)
                {
                    if (raycastHits[h].collider.transform == player)
                    {
                        // Player detected by enemy
                        Debug.Log($"Enemy {enemy.name} hit the player! Enemy health {enemy.health}");
                        // Optional: apply damage to player here
                    }
                }

                // String allocation: log dynamic enemy health
                Debug.Log("Enemy health: " + enemy.health);
            }
        }
    }
}