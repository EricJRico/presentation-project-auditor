using System.Text;
using UnityEngine;

namespace ProjectAuditorDemos._02_GCAllocations
{
    public class EnemyManager1Fixed : MonoBehaviour
    {
        public Enemy1[] enemies;
        public string[] activeEnemyNames;
        public Transform player;

        private RaycastHit[] hitBuffer = new RaycastHit[5];
        private Enemy1[] activeEnemiesBuffer;
        private StringBuilder sb = new StringBuilder(128);

        void Awake()
        {
            // Preallocate buffer for active enemies
            activeEnemiesBuffer = new Enemy1[enemies.Length];
            activeEnemyNames = new string[enemies.Length];

            for (int i = 0; i < enemies.Length; i++)
            {
                activeEnemyNames[i] = enemies[i].name;
            }
        }

        void Update()
        {
            if (player == null) return;

            int activeCount = 0;

            // 1️⃣ Filter active enemies without LINQ allocations
            for (int i = 0; i < enemies.Length; i++)
            {
                if (enemies[i].gameObject.activeSelf)
                {
                    activeEnemiesBuffer[activeCount++] = enemies[i];
                }
            }

            for (int i = 0; i < activeCount; i++)
            {
                Enemy1 enemy1 = activeEnemiesBuffer[i];

                // 2️⃣ Rotate enemy toward the player
                enemy1.RotateTowards(player.position);

                // 3️⃣ Physics: non-allocating cast
                Vector3 start = enemy1.transform.position + Vector3.up;
                Vector3 end = enemy1.transform.position + Vector3.up * 2;
                float radius = 0.5f;
                float distance = 1f;

                int hitCount =
                    Physics.CapsuleCastNonAlloc(start, end, radius, enemy1.transform.forward, hitBuffer, distance);

                for (int h = 0; h < hitCount; h++)
                {
                    if (hitBuffer[h].collider.transform == player)
                    {
                        // Player detected by enemy
                        sb.Clear();
                        sb.Append("Enemy ").Append(activeEnemyNames[i]).Append(" hit the player!")
                            .Append(" Enemy health ").Append(enemy1.health);
                        Debug.Log(sb.ToString());
                        // Optional: apply damage to player here
                    }
                }

                // 4️⃣ StringBuilder: log enemy health without allocation
                sb.Clear();
                sb.Append("Enemy health: ");
                sb.Append(enemy1.health);
                Debug.Log(sb.ToString());
            }
        }
    }
}