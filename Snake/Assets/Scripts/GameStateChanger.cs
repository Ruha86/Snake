using TMPro;
using UnityEngine;

public class GameStateChanger : MonoBehaviour
{
    // Скрипт игрового поля
    public GameField GameField;

    // Скрипт движения змейки
    public Snake Snake;

    // Массив из обычного и бонусного яблок
    public AppleSpawner[] AppleSpawners;

    // Скрипт ведения счёта
    public Score Score;

    // Экран игры
    public GameObject GameScreen;

    // Экран конца игры
    public GameObject GameEndScreen;

    // Надпись о конце игры
    public TextMeshProUGUI GameEndScoreText;

    // Надпись о рекорде игрока
    public TextMeshProUGUI BestScoreText;

    // Флаг состояния игры (начата или нет)
    private bool _isGameStarted;

    private void Start()
    {
        // Вызываем метод FirstStartGame() при запуске игры
        FirstStartGame();
    }

    private void FirstStartGame()
    {
        // Вызываем метод FillCellsPositions() из скрипта GameField, чтобы заполнить позиции ячеек
        GameField.FillCellsPositions();

        // Вызываем метод CreateSnake() из скрипта Snake, чтобы создать змейку
        StartGame();
    }

    public void StartGame()
    {
        // НОВОЕ: Устанавливаем флаг начала игры
        _isGameStarted = true;

        Snake.StartGame();

        // НОВОЕ: Проходим по всем объектам AppleSpawner в массиве
        for (int i = 0; i < AppleSpawners.Length; i++)
        {
            // НОВОЕ: Создаём яблоко в каждом объекте AppleSpawner
            AppleSpawners[i].CreateApple();
        }
        SwitchScreens(true);
    }

    public void EndGame()
    {
        // НОВОЕ: Если игра не начата
        if (!_isGameStarted)
        {
            // НОВОЕ: Выходим из метода
            return;
        }
        // НОВОЕ: Устанавливаем флаг конца игры
        _isGameStarted = false;

        Snake.StopGame();
        RefreshScores();
        SwitchScreens(false);
    }

    public void RestartGame()
    {
        // НОВОЕ: Устанавливаем флаг начала игры
        _isGameStarted = true;

        Snake.RestartGame();

        // НОВОЕ: Проходим по всем объектам AppleSpawner в массиве
        for (int i = 0; i < AppleSpawners.Length; i++)
        {
            // НОВОЕ: Перезапускаем каждый объект AppleSpawner
            AppleSpawners[i].Restart();
        }
        Score.Restart();
        SwitchScreens(true);
    }

    private void SwitchScreens(bool isGame)
    {
        // Активируем экран игры
        GameScreen.SetActive(isGame);

        // Скрываем экран завершения игры
        GameEndScreen.SetActive(!isGame);
    }

    private void RefreshScores()
    {
        // Получаем текущий счёт
        int score = Score.GetScore();

        // Получаем прежний лучший счёт
        int oldBestScore = Score.GetBestScore();

        // Проверяем, побил ли игрок рекорд
        bool isNewBestScore = CheckNewBestScore(score, oldBestScore);

        // В зависимости от результата показываем или скрываем текст про новый рекорд
        SetActiveGameEndScoreText(!isNewBestScore);

        // Если игрок побил рекорд
        if (isNewBestScore)
        {
            // Устанавливаем новый лучший счёт
            Score.SetBestScore(score);

            // Задаём текст о новом рекорде
            SetNewBestScoreText(score);
        }
        // Иначе
        else
        {
            // Устанавливаем текст о текущем счёте
            SetGameEndScoreText(score);

            // Задаём текст о прежнем рекорде
            SetOldBestScoreText(oldBestScore);
        }
    }

    private bool CheckNewBestScore(int score, int oldBestScore)
    {
        // Возвращаем результат проверки того, что текущий счёт выше лучшего (true или false)
        return score > oldBestScore;
    }

    private void SetGameEndScoreText(int value)
    {
        // Обновляем надпись конца игры
        GameEndScoreText.text = $"Количество очков: {value}";
    }

    private void SetOldBestScoreText(int value)
    {
        // Обновляем надпись лучшего счёта
        BestScoreText.text = $"Лучший результат: {value}";
    }

    private void SetNewBestScoreText(int value)
    {
        // Обновляем надпись нового рекорда
        BestScoreText.text = $"Новый рекорд: {value}!";
    }

    private void SetActiveGameEndScoreText(bool value)
    {
        // Устанавливаем активность текстового поля счёта в конце игры в зависимости от значения value
        GameEndScoreText.gameObject.SetActive(value);
    }
}
