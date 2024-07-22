using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]

    public class Weapon : ScriptableObject
    {
        [SerializeField] AnimatorOverrideController animationOverride = null;
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] Bullet bullet = null;
        [SerializeField] float weaponRange = 2f, weaponDamage = 10f;
        [SerializeField] bool isRightHand = true;

        const string weaponName = "Weapon";

        public bool HasBullet()
            { return bullet != null; }

        public float GetDamage()
            { return weaponDamage; }

        public float GetRange()
            { return weaponRange; }
        
        void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
                oldWeapon = leftHand.Find(weaponName);
            
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYED";
            Destroy(oldWeapon.gameObject);
        }

        public void Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (weaponPrefab != null)
            {
                Transform handTransform = WhichHand(rightHand, leftHand);
                GameObject weapon = Instantiate(weaponPrefab, handTransform);
                weapon.name = weaponName;
            }

            if (animationOverride != null)
                animator.runtimeAnimatorController = animationOverride;
        }
        
        public void LaunchBullet(Transform rightHand, Transform leftHand, Health target)
        {
            Transform handTransform = WhichHand(rightHand, leftHand);
            Bullet bulletInstance = Instantiate(bullet, handTransform.position, Quaternion.identity);
            bulletInstance.SetTarget(target, weaponDamage);
        }

        Transform WhichHand(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;

            if (isRightHand)
                handTransform = rightHand;
            else
                handTransform = leftHand;

            return handTransform;
        }

    }
}