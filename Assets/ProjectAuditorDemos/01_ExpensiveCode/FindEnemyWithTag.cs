using UnityEngine;

namespace ProjectAuditorDemos._01_ExpensiveCode
{
    public class FindEnemyWithTag : MonoBehaviour
    {
        void Update()
        {
            // BAD: Scene-wide search every frame
            var enemies = GameObject.FindGameObjectsWithTag("Enemy");

            if (enemies.Length > 0)
            {
                enemies[0].transform.Rotate(Vector3.up);
            }
            
            // Other examples
            // GameObject.Find("Main Camera")
            // var enemy = FindAnyObjectByType<Enemy>()
            // var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            // var enemies = FindObjectsOfType<Enemy>();
        } 
    }
}