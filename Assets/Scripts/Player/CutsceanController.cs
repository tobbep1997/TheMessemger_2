using UnityEngine;
using System.Collections;

public class CutsceanController : MonoBehaviour {

    [SerializeField]
    private float speed,stopingDistance;
    private Vector3 currentTarget = Vector3.zero;

    private bool _reachedDestination;
    public bool ReachedDestination
    {
        get { return _reachedDestination; }
    }
    private Rigidbody playerRigidbody;
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }
    public void SetDestenation(Vector3 newTarget)
    {
        currentTarget = newTarget;
    }
    private void Update()
    {
        MovePlayer();
        CheckRemainingDistance();
    }
    private void MovePlayer()
    {
        if (!_reachedDestination)
            playerRigidbody.MovePosition(transform.position - (transform.position - currentTarget).normalized * speed * Time.deltaTime);
    }
    private void CheckRemainingDistance()
    {
        if (Vector2.Distance(new Vector2(transform.position.x,transform.position.z), new Vector2(currentTarget.x,currentTarget.z)) <= stopingDistance)
        {
            _reachedDestination = true;
        }
        else
        {
            _reachedDestination = false;
        }
    }
}
