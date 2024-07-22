using UnityEngine;
using UnityEngine.Playables;
using RPG.Controller;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        GameObject player;
        Animator animator;

        void Start()
        {
            player = GameObject.FindWithTag("Player");
            GetComponent<PlayableDirector>().played += DisabledControl;
            GetComponent<PlayableDirector>().stopped += EnabledControl;
        }

        void DisabledControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnabledControl(PlayableDirector pd)
        {
            player.GetComponent<PlayerController>().enabled = true;
        }

    }
}