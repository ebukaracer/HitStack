using UnityEngine;
using UnityEngine.Events;


public interface IPlayerState
{
    event UnityAction<GameObject> OnHitGoodStack;
    event UnityAction<GameObject> OnHitBadStack;
    event UnityAction<GameObject> OnHitCheckpoint;
}
