using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] float speed = 1f;

        Health target = null;
        float damage = 0;

        public void SetTarget(Health target, float damage)
        {
            this.target = target;
            this.damage = damage;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();

            if (targetCollider == null)
            {
                return target.transform.position;
            }

            return target.transform.position + Vector3.up * targetCollider.height / 1.4f;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;

            target.TakeDamage(damage);
            Destroy(gameObject);
        }

        void Update()
        {
            if (target == null) return;

            transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
    }
}
