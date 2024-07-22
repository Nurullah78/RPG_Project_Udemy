using UnityEngine;

namespace RPG.Core
{
    public class PersistantObjectsSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistantGameObjectPrefab;

        static bool hasSpawned = false;

        private void Awake()
        {
            if (hasSpawned) return;

            SpawnPersistantObject();

            hasSpawned = true;
        }

        void SpawnPersistantObject()
        {
            GameObject persistantObject = Instantiate(persistantGameObjectPrefab);
            DontDestroyOnLoad(persistantObject);
        }
    }

}