using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlSplash : MonoBehaviour
{
    public ResetHandler resetHandler;
    // Start is called before the first frame update
    void Start()
    {
        resetHandler.ResetEvent += OnReset;
        StartCoroutine(TurnOff());
    }

    public void OnReset(object sender)
    {
        gameObject.SetActive(true);
        StartCoroutine(TurnOff());
    }

    IEnumerator TurnOff()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
