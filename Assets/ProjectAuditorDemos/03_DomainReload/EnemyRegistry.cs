using UnityEngine;

namespace ProjectAuditorDemos._03_DomainReload
{
    public class EnemyRegistry : MonoBehaviour
    {
        public static int EnemyCount { get; private set; }

        void OnEnable()
        {
            EnemyCount++;
            Debug.Log($"Enemies alive: {EnemyCount}");
        }
    }
}