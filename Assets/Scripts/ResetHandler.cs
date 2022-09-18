using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetHandler : MonoBehaviour
{
    public delegate void ResetEventHandler(object sender);
    public event ResetEventHandler ResetEvent;

    public void Reset()
    {
        ResetEvent.Invoke(this);
    }
}
