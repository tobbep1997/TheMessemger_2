using UnityEngine;
using System.Collections;

[RequireComponent(typeof (NavMeshAgent))]
public class EnemyUnityController : MonoBehaviour {

    [SerializeField]
    MasterEnemyController masterController;

    [SerializeField]
    bool UseAnimation = true;


    //Path and destination
    [Header("Movment and destination")]
    public Transform[] Path;
    private Transform CurrentTargetPosition;
    private float OffsetDistance = 1;
    [SerializeField]
    private float UnitSpeed = 2;

    //Navigation/animations
    private NavMeshAgent agent;
    private Animator anim;
    [SerializeField]
    private float AnimSpeedDevider = 8;

    public enum EnemyState { walking, patroling, checkCurrentArea, chasing, attacking, checkLastSeenArea }
    [SerializeField]
    private EnemyState CurrentEnemyState = EnemyState.walking;

    //CheckCurrentArea
    [Header("CheckArea")]
    [SerializeField]
    [Range(1,180)]
    private float rotationWidth = 90;
    [SerializeField]
    private float waitTime = 2;
    private float rotationWaitTimer;
    [SerializeField]
    [Range(0.001f,1)]
    private float SinMultiplier;
    private float RotationTimer, startAngle;    
    Vector3 lookPos;
    private bool lookAtLookPos;
    [SerializeField]
    private Transform HeadPos;

    //Detection
    [Header("Detection")]
    [SerializeField]
    private Transform Target;  
    private Collider TargetCollider;    
    private Vector3? TargetPosition, lastSeenPosition;
    [SerializeField]
    private Transform StartDetectionRayPosition;
    [SerializeField]
    private float visionDetectionRange = 5, smallDetectionRadious = 2, bigDetectionRadious = 5;
    [SerializeField]
    private bool _PlayerDetected = false;
    public bool PlayerDetected
    {
        get { return _PlayerDetected; }
    }

    //DetectionColliders
    [SerializeField]
    private SphereCollider SphereDetectionCollider;

    //Chase
    [Header("Chase")]
    [SerializeField]
    private float ChasingSpeed = 6;

    //Attack
    [Header("Attack")]
    [SerializeField]
    private GameObject Bullet;
    [SerializeField]
    private float attackDistance = 4;
    [SerializeField]
    private float timeBeforeShot = 2;
    private float gunTimer;
    
    //Deactivaton
    [Header("Deactivation")]
    private float deactivateTime;
    private float deactivationTimer;
    private bool deactivated;

    //Extras
    [Header("Extras")]
    [SerializeField]
    private Transform LightPosition;
    public bool UseFlaslight = false;

    //----------------------------- Unity standard functions
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        masterController = GameObject.FindGameObjectWithTag("GameController").GetComponent<MasterEnemyController>();
        if (LightPosition != null)
            LightPosition.gameObject.SetActive(UseFlaslight);     
    }
    void Update()
    {
        if (deactivated)
        {
            DeactivateUpdate();
            return;
        }
        ManageDetectionColliders();
        if (CurrentEnemyState != EnemyState.attacking)
        {
            gunTimer = 0;
        }
        switch (CurrentEnemyState)
        {
            case EnemyState.walking:
                SelectPosition();
                break;
            case EnemyState.patroling:
                break;
            case EnemyState.checkCurrentArea:
                CheckCurrentArea();
                break;
            case EnemyState.chasing:
                ChasePlayer();
                break;
            case EnemyState.attacking:
                AttackPlayer();
                break;
            case EnemyState.checkLastSeenArea:
                CheckLastSeenArea();
                break;
            default:
                break;
        }
        
    }
    //----------------------------- Movment
    void SelectPosition()
    {
        if (Path.Length <= 0 || Path == null)        
            return;        
        if (CurrentTargetPosition == null)
            CurrentTargetPosition = Path[0];
    

        if (ReachedDestination())
        {
            CurrentEnemyState = EnemyState.checkCurrentArea;
            startAngle = transform.eulerAngles.y;
            //CurrentTargetPosition = GetNextPosition(Path, CurrentTargetPosition);           
        }
        else
            MovePlayer(CurrentTargetPosition.position);
    }
    void MovePlayer(Vector3 TargetPostion)
    {
        agent.Resume();
        agent.SetDestination(TargetPostion);
        agent.speed = UnitSpeed;
        SetAnimations(true);
    }
    bool ReachedDestination()
    {
        if (Vector3.Distance(transform.position, CurrentTargetPosition.position) <= OffsetDistance && agent.pathStatus == NavMeshPathStatus.PathComplete)        
            return true;       
        else
            return false;
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
    //----------------------------- CheckCurrentArea
    void CheckCurrentArea()
    {
        agent.velocity = Vector3.zero;    
        agent.Stop();

        rotationWaitTimer += Time.deltaTime;
        if (rotationWaitTimer >= waitTime)
        {
            RotationTimer += Time.deltaTime;
            float rotationAmout = rotationWidth * Mathf.Sin(RotationTimer * SinMultiplier);            
            HeadPos.eulerAngles = new Vector3(HeadPos.rotation.x, startAngle + rotationAmout, HeadPos.rotation.z);

            SetAnimations(false, null);

            if (RotationTimer > (Mathf.PI*2)/SinMultiplier)
            {
                CurrentEnemyState = EnemyState.walking;
                CurrentTargetPosition = GetNextPosition(Path, CurrentTargetPosition);
                ResetCheckCurrentArea();
            }
        }        
    }
    void ResetCheckCurrentArea()
    {
        agent.Resume();
        lookAtLookPos = false;
        RotationTimer = 0;
        rotationWaitTimer = 0;
        SetLookPos(true);
    }
    //----------------------------- Detection
    public void SetPlayerObject(Collider PlayerCollider)
    {
        if (Vector3.Distance(PlayerCollider.bounds.center, transform.position) <= visionDetectionRange)
            if (CheckDirectVision(PlayerCollider) && CurrentEnemyState != EnemyState.attacking)
            {
                if (PlayerDetected == false)
                {
                    masterController.CallNearEnemys(this, PlayerCollider);
                }
                _PlayerDetected = true;
                ResetCheckCurrentArea();
                Target = PlayerCollider.transform.root;
                TargetPosition = PlayerCollider.transform.root.position;
                TargetCollider = PlayerCollider;
                CurrentEnemyState = EnemyState.chasing;
                
            }       
    }
    public void CallGuard(Collider PlayerCollider)
    {        
        if (CurrentEnemyState != EnemyState.attacking)
        {
            _PlayerDetected = true;
            ResetCheckCurrentArea();
            Target = PlayerCollider.transform.root;
            TargetPosition = PlayerCollider.transform.root.position;
            TargetCollider = PlayerCollider;
            CurrentEnemyState = EnemyState.chasing;

        }
    }
    bool CheckDirectVision(Collider collider)
    {
        if (collider == null)        
            throw new System.Exception("The Collider has no assinged value");        
        
        Ray ray = new Ray(StartDetectionRayPosition.position, collider.bounds.center - StartDetectionRayPosition.position);
        RaycastHit[] hit = Physics.RaycastAll(StartDetectionRayPosition.position, ray.direction, Vector3.Distance(collider.bounds.center, StartDetectionRayPosition.position));

        Transform RootObject = collider.transform.root;
        bool directVisionToTarget = false;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform.gameObject.layer != LayerMask.NameToLayer("Player") && hit[i].transform.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                if (hit[i].transform.root == transform)                    
                    continue;
                directVisionToTarget = false;
                break;  
            }
            else            
                directVisionToTarget = true;            
        }
        return directVisionToTarget;
    }
    bool CheckDirectVision(Transform targetTransform)
    {
        if (targetTransform == null)
            throw new System.Exception("The Collider has no assinged value");

        Ray ray = new Ray(StartDetectionRayPosition.position, targetTransform.position - StartDetectionRayPosition.position);
        RaycastHit[] hit = Physics.RaycastAll(StartDetectionRayPosition.position, ray.direction, Vector3.Distance(targetTransform.position, StartDetectionRayPosition.position));

        Transform RootObject = targetTransform.root;
        bool directVisionToTarget = false;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].transform.gameObject.layer != LayerMask.NameToLayer("Player") && hit[i].transform.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            {
                if (hit[i].transform.root == transform)
                    continue;
                directVisionToTarget = false;
                break;
            }
            else
                directVisionToTarget = true;
        }
        return directVisionToTarget;
    }
    //----------------------------- Colliders
    void ManageDetectionColliders()
    {
        if (PlayerDetected)        
            SphereDetectionCollider.radius = bigDetectionRadious;        
        else
            SphereDetectionCollider.radius = smallDetectionRadious;
    }
    //----------------------------- Chase
    void ChasePlayer()
    {
        agent.Resume();
        agent.SetDestination(Target.position);
        SetAnimations(true);
        agent.speed = ChasingSpeed;
        if (Vector3.Distance(Target.position, transform.position) <= attackDistance)
        {
            lastSeenPosition = Target.position;
            CurrentEnemyState = EnemyState.attacking;
        }
        if (Vector3.Distance(Target.position,transform.position) >= visionDetectionRange || !CheckDirectVision(TargetCollider))
        {
            lastSeenPosition = Target.position;
            CurrentEnemyState = EnemyState.checkLastSeenArea;
        }

    }
    void CheckLastSeenArea()
    {
        agent.Resume();
        agent.SetDestination((Vector3)lastSeenPosition);
        agent.speed = ChasingSpeed;
        SetAnimations(true);
        _PlayerDetected = false;
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            CurrentEnemyState = EnemyState.checkCurrentArea;
        }
    }
    //----------------------------- AttackPlayer
    void AttackPlayer()
    {
        if (Target == null)
        {
            CurrentEnemyState = EnemyState.patroling;
            throw new System.Exception("Target was never assigned");
        }
        agent.velocity = Vector3.zero;     
        agent.Stop();
        SetAnimations(false);
        if (CheckDirectVision(TargetCollider))
        {
            if (Vector3.Distance(transform.position,Target.position) > attackDistance)
            {
                lastSeenPosition = Target.position;
                CurrentEnemyState = EnemyState.chasing;
                return;
            }

            gunTimer += Time.deltaTime;
            if (gunTimer >= timeBeforeShot)
            {
                gunTimer = 0;
                FireBullet();
            }            
        }
        else
        {

            lastSeenPosition = Target.position;
            CurrentEnemyState = EnemyState.chasing;
        }
    }
    void FireBullet()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    //----------------------------- Animations
    void SetAnimations(bool moving, Vector3? rotateHead)
    {
        if (!UseAnimation)
            return;
        if (moving)
        {
            anim.SetFloat("Forward", agent.speed / AnimSpeedDevider);
        }
        else
            anim.SetFloat("Forward", 0);
        this.lookAtLookPos = true;
        SetLookPos(rotateHead);
    }
    void SetAnimations(bool moving)
    {
        if (!UseAnimation)
            return;
        if (moving)
        {
            anim.SetFloat("Forward", agent.speed / AnimSpeedDevider);
        }
        else
            anim.SetFloat("Forward", 0);
        SetLookPos(true);
        this.lookAtLookPos = false;
    }
    void DeactivateAnimation()
    {

    }
    void SetLookPos(Vector3? LookPosition)
    {
        if (LookPosition != null)
        {
            lookPos = (Vector3)LookPosition;
        }
        else
            lookPos = HeadPos.position + (HeadPos.forward * 10);        
    }
    void SetLookPos(bool reset)
    {
        if (reset)        
            HeadPos.localEulerAngles = Vector3.zero;        
    }
    void OnAnimatorIK()
    {
        if (lookAtLookPos)
        {
            anim.SetLookAtWeight(1); 
            anim.SetLookAtPosition(lookPos);            
        }
        if (LightPosition != null && UseFlaslight)
        {
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

            anim.SetIKPosition(AvatarIKGoal.RightHand, LightPosition.position);
            anim.SetIKRotation(AvatarIKGoal.RightHand, LightPosition.rotation);
        }

    }
    //----------------------------- DeActivate
    public void DeactivateEnemy(float duration)
    {
        DeactivateAnimation();
        deactivateTime = duration;
        deactivated = true;
    }
    private void DeactivateUpdate()
    {
        deactivationTimer += Time.deltaTime;
        if (deactivationTimer >= deactivateTime)
        {
            deactivated = false;
            deactivationTimer = 0;
        }
    }
    //----------------------------- Functions
}
