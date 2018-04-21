using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class MouseHandlerBoard : MonoBehaviour
{
    private bool _isDragging;
    private GameObject _followTemplate;

    public LayerMask LayerIdForTiles;
    public GameObject SelectedObject { get; set; }

    private Vector3 _oldPosition;

    private PlayBoard _playBoard;

    // Use this for initialization
    void Start()
    {
        _playBoard = FindObjectOfType<PlayBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectedObject = getClickedTileObj();

            if (SelectedObject != null)
            {
                _oldPosition = SelectedObject.transform.position;
                _isDragging = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (SelectedObject != null)
            {
                if (_checkMakesThree())
                {
                    _playBoard.ApplyShiftBoard();
                    _playBoard.LogGrids();
                    _playBoard.ShowShiftBoard();
                    if (_playBoard.Score() > 0)
                    {
                    }
                }
                else
                {
                    SelectedObject.transform.position = _oldPosition;
                    _playBoard.ResetShiftBoard();
                    _playBoard.ShowShiftBoard();
                }

                Destroy(SelectedObject);
            }

            SelectedObject = null;
            _isDragging = false;
        }


        if (_isDragging && SelectedObject != null)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            SelectedObject.transform.position = curPosition;

            int dropX = (int) Mathf.Floor(SelectedObject.transform.position.x + 0.5f);
            int dropY = (int) Mathf.Floor(SelectedObject.transform.position.y + 0.5f);

            _playBoard.ShiftHorizontally((int) _oldPosition.x, (int) _oldPosition.y, dropX, dropY);
            _playBoard.ShiftVertically((int) _oldPosition.x, (int) _oldPosition.y, dropX, dropY);
        }
    }


    private GameObject getClickedTileObj()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(
            new Vector2(
                Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                Camera.main.ScreenToWorldPoint(Input.mousePosition).y
            ),
            Vector2.zero,
            0f,
            LayerIdForTiles.value
        );
        if (hitInfo.collider != null)
        {
            // Something got hit
            // The collider is a child of the "correct" game object that we want.
            GameObject tile = hitInfo.collider.gameObject;
            return Instantiate(tile, tile.transform.position, Quaternion.identity, transform) as GameObject;
        }

        return null;
    }

    private bool _checkMakesThree()
    {
        int dropY = (int) Mathf.Floor(SelectedObject.transform.position.y + 0.5f);
        int dropX = (int) Mathf.Floor(SelectedObject.transform.position.x + 0.5f);

        dropY = Math.Min(dropY, 8);
        dropX = Math.Min(dropX, 8);

        dropY = Math.Max(dropY, 0);
        dropX = Math.Max(dropX, 0);

        if (CheckMakesThreeHiorzontally(dropY) || CheckMakesThreeVerically(dropX))
        {
            return true;
        }

        return false;
    }

    private bool CheckMakesThreeVerically(int dropX)
    {
        int matchVerticaly = 0;
        var currentType = -1;
        for (var y = 0; y < 8; y++)
        {
            if (_playBoard.ShiftBoard[dropX, y] != currentType)
            {
                matchVerticaly = 1;
                currentType = _playBoard.ShiftBoard[dropX, y];
            }
            else
            {
                matchVerticaly++;
            }

            if (matchVerticaly >= 3)
            {
                Debug.Log("match v");
                return true;
            }
        }

        return false;
    }

    private bool CheckMakesThreeHiorzontally(int dropY)
    {
        int currentType = -1;
        int matchHorizontally = 1;
        for (var x = 0; x < 8; x++)
        {
            if (_playBoard.ShiftBoard[x, dropY] != currentType)
            {
                matchHorizontally = 1;
                currentType = _playBoard.ShiftBoard[x, dropY];
            }
            else
            {
                matchHorizontally++;
            }

            if (matchHorizontally >= 3)
            {
                Debug.Log("match h");
                return true;
            }
        }

        return false;
    }
}