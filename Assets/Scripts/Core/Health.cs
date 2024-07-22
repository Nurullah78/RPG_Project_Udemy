using UnityEngine;
using UnityEngine.Events;
using RPG.Controller;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] public float health;
        [SerializeField] UnityEvent onDamage, onDie;

        PlayerController playerController;

        bool isDead = false;
        float maxHealth;

        //Metotlar

        void Start()
        {
            maxHealth = health;
        }

        public bool IsDead(){ return isDead; }

        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);
            if(health > 0)
            {
                onDamage.Invoke();
            }
            else
            {
                onDie.Invoke();
                Die();
            }
        }

        public void Heal(float healthToRestore)
        {
            health = Mathf.Min(health + healthToRestore, maxHealth);
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
