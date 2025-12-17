using UnityEngine;

namespace ProjectAuditorDemos._03_DomainReload
{
    public class EnemyRegistryFixed : MonoBehaviour
    {
        public static int EnemyCount { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            EnemyCount = 0;
        }

        void OnEnable()
        {
            EnemyCount++;
            Debug.Log($"Enemies alive: {EnemyCount}");
        }
    }
}