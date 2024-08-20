using UnityEngine;

public class Snake : MonoBehaviour
{
    public GameField GameField;
    public GameFieldObject HeadPrefab;
    public GameFieldObject BodyPrefab;
    public Vector2Int StartCellId = new Vector2Int(5, 5);
    public float MoveDelay = 1.3f;
    private GameFieldObject[] _parts;
    private Vector2Int _moveDirection = Vector2Int.up;
    private float _moveTimer;
    public GameStateChanger GameStateChanger;
    public AppleSpawner AppleSpawner;
    private bool _isActive;
    public Score Score;
    public AppleSpawner BonusAppleSpawner;
    public float ExplosionForce = 60;

    public void CreateSnake()
    {
        _parts = new GameFieldObject[0];
        AddPart(HeadPrefab, StartCellId);
        AddPart(BodyPrefab, StartCellId + Vector2Int.down);
    }

    private void AddPart(GameFieldObject partPrefab, Vector2Int cellId)
    {
        ChangePartsArrayLenght(1);
        GameFieldObject newSnakePart = Instantiate(partPrefab);
        _parts[_parts.Length - 1] = newSnakePart;
        GameField.SetObjectCell(newSnakePart, cellId);
    }

    private void ChangePartsArrayLenght(int count)
    {
        GameFieldObject[] tempParts = _parts;

        _parts = new GameFieldObject[tempParts.Length + count];
        
        for (int i = 0; i < _parts.Length; i++)
        {
            if (i >= tempParts.Length)
            {
                break;
            }
            _parts[i] = tempParts[i];
        }
    }

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        GetMoveDirection();
        MoveTimerTick();
    }

    private void GetMoveDirection()
    {
        // Если пользователь нажал на клавишу W или стрелку вверх и текущее направление не вниз
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && _moveDirection != Vector2Int.down)
        {
            // Устанавливаем направление движения вверх
            SetMoveDirection(Vector2Int.up);
        }
        // Если пользователь нажал на клавишу A или стрелку влево и текущее направление — не вправо
        else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && _moveDirection != Vector2Int.right)
        {
            // Устанавливаем направление движения влево
            SetMoveDirection(Vector2Int.left);
        }
        // Если пользователь нажал на клавишу S или стрелку вниз и текущее направление не вверх
        else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && _moveDirection != Vector2Int.up)
        {
            // Устанавливаем направление движения вниз
            SetMoveDirection(Vector2Int.down);
        }
        // Если пользователь нажал на клавишу D или стрелку вправо и текущее направление — не влево
        else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && _moveDirection != Vector2Int.left)
        {
            // Устанавливаем направление движения вправо
            SetMoveDirection(Vector2Int.right);
        }
    }

    private void SetMoveDirection(Vector2Int moveDirection)
    {
        // Устанавливаем новое направление движения
        _moveDirection = moveDirection;

        // Поворачиваем голову змейки по новому направлению
        SetHeadRotation(moveDirection);

        // Двигаем змейку по карте
        Move();
    }

    private void MoveTimerTick()
    {
        // Увеличиваем значение таймера на время, которое прошло с последнего кадра
        _moveTimer += Time.deltaTime;

        // Если значение таймера достигло значения задержки
        if (_moveTimer >= MoveDelay)
        {
            // Двигаем змейку по карте
            Move();
        }
    }

    private void Move()
    {
        _moveTimer = 0;

        // Получаем ячейку последней части змейки
        Vector2Int lastPartCellId = _parts[_parts.Length - 1].GetCellId();

        // Получаем новую ячейку для головы змейки в зависимости от текущего направления
        Vector2Int headNewCell = MoveCellId(_parts[0].GetCellId(), _moveDirection);

        // Освобождаем ячейку последней части змейки на игровом поле
        GameField.SetCellIsEmpty(lastPartCellId.x, lastPartCellId.y, true);

        for (int i = _parts.Length - 1; i >= 0; i--)
        {
            Vector2Int partCellId = _parts[i].GetCellId();

            if (i == 0)
            {
                // Задаём новую ячейку для головы змейки
                partCellId = headNewCell;
            }
            else
            {
                partCellId = _parts[i - 1].GetCellId();
            }
            // Устанавливаем текущую часть змейки в новую ячейку на игровом поле
            GameField.SetObjectCell(_parts[i], partCellId);
        }
        // Проверяем, есть ли в следующей ячейке столкновение
        CheckNextCellFail(headNewCell);

        // Проверяем, есть ли в следующей ячейке яблоко
        CheckNextCellApple(headNewCell, lastPartCellId);
    }

    private Vector2Int MoveCellId(Vector2Int cellId, Vector2Int direction)
    {
        // Увеличиваем значение cellId на значения direction, чтобы получить новую позицию клетки змейки
        cellId += direction;

        // Если новая позиция по оси x больше или равна количеству клеток в ряду игрового поля
        if (cellId.x >= GameField.CellsInRow)
        {
            // Обнуляем x, чтобы змейка переместилась на начало ряда
            cellId.x = 0;
        }
        // Иначе, если новая позиция по оси x меньше 0
        else if (cellId.x < 0)
        {
            // Делаем x равным количеству клеток в ряду игрового поля минус 1, чтобы змейка переместилась в конец ряда
            cellId.x = GameField.CellsInRow - 1;
        }
        // Если новая позиция по оси y больше или равна количеству клеток в ряду игрового поля
        if (cellId.y >= GameField.CellsInRow)
        {
            // Обнуляем y, чтобы змейка переместилась на начало ряда
            cellId.y = 0;
        }
        // Иначе, если новая позиция по оси y меньше 0
        else if (cellId.y < 0)
        {
            // Делаем y равным количеству клеток в ряду игрового поля минус 1, чтобы змейка переместилась в конец ряда
            cellId.y = GameField.CellsInRow - 1;
        }
        // Возвращаем новую позицию
        return cellId;
    }

    private void SetHeadRotation(Vector2Int direction)
    {
        // Создаём новый вектор, который будет использоваться для поворота головы змейки
        Vector3 headEuler = Vector3.zero;

        // Если направление движения по оси x равно 0 и направление движения по оси y равно 1
        if (direction.x == 0 && direction.y == 1)
        {
            // Делаем вектор равным (0, 0, 0), чтобы голова змейки была направлена вверх
            headEuler = new Vector3(0f, 0f, 0f);
        }
        // Иначе, если направление движения по оси x равно 1 и направление движения по оси y равно 0
        else if (direction.x == 1 && direction.y == 0)
        {
            // Делаем вектор равным (0, 0, -90), чтобы голова змейки была направлена вправо
            headEuler = new Vector3(0f, 0f, -90f);
        }
        // Иначе, если направление движения по оси x равно 0 и направление движения по оси y равно -1
        else if (direction.x == 0 && direction.y == -1)
        {
            // Делаем вектор равным (0, 0, 180), чтобы голова змейки была направлена вниз
            headEuler = new Vector3(0f, 0f, 180f);
        }
        // Иначе, если направление движения по оси x равно -1 и направление движения по оси y равно 0
        else if (direction.x == -1 && direction.y == 0)
        {
            // Делаем вектор равным (0, 0, 90), чтобы голова змейки была направлена влево
            headEuler = new Vector3(0f, 0f, 90f);
        }
        // Приравниваем углы поворота головы змейки к вектору
        _parts[0].transform.eulerAngles = headEuler;
    }

    public void StartGame()
    {
        // Создаём змейку
        CreateSnake();

        // Устанавливаем флаг активности
        _isActive = true;
    }

    public void StopGame()
    {
        // Снимаем флаг активности
        _isActive = false;
    }

    public int GetSnakePartsLength()
    {
        // Возвращаем длину змейки
        return _parts.Length;
    }

    private void CheckNextCellFail(Vector2Int nextCellId)
    {
        // Проходим по частям змейки
        for (int i = 1; i < _parts.Length; i++)
        {
            // Если индекс (положение) проверяемой в цикле части змейки совпадает с проверяемым индексом (положением) ячейки
            // То есть если змейка заползает на саму себя
            if (_parts[i].GetCellId() == nextCellId)
            {
                // Завершаем игру
                GameStateChanger.EndGame();

                // НОВОЕ: Получаем позицию этой части змейки на игровом поле
                Vector2 partPosition = GameField.GetCellPosition(_parts[i].GetCellId());

                // НОВОЕ: Взрываем змейку на позиции этой части
                ExplodeSnake(partPosition);
            }
        }
    }

    private void CheckNextCellApple(Vector2Int nextCellId, Vector2Int cellIdForAddPart)
    {
        if (AppleSpawner.GetAppleCellId() == nextCellId)
        {
            AddPart(BodyPrefab, cellIdForAddPart);
            AppleSpawner.SetNextApple();

            // НОВОЕ: Устанавливаем позицию для бонуса
            BonusAppleSpawner.SetNextApple();

            // Добавляем очко за яблоко
            Score.AddScore(1);
        }
        // НОВОЕ: Иначе, если координаты ячейки с бонусом совпадают со следующей ячейкой, куда придёт змейка
        else if (BonusAppleSpawner.GetAppleCellId() == nextCellId)
        {
            // НОВОЕ: Задаём количество частей змейки, которые нужно удалить (вы можете позже указать здесь своё значение)
            int countToRemove = 2;

            // НОВОЕ: Проходим по этим частям
            for (int i = 0; i < countToRemove; i++)
            {
                // НОВОЕ: Удаляем последнюю часть змейки
                RemovePart();
            }
            // НОВОЕ: Скрываем бонусное яблоко
            BonusAppleSpawner.HideApple();
        }
    }

    public void RestartGame()
    {
        // Удаляем части змейки
        DestroySnake();

        // Начинаем игру заново
        StartGame();
    }

    private void DestroySnake()
    {
        // Проходим по частям змейки
        for (int i = 0; i < _parts.Length; i++)
        {
            // Стираем каждую часть
            Destroy(_parts[i].gameObject);
        }
    }

    private void RemovePart()
    {
        // Удаляем объект последней части змейки
        Destroy(_parts[_parts.Length - 1].gameObject);

        // Уменьшаем длину массива частей змейки на 1
        ChangePartsArrayLenght(-1);
    }

    private void ExplodeSnake(Vector2 explodePosition)
    {
        // Проходим по всем частям змейки
        for (int i = 0; i < _parts.Length; i++)
        {
            // Получаем компонент Rigidbody2D этой части змейки
            Rigidbody2D partRigid = _parts[i].GetComponent<Rigidbody2D>();

            // Включаем для неё симуляцию физики
            partRigid.simulated = true;

            // Получаем её позицию на игровом поле
            Vector2 partPosition = GameField.GetCellPosition(_parts[i].GetCellId());

            // Вычисляем направление от этой части змейки до заданной позиции взрыва
            Vector2 explodeDirection = partPosition - explodePosition;

            // Придаём силу этой части змейки, чтобы она отлетела от позиции взрыва
            // Сила направлена к ней и слегка вверх. Она увеличивается с помощью ExplosionForce
            partRigid.AddForce((explodeDirection.normalized + Vector2.up) * ExplosionForce, ForceMode2D.Impulse);
        }
    }
}
