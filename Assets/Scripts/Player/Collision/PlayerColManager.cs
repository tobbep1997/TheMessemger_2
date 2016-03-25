using UnityEngine;
using System.Collections;

public class PlayerColManager : MonoBehaviour {

    [SerializeField]
    private Collider standing, sneaking, sliding;

    public enum ColliderState { Standing, Sneaking, Sliding }
    [SerializeField]
    private ColliderState currentColliderState = ColliderState.Standing;

    public ColliderState CurrentColliderState
    {
        get { return currentColliderState; }
    }
    private void FixedUpdate()
    {
        SelectCurrentColliderState();
    }
    private void SelectCurrentColliderState()
    {
        switch (currentColliderState)
            {
                case ColliderState.Standing:
                standing.enabled = true;
                sneaking.enabled = false;
                sliding.enabled = false;
                    break;
                case ColliderState.Sneaking:
                standing.enabled = false;
                sneaking.enabled = true;
                sliding.enabled = false;
                    break;
                case ColliderState.Sliding:
                standing.enabled = false;
                sneaking.enabled = false;
                sliding.enabled = true;
                    break;
                default:
                    break;
            }
    }
    public void SetColliderState(ColliderState s)
    {
        currentColliderState = s;
    }


}
