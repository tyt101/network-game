using UnityEngine;
using System.Collections;

public class FollowPlayer2 : MonoBehaviour
{
    public Vector3 offset;          // The offset at which the Health Bar follows the player.

    private Transform player;       // Reference to the player.


    void Awake()
    {
        // Setting up the reference.
        player = GameObject.Find("hero2").transform;
    }

    void Update()
    {
        // Set the position to the player's position with the offset.
        if (player == null)
        {
            GameObject temp = GameObject.Find("hero2");
            if (temp != null)
            {
                player = temp.transform;
            }
        }
        else
        {
            transform.position = player.position + offset;
        }
    }
}
