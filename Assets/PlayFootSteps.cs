using UnityEngine;
using System.Collections;

public class PlayFootSteps : MonoBehaviour {

    [SerializeField]
    private AudioClip[] FootStep;
    private AudioSource audioSorce;

    private GroundColision Gc;
    private PlayerMovement Pm;
    private PlayerSneek Ps;

    [SerializeField]
    private float TimeBetwenSteps = .5f;
    private float Timer;

    void Start()
    {
        audioSorce = GetComponent<AudioSource>();
        Pm = GetComponent<PlayerMovement>();
        Gc = GetComponent<GroundColision>();
        Ps = GetComponent<PlayerSneek>();
    }

	void Update () {

        if (FootStep == null || FootStep.Length == 0)
        {         
            return;
        }

        Timer += Time.deltaTime;
        if (Gc.OnGround && Pm.IsMoving && !Ps.Sneaking && Timer >= TimeBetwenSteps)
        {
            if (!audioSorce.isPlaying)
            {
                audioSorce.PlayOneShot(FootStep[Random.Range(0, FootStep.Length - 1)]);
                Timer = 0;
            }
        }
    }
}
