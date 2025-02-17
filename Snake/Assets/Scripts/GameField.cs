using UnityEngine;

public class GameField : MonoBehaviour
{
    // ������� ������ ������
    public Transform FirstCellPoint;

    // ������ ������ (�� X � Y)
    public Vector2 CellSize;

    // ���������� ����� � ����� ����
    public int CellsInRow = 12;

    // ��������� ������ �� ������� ������ ������
    private GameFieldCell[,] _cells;

    public void FillCellsPositions()
    {
        // �����: ������ ������ ����� ������������ CellsInRow x CellsInRow
        _cells = new GameFieldCell[CellsInRow, CellsInRow];

        for (int i = 0; i < CellsInRow; i++)
        {
            for (int j = 0; j < CellsInRow; j++)
            {
                // �����: ��������� ������� ������� ������
                Vector2 cellPosition = (Vector2)FirstCellPoint.position + Vector2.right * i * CellSize.x + Vector2.up * j * CellSize.y;

                // �����: ������ ����� ������
                GameFieldCell newCell = new GameFieldCell(cellPosition);

                // �����: ���������� ��� ������ � ������ _cells
                _cells[i, j] = newCell;
            }
        }
    }

    public Vector2 GetCellPosition(int x, int y)
    {
        // �����: �������� ������ �� �������� �����������
        GameFieldCell cell = GetCell(x, y);

        // �����: ���� ������ �� ���� �������, ���������� (0, 0)
        if (cell == null)
        {
            return Vector2.zero;
        }
        // �����: ���������� ������� ��������� ������
        return _cells[x, y].GetPosition();
    }

    public void SetObjectCell(GameFieldObject obj, Vector2Int newCellId)
    {
        // �������� ������� ������ �� �������� �����������
        Vector2 cellPosition = GetCellPosition(newCellId.x, newCellId.y);

        // ������������� ������ �� ��������� ������
        obj.SetCellPosition(newCellId, cellPosition);

        // ����� �������� ��������� ������
        SetCellIsEmpty(newCellId.x, newCellId.y, false);
    }

    public bool GetCellIsEmpty(int x, int y)
    {
        // �������� ������ �� ��������� �����������
        GameFieldCell cell = GetCell(x, y);

        // ���� ������ �� ���� �������
        if (cell == null)
        {
            // ���������� false
            return false;
        }
        // ���������� �������� ��������� ��������� ������
        return cell.GetIsEmpty();
    }

    public void SetCellIsEmpty(int x, int y, bool value)
    {
        // �������� ������ �� ��������� �����������
        GameFieldCell cell = GetCell(x, y);

        // ���� ������ �� ���� �������
        if (cell == null)
        {
            // ������� �� ������
            return;
        }
        // ������������� �������� ��������� ������
        _cells[x, y].SetIsEmpty(value);
    }

    private GameFieldCell GetCell(int x, int y)
    {
        // ���� ���������� ������� �� ������� �������� ����
        if (x < 0 || y < 0 || x >= CellsInRow || y >= CellsInRow)
        {
            // ���������� null
            return null;
        }
        // ���������� ������ � ��������� ������������
        return _cells[x, y];
    }

    public Vector2 GetCellPosition(Vector2Int cellId)
    {
        // ���������� ������� ��������� ������
        return GetCellPosition(cellId.x, cellId.y);
    }
}
