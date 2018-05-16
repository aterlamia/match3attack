using UnityEngine;

public class Board
{
    private int[,] _shadowBoard;
    private int[,] _playBoard;
    private readonly int _boardWidth;
    private readonly int _boardHeight;
    private int _maxPieces;

    public Board(int boardWidth, int boardHeight, int maxPieces)
    {
        _boardWidth = boardWidth;
        _boardHeight = boardHeight;
        _maxPieces = maxPieces;
    }

    public void GenerateBoard(string seed)
    {
        _shadowBoard = new int[_boardWidth, _boardHeight];
        _playBoard = new int[_boardWidth, _boardHeight];
        CreateBoard(seed);
        CheckBoard();
    }

    public int[,] ShadowBoard
    {
        get { return _shadowBoard; }
        set { _shadowBoard = value; }
    }

    public int[,] PlayBoard
    {
        get { return _playBoard; }
        set { _playBoard = value; }
    }

    private void CreateBoard(string seed)
    {
        var pseudoRandom = new System.Random(seed.GetHashCode());

        for (var x = 0; x < _boardWidth; x++)
        {
            for (var y = 0; y < _boardHeight; y++)
            {
                _shadowBoard[x, y] = pseudoRandom.Next(0, _maxPieces);
            }
        }
    }

    private void CheckBoard()
    {
        var found = true;
        var counter = 0;
        while (found)
        {
            counter++;

            var foundVertically = DeDoubleVertically();
            var foundHorizontaly = DeDeboubleHorizontally();

            found = foundHorizontaly || foundVertically;
            if (found)
            {
                Debug.Log("Duplicates removed");
            }

            // Try to dedouble 20 times after that give up to not get in an forever loop.
            if (counter == 20)
            {
                found = false;
                Debug.Log("Emergency break");
            }
        }
    }

    private bool DeDoubleVertically()
    {
        bool found = false;
        for (var x = 0; x < _boardWidth; x++)
        {
            for (var y = 0; y < _boardHeight - 3; y++)
            {
                if (_shadowBoard[x, y] == _shadowBoard[x, y + 1] &&
                    _shadowBoard[x, y + 1] == _shadowBoard[x, y + 2])
                {
                    _shadowBoard[x, y + 1] = Random.Range(0, _maxPieces);

                    found = true;
                }
            }
        }

        return found;
    }

    private bool DeDeboubleHorizontally()
    {
        bool found = false;
        for (var y = 0; y < _boardHeight; y++)
        {
            for (var x = 0; x < _boardWidth - 3; x++)
            {
                if (_shadowBoard[x, y] == _shadowBoard[x + 1, y] && _shadowBoard[x + 1, y] == _shadowBoard[x + 2, y])
                {
                    // Todo add  new Class TutorialBoard
                    _shadowBoard[x + 1, y] = Random.Range(0, _maxPieces);

                    found = true;
                }
            }
        }

        return found;
    }
}