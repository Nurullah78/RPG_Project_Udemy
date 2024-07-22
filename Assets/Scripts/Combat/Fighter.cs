using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction
    {

//---------------Nesneler---------------

        [SerializeField] Transform rightHandTransform = null, leftHandTransform = null;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Weapon defaultWeapon = null;

        Health targetObject;
        float timeSinceLastAttack;

//---------------Metotlar---------------

        void Start()
        {
            SpawnWeapon(defaultWeapon);
        }

        public void SpawnWeapon(Weapon weapon)
        {
            defaultWeapon = weapon;

            Animator animator = GetComponent<Animator>();
            defaultWeapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public void Attack(GameObject target)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            targetObject = target.GetComponent<Health>();
        }
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, targetObject.transform.position) < defaultWeapon.GetRange();
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        void AttackMethod()
        {
            transform.LookAt(targetObject.transform.position);
            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null)
            {
                return false;
            }
            Health healthToTest = GetComponent<Health>();
            return healthToTest != null && !healthToTest.IsDead();
        }

        void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public void Cancel()
        {
            StopAttack();
            targetObject = null;
        }

        public void Hit()
        {
            if (targetObject == null)
            {
                return;
            }
            
            if (defaultWeapon.HasBullet())
            {
                defaultWeapon.LaunchBullet(rightHandTransform, leftHandTransform, targetObject);
            }
            else
            {
                targetObject.TakeDamage(defaultWeapon.GetDamage());
            }
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (targetObject == null)
            {
                return;
            }

            if(targetObject.IsDead() == true)
            {
                GetComponent<Animator>().ResetTrigger("attack");
                Cancel();
                return;
            }

        }
    }

}