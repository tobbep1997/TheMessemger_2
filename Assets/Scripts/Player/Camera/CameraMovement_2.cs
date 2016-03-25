using UnityEngine;
using System.Collections;
using System.Threading;

public class CameraMovement_2 : MonoBehaviour {

    [SerializeField]
    private Transform targetTransform,NeckJointTransform;
    [SerializeField]
    private bool moveY, rotateX;
    [SerializeField]
    [Range(0.01f,.5f)]
    private float moveYSmoothTimer = .4f;
    private float refMoveY;
    [SerializeField]
    [Range(0.01f,.5f)]
    private float rotateYSmoothTimer = .4f;
    private float refRotateY;
    [SerializeField]
    [Range(0.1f, 0.001f)]
    private float offset = 0.005f;

    [SerializeField]
    private float lookSensitivity = 1.5f;
    [SerializeField]
    private float LockedCharacterRangeOfViewe = 30;
    private float yRotation, xRotation;
    private float currentYRotation, currentXRotation;
    private float yRotationVel, xRotationVel;
    [SerializeField]
    private float lookSmoothDamp = 0.1f;
    [SerializeField]
    private Transform PlayerTransform;

    private bool _LockCharacter = false;
    private float _CharacterLockedYRot;
    public bool LockCharacter
    {
        get { return _LockCharacter; }
        set
        {
            if (_LockCharacter != value)
            {
                _LockCharacter = value;
                if (_LockCharacter == true)
                {
                    _CharacterLockedYRot = PlayerTransform.rotation.y;
                }
            }
        }
    }

    private bool _TakeInputs = true;
    public bool TakeInputs
    {
        set { _TakeInputs = value; }
    }

    

    private void Update()
    {

    }
    void FixedUpdate()
    {

        if (!_TakeInputs)
            return;

        GetMouseInputs();
        if (moveY)
            MoveY();
        if (rotateX && currentXRotation.CloseTo(xRotation, offset) && currentYRotation.CloseTo(yRotation, offset))
            RotateX();
    }
    private void GetMouseInputs()
    {


            yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
            xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;

            xRotation = Mathf.Clamp(xRotation, -90, 60);



            currentXRotation = Mathf.SmoothDamp(currentXRotation, xRotation, ref xRotationVel, lookSmoothDamp);
            currentYRotation = Mathf.SmoothDamp(currentYRotation, yRotation, ref yRotationVel, lookSmoothDamp);
        

        if (!currentXRotation.CloseTo(xRotation, offset))        
        {
            transform.rotation = Quaternion.Euler(currentXRotation, transform.rotation.eulerAngles.y, 0);
            NeckJointTransform.transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        }
        if (!currentYRotation.CloseTo(yRotation, offset))
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, currentYRotation, 0);
            NeckJointTransform.transform.rotation = Quaternion.Euler(currentXRotation, currentYRotation, 0);
        }

        if (!currentYRotation.CloseTo(yRotation, offset) && !_LockCharacter)
        {
            PlayerTransform.rotation = Quaternion.Euler(0, currentYRotation, 0);
        }
    }
    private void MoveY()
    {
        float newY = Mathf.SmoothDamp(transform.position.y, targetTransform.position.y, ref xRotationVel, moveYSmoothTimer);
        transform.position = new Vector3(targetTransform.position.x,newY,targetTransform.position.z);
    }
    private void RotateX()
    {
        float newY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetTransform.eulerAngles.y, ref yRotationVel, rotateYSmoothTimer);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, newY, transform.eulerAngles.z);
    }
    public void LookAt(Vector3 Point)
    {
        Debug.DrawLine(transform.position, Point, Color.green);
        Debug.DrawRay(transform.position, transform.forward * Vector3.Distance(transform.position, Point), Color.red);
        Debug.DrawLine(transform.position + transform.forward * Vector3.Distance(transform.position, Point), Point, Color.blue);

        Quaternion targetRot = Quaternion.LookRotation(Point - transform.position);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5 * Time.deltaTime);

        //PlayerTransform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        
    }   
}
