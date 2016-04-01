using UnityEngine;
using System.Collections;

public class FallRestart : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            RestartGame.Restart();
        }
    }
}
