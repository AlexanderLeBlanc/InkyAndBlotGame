using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallController : MonoBehaviour
{
    [SerializeField] private float reducedHeight;
    [SerializeField] private float fullHeight;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent.gameObject.name.ToLower().Contains("blot") && collision.GetComponentInParent<InkShotController>().GetIsPlatform())
        {
            transform.localPosition = new Vector2(transform.position.x, reducedHeight);
            collision.transform.parent.GetComponent<InkShotController>().StopWaterfall(gameObject);
            //Debug.Log("Waterfall h: " + transform.position);
        }
    }

    public void ExitPlatform() {
        transform.localPosition = new Vector2(transform.position.x, fullHeight);
    }
}
