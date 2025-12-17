using UnityEngine;

namespace ProjectAuditorDemos._02_GCAllocations
{
    public class Enemy1 : MonoBehaviour
    {
        public int health = 3;
        public float rotationSpeed = 90f;

        public void TakeDamage(int amount)
        {
            health -= amount;
            if (health <= 0)
            {
                gameObject.SetActive(false); // Enemy dies
            }
        }

        public void RotateTowards(Vector3 targetPosition)
        {
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0; // horizontal rotation only

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }
}