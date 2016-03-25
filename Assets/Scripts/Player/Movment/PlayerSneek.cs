using UnityEngine;
using System.Collections;

public class PlayerSneek : MonoBehaviour
{

    private bool sneaking;
    public bool Sneaking
    {
        get { return sneaking; }
    }

    [SerializeField]
    private KeyCode SneakKey = KeyCode.LeftAlt;

    private void Update()
    {
        sneaking = Input.GetKey(SneakKey);
    }
}
