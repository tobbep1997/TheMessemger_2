using UnityEngine;
using System.Collections;
using System;

public class PlayerAnimController : MonoBehaviour
{

    private PlayerMovement playerMovment;
    private ParkourMovement parkourMovment;
    private PlayerSneek sneakMovment;
    private Rigidbody playerRigid;

    private Animator animator;

    private void Start()
    {
        playerMovment = GetComponent<PlayerMovement>();
        parkourMovment = GetComponent<ParkourMovement>();
        sneakMovment = GetComponent<PlayerSneek>();
        animator = GetComponent<Animator>();
        playerRigid = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        animator.SetFloat("Running", Convert.ToInt32(playerMovment.IsMoving));
        
    }

}
