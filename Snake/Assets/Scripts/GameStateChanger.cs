using TMPro;
using UnityEngine;

public class GameStateChanger : MonoBehaviour
{
    // ������ �������� ����
    public GameField GameField;

    // ������ �������� ������
    public Snake Snake;

    // ������ �� �������� � ��������� �����
    public AppleSpawner[] AppleSpawners;

    // ������ ������� �����
    public Score Score;

    // ����� ����
    public GameObject GameScreen;

    // ����� ����� ����
    public GameObject GameEndScreen;

    // ������� � ����� ����
    public TextMeshProUGUI GameEndScoreText;

    // ������� � ������� ������
    public TextMeshProUGUI BestScoreText;

    // ���� ��������� ���� (������ ��� ���)
    private bool _isGameStarted;

    private void Start()
    {
        // �������� ����� FirstStartGame() ��� ������� ����
        FirstStartGame();
    }

    private void FirstStartGame()
    {
        // �������� ����� FillCellsPositions() �� ������� GameField, ����� ��������� ������� �����
        GameField.FillCellsPositions();

        // �������� ����� CreateSnake() �� ������� Snake, ����� ������� ������
        StartGame();
    }

    public void StartGame()
    {
        // �����: ������������� ���� ������ ����
        _isGameStarted = true;

        Snake.StartGame();

        // �����: �������� �� ���� �������� AppleSpawner � �������
        for (int i = 0; i < AppleSpawners.Length; i++)
        {
            // �����: ������ ������ � ������ ������� AppleSpawner
            AppleSpawners[i].CreateApple();
        }
        SwitchScreens(true);
    }

    public void EndGame()
    {
        // �����: ���� ���� �� ������
        if (!_isGameStarted)
        {
            // �����: ������� �� ������
            return;
        }
        // �����: ������������� ���� ����� ����
        _isGameStarted = false;

        Snake.StopGame();
        RefreshScores();
        SwitchScreens(false);
    }

    public void RestartGame()
    {
        // �����: ������������� ���� ������ ����
        _isGameStarted = true;

        Snake.RestartGame();

        // �����: �������� �� ���� �������� AppleSpawner � �������
        for (int i = 0; i < AppleSpawners.Length; i++)
        {
            // �����: ������������� ������ ������ AppleSpawner
            AppleSpawners[i].Restart();
        }
        Score.Restart();
        SwitchScreens(true);
    }

    private void SwitchScreens(bool isGame)
    {
        // ���������� ����� ����
        GameScreen.SetActive(isGame);

        // �������� ����� ���������� ����
        GameEndScreen.SetActive(!isGame);
    }

    private void RefreshScores()
    {
        // �������� ������� ����
        int score = Score.GetScore();

        // �������� ������� ������ ����
        int oldBestScore = Score.GetBestScore();

        // ���������, ����� �� ����� ������
        bool isNewBestScore = CheckNewBestScore(score, oldBestScore);

        // � ����������� �� ���������� ���������� ��� �������� ����� ��� ����� ������
        SetActiveGameEndScoreText(!isNewBestScore);

        // ���� ����� ����� ������
        if (isNewBestScore)
        {
            // ������������� ����� ������ ����
            Score.SetBestScore(score);

            // ����� ����� � ����� �������
            SetNewBestScoreText(score);
        }
        // �����
        else
        {
            // ������������� ����� � ������� �����
            SetGameEndScoreText(score);

            // ����� ����� � ������� �������
            SetOldBestScoreText(oldBestScore);
        }
    }

    private bool CheckNewBestScore(int score, int oldBestScore)
    {
        // ���������� ��������� �������� ����, ��� ������� ���� ���� ������� (true ��� false)
        return score > oldBestScore;
    }

    private void SetGameEndScoreText(int value)
    {
        // ��������� ������� ����� ����
        GameEndScoreText.text = $"���������� �����: {value}";
    }

    private void SetOldBestScoreText(int value)
    {
        // ��������� ������� ������� �����
        BestScoreText.text = $"������ ���������: {value}";
    }

    private void SetNewBestScoreText(int value)
    {
        // ��������� ������� ������ �������
        BestScoreText.text = $"����� ������: {value}!";
    }

    private void SetActiveGameEndScoreText(bool value)
    {
        // ������������� ���������� ���������� ���� ����� � ����� ���� � ����������� �� �������� value
        GameEndScoreText.gameObject.SetActive(value);
    }
}
