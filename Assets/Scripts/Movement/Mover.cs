using RPG.Combat;
using RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{

    public class Mover : MonoBehaviour, IAction
    {

//---------------Nesneler---------------

        [SerializeField] float maxSpeed = 5.7f;

        NavMeshAgent navMeshAgent;

//---------------Metotlar---------------

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 hit, float speedFraction)
        {
            navMeshAgent.speed = maxSpeed * speedFraction;
            navMeshAgent.isStopped = false;
            navMeshAgent.destination = hit;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        public void BackToMove()
        {
            navMeshAgent.isStopped = false;
        }

        public void StartMoveAction(Vector3 hit, float speedFraction)
        {
            MoveTo(hit, speedFraction);
            BackToMove();
            GetComponent<Fighter>().Cancel();
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void SetAnimator(float forwardSpeed, float dampTime)
        {
            GetComponent<Animator>().SetFloat("forwardSpeed", forwardSpeed, dampTime, Time.deltaTime);
        }

        public void UpdateAnimator(Vector3 movementDirection, float maxSpeed)
        {
            float forwardSpeed = Mathf.Clamp01(movementDirection.magnitude); //Düzleme aktarılan değerlerin uzunluğunu(magnitude), 0 ile 1 arasında değer döndürerek(Clamp01), değişkene aktarır
            
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                forwardSpeed /= 2.25f; //Sol ve sağ shift tuşlarının aktifleşmesiyle hızı yavaşlatır
            }

            float speed = forwardSpeed * maxSpeed;
            
            movementDirection.Normalize(); //Sabit bir yönde 'magnitude' değerini 1 olarak sabitler
            
            SetAnimator(speed, 0.05f); //Animator bileşeninin içerisindeki parametrenin değerini günceller
        }
    }
}
