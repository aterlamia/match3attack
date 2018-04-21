using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayBoard : MonoBehaviour
{
    public Sprite[] Pieces;

    public GameObject BasePiece;

    public int BoardWidth;

    public int BoardHeight;

    private int[,] _board;

    private int[,] _shiftBoard;

    public Text Defence;
    public Text Ammo;
    public Text Money;
    public Text Oil;
    public Text Health;
    public Text Turns;
    public Text Bricks;

    private int _defence = 0;
    private int _ammo = 0;
    private int _money = 0;
    private int _oil = 0;
    private int _health = 100;
    private int _turns = 5;
    private int _bricks = 0;

    public Resource[,] BoardObjects { get; private set; }

    public int[,] Board
    {
        get { return _board; }
        set { _board = value; }
    }

    public int[,] ShiftBoard
    {
        get { return _shiftBoard; }
        set { _shiftBoard = value; }
    }

    public string Seed;

    // Use this for initialization
    void Start()
    {
        UpdateUi();
        Board = new int[BoardWidth, BoardHeight];
        ShiftBoard = new int[BoardWidth, BoardHeight];
        BoardObjects = new Resource[BoardWidth, BoardHeight];
        // create intial board
        CreateBoard();
        // check for initial 3's
        CheckBoard();
        RenderBoard();
        ResetShiftBoard();
    }

    private void UpdateUi()
    {
        Defence.text = _defence.ToString();
        Ammo.text = _ammo.ToString();
        Money.text = _money.ToString();
        Oil.text = _oil.ToString();
        Health.text = _health.ToString();
        Defence.text = _defence.ToString();
        Turns.text = _turns.ToString();
        Bricks.text = _bricks.ToString();
    }

    private void CheckBoard()
    {
        var found = true;
        var counter = 0;
        while (found)
        {
            found = false;
            counter++;
            for (var x = 0; x < BoardWidth; x++)
            {
                for (var y = 0; y < BoardHeight - 3; y++)
                {
                    if (Board[x, y] == Board[x, y + 1] && Board[x, y + 1] == Board[x, y + 2])
                    {
                        Board[x, y + 1] = Random.Range(0, Pieces.Length);
                        found = true;
                    }
                }
            }


            for (var y = 0; y < BoardHeight; y++)
            {
                for (var x = 0; x < BoardWidth - 3; x++)
                {
                    if (Board[x, y] == Board[x + 1, y] && Board[x + 1, y] == Board[x + 2, y])
                    {
                        Board[x + 1, y] = Random.Range(0, Pieces.Length);
                        found = true;
                    }
                }
            }

            if (found)
            {
                Debug.Log("Duplicates removed");
            }

            if (counter == 10)
            {
                found = false;
                Debug.Log("Emergency break");
            }
        }
    }

    public void ApplyShiftBoard()
    {
        Debug.Log("Setting shiftboard to holder board");
        _board = (int[,]) _shiftBoard.Clone();
        for (var x = 0; x < BoardWidth; x++)
        {
            for (var y = 0; y < BoardHeight; y++)
            {
                BoardObjects[x, y].Enable();
            }
        }
    }

    public void ResetShiftBoard()
    {
        Debug.Log("Setting holder board to board");
        ShiftBoard = (int[,]) Board.Clone();
        for (var x = 0; x < BoardWidth; x++)
        {
            for (var y = 0; y < BoardHeight; y++)
            {
                BoardObjects[x, y].Enable();
            }
        }
    }

    public void ShowShiftBoard()
    {
//        Debug.Log("Updating view with shiftboard");
        for (var x = 0; x < BoardWidth; x++)
        {
            for (var y = 0; y < BoardHeight; y++)
            {
                var tileObj = BoardObjects[x, y];
                tileObj.SetSprite(Pieces[ShiftBoard[x, y]]);
                tileObj.Type = ShiftBoard[x, y];
            }
        }
    }

    public void ShiftHorizontally(int sourceX, int sourceY, int targetX, int targetY)
    {
        // Prepare row.
        for (var x = 0; x < BoardWidth; x++)
        {
            BoardObjects[x, sourceY].Enable();
            ShiftBoard[x, sourceY] = Board[x, sourceY];
        }

        // If not on same row return
        if (targetY != sourceY)
        {
            return;
        }

        if (targetX > sourceX && targetX < BoardWidth)
        {
            ShiftBoard[targetX, sourceY] = Board[sourceX, sourceY];

            for (var x = sourceX; x < targetX; x++)
            {
                ShiftBoard[x, sourceY] = Board[x + 1, sourceY];
            }

            BoardObjects[targetX, sourceY].Disable();
            ShowShiftBoard();
        }
        else if (targetX <= sourceX && targetX >= 0)
        {
            ShiftBoard[targetX, sourceY] = Board[sourceX, sourceY];

            for (var x = targetX; x < sourceX; x++)
            {
                ShiftBoard[x + 1, sourceY] = Board[x, sourceY];
                BoardObjects[x, sourceY].Enable();
            }

            BoardObjects[targetX, sourceY].Disable();
            ShowShiftBoard();
        }
    }

    public void ShiftVertically(int sourceX, int sourceY, int targetX, int targetY)
    {
        // Prepare row.
        for (var y = 0; y < BoardHeight; y++)
        {
            BoardObjects[sourceX, y].Enable();
            ShiftBoard[sourceX, y] = Board[sourceX, y];
        }


        // If not on same row return
        if (targetX != sourceX)
        {
            return;
        }

        if (targetY > sourceY && targetY < BoardHeight)
        {
            ShiftBoard[sourceX, targetY] = Board[sourceX, sourceY];

            for (var y = sourceY; y < targetY; y++)
            {
                ShiftBoard[sourceX, y] = Board[sourceX, y + 1];
            }

            BoardObjects[sourceX, targetY].Disable();
            ShowShiftBoard();
        }
        else if (targetY <= sourceY && targetY >= 0)
        {
            ShiftBoard[sourceX, targetY] = Board[sourceX, sourceY];

            for (var y = targetY; y < sourceY; y++)
            {
                ShiftBoard[sourceX, y + 1] = Board[sourceX, y];
                BoardObjects[sourceX, y].Enable();
            }

            BoardObjects[sourceX, targetY].Disable();
            ShowShiftBoard();
        }
    }


    private void CreateBoard()
    {
        var pseudoRandom = new System.Random(Seed.GetHashCode());

        for (var x = 0; x < BoardWidth; x++)
        {
            for (var y = 0; y < BoardHeight; y++)
            {
                Board[x, y] = pseudoRandom.Next(0, Pieces.Length);
            }
        }
    }

    private void RenderBoard()
    {
        for (var x = 0; x < BoardWidth; x++)
        {
            for (var y = 0; y < BoardHeight; y++)
            {
                var pos = new Vector3(x, y, 0);

                var tileObj = Instantiate(BasePiece, pos, Quaternion.identity, transform) as GameObject;

                tileObj.name = string.Format("p: x-{0},y-{1}", x, y);
                tileObj.GetComponentInChildren<TextMesh>().text = Board[x, y].ToString();
                var spriteRenderer = tileObj.GetComponentInChildren<SpriteRenderer>();
                spriteRenderer.sprite = Pieces[Board[x, y]];

                BoardObjects[x, y] = new Resource(tileObj, Board[x, y]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ScoreButton()
    {
        LogGrids();
        Score();
    }

    public void LogGrids()
    {
        Debug.Log(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 7], _board[1, 7], _board[2, 7],
                      _board[3, 7], _board[4, 7], _board[5, 7], _board[6, 7], _board[7, 7]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 6], _board[1, 6], _board[2, 6],
                      _board[3, 6], _board[4, 6], _board[5, 6], _board[6, 6], _board[7, 6]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 5], _board[1, 5], _board[2, 5],
                      _board[3, 5], _board[4, 5], _board[5, 5], _board[6, 5], _board[7, 5]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 4], _board[1, 4], _board[2, 4],
                      _board[3, 4], _board[4, 4], _board[5, 4], _board[6, 4], _board[7, 4]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 3], _board[1, 3], _board[2, 3],
                      _board[3, 3], _board[4, 3], _board[5, 3], _board[6, 3], _board[7, 3]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 2], _board[1, 2], _board[2, 2],
                      _board[3, 2], _board[4, 2], _board[5, 2], _board[6, 2], _board[7, 3]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 1], _board[1, 1], _board[2, 1],
                      _board[3, 1], _board[4, 1], _board[5, 1], _board[6, 1], _board[7, 1]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _board[0, 0], _board[1, 0], _board[2, 0],
                      _board[3, 0], _board[4, 0], _board[5, 0], _board[6, 0], _board[7, 0]));

        Debug.Log(string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 7], _shiftBoard[1, 7],
                      _shiftBoard[2, 7], _shiftBoard[3, 7], _shiftBoard[4, 7], _shiftBoard[5, 7], _shiftBoard[6, 7],
                      _shiftBoard[7, 7]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 6], _shiftBoard[1, 6],
                      _shiftBoard[2, 6], _shiftBoard[3, 6], _shiftBoard[4, 6], _shiftBoard[5, 6], _shiftBoard[6, 6],
                      _shiftBoard[7, 6]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 5], _shiftBoard[1, 5],
                      _shiftBoard[2, 5], _shiftBoard[3, 5], _shiftBoard[4, 5], _shiftBoard[5, 5], _shiftBoard[6, 5],
                      _shiftBoard[7, 5]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 4], _shiftBoard[1, 4],
                      _shiftBoard[2, 4], _shiftBoard[3, 4], _shiftBoard[4, 4], _shiftBoard[5, 4], _shiftBoard[6, 4],
                      _shiftBoard[7, 4]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 3], _shiftBoard[1, 3],
                      _shiftBoard[2, 3], _shiftBoard[3, 3], _shiftBoard[4, 3], _shiftBoard[5, 3], _shiftBoard[6, 3],
                      _shiftBoard[7, 3]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 2], _shiftBoard[1, 2],
                      _shiftBoard[2, 2], _shiftBoard[3, 2], _shiftBoard[4, 2], _shiftBoard[5, 2], _shiftBoard[6, 2],
                      _shiftBoard[7, 3]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 1], _shiftBoard[1, 1],
                      _shiftBoard[2, 1], _shiftBoard[3, 1], _shiftBoard[4, 1], _shiftBoard[5, 1], _shiftBoard[6, 1],
                      _shiftBoard[7, 1]) +
                  string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\n", _shiftBoard[0, 0], _shiftBoard[1, 0],
                      _shiftBoard[2, 0], _shiftBoard[3, 0], _shiftBoard[4, 0], _shiftBoard[5, 0], _shiftBoard[6, 0],
                      _shiftBoard[7, 0]));
    }

    public int Score()
    {
        List<Vector2> score = new List<Vector2>();
        var currentType = -1;
        
        // For every row check if the column has 3
        for (var x = 0; x < BoardWidth; x++)
        {
            List<Vector2> columns = new List<Vector2>();
            for (var y = 0; y < BoardHeight; y++)
            {
                if (ShiftBoard[x, y] != currentType)
                {
                    AddScore(columns, score);
                    Debug.Log("2 has 3");
                    currentType = ShiftBoard[x, y];
                    columns.Clear();
                }

                columns.Add(new Vector2(x, y));
            }

            AddScore(columns, score);
            columns.Clear();
        }


        // For every column check if the row has 3
        for (var y = 0; y < BoardHeight; y++)
        {
            // For each row reset
            currentType = -1;

            List<Vector2> rows = new List<Vector2>();
            for (var x = 0; x < BoardWidth; x++)
            {
                if (ShiftBoard[x, y] != currentType)
                {
                    AddScore(rows, score);
                    Debug.Log("1 has 3");
                    currentType = ShiftBoard[x, y];
                    rows.Clear();
                }
                else

                    rows.Add(new Vector2(x, y));
            }

            // When at end we need to check if we have a leaast 3 rows
            AddScore(rows, score);
            rows.Clear();
        }

        int scoreCount = 0;
        foreach (var s in score)
        {
            BoardObjects[(int) s.x, (int) s.y].Disable();
            scoreCount += 10;
        }


        if (scoreCount > 0)
        {
            RefillBoard();
            //Make sure that new matches are scored as well
            scoreCount += Score();
        }

        return scoreCount;
    }

    private void AddScore(List<Vector2> rows, List<Vector2> score)
    {
        if (rows.Count >= 3)
        {
            Debug.Log("has 3");
            foreach (var row in rows)
            {
                Debug.Log(_shiftBoard[(int) row.x, (int) row.y]);
                switch (_shiftBoard[(int) row.x, (int) row.y])
                {
                    case 0:
                        _ammo += 10;
                        break;
                    case 1:
                        _health -= 10;
                        break;
                    case 2:
                        _bricks += 10;
                        break;
                    case 3:
                        _defence += 10;
                        break;
                    case 4:
                        _oil += 10;
                        break;
                    case 5:
                        _health += 10;
                        break;
                    case 6:
                        _money += 10;
                        break;
                    case 7:
                        _turns += 1;
                        break;
                }

                UpdateUi();
                score.Add(row);
            }
        }
    }

    public void RefillBoard()
    {
        for (var x = 0; x < BoardWidth; x++)
        {
            for (var y = 0; y < BoardHeight; y++)
            {
                if (BoardObjects[x, y].IsDisabled())
                {
                    Falldown(x, y, y);
                    BoardObjects[x, y].Enable();
                }
            }
        }


        // Make boards equal again
//        ApplyShiftBoard();
        ShowShiftBoard();
    }

    public void Falldown(int x, int y, int originalY)
    {
        if (y >= BoardHeight - 1)
        {
            ShiftBoard[x, originalY] = Random.Range(0, Pieces.Length);
            return;
        }

        if (BoardObjects[x, y + 1].IsDisabled())
        {
            Falldown(x, y + 1, originalY);
            return;
        }

        ShiftBoard[x, originalY] = Board[x, y + 1];
        BoardObjects[x, y + 1].Disable();
    }
}