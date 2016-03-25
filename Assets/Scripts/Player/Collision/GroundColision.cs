using UnityEngine;
using System.Collections;

public class GroundColision : MonoBehaviour
{

    private Rigidbody PlayerRigid;

    [SerializeField]
    private bool onGround;
    public bool OnGround
    {
        get { return onGround; }
    }


    [SerializeField]
    private Transform StartTransform;
    [SerializeField]
    private float CheckDistance = .1f;

    private Ray ray;
    private RaycastHit[] hit;


    void Start()
    {
        PlayerRigid = GetComponent<Rigidbody>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag.Contains("Guard"))        
            return;
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Parkourable"))
            onGround = true;
        else
            onGround = false;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("Guard"))
            return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Parkourable"))
            onGround = false;
    }
    void FixedUpdate()
    {
        RayCheckGroundCollision();
    }
    void RayCheckGroundCollision()
    {
        
        if (PlayerRigid.isKinematic)
        {
            ray = new Ray(StartTransform.position, Vector3.down);
            hit = Physics.RaycastAll(ray,CheckDistance);

            if (hit.Length >= 0)
            {
                onGround = false;
                return;
            }

            for (int i = 0; i < hit.Length; i++)
            {
                if (hit[i].transform.gameObject.layer == LayerMask.NameToLayer("Ground") || hit[i].transform.gameObject.layer == LayerMask.NameToLayer("Parkourable"))
                {
                    onGround = true;
                    break;
                }
                else
                    onGround = false;
            }
            
        }
    }
}
