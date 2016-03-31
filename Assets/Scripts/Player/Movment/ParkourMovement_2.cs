using UnityEngine;
using System.Collections;
using System.Threading;

public class ParkourMovement_2 : MonoBehaviour
{

    private Rigidbody playerRigi;
    private GroundColision groundCollision;
    [SerializeField]
    private CameraMovment_3 camMove;
    [SerializeField]
    private PlayerMovement playerMovment;
    [SerializeField]
    private PlayerColManager playerColManager;


    private enum PlayerState { CheckForObject, Climb, Hang, ClimbOverver, MoveForward, PlacePlayer, Stop, Reset }
    [SerializeField]
    private PlayerState playerState = PlayerState.Reset;

    //private enum ColliderState { Standing, Crouching, Sliding }
    private PlayerColManager.ColliderState colliderState;

    [SerializeField]
    private Transform playerGround, playerMiddle, playerHead, playerOverHead;
    private Transform[] transArray = new Transform[4];
    [SerializeField]
    private float checkDistance = 1.5f;
    private bool CanParkoure = false;
    [SerializeField]
    private Collider standingColider, crouchColider, slidingColider;
    [SerializeField]
    private float maxClimbTime = 1, Distance = 1;
    private Vector3 refVector, OriginalPosition;
    [SerializeField]
    private float MoveSmoothTime = .3f;

    private float climbTimer;
    private bool PlayerPressingClimb;
    private Collider currentClimbingObject;


    [SerializeField]
    private GameObject wallJumpChecker;

    private bool wallJumpRayTouchObject = false;
    private Ray wallJumpRay = new Ray();

    [SerializeField]
    private float wallJumpForce = 10;

    [SerializeField]
    private float slideTime = .2f, slideCD = .1f, slideForce, afterSlideCD = .2f;
    private float slideTimer = 0, slideCDTimer, afterSlideTimer;
    private bool bSliding = false, bCanSlide = true, addedForce = false, bAfterSlide = false;

    private bool _LockParcore = false;
    public bool LockParcore
    {
        get { return _LockParcore; }
        set { _LockParcore = value; }
    }



    private void Awake()
    {
        playerRigi = GetComponent<Rigidbody>();
        groundCollision = GetComponent<GroundColision>();     
        
    }
    private void Start()
    {
        transArray = new Transform[3] { playerGround, playerMiddle, playerHead};
        playerMovment = GetComponent<PlayerMovement>();
        playerColManager = GetComponent<PlayerColManager>();
    }

    private void FixedUpdate()
    {
        if (_LockParcore)
            return;
        CallFunctions();
    }

    private void CallFunctions()
    {
        GetPlayerInput();
        SwitchState();
        Sliding();
    }

    private void GetPlayerInput()
    {
        PlayerPressingClimb = Input.GetKey(KeyCode.LeftShift);
        if (PlayerPressingClimb && playerState == PlayerState.Reset && CanParkoure)
        {
            CanParkoure = false;
            playerState = PlayerState.CheckForObject;
        }        
    }
    private void SwitchState()
    {
        switch (playerState)
        {
            case PlayerState.CheckForObject:
                if (CheckObjects())
                    playerState = PlayerState.Climb;
                else
                    playerState = PlayerState.Reset;                          
                break;
            case PlayerState.Climb:
                Climb();
                break;
            case PlayerState.Hang:
                Hang();
                break;
            case PlayerState.ClimbOverver:
                ClimbOver();
                break;
            case PlayerState.MoveForward:
                MoveForward();
                break;
            case PlayerState.PlacePlayer:
                break;
            case PlayerState.Stop:
                break;
            case PlayerState.Reset:
                Reset();
                break;
            default:
                break;
        }
    }
    private bool CheckObjects()
    {
        Ray ray;
        RaycastHit[] hit;
        
        for (int i = 0; i < transArray.Length; i++)
        {            
            ray = new Ray(transArray[i].position, transform.forward);
            ///Debug
            Debug.DrawRay(ray.origin, ray.direction, Color.green);
            ///
            hit = Physics.RaycastAll(ray, checkDistance, 1 << LayerMask.NameToLayer("Parkourable"));
            if (hit.Length > 0)
            {
                for (int x = 0; x < hit.Length; x++)
                {
                    currentClimbingObject = hit[x].collider;
                    if (currentClimbingObject != null)                    
                        return true;                    
                }
                return true;
            }
        }
        currentClimbingObject = null;
        return false;
    }
    private bool CheckIfOverObject(Transform trans)
    {
        if (trans.position.y >= currentClimbingObject.bounds.max.y)        
            return true;        
        else        
            return false;        
    }
    private bool CheckIfHangPossible()
    {
        if (playerOverHead.transform.position.y >= currentClimbingObject.bounds.max.y)        
            return true;        
        else
            return false;
    }
    private void Climb()
    {
        camMove.LockCharacter = true;
        if (PlayerPressingClimb)
        {
            playerRigi.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            playerRigi.constraints = RigidbodyConstraints.FreezeRotation;
            playerRigi.isKinematic = true;
            PlayerMovement.LockCharacterMovment = true; 
            playerRigi.position += new Vector3(0, 0.2f, 0);
        }
        else
        {
            playerRigi.constraints = RigidbodyConstraints.FreezeRotation;
            if (CheckIfHangPossible())
            {
                playerState = PlayerState.Hang;
            }
            else
                playerState = PlayerState.Reset;
        }

        climbTimer += Time.deltaTime;
        if (climbTimer > maxClimbTime)
        {
            if (CheckIfOverObject(playerHead))
            {
                playerState = PlayerState.ClimbOverver;
            }
            else
                playerState = PlayerState.Reset;
        }
        else
        {
            if (CheckIfHangPossible())
            {
                playerState = PlayerState.Hang;
            }            
        }
    }
    private void Hang()
    {
        camMove.LockCharacter = true;
        if (PlayerPressingClimb)
        {
            playerState = PlayerState.ClimbOverver;
        }
    }
    private void ClimbOver()
    {
        playerRigi.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        playerRigi.constraints = RigidbodyConstraints.FreezeRotation;
        playerRigi.isKinematic = true;
        PlayerMovement.LockCharacterMovment = true;
        playerRigi.position += new Vector3(0, 0.2f, 0);

        if (CheckIfOverObject(playerGround))
        {
            playerState = PlayerState.MoveForward;
            OriginalPosition = playerRigi.position;
            refVector = Vector3.zero;
        }        
    }
    private void MoveForward()
    {
        playerRigi.position = Vector3.SmoothDamp(playerRigi.position, transform.position + transform.forward * Distance, ref refVector, MoveSmoothTime);

        if (Vector3.Distance(OriginalPosition,playerRigi.position) >= Distance)
        {
            playerState = PlayerState.Reset;
        }
    }

    private void Reset() 
    {
        
        playerRigi.isKinematic = false;
        playerRigi.constraints = RigidbodyConstraints.None;
        playerRigi.constraints = RigidbodyConstraints.FreezeRotation;

        if (!groundCollision.OnGround)        
            return;
        if (bSliding || !bCanSlide)
            return;

        PlayerMovement.LockCharacterMovment = false;
        CanParkoure = true;
        climbTimer = 0;
    }

    void WallJump()
    {        
        wallJumpRay.origin = playerMiddle.position;
        wallJumpRay.direction = (playerMiddle.position - wallJumpChecker.transform.position) * -1;

        RaycastHit wallJumpRayHit;
        if (Physics.Raycast(wallJumpRay, out wallJumpRayHit, Vector3.Distance(wallJumpRay.origin, wallJumpChecker.transform.position), 1 << LayerMask.NameToLayer("Parkourable")))
        {
            if (wallJumpRayHit.transform.tag == "ParkourObject")
            {
                wallJumpRayTouchObject = true;
            }
        }
        else
            wallJumpRayTouchObject = false;

        //walljumps
        if (Input.GetKey(KeyCode.A) & !Input.GetKey(KeyCode.D))
        {
            wallJumpChecker.transform.localPosition = new Vector3(-8, 0, 0);
        }
        if (Input.GetKey(KeyCode.D) & !Input.GetKey(KeyCode.A))
        {
            wallJumpChecker.transform.localPosition = new Vector3(8, 0, 0);
        }

        if (wallJumpRayTouchObject && !groundCollision.OnGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (wallJumpChecker.transform.localPosition == new Vector3(-8, 0, 0))
                {
                    //apply force right-forward
                    playerRigi.AddForce(transform.up * (wallJumpForce + 10));
                    playerRigi.AddForce((transform.forward + transform.right) * wallJumpForce);
                }
                if (wallJumpChecker.transform.localPosition == new Vector3(8, 0, 0))
                {
                    //apply force left-forward
                    playerRigi.AddForce(transform.up * (wallJumpForce + 10));
                    playerRigi.AddForce((transform.forward + -transform.right) * wallJumpForce);
                }
            }
        }   
    }
    void Sliding()
    {
        if (bCanSlide && !bSliding && Input.GetKey(KeyCode.LeftControl) && groundCollision.OnGround)
        {
            bCanSlide = false;
            bSliding = true;
        }
        if (bSliding)
        {
            colliderState = PlayerColManager.ColliderState.Sliding;
            PlayerMovement.LockCharacterMovment = true;

            playerColManager.SetColliderState(PlayerColManager.ColliderState.Sliding);

            if (!addedForce)
            {
                addedForce = true;
                playerRigi.AddForce(transform.forward * slideForce);                
            }

            slideTimer += Time.deltaTime;

            if (slideTimer >= slideTime)
            {
                
                bSliding = false;
            }
        }
        if (!bSliding && !bCanSlide)
        {
            slideCDTimer += Time.deltaTime;
            if (slideCDTimer >= slideCD)
            {                
                slideCDTimer = 0;
                slideTimer = 0;
                addedForce = false;
                bAfterSlide = true;
                PlayerMovement.LockCharacterMovment = false;
                colliderState = PlayerColManager.ColliderState.Standing;
                playerColManager.SetColliderState(PlayerColManager.ColliderState.Standing);

            }
        }
        if (bAfterSlide)
        {
            afterSlideTimer += Time.deltaTime;
            if (afterSlideTimer >= afterSlideCD)
            {
                afterSlideTimer = 0;
                bCanSlide = true;
                bAfterSlide = false;
            }

        }
    }
}

