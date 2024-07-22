using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;

namespace RPG.Controller
{
    public class PlayerController : MonoBehaviour, IAction
    {

//---------------Nesneler---------------

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        [SerializeField] private
            float maximumSpeed, rotationSpeed;

        [SerializeField] private
            Transform cameraTransform;

        [SerializeField] CinemachineFreeLook freeLook;

        [SerializeField]
            CursorMapping[] cursorMappings = null;

        enum CursorType
        {
            None,
            Movement,
            Combat,
            Rotate
        }
        
        public Text healthStatus;
        CharacterController _characterController;
        NavMeshAgent navMeshAgent;
        GameObject introSequence;
        Health _health;
        Mover mover;

        float speedFraction;
        bool isEntered;

//---------------Metotlar---------------

        void Start()
        {
            freeLook = GameObject.FindGameObjectWithTag("FollowCamera").GetComponent<CinemachineFreeLook>();
            mover = GetComponent<Mover>();
            introSequence = GameObject.FindWithTag("Intro");
            navMeshAgent = GetComponent<NavMeshAgent>();
            _characterController = GetComponent<CharacterController>();
            _health = GetComponent<Health>();
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == introSequence && isEntered == false)
            {
                navMeshAgent.isStopped = true;
                isEntered = true;
            }
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        public bool InteractWithMovement()
        {
            RaycastHit hit;

            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (hasHit)
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    speedFraction = 0.33333f;
                }
                else
                {
                    speedFraction = 1;
                }

                return true;
            }

            Cancel();
            return false;
        }

        public void Cancel()
        {
            GetComponent<Fighter>().Cancel(); //Saldırı animasyonunu durdurur
        }

        private bool KeyboardWithMovement()
        {
            float horizontalInput = Input.GetAxis("Horizontal"); //Yatay düzlemde girdi alıntısını değişkene aktarır
            float verticalInput = Input.GetAxis("Vertical"); //Dikey düzlemde girdi alıntısını değişkene aktarır

            if (horizontalInput != 0 || verticalInput != 0)
            {
                if (Input.GetMouseButton(1))
                {
                    SetCursor(CursorType.Rotate);
                    freeLook.m_XAxis.Value += Input.GetAxis("Mouse X") * 10f;
                }

                navMeshAgent.isStopped = true; //NavMeshAgent bileşenini sonlandırır
                GetComponent<ActionScheduler>().StartAction(this);
                
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    horizontalInput /= 3;
                    verticalInput /= 3;
                }

                Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput); //Girdilerin 3 boyutlu düzleme aktarılması
                
                movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
                //Karakterin yönünü kameranın baktığı yöne çevirir

                mover.UpdateAnimator(movementDirection, maximumSpeed);

                Vector3 velocity = movementDirection * maximumSpeed; //Hareket doğrultusunu maximum hızla çarparak hareketi oluşturur

                _characterController.Move(velocity * Time.deltaTime); //Alınan değerleri karaktere aktararak karakterin hareket etmesini sağlar

                if (movementDirection != Vector3.zero) // movementDirection != (0,0,0)
                {
                    Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

                    transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                    //Alınan değerlere göre karakterin yönünü değiştirir
                }

                Cancel();
                return true;
            }

            SetCursor(CursorType.Movement);
            return false;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();

                if (target == null)
                {
                    continue;
                }

                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }

                if (target == null)
                {
                    continue;
                }

                if (Input.GetMouseButtonDown(1))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                SetCursor(CursorType.Combat);
                return true;
            }
            return false;
        }

        private void Update()
        { 
            healthStatus.text = "Health: " + _health.health;
            if (_health.IsDead()) return;
            if (InteractWithCombat()) return;
            if (KeyboardWithMovement()) return;
            if (InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }
    }
}