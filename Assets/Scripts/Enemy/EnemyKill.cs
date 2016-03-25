using UnityEngine;
using System.Collections;

public class EnemyKill : MonoBehaviour {
    private EnemyController controller;
    private Rigidbody enemyRigidbody;
    private NavMeshAgent agent;
    private void Start()
    {
        controller = transform.GetComponent<EnemyController>();
        agent = GetComponent<NavMeshAgent>();
        enemyRigidbody = GetComponent<Rigidbody>();
    }
    public void Update()
    {
        if (Input.GetKey(KeyCode.G))
        {
            Kill();
        }
    }
    public void Kill()
    {
        //if (Input.GetKey(KeyCode.G))
        if (controller.InstanceState == EnemyController.enemyState.walking || controller.InstanceState == EnemyController.enemyState.CheckArea)
        {
            controller.InstanceState = EnemyController.enemyState.dead;
            agent.enabled = false;
            enemyRigidbody.isKinematic = false;
            enemyRigidbody.AddTorque(transform.right * 5);
        }
    }
}
