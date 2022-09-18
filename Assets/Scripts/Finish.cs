using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject winText;
    public BallMovement player;
    public ResetHandler resetHandler;

    public void OnTriggerEnter(Collider other)
    {
        winText.SetActive(true);
        StartCoroutine(Reset());
    }

    private IEnumerator Reset()
    {
        yield return new WaitForSeconds(5);
        winText.SetActive(false);
        resetHandler.Reset();
    }
}
