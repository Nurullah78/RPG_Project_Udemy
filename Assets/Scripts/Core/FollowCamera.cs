using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    
    void Update()
    {
        transform.position = player.position;
    }
}
