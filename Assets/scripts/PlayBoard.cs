using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class PlayBoard : MonoBehaviour
{
    public AudioClip Nope;
    public AudioClip Moneys;

    public AudioClip Boom;
    public AudioClip HealthSound;

    public AudioClip FuelSound;
    public AudioClip Timesound;
    public AudioClip Bricksound;
    public AudioClip Ammosound;
    public AudioClip Defencesound;
    AudioSource _audioSource;

    public int lvl = 1;
    private bool attackPhase = false;
    
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

    private int _tusnsDone = 0;
    private int _defence = 0;
    private int _ammo = 0;
    private int _money = 0;
    private int _oil = 0;
    private int _health = 20;
    private int _turns = 4;
    private int _bricks = 0;

    public GameObject Barrack;
    public GameObject Unit;
    public GameObject Turret;

    public GameObject Panel;
    public GameObject[] EnemyUnits;

    public GameObject EnemySpawn;
    public GameObject EnemyHq;
    public Enemy Enemy;
    public User User;

    public bool Intro = true;
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

    public int TurnsLeft
    {
        get { return _turns; }
        set { _turns = value; }
    }

    public string Seed;
    private int scorecounter = 0;

    private List<int> tutorialTokens;

    // Use this for initialization
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
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
        Enemy = new Enemy();
        User = new User();
        tutorialTokens = new List<int>();

        if (Intro)
        {
            fillTutorialTokens();
        }
    }

    private void fillTutorialTokens()
    {
//        tutorialTokens.Add(3);
        tutorialTokens.Add(4);
        tutorialTokens.Add(5);
        tutorialTokens.Add(6);
        tutorialTokens.Add(0);
        tutorialTokens.Add(1);
        tutorialTokens.Add(2);
        tutorialTokens.Add(3);
        tutorialTokens.Add(4);
        tutorialTokens.Add(5);
        tutorialTokens.Add(6);
        tutorialTokens.Add(0);
        tutorialTokens.Add(1);
        tutorialTokens.Add(0);
        tutorialTokens.Add(2);
        tutorialTokens.Add(3);
        tutorialTokens.Add(4);
        tutorialTokens.Add(5);
        tutorialTokens.Add(6);
        tutorialTokens.Add(0);
        tutorialTokens.Add(1);
        tutorialTokens.Add(2);
        tutorialTokens.Add(3);
        tutorialTokens.Add(4);
        tutorialTokens.Add(5);
        tutorialTokens.Add(6);
        tutorialTokens.Add(0);
        tutorialTokens.Add(1);
        tutorialTokens.Add(0);
        tutorialTokens.Add(2);
        tutorialTokens.Add(3);
        tutorialTokens.Add(4);
        tutorialTokens.Add(5);
        tutorialTokens.Add(6);
        tutorialTokens.Add(0);
        tutorialTokens.Add(1);
        tutorialTokens.Add(2);
        tutorialTokens.Add(3);
        tutorialTokens.Add(4);
        tutorialTokens.Add(5);
        tutorialTokens.Add(6);
        tutorialTokens.Add(0);
        tutorialTokens.Add(1);
    }

    private void UpdateUi()
    {
        Defence.text = _defence.ToString();
        Ammo.text = _ammo.ToString();
        Money.text = _money.ToString();
        Oil.text = _oil.ToString();
        Health.text = _health.ToString();
        Defence.text = _defence.ToString();
        Turns.text = TurnsLeft.ToString();
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
                        Debug.Log(string.Format("{0},{1}", x, y + 1));
                    }
                }
            }


            for (var y = 0; y < BoardHeight; y++)
            {
                for (var x = 0; x < BoardWidth - 3; x++)
                {
                    if (Board[x, y] == Board[x + 1, y] && Board[x + 1, y] == Board[x + 2, y])
                    {
                        if (Intro)
                        {
                            // for intro make it money
                            Board[x + 1, y] = 6;
                        }
                        else
                        {
                            Board[x + 1, y] = Random.Range(0, Pieces.Length);
                        }

                        Debug.Log(string.Format("s{0},{1}", x + 1, y));
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
        }


        // If not on same row return
        if (targetX != sourceX)
        {
            return;
        }

        // If moving to the left
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
        //if moving to the right
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

        if (attackPhase)
        {
            if (User.Units.Count <= 0 && EnemyUnits.Length <= 0)
            {
                if (_health <=0)
                {
                    Debug.Log("defeat");
                } else  if (Enemy.HitPoints <= 0)
                {
                    Debug.Log("victory");
                }
                else
                {
                    Debug.Log("draw");
                }
            }
        }
    }

    public void ScoreButton()
    {
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
        //tml

        List<Vector2> score = new List<Vector2>();
        var currentType = -1;

        // For every row check if the column has 3
        for (var x = 0; x < BoardWidth; x++)
        {
            currentType = -1;
            List<Vector2> columns = new List<Vector2>();
            for (var y = 0; y < BoardHeight; y++)
            {
                if (ShiftBoard[x, y] != currentType)
                {
                    AddScore(columns, score);
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
                    currentType = ShiftBoard[x, y];
                    rows.Clear();
                }

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
            Debug.Log("hier");
            RefillBoard();
            //Make sure that new matches are scored as well
            scoreCount += Score();
            Debug.Log("Daar");
        }

        return scoreCount;
    }

    private void AddScore(List<Vector2> rows, List<Vector2> score)
    {
        bool turretCreated = false;
        if (rows.Count >= 3)
        {
            foreach (var row in rows)
            {
                Debug.Log("hier2");
                var coroutine = Grow((int) row.x, (int) row.y);
                StartCoroutine(coroutine);
                Debug.Log("Daar2");
                switch (_shiftBoard[(int) row.x, (int) row.y])
                {
                    case 0:
                        _audioSource.PlayOneShot(Ammosound, 0.7f);
                        _ammo += 10;
                        break;
                    case 1:
                        _audioSource.PlayOneShot(Boom, 0.7f);
                        _health -= 3;
                        break;
                    case 2:
                        _audioSource.PlayOneShot(Bricksound, 0.7f);
                        _bricks += 10;
                        break;
                    case 3:
                        if (turretCreated == false)
                        {
                            _audioSource.PlayOneShot(Defencesound, 0.7f);
                            turretCreated = true;
                            int extra = 10 * (rows.Count - 3);
                            AddDefence(20 + extra);
                        }

                        break;
                    case 4:
                        _audioSource.PlayOneShot(FuelSound, 0.7f);
                        _oil += 10;
                        break;
                    case 5:
                        _audioSource.PlayOneShot(HealthSound, 0.7f);
                        _health += 3;
                        break;
                    case 6:
                        _money += 10;
                        _audioSource.PlayOneShot(Moneys, 0.7f);
                        break;
                    case 7:
                        _audioSource.PlayOneShot(Timesound, 0.7f);
                        TurnsLeft += 1;
                        break;
                }

                score.Add(row);
                UpdateUi();
            }
        }
    }

    IEnumerator Grow(int x, int y)
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            yield return new WaitForSeconds(.01f);
            BoardObjects[x, y].GameObject.transform.localScale = new Vector3(1.0f - f, 1.0f - f, 1.0f);
        }
    }


    private void AddDefence(int hitpoints)
    {
        var tileObj =
            Instantiate(Turret, GameObject.Find(string.Format("Pdef{0}", User.GetFreeSpot())).transform.position,
                Quaternion.identity,
                transform) as GameObject;
        User.AddDefence(new Turret(tileObj, hitpoints));
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
        ApplyShiftBoard();
        ShowShiftBoard();
    }

    public void Falldown(int x, int y, int originalY)
    {
        if (y >= BoardHeight - 1)
        {
            if (Intro)
            {
                if (x == 1 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 2 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 3 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 5 && originalY == 5)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 5 && originalY == 6)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 5 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 6 && originalY == 5)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 6 && originalY == 6)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 6 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 0 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 4 && originalY == 5)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 4 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
                else if (x == 4 && originalY == 7)
                {
                    ShiftBoard[x, originalY] = tutorialTokens[0];
                    tutorialTokens.RemoveAt(0);
                }
            }
            else
            {
                ShiftBoard[x, originalY] = Random.Range(0, Pieces.Length);
            }

            BoardObjects[x, originalY].GameObject.GetComponent<TokenBehaviour>().Shift();

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

    public void DoTurn()
    {
        _tusnsDone++;
        TurnsLeft--;

        UpdateUi();
        switch (_tusnsDone)
        {
            case 2:
            case 5:
                EnemyBuildsUnit();
                break;
        }

        if (TurnsLeft == 0)
        {
            Panel.SetActive(true);
        }
    }

    private void EnemyBuildsUnit()
    {
        var tileObj =
            Instantiate(Unit, EnemySpawn.transform.position, Quaternion.identity,
                transform) as GameObject;

        tileObj.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        tileObj.GetComponent<UnitMover>().target = GameObject.Find("PlayerHq").transform;
        tileObj.GetComponent<UnitMover>().board = this;
        tileObj.GetComponent<UnitMover>().Attack();
    }

    public void BuildBarrack()
    {
        if (_bricks < 30)
        {
            return;
        }

        _bricks -= 30;
        UpdateUi();
        var placeholder = GameObject.Find("PlayerBuildingPlaceholder1");
        var tileObj =
            Instantiate(Barrack, placeholder.transform.position, Quaternion.identity, transform) as GameObject;
        tileObj.transform.localScale = new Vector3(1.0f, -1.0f, 1.0f);
        User.AddBarack(tileObj);
    }

    public void BuildUnit()
    {
        if (_money < 30 || User.Baracks.Count == 0)
        {
            return;
        }

        _money -= 30;
        UpdateUi();
        GameObject barack = User.Baracks.First();
        var pos = barack.GetComponentInChildren<BoxCollider2D>().transform.position;
        pos = pos + new Vector3(User.Units.Count * 0.5f, User.Units.Count * 0.5f, 0f);
        var tileObj =
            Instantiate(Unit, pos, Quaternion.identity,
                transform) as GameObject;
        tileObj.GetComponent<UnitMover>().target = GameObject.Find("EnemyyHq").transform;
        tileObj.GetComponent<UnitMover>().board = this;
        tileObj.GetComponent<UnitMover>().playerUnit = true;
        User.AddUnit(tileObj);
    }

    public void DamageHq(int i)
    {
        _health -= i;
        UpdateUi();
    }

    public void StartAttack()
    {
        for (var x = 0; x < EnemyUnits.Length; x++)
        {
            EnemyUnits[x].GetComponent<UnitMover>().Attack();
        }

        foreach (var unit in User.Units)
        {
            unit.GetComponent<UnitMover>().Attack();
        }
    }

    public void PlayNope()
    {
        _audioSource.PlayOneShot(Nope, 0.7f);
    }

    public void DamageEnemy(int i)
    {
        if (Enemy.HitPoints - i == 0)
        {
            _audioSource.PlayOneShot(Boom, 0.8f);
            Destroy(EnemyHq);
            EnemyHq = null;
            Application.LoadLevel("lvl2");
        }

        Enemy.HitPoints -= 9;
    }

}