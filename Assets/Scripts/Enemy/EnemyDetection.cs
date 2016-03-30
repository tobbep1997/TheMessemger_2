using UnityEngine;
using System.Collections;

public class EnemyDetection : MonoBehaviour {

    [SerializeField]
    private EnemyUnityController UnitController;
    private void Start()
    {
        if (UnitController == null)
            UnitController = transform.root.GetComponent<EnemyUnityController>();

    }
    private void OnTriggerStay(Collider other)
    {

        if (UnitController == null)
        {
            throw new System.Exception("Unity Controller is null");

        }
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
            UnitController.SetPlayerObject(other);   
    }
}
