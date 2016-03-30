using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour {

    [SerializeField]
    Transform Target;
    [SerializeField]
    float SmoothTime;

    Vector3 targetPosition;

    [SerializeField]
    float SinHightMult, SinMult;
    float Timer;

    void FixedUpdate()
    {
        MoveToTargetPosition();
        RotateObject();
    }
    void MoveToTargetPosition()
    {
        Timer += Time.deltaTime;
        targetPosition = new Vector3(Target.position.x, Target.position.y + (Mathf.Sin(Timer * SinMult) * SinHightMult), Target.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, SmoothTime * Time.deltaTime);
    }
    void RotateObject()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, SmoothTime * Time.deltaTime);
    }
    
}
