using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    public void ResetAnimation()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>().SetTrigger("Idle");
    }
}
