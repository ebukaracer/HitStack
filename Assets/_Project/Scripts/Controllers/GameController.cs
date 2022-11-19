using Racer.SaveSystem;
using Racer.Utilities;
using UnityEngine;

internal class GameController : MonoBehaviour
{
    private IStackController _stackController;
    private IPlayerState _playerState;

    private ChildStack _childStack;
    private PlayerProperties _playerProperties;

    private float _platformCount;
    private bool _isOnPowerup;

    [SerializeField, Tooltip("Number of stacks to destroy in other to start increasing the power-up fill image")]
    private float maxPlatformCount = 3;

    [SerializeField, Range(0, 1), Tooltip("Rate to decrease attained [platformCount]")]
    private float platformCountDecreaseRate = .1f;

    [SerializeField, Range(0, 1), Tooltip("Rate to increase power-up fill")]
    private float fillIncreaseRate = .5f;

    [SerializeField, Range(0, 1), Tooltip("Rate to decrease power-up fill")]
    private float fillDecreaseRate = .5f;

    /// <summary>
    /// The order of this list works hand-in-hand with the index numbers assigned publicly to each 
    /// purchase-ball-button in the inspector. Any reshuffle might unlock the wrong ball saved in
    /// the player-preferences.
    /// It is better to look at the purchase-ball-buttons(located somewhere in the UI) or ballSwitcher Script 
    /// in-accordance with this particular list before modifying it.
    ///</summary>
    [Space(10), SerializeField, Tooltip("List of player items to be unlocked")]
    private GameObject[] availablePlayerItems;

    /// <summary>
    /// Broken balls to be spawned, depending on the ball being used.
    /// This list works exactly like the <see cref="availablePlayerItems"/> list
    /// The order and index in which the balls are stored in the list matters
    /// </summary>
    [SerializeField] private GameObject[] brokenPlayerItems;

    [Space(10), SerializeField] private Regenerator regenerator;

    // Index of balls assigned in the inspector, initially set to 0(for the first free-ball)
    private static int AvailablePlayerIndex => SaveSystem.GetData<int>("PlayerItem");

    private void Awake()
    {
        _stackController = gameObject.GetComponent<IStackController>();

        _playerState = gameObject.GetComponent<IPlayerState>();

        _playerProperties = GetComponent<PlayerProperties>();

        availablePlayerItems[AvailablePlayerIndex].ToggleActive(true);
    }


    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;

        regenerator.OnUsePowerup += Regenerator_OnUsePowerup;

        _playerState.OnHitBadStack += PlayerState_OnHitBadStack;
        _playerState.OnHitGoodStack += PlayerState_OnHitGoodStack;
        _playerState.OnHitCheckpoint += PlayerState_OnHitCheckpoint;
    }

    private void GameManager_OnCurrentState(GameState currentState)
    {
        if (currentState == GameState.GameOver)
            SpawnBrokenPlayerPrefab();
    }

    private void Update()
    {
        _stackController.StartBounce(InputController.HasClicked);

        // As the player is clicking, and the number of destroyed stacks is greater than a certain amount,
        // The mana-bar gradually increases with time, otherwise decreases simultaneously.
        // The mana-bar gets full as the number of destroyed stacks keep increasing overtime without delay
        if (InputController.HasClicked)
        {
            if (_platformCount > maxPlatformCount)
                regenerator.ModifyMana((fillIncreaseRate * 100) * Time.deltaTime);
        }
        else if (!_isOnPowerup)
        {
            // Decreases when the player isn't clicking/tapping or destroying stacks
            _platformCount -= (platformCountDecreaseRate * 100) * Time.deltaTime;

            regenerator.ModifyMana(-(fillDecreaseRate * 100) * Time.deltaTime);
        }

        _platformCount = Mathf.Max(0, _platformCount);
    }

    private void Regenerator_OnUsePowerup(bool isOnPowerup)
    {
        _isOnPowerup = isOnPowerup;

        _playerProperties.OnPlayerPowerUp(_isOnPowerup);
    }

    // Freely destroys the bad stacks while on power up, otherwise gameover
    private void PlayerState_OnHitBadStack(GameObject stack)
    {
        if (_isOnPowerup)
        {
            DestroyStack(stack);
            _platformCount = 0;
        }
        else
        {
            Haptics.Vibrate(100);
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
    }

    // Destroys the good stacks, while not on powerup
    private void PlayerState_OnHitGoodStack(GameObject stack)
    {
        DestroyStack(stack);

        if (!_isOnPowerup)
            _platformCount++;
    }

    /// <summary>
    /// When the player has reached the final checkpoint,
    /// set game over to true, to suspend all running scripts,
    /// NB: This is different from the other game over(in the case of the player diving into a bad-stack)
    /// </summary>
    private void PlayerState_OnHitCheckpoint(GameObject obj)
    {
        GameManager.Instance.SetGameState(GameState.Completed);

        Haptics.Vibrate(100);

        _playerProperties.OnHitCheckpoint(obj);
    }

    private static void UpdateScore()
    {
        UIController.Instance.UpdateFill(-1);
        UIController.Instance.UpdateScore(1);
    }

    private void DestroyStack(GameObject stack)
    {
        var amount = stack.transform.parent.childCount;

        for (int i = 0; i < amount; i++)
        {
            if (stack.transform.parent.GetChild(i).TryGetComponent(out _childStack))
            {
                _childStack.ApplyExpForce(stack.transform);
            }
        }
        // Score Updates on every stack destruction
        // (with or without power-up), as long as game is not over.
        UpdateScore();
    }

    /// <summary>
    /// Disable current player-item in use,
    /// Spawns a broken clone of the particular player-item being used,
    /// Enable the broken player counterpart of the current player.
    /// </summary>
    private void SpawnBrokenPlayerPrefab()
    {
        availablePlayerItems[AvailablePlayerIndex].ToggleActive(false);

        brokenPlayerItems[AvailablePlayerIndex].ToggleActive(true);

        Invoke(nameof(DisableBrokenPlayer), 2f);

        _playerProperties.OnPlayerLost();
    }

    private void DisableBrokenPlayer()
    {
        brokenPlayerItems[AvailablePlayerIndex].ToggleActive(true);
    }

    private void OnDisable()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;

        regenerator.OnUsePowerup -= Regenerator_OnUsePowerup;

        _playerState.OnHitBadStack -= PlayerState_OnHitBadStack;
        _playerState.OnHitGoodStack -= PlayerState_OnHitGoodStack;
        _playerState.OnHitCheckpoint -= PlayerState_OnHitCheckpoint;
    }
}
