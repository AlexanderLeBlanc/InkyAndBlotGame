using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11) {
            collision.GetComponent<CharacterController>().SetSpawn(transform);
            this.enabled = false;
        }
    }
}
