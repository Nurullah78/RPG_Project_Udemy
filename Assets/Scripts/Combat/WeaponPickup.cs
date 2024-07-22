using System;
using System.Collections;
using RPG.Core;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] float respawnTime = 5f, healthToRestore = 0;
        [SerializeField] Weapon weapon;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                if (weapon != null)
                {
                    other.GetComponent<Fighter>().SpawnWeapon(weapon);
                }

                if (healthToRestore > 0)
                {
                    other.GetComponent<Health>().Heal(healthToRestore);
                }

                StartCoroutine(HideForSeconds(respawnTime));
            }
        }

        void HideWeapon()
        {
            GetComponent<Collider>().enabled = false;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
        }
       
        void ShowWeapon()
        {
            GetComponent<Collider>().enabled = true;
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
 
        IEnumerator HideForSeconds(float seconds)
        {
            HideWeapon();
            yield return new WaitForSeconds(seconds);
            ShowWeapon();
        }
    }
}
