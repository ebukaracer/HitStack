using System;
using UnityEngine;


public interface IPlayerState
{
    event Action<GameObject> OnHitGoodStack;
    event Action<GameObject> OnHitBadStack;
    event Action<GameObject> OnHitCheckpoint;
}
