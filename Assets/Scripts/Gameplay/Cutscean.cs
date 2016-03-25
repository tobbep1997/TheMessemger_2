using UnityEngine;
using System.Collections;

public class Cutscean : MonoBehaviour
{
    [SerializeField]
    private CutsceanFrames[] PlayerWalkPositions;
    private CutsceanFrames currentCutFrame;
    [SerializeField]
    private bool loadNextLevel = false;
    [SerializeField]
    private Collider TriggerCol;

    private GameObject player;
    private Rigidbody playerRigibody;
    private CutsceanController navAgent;
    private PlayerMovement pm;
    private ParkourMovement_2 parm;
    private CameraMovement_2 camMove;

    [SerializeField]
    private bool cutsceanStarted = false;
    private enum PlayerCutsceanState { Walk,Wait}
    private PlayerCutsceanState currentState = PlayerCutsceanState.Walk;

    private float timer;
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.CompareTag("Player"))
        {
            player = col.gameObject;
            navAgent = col.GetComponent<CutsceanController>();
            pm = col.GetComponent<PlayerMovement>();
            parm = col.GetComponent<ParkourMovement_2>();
            camMove = Camera.main.GetComponent<CameraMovement_2>();
            StartCutScean();
        }
    }
    private void Update()
    {
        if (!cutsceanStarted)
            return;
        switch (currentState)
        {
            case PlayerCutsceanState.Walk:
                MovePlayer();
                break;
            case PlayerCutsceanState.Wait:
                PlayAudio();
                WaitAtPosition();
                MoveCamera();
                break;
            default:
                break;
        }
    }
    private void StartCutScean()
    {
        pm.enabled = false;
        parm.enabled = false;
        navAgent.enabled = true;
        cutsceanStarted = true;
        if (PlayerWalkPositions.Length > 0)        
            currentCutFrame = PlayerWalkPositions[0];
        
    }
    private void GetNextFrame()
    {
        if (currentCutFrame == GetNextInArray<CutsceanFrames>(PlayerWalkPositions, currentCutFrame))
        {
            StopCutscean();
        }
        else
        {
            currentCutFrame = GetNextInArray<CutsceanFrames>(PlayerWalkPositions, currentCutFrame);
            currentState = PlayerCutsceanState.Walk;
        }       
    }
    private void MovePlayer()
    {
        navAgent.SetDestenation(currentCutFrame.playerTargetPosition.position);
        if (navAgent.ReachedDestination)
        {
            currentState = PlayerCutsceanState.Wait;
        }
    }
    private void MoveCamera()
    {
        camMove.LockCharacter = true;
        camMove.TakeInputs = false;
        camMove.LookAt(currentCutFrame.LookAtPosition.position);
    }
    private void WaitAtPosition()
    {
        timer += Time.deltaTime;
        if (timer >= currentCutFrame.TimeWaitAtPosition)
        {
            timer = 0;
            GetNextFrame();            
        }
    }
    private void WriteSubtitle()
    {

    }
    private void PlayAudio()
    {

    }
    private void StopCutscean()
    {
        if (loadNextLevel)
        {
            Application.LoadLevel(Application.loadedLevel + 1);
        }
        
        navAgent.enabled = false;
        pm.enabled = true;
        parm.enabled = true;
        TriggerCol.enabled = false;
        camMove.LockCharacter = true;
    }


    private T GetNextInArray<T>(T[] array, T currentObject)       
    {
        if (array.Length == 0)
            return currentObject;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(currentObject))
            {
                if (i < array.Length - 1)
                {
                    return array[i + 1];
                }
                else
                    return currentObject;
            }        
        }
        return array[0];
    }
}
[System.Serializable]
public class CutsceanFrames
{    
    public Transform playerTargetPosition;
    public Transform LookAtPosition;
    public float TimeWaitAtPosition;
    public AudioClip AudioPlayAtPosition;
    public string SubtitleAtPosition;    
}

