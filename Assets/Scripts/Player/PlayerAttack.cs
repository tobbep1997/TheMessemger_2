using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{

    [SerializeField]
    private float attackDistance = 3;
    [SerializeField]
    private Canvas attackCanvas;
    [SerializeField]
    private Camera cam;

    private Ray ray;
    private RaycastHit[] hits;
    [SerializeField]
    private bool attack;
    private GameObject currentTarget;

    private void Update()
    {
        GetTarget();
        CheckTarget();
        Attack();
        DisplayHud();
    }
    private void GetTarget()
    {
        ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
    }
    private void CheckTarget()
    {
        hits = Physics.RaycastAll(ray, attackDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject.tag == "Guard" && hits[i].collider.isTrigger == false)
            {
                attack = true;
                currentTarget = hits[i].transform.gameObject;
                break;
            }
            else
            {
                attack = false;
                currentTarget = null;
            }
        }
    }
    private void DisplayHud()
    {
        attackCanvas.enabled = attack;
    }
    private void Attack()
    {
        if (Input.GetKey(KeyCode.F) && currentTarget != null)
        {
            currentTarget.GetComponent<EnemyKill>().Kill();
        }
    }
}
