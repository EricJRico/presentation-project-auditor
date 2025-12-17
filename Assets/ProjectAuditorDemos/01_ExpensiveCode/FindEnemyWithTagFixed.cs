using UnityEngine;

namespace ProjectAuditorDemos._01_ExpensiveCode
{
    public class FindEnemyWithTagFixed : MonoBehaviour
    {
        private GameObject[] _enemies;

        void Start()
        {
            _enemies = GameObject.FindGameObjectsWithTag("Enemy");
        }

        void Update()
        {
            if (_enemies.Length > 0)
                _enemies[0].transform.Rotate(Vector3.up);
        }
    }
}