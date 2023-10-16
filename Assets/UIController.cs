using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController instance { get; private set; }

    [SerializeField] private List<GameObject> HPDisplay;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else { 
            instance = this;
        }
    }

    public void ResetHealth() {
        foreach (GameObject go in HPDisplay) { 
            go.SetActive(true);
        }
    }

    public void LoseHealth() {
        for (int i = HPDisplay.Count - 1; i >= 0; i--) {
            if (HPDisplay[i].activeSelf) {
                HPDisplay[i].SetActive(false);
                break;
            }
        }
    }
}
