using UnityEngine;
using System.Collections;


public class EnemyController : MonoBehaviour {

    [SerializeField]    
    private float walkingSpeed, runningSpeed = 5, loseAggroTime = 5, guardsCallDistance = 10, guardVisionRange = 4, stayBeforeContinueTime = 4, turnRate = 80,patrollTurnRate = 50;
    [SerializeField]
    private float detectRange = 4, sneakDetectRange = 2;
    private float currentSpeed, aggroTimer = 0, waitTimer = 0;
    private float refCurrentVel, currentTargetRotation, rotationTime,rotationTimer, currentYValue;
    private int stateOfRotate = 0;
    [SerializeField]
    private float range, damage, timeBetweenShots, bulletSpeed;
    private float bulletTimer;
    [SerializeField]
    private Transform muzzlePositon;
    [SerializeField]
    private GameObject bulletObject;
    [SerializeField]
    private Transform[] walkPositions;
    private Transform currentWalkPos;

    [SerializeField]
    private Collider bigDetectionCollider;
    [SerializeField]
    private Transform coneCollider;

    public enum enemyState { walking, chasing, CheckArea,dead}
    private bool dead = false;
    [SerializeField]
    private enemyState instanceState = enemyState.walking;
    public enemyState InstanceState
    {
        get { return instanceState; }
        set { instanceState = value; }
    }

    [SerializeField]
    private Transform currentTarget, lastTarget, lastSeenTargetPosition;
    private Vector3 lastSeenPos;
    private NavMeshAgent agent;
    private GameObject player;
    private PlayerSneek playerSneak;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerSneak = player.GetComponent<PlayerSneek>();
    }
    private void FixedUpdate()
    {
        if (instanceState == enemyState.dead)
        {
            dead = true;            
        }
        if (dead)
        {
            instanceState = enemyState.dead;
            gameObject.tag = "DeadGuard";
            return;
        }
                       
        //This switches the state based on if the instances has an target
        if (currentTarget == null)        
            instanceState = enemyState.walking;
        LookForPlayer();             
        switch (instanceState)
        {
            case enemyState.walking:
                Walking();
                bigDetectionCollider.enabled = false;
                break;
            case enemyState.chasing:                
                Chasing();
                ResetLookRot();
                bigDetectionCollider.enabled = false;
                break;
            case enemyState.CheckArea:                
                CheckArea();
                //ResetLookRot();
                bigDetectionCollider.enabled = true;
                break;
        }
    }
    private void LookForPlayer()//Checks the direct vision and if his in a sneek mode
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);        
        if (playerSneak.Sneaking)
        {
            if (distance <= sneakDetectRange && CheckDirectVision(player.transform))
            {
                EnemySeen(player.GetComponent<Collider>());
            }
        }
        else
        {
            if (distance <= detectRange && CheckDirectVision(player.transform))
            {
                EnemySeen(player.GetComponent<Collider>());
            }
        }
    }
    private void Shooting()//Attacks
    {
        bulletTimer += Time.deltaTime;
        if (bulletTimer >= timeBetweenShots)
        {
            GameObject newBullet = (GameObject)Instantiate(bulletObject, muzzlePositon.position, Quaternion.identity);
            
            newBullet.GetComponent<Rigidbody>().AddForce(muzzlePositon.transform.forward * bulletSpeed);
            bulletTimer = 0;
        }
    }
    private void Walking()//Moves the enemy to the target location
    {
        //This walks to the current target possitions and makes sure to the next on in the array and resets to zero when it reaches the max array index
        agent.speed = walkingSpeed;
        transform.tag = "Guard";
        if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= agent.stoppingDistance + 0.01 || agent.pathStatus == NavMeshPathStatus.PathInvalid || agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            if (Wait(walkPositions, currentWalkPos, stayBeforeContinueTime))
            {
                currentWalkPos = GetNextPosition(walkPositions, currentWalkPos);
                ResetLookRot();
            }
            else
                LookAround(patrollTurnRate, 0, stayBeforeContinueTime);
        }
        else
            ResetLookRot();
        
        if (currentWalkPos != null)
            agent.SetDestination(currentWalkPos.position);
    }
    private bool Wait(Transform[] positions, Transform currentPosition, float waitTime)//
    {
        if (positions == null)
            return false;
        if (currentPosition == null)
            return true;

        bool wait = false;
        if (currentPosition == positions[0])
            wait = true;
        else if (currentPosition == positions[positions.Length - 1])
            wait = true;
        if (wait)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waitTimer = 0;
                return true;                
            }
            else
                return false;
        }
        return true;
        
    }
    private void LookAround(float yRoatation,float startRotPosition,float time)//This rotates the coneCollider to the right by yRotation then to the left by the same amount and all under the time that you set
    {
        rotationTime = (time / 4) - 0.25f;
        rotationTimer += Time.deltaTime;
        for (int i = 0; i < 5; i++)
        {
            if (rotationTimer >= rotationTime * i)
            {
                if (i != stateOfRotate && i > stateOfRotate)
                {
                    stateOfRotate = i;
                    refCurrentVel = 0;
                    currentYValue = 0;                 
                }
            }
        }
        if (rotationTimer <= 0.25f)
        {
            startRotPosition = 0;
        }
        //print(stateOfRotate);
        currentYValue = Mathf.SmoothDamp(currentYValue, yRoatation, ref refCurrentVel, rotationTime - 0.5f);
        //print(currentTargetRotation);
        switch (stateOfRotate)
        {
            case 1:
                currentTargetRotation = startRotPosition + currentYValue;                
                break;
            case 2:
                currentTargetRotation = (startRotPosition + yRoatation) - currentYValue;                
                break;
            case 3:
                currentTargetRotation = 0 - currentYValue;
                break;
            case 4:
                currentTargetRotation = (startRotPosition - yRoatation) + currentYValue;                               
                break;
        }
        coneCollider.localEulerAngles = new Vector3(coneCollider.eulerAngles.x,
            currentTargetRotation,
            transform.eulerAngles.z);
    }
    private void ResetLookRot()//Resets LookAround()
    {
        coneCollider.localEulerAngles = new Vector3(coneCollider.localEulerAngles.x, 0, 0);
        stateOfRotate = 0;
        rotationTimer = 0;
    }
    private void Chasing()//Enemy runs after player
    {
        transform.tag = "AggroGuard";
        //This chases after the currentTarget
        CheckPlayer();
        agent.speed = runningSpeed;
        if (currentTarget != null)
        {
            if (Vector3.Distance(currentTarget.position, transform.position) <= range)
            {
                //agent.SetDestination(transform.position);                
                //Shooting();
                agent.SetDestination(currentTarget.position);
            }
            else
            {
                agent.SetDestination(currentTarget.position);
            }
        }
    }
    private void CheckArea()//This gose to the last area the target was last seen and scans the area 
    {
        transform.tag = "Guard";
        //This gose to the last area the target was last seen and scans the area 
        agent.speed = walkingSpeed;
        agent.SetDestination(lastSeenPos);
               
        if (agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance <= agent.stoppingDistance + 0.01)
        {
            aggroTimer += Time.deltaTime;
            if (aggroTimer >= loseAggroTime)
            {
                aggroTimer = 0;
                currentTarget = null;
                instanceState = enemyState.walking;
                ResetLookRot();
            }
            else
                LookAround(turnRate, 0, loseAggroTime);
        }
    }
    private void CheckPlayer()//This checks if the target is still in rage and in direct vision
    {
        //This checks if the target is still in rage and in direct vision
        if (Vector3.Distance(transform.position,currentTarget.position) > guardVisionRange || !CheckDirectVision(currentTarget))
        {
            instanceState = enemyState.CheckArea;
            lastSeenPos = currentTarget.position;            
        }
    }
    public void OnTriggerStay(Collider other)
    {
        if (instanceState == enemyState.dead)
        {
            return;
        }
        foreach (var item in other.transform.ReturnAllObjectsRelatedToCurrent())
        {
            if (CheckDirectVision(item))
            {
                Debug.DrawRay(transform.position, (item.position - transform.position).normalized * Vector3.Distance(transform.position, item.position), Color.green);
            }
        }
        //This checks if the player is spotted
        if (other.tag == "Player" && CheckDirectVision(other.transform))
        {
            
            instanceState = enemyState.chasing;
            lastTarget = currentTarget;
            currentTarget = other.transform;
            CallForGuards();            
        }
    }
    public void EnemySeen(Collider other)
    {
        if (instanceState == enemyState.dead)
            return;
        //This calls this 
        OnTriggerStay(other);
    }
    private bool CheckDirectVision(Transform target)
    {
        System.Collections.Generic.List<GameObject> ChildTransforms = new System.Collections.Generic.List<GameObject>();
        int _children = target.root.childCount;
        for (int i = 0; i < _children; i++)
        {
            ChildTransforms.Add(target.root.GetChild(i).gameObject);
        }
        //this checks if the pleyr is in direct vision
        float distance = Vector3.Distance(transform.position, target.position);
        Ray ray = new Ray(transform.position, (target.position - transform.position).normalized * distance);        
        RaycastHit[] raycastHit = Physics.RaycastAll(ray, distance);

        
        for (int i = 0; i < raycastHit.Length; i++)
        {
            //if (raycastHit[i].transform != target && raycastHit[i].transform.tag != "Guard" && raycastHit[i].transform.tag != "GuardObject")
            //{
            //    foreach (GameObject _childObject in ChildTransforms)
            //    {
            //        if (raycastHit[i].transform.gameObject != _childObject)
            //        {
            //            return false;
            //        }
            //    }
            //}
            
            foreach (GameObject _childObject in ChildTransforms)
            {
                if (raycastHit[i].transform == _childObject.transform && raycastHit[i].transform.tag != "Guard" && raycastHit[i].transform.tag != "GuardObject")
                {
                    Debug.DrawRay(transform.position, (_childObject.transform.position - transform.position).normalized * Vector3.Distance(transform.position, _childObject.transform.position), Color.green);
                }
            }
        }
        return false;
    }

    private void CallForGuards()
    {
        //This calls for all the guards in an area of the guard that calls
        Collider[] guardsInArea = Physics.OverlapSphere(transform.position, guardsCallDistance);
        System.Collections.Generic.List<GameObject> calledGuards = new System.Collections.Generic.List<GameObject>();
        for (int i = 0; i < guardsInArea.Length; i++)
        {
            if (guardsInArea[i].tag == "Guard" && !calledGuards.Contains(guardsInArea[i].gameObject) 
                && guardsInArea[i].gameObject != gameObject && guardsInArea[i].tag != "GuardObjects")
            {
                calledGuards.Add(guardsInArea[i].gameObject);
                guardsInArea[i].gameObject.GetComponent<EnemyController>().SetTarget(currentTarget.gameObject);
            }
        }
    }
    public void SetTarget(GameObject curretTarget)
    {
        //This sets the target
        instanceState = enemyState.chasing;        
        lastTarget = currentTarget;
        this.currentTarget = curretTarget.transform;
    }
    private Transform GetNextPosition(Transform[] positions, Transform current)
    {        
        //This return the next instance in an array
        if (current == null)
            if (positions.Length > 0)
                return positions[0];
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] == current)
            {
                if (i + 1 >= positions.Length)
                {
                    return positions[0];
                }
                else
                    return positions[i + 1];
            }
        }
        return current;
    }

    
    
}