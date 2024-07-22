using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Controller
{
    public class AIController : MonoBehaviour
    {
        [Range(0,1)] [SerializeField] float patrolSpeedFraction = 0.2f;
        
        [SerializeField] float
            chaseDistance = 5f, suspicionTime = 5f,
            waypointTolerance = 1f, waypointLifeTime = 3f,
            aggroCooldownTime = 5f, shoutDistance = 5f;
        
        [SerializeField] PatrolPath patrolPath;

        Vector3 enemyLocation, nextPosition;
        GameObject player;
        Fighter fighter;
        Health health;
        Mover mover;

        float timeSinceLastSawPlayer, timeSinceArrivedWaypoint,
            maxSpeed, timeSinceAggrevate = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Start()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            enemyLocation = transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

        private float DistanceToPlayer()
        {
            return Vector3.Distance(player.transform.position, transform.position);
        }

        private bool AtWaypoint()
        {
            float distanceWaypoint = Vector3.Distance(transform.position, GetNextWaypoint());
            return distanceWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetNextWaypoint()
        {
            return patrolPath.GetWaypointPosition(currentWaypointIndex);
        }

        public void Aggrevate()
        {
            timeSinceAggrevate = 0;
        }

        private bool IsAggrevated()
        {
            return DistanceToPlayer() < chaseDistance || timeSinceAggrevate < aggroCooldownTime;
        }
        
        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0);
            foreach (RaycastHit hit in hits)
            {
                AIController ai = hit.collider.GetComponent<AIController>();
                if (ai == null) continue;

                ai.Aggrevate();
            }
        }

        private void Update()
        {
            if (health.IsDead())
            {
                return;
            }
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                maxSpeed = 5.7f;
                timeSinceLastSawPlayer = 0;
                fighter.Attack(player);

                AggrevateNearbyEnemies();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                maxSpeed = 2.5f;
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            else
            {
                nextPosition = enemyLocation;

                if (patrolPath != null)
                {
                    if (AtWaypoint())
                    {
                        timeSinceArrivedWaypoint = 0;
                        CycleWaypoint();
                    }
                    nextPosition = GetNextWaypoint();
                }

            }

            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedWaypoint += Time.deltaTime;
            timeSinceAggrevate += Time.deltaTime;
        }
    }
}
