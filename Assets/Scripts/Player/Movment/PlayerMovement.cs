using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{

    //public GameObject groundChecker;

    public float jumpHeight = 0;
    public float originalSpeed = 0, speed = 0, strafeSpeed = 0, sneakSpeed = 0;

    private bool _isMoving;
    public bool IsMoving
    {
        get { return _isMoving; }
    }

    
    private static bool _LockCharacterMovment;
    public static bool LockCharacterMovment
    {
        set { _LockCharacterMovment = value; }
        get { return _LockCharacterMovment; }
    }

    [SerializeField]
    private bool RemoveMe;

    Rigidbody playerRigid;
    PlayerSneek ps;
    GroundColision groundCollision;

    float x, y;


    void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        ps = GetComponent<PlayerSneek>();
        groundCollision = GetComponent<GroundColision>();
    }

    void Update()
    {
        RemoveMe = _LockCharacterMovment;
        if (_LockCharacterMovment)
        {
            _isMoving = false;
            return;
        }

        BasicMovement();
        Jump();
    }

    void BasicMovement()
    {
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
            speed = strafeSpeed;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) ||
                 Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            speed = originalSpeed;
        if (ps.Sneaking)
            speed = sneakSpeed;


        if (groundCollision.OnGround)
        {
            x = Input.GetAxis("Horizontal");
            y = Input.GetAxis("Vertical");
            if (x != 0 || y != 0)
            {
                _isMoving = true;
            }
            else
                _isMoving = false;
        }
        else
            _isMoving = false;


        playerRigid.MovePosition(playerRigid.position + transform.right * speed * Time.deltaTime * x);
        playerRigid.MovePosition(playerRigid.position + transform.forward * speed * Time.deltaTime * y);

    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) & groundCollision.OnGround)
            playerRigid.AddForce(new Vector3(0, jumpHeight, 0));
    }
}
