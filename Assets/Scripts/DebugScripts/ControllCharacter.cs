using UnityEngine;
using System.Collections;

public class ControllCharacter : MonoBehaviour
{
    [SerializeField]
    private float Char_Walk_Speed, Char_Run_Speed, Char_Rotate_Speed;    
    private bool Moving, Running;
    [SerializeField]
    private bool ApplyMotion = true;

    [SerializeField]
    [Range(0.01f, 1)]
    private float Character_Backwards_Movment_Mult = .8f;

    private bool Char_Dead = false;

    private Rigidbody CharRigid;
    [SerializeField]
    private Animator animator;

    private void Start()
    {
        CharRigid = GetComponent<Rigidbody>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        MoveCharacter();
        RotateCharacter();
        Kill();
        ApplyAnimations();
    }

    private void MoveCharacter()
    {
        Running = Input.GetKey(KeyCode.RightShift);

        if (!ApplyMotion)
        {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
            {
                Moving = true;
            }
            else
                Moving = false;

            return;
        }

        Vector3 Character_Translate_Delta;

        if (Running)        
            Character_Translate_Delta = CharRigid.position + transform.forward * Char_Run_Speed * Time.deltaTime;        
        else
            Character_Translate_Delta = CharRigid.position + transform.forward * Char_Walk_Speed * Time.deltaTime;        

        Moving = true;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            CharRigid.position = Character_Translate_Delta;
            return;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            CharRigid.position = -Character_Translate_Delta;
            return;
        }

        Moving = false;
    }
    private void RotateCharacter()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up * Char_Rotate_Speed);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up * -Char_Rotate_Speed);
        }
    }
    private void Kill()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Char_Dead = !Char_Dead;
        }
    }
    private void ApplyAnimations()
    {
        if (Moving)
        {
            animator.SetFloat("Speed", 5);
            if (Running)
            {
                animator.SetFloat("Speed", 15);
            }
        }
        else
            animator.SetFloat("Speed", 0);

        animator.SetBool("Dead", Char_Dead);

    }
}