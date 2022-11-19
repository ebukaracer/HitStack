using System;
using UnityEngine;
using static Racer.Utilities.SingletonPattern;

/// <summary>
/// Game states available for transitioning.
/// </summary>
internal enum GameState
{
    Loading,
    Playing,
    GameOver, 
    Completed
}

/// <summary>
/// This manages the various states of the game.
/// </summary>
[DefaultExecutionOrder(-5)]
internal class GameManager : Singleton<GameManager>
{
    public static event Action<GameState> OnCurrentState;

    [field: SerializeField]
    public GameState CurrentState { get; private set; }

    [field: SerializeField]
    public float StartDelay { get; private set; }


    /// <summary>
    /// Sets the current state of the game.
    /// </summary>
    /// <param name="state">Actual state to transition to.</param>
    public void SetGameState(GameState state)
    {
        switch (state)
        {
            case GameState.Loading:
                break;
            case GameState.Playing:
                break;
            case GameState.GameOver:
                break;
            case GameState.Completed:
                break;
        }

        CurrentState = state;

        OnCurrentState?.Invoke(CurrentState);
    }
}