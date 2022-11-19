using Racer.SaveSystem;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Racer.Utilities.SingletonPattern;

/// <summary>
/// Handles various UI states.
/// For saving, See also: <seealso cref="SaveSystem"/>
/// </summary>
internal class UIController : Singleton<UIController>
{
    private int _scoreCount;
    private int _highscore;
    private int _scoreCounter;
    private int _postLevelCount = 1;

    // Player's actual level.
    public int PreLevelCount { get; private set; }

    private float _changeInAmount;
    private float _initialAmount;
    private float _currentAmount;

    [Header("TEXTS")]
    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    [SerializeField]
    private TextMeshProUGUI
        preLevelText,
        postLevelText,
        highscoreText;

    [Header("MISC"), Space(5)]
    [SerializeField] private Image levelImage;

    [SerializeField] private Fader wonFader;
    [SerializeField] private Fader loseFader;
    [SerializeField] private Canvas shopCanvas;


    protected override void Awake()
    {
        base.Awake();

        _scoreCount = SaveSystem.GetData<int>("Score");
        _highscore = SaveSystem.GetData<int>("Highscore");
        PreLevelCount = SaveSystem.GetData<int>("Level01");
        _postLevelCount = SaveSystem.GetData("Level02", 1);
    }

    private void Start()
    {
        GameManager.OnCurrentState += GameManager_OnCurrentState;

        InitProgress();
    }

    private void GameManager_OnCurrentState(GameState currentState)
    {
        switch (currentState)
        {
            case GameState.GameOver:
                Gameover();
                break;

            case GameState.Completed:
                SaveProgressOnWin();
                wonFader.FadeIn();
                break;
        }
    }

    private void InitProgress()
    {
        _initialAmount = StackGenerator.Instance.GeneratedStackAmount;

        _currentAmount = _initialAmount;

        scoreTexts[0].SetText("{0}", _scoreCount);
        highscoreText.SetText("Best: {0}", _highscore);
        postLevelText.SetText("{0}", _postLevelCount);
        preLevelText.SetText("{0}", PreLevelCount);
    }

    public void UpdateFill(float amount)
    {
        // Increase the level-image fill amount from (0-1)
        // based on the number of generated stacks at initial.
        _currentAmount += (int)amount;

        // Decreases from (1 - 0); 100 / 100 = 1, 100 / 101 = .9, etc
        _changeInAmount = _currentAmount / _initialAmount;

        // Increases from (0 - 1); 1 - 1 = 0, 1 - .9 = .1, etc
        levelImage.fillAmount = 1 - _changeInAmount;
    }

    public void UpdateScore(int amount)
    {
        // overall current score.
        _scoreCount += amount;

        // current score for each session.
        _scoreCounter += amount;

        scoreTexts[0].SetText("{0}", _scoreCount);
    }

    private void Gameover()
    {
        // overall current score resets when game is over,
        // (in the case of the player diving into a bad stack)
        // We save before resetting the values.
        scoreTexts[1].SetText("Score: {0}", _scoreCount);

        SaveHighscoreOnGameover();

        _scoreCount = 0;

        loseFader.FadeIn();

        SaveSystem.SaveData("Score", _scoreCount);
    }

    private void SaveProgressOnWin()
    {
        if (_scoreCounter == (int)_initialAmount)
        {
            PreLevelCount = _postLevelCount++;

            postLevelText.SetText("{0}", _postLevelCount);
            preLevelText.SetText("{0}", PreLevelCount);

            SaveSystem.SaveData("Level01", PreLevelCount);
            SaveSystem.SaveData("Level02", _postLevelCount);
        }

        SaveSystem.SaveData("Score", _scoreCount);
    }

    private void SaveHighscoreOnGameover()
    {
        if (_scoreCount >= _highscore)
        {
            _highscore = _scoreCount;

            highscoreText.SetText("Best: {0}", _highscore);

            SaveSystem.SaveData("Highscore", _highscore);
        }
    }

    // Assigned to 'play' text in UI
    public void StartGame()
    {
        GameManager.Instance.SetGameState(GameState.Playing);

        shopCanvas.enabled = false;
    }

    // Assigned to 'reload' icon in UI
    public void ReloadGame() => SceneManager.LoadScene(0);

    private void OnDisable()
    {
        GameManager.OnCurrentState -= GameManager_OnCurrentState;
    }
}