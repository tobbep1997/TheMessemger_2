using UnityEngine;
using System.Collections;

/// <summary>
/// This
/// </summary>

public class ParkourMovement : MonoBehaviour
{

    public bool headRayTouchObject;
    public GameObject headHeight;
    public GameObject groundChecker;
    public GameObject wallJumpChecker;
    public Collider playerColStanding, playerColSliding;

    [SerializeField]
    private Transform rayStartPoint;

    public bool climbMode;
    bool passedObject = false, vaultBoostGiven = false;

    Ray headRay;
    RaycastHit headHit;
    Ray wallJumpRay;
    RaycastHit wallJumpRayHit;

    Rigidbody playerRigid;

    private bool wallJumpRayTouchObject = false;
    public float wallJumpForce = 0;

    private float headHitInfoStore;
    public float vaultSpeedBoost;

    public float timer = 0, timerMax;

    public float slideTimer = 0, slideTimerMax;
    public float slideSpeed, slideSpeedOriginal, decayRate;
    private bool isSliding = false;

    public float slideCDtimer = 0, slideCDMax;
    private bool hasSlided = false;

    public float boostAmount = 0;
    public bool ableSpeedBoost = false;
    public float inAirTimer = 0, inAirMax = 0;
    public float speedBoostTimer = 0, speedBoostMax = 0;

    private PlayerSneek ps;
    private GroundColision groundCollision;

    void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
        slideSpeedOriginal = slideSpeed;
        //playerColSliding.enabled = false;
        ps = GetComponent<PlayerSneek>();
        groundCollision = GetComponent<GroundColision>();
    }

    void Update()
    {
        ObjectIdentifier();
        Modes();
        AdvancedMovement();
    }

    void ObjectIdentifier()
    {
        headRay.origin = rayStartPoint.position;
        headRay.direction = (rayStartPoint.position - headHeight.transform.position) * -1;

        wallJumpRay.origin = rayStartPoint.position;
        wallJumpRay.direction = (rayStartPoint.position - wallJumpChecker.transform.position) * -1;

        if (Physics.Raycast(headRay, out headHit, Vector3.Distance(rayStartPoint.position, headHeight.transform.position), 1 << LayerMask.NameToLayer("Parkourable")))
        {
            if (headHit.transform.tag == "ParkourObject")
            {
                headHitInfoStore = headHit.collider.bounds.max.y;
                headRayTouchObject = true;
            }
        }
        else
            headRayTouchObject = false;

        if (Physics.Raycast(wallJumpRay, out wallJumpRayHit, Vector3.Distance(wallJumpRay.origin, wallJumpChecker.transform.position), 1 << LayerMask.NameToLayer("Parkourable")))
        {
            if (wallJumpRayHit.transform.tag == "ParkourObject")
            {
                wallJumpRayTouchObject = true;
            }
        }
        else
            wallJumpRayTouchObject = false;
    }
    void Modes()
    {
        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && !ps.Sneaking)
        {
            climbMode = true;
        }
        else
        {
            climbMode = false;
            ResetParkour();
        }

        if (climbMode && timer <= timerMax)
        {
            if (headRayTouchObject)
            {
                inAirTimer = 0;
                timer += Time.deltaTime;
                playerRigid.isKinematic = true;

                if (groundChecker.transform.position.y <= headHitInfoStore)
                {
                    passedObject = false;
                }
            }
            else
                playerRigid.isKinematic = false;

            if (!passedObject)
            {
                playerRigid.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
                playerRigid.constraints = RigidbodyConstraints.FreezeRotation;
                //transform.position += new Vector3(0, 0.2f, 0);
                playerRigid.position += new Vector3(0, 0.2f, 0);
            }

            if (groundChecker.transform.position.y > headHitInfoStore && !vaultBoostGiven)
            {
                playerRigid.constraints = RigidbodyConstraints.None;
                playerRigid.constraints = RigidbodyConstraints.FreezeRotation;
                playerRigid.AddForce((transform.forward + transform.up).normalized * vaultSpeedBoost);
                vaultBoostGiven = true;
                passedObject = true;
                playerRigid.Sleep();
                playerRigid.WakeUp();
            }
        }
        else
        {
            playerRigid.isKinematic = false;
            playerRigid.constraints = RigidbodyConstraints.None;
            playerRigid.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (groundCollision.OnGround)
            ResetParkour();
    }
    void ResetParkour()
    {
        passedObject = true;
        playerRigid.isKinematic = false;
        timer = 0;
        vaultBoostGiven = true;
        climbMode = false;
    }

    void AdvancedMovement()
    {
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
                    playerRigid.AddForce(transform.up * (wallJumpForce + 10));
                    playerRigid.AddForce((transform.forward + transform.right) * wallJumpForce);
                }
                if (wallJumpChecker.transform.localPosition == new Vector3(8, 0, 0))
                {
                    //apply force left-forward
                    playerRigid.AddForce(transform.up * (wallJumpForce + 10));
                    playerRigid.AddForce((transform.forward + -transform.right) * wallJumpForce);
                }
            }
        }

        //onlanding speedboost with "W" tap
        if (!groundCollision.OnGround && inAirTimer >= inAirMax)
            ableSpeedBoost = true;

        if (ableSpeedBoost && Input.GetKeyDown(KeyCode.W))
        {
            if (groundCollision.OnGround)
            {
                playerRigid.AddForce(transform.forward * boostAmount * Time.deltaTime);
                ableSpeedBoost = false;
            }
        }
        if (!groundCollision.OnGround)
        {
            inAirTimer += Time.deltaTime;
        }
        else
            inAirTimer = 0;

        if (speedBoostTimer >= speedBoostMax)
        {
            ableSpeedBoost = false;
            speedBoostTimer = 0;
        }
        if (ableSpeedBoost)
            speedBoostTimer += Time.deltaTime;
       // if (!ps.Sneaking)
         //   Sliding();
        //else
        //{
        //    playerColSliding.enabled = true;
        //    playerColStanding.enabled = false;
        //}
    }
    void Sliding()
    {
        //sliding with CTRL
        if (Input.GetKey(KeyCode.LeftControl) && groundCollision.OnGround && slideCDtimer <= 0)
        {
            if (playerRigid.velocity.x != 0 && !isSliding || playerRigid.velocity.z != 0 && !isSliding)
            {
                slideSpeed = slideSpeedOriginal;
                isSliding = true;
            }

            if (isSliding && slideTimer <= slideTimerMax)
            {
                slideTimer += Time.deltaTime;
                slideSpeed -= Time.deltaTime * decayRate;
                playerRigid.MovePosition(transform.position + transform.forward * slideSpeed * Time.deltaTime);
            }
            if (slideTimer > slideTimerMax)
            {
                hasSlided = true;
                isSliding = false;
                slideTimer = 0;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            hasSlided = true;
            isSliding = false;
            slideTimer = 0;
        }

        if (playerRigid.velocity.x == 0 || playerRigid.velocity.z == 0 && hasSlided)
        {
            slideCDtimer += Time.deltaTime;
        }

        if (isSliding)
        {
            playerColSliding.enabled = true;
            playerColStanding.enabled = false;

        }
        else
        {
            playerColSliding.enabled = false;
            playerColStanding.enabled = true;
        }

        if (hasSlided)
        {
            slideCDtimer += Time.deltaTime;
        }
        else
            slideCDtimer = 0;

        if (slideCDtimer > slideCDMax)
            hasSlided = false;
    }

}
