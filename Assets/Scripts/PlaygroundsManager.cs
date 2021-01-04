using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaygroundsManager : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player.position.y < -30)
        {
            player.position = new Vector3(0, 2, 0.5f);
        }
    }
}
