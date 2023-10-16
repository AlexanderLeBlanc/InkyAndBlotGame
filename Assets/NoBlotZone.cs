using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBlotZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            collision.GetComponent<CharacterController>().EnterNoBlotZone();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 11)
        {
            collision.GetComponent<CharacterController>().LeaveNoBlotZone();
        }
    }
}
