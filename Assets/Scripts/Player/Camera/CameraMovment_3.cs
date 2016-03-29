using UnityEngine;
using System.Collections;

public class CameraMovment_3 : MonoBehaviour
{


    //This is the look sens and the smoothnes that is applyed (recomended to keep it belove .15 becuse its a small delay when used)
    //The smoothness sould be able to turn of in the settings 
    //-------------------------------------------------------------
    [SerializeField]
    private float lookSensitivity = 1.5f;
    [SerializeField]
    [Range(0,1)]
    private float lookSmoothDamp = .1f;
    [SerializeField]
    [Range(0, 1)]
    private float inAirControllMult = .45f;
    //-------------------------------------------------------------
    private float yRotation, xRotation;
    private float currentXRotation, currentYRotation;
    private float xRotationVel, yRotationVel;
    //-------------------------------------------------------------
    //OtherScripts  
    private GroundColision characterGroundCollision;
    private PlayerMovement playerMovement;
    //-------------------------------------------------------------
    //Other objects
    [SerializeField]
    private Transform CharacterTransform;    
    [SerializeField]
    private Transform CameraOringinTransform;
    private Rigidbody characterRigid;
    //-------------------------------------------------------------
    [SerializeField]
    private Transform WalkingBasePos, CrunchingBasePos, SlidingBasePos;
    private Vector3 refVector;
    private float refFloat;
    [SerializeField]
    [Range(0, 1)]
    private float CameraMoveToBaseTime = .2f;
    [SerializeField]
    private PlayerColManager playerColManager;
    //-------------------------------------------------------------
    private bool _LockCharacter = false;
    public bool LockCharacter
    {
        set { _LockCharacter = value; }
    }
    //-------------------------------------------------------------
    //From this points its varibles that are used for the cam shake when the character is moving 
    private float timeMoved;
    [SerializeField]
    [Range(0,2)]
    private float DistanceToMoveInYAxis = 1.4f;
    [SerializeField]
    [Range(0,20)]
    private float Mult;
    //-------------------------------------------------------------
    public enum Angels { Horizontal, Vertical, Both }

    //-------------------------------------------------------------
    void Awake()
    {
        yRotation = transform.rotation.eulerAngles.y;
        currentYRotation = yRotation;
    }
    void Start()
    {
        characterRigid = CharacterTransform.gameObject.GetComponent<Rigidbody>();
        characterGroundCollision = CharacterTransform.gameObject.GetComponent<GroundColision>();
        playerMovement = CharacterTransform.gameObject.GetComponent<PlayerMovement>();
        CameraOringinTransform = transform.parent;
    }
    //-------------------------------------------------------------
    void FixedUpdate()
    {
        TakeInputs();
        ApplyInputs();
        SelectCamBasePosition();
        ApplyCamStepMove();
        RotateThowards(Vector3.zero, Angels.Both);

    }
    //-------------------------------------------------------------
    void TakeInputs()
    {
        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;

        xRotation = Mathf.Clamp(xRotation, -90, 60);

        currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationVel, lookSmoothDamp);
        currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationVel, lookSmoothDamp);
    }
    //-------------------------------------------------------------
    void ApplyInputs()
    {
        transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);

        CharacterTransform.rotation = Quaternion.Euler(0, currentYRotation, 0);
    }
    //-------------------------------------------------------------
    void ApplyCamStepMove()
    {
        if (playerMovement.IsMoving == true)
        {
            timeMoved += Time.deltaTime;
            //}
            //else
            //    timeMoved = 0;

            transform.position = Vector3.SmoothDamp(transform.position,CameraOringinTransform.position + (Vector3.up * Mathf.Sin(timeMoved * Mult) * DistanceToMoveInYAxis),ref refVector,CameraMoveToBaseTime);
        }
        else
            MoveToBasePosition();
    }
    //-------------------------------------------------------------
    void SelectCamBasePosition()
    {
        //Check the current possition
        //Check what is the target position
        switch (playerColManager.CurrentColliderState)
        {
            case PlayerColManager.ColliderState.Standing:
                CameraOringinTransform = WalkingBasePos;
                break;
            case PlayerColManager.ColliderState.Sneaking:
                CameraOringinTransform = CrunchingBasePos;
                break;
            case PlayerColManager.ColliderState.Sliding:
                CameraOringinTransform = SlidingBasePos;
                break;
        }

    }
    //-------------------------------------------------------------
    void MoveToBasePosition()
    {
        //Move Using Some kind of smoothing to the new location and then based on what location it is it sould deside if there sould be some sin() shit going on ,
        transform.position = Vector3.SmoothDamp(transform.position, CameraOringinTransform.position, ref refVector, CameraMoveToBaseTime);
       
        //transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(transform.position.y,CameraOringinTransform.position.y,ref refFloat, CameraMoveToBaseTime));

    }
    //-------------------------------------------------------------
    public void RotateThowards(Transform Target_Obj, Angels angles)
    {
        //check what the target rotatation sould be
        //Check if its faster to add or to remove to get to the deiserad angel
        //Apply the rotation by using Mathf.Pow(x^2) where x is the different and make sure to be able to change the 2 with a slider so it sould be easy to adjust if needed :#
        Vector3 TargetDir = (Target_Obj.position - transform.position).normalized;
        float Diffrence_X = transform.rotation.x - TargetDir.x;
        float MoveForce = Mathf.Pow(Diffrence_X, 25);
        
    }
    public void RotateThowards(Vector3 EulerRotation, Angels angles)
    {
        float Diffrence_Y = EulerRotation.x - transform.eulerAngles.x;
        float MoveForce = Mathf.Pow(Diffrence_Y, 25);
        
    }
    //-------------------------------------------------------------
}
