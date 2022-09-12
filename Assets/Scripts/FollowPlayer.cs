using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    Player player;
    public Vector3 forward;
    // Start is called before the first frame update
    void Start()
    {
        forward = player.forward;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        forward = Vector3.Lerp(forward, player.forward, 0.1f);
        transform.rotation = Quaternion.LookRotation(player.forward);
    }
}
