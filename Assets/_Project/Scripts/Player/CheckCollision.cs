using UnityEngine;
using UnityEngine.Events;

internal class CheckCollision : MonoBehaviour, IStackController, IPlayerState
{
    protected bool HasClicked;

    // From Interface
    public event UnityAction<GameObject> OnHitBadStack;
    public event UnityAction<GameObject> OnHitGoodStack;
    public event UnityAction<GameObject> OnHitCheckpoint;


    public void StartBounce(bool hasClicked)
    {
        this.HasClicked = hasClicked;
    }


    public virtual void OnCollisionEnter(Collision collision)
    {
        var found = collision.gameObject;

        if (HasClicked)
        {
            if (found.CompareTag("Good"))
                OnHitGoodStack?.Invoke(found);

            if (found.CompareTag("Bad"))
                OnHitBadStack?.Invoke(found);
        }

        if (found.CompareTag("Checkpoint"))
            OnHitCheckpoint?.Invoke(found);
    }
}
