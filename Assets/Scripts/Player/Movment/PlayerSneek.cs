using UnityEngine;
using System.Collections;

public class PlayerSneek : MonoBehaviour
{

    private bool sneaking;
    public bool Sneaking
    {
        get { return sneaking; }
    }

    private PlayerMovement playerMovment;
    private ParkourMovement_2 parkourMovment;
    private PlayerColManager colManager;

    [SerializeField]
    private float SneakSpeed, sneakStrafeSpeed;

    [SerializeField]
    private KeyCode SneakKey = KeyCode.LeftAlt;

    private void Start()
    {
        playerMovment = GetComponent<PlayerMovement>();
        parkourMovment = GetComponent<ParkourMovement_2>();
        colManager = GetComponent<PlayerColManager>();
    }

    private void Update()
    {
        sneaking = Input.GetKey(SneakKey);

        if (sneaking)
        {
            parkourMovment.LockParcore = true;

            colManager.SetColliderState(PlayerColManager.ColliderState.Sneaking);
        }
        else
        {
            parkourMovment.LockParcore = false;

            colManager.SetColliderState(PlayerColManager.ColliderState.Standing);
        }
    }
}
