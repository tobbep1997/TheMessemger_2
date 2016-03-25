using UnityEngine;
using System.Collections;

public class EnemyDetection : MonoBehaviour {

    private EnemyUnityController UnitController;
    private void Start()
    {
        UnitController = transform.root.GetComponent<EnemyUnityController>();
    }
    private void OnTriggerStay(Collider other)
    {
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        UnitController.SetPlayerObject(other);   
    }
}
