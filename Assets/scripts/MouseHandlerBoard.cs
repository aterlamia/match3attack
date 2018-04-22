using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class MouseHandlerBoard : MonoBehaviour
{
    private bool _isDragging;
    private GameObject _followTemplate;

    public GameObject Border;
    public LayerMask LayerIdForTiles;
    public GameObject SelectedObject { get; set; }

    private Vector3 _oldPosition;

    private PlayBoard _playBoard;


    private int _introStep = 0;
    public Text introText;

    private bool _forcePiece = false;

    private Vector2 _pieceToForce;

    // Use this for initialization
    void Start()
    {
        _playBoard = FindObjectOfType<PlayBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move to inouthandler

        if (_playBoard.Intro)
        {
            if (Input.GetKeyUp(KeyCode.Return) && !_forcePiece )
            {
                ProcessIntro();
            }
        }

        if (_playBoard.TurnsLeft <= 0 || (_playBoard.Intro && !_forcePiece))
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            SelectedObject = getClickedTileObj();

            if (SelectedObject != null)
            {
                if (_forcePiece && (Math.Abs(_pieceToForce.x - SelectedObject.transform.position.x) > 0.5f ||
                    Math.Abs(_pieceToForce.y - SelectedObject.transform.position.y) > 0.5f))
                {
                    Destroy(SelectedObject);
                    SelectedObject = null;
                    return;
                }

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
                    _playBoard.ShowShiftBoard();
                    if (_playBoard.Score() > 0)
                    {
                        if (_forcePiece)
                        {
                            _forcePiece = false;
                            ProcessIntro();
                        }

                        _playBoard.DoTurn();
                    }
                }
                else
                {
                    _playBoard.PlayNope();
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

    private void ProcessIntro()
    {
        _introStep++;
        if (_introStep == 1)
        {
            _forcePiece = true;
            _pieceToForce = new Vector2(2, 6);
            introText.text =
                "To defeat the enemy you will need soldiers, for soldiers we will need baracks. To get bricks for a barack match 3 of the red tokens. Move the indicated one";
            Border.transform.position = _playBoard.BoardObjects[2, 6].GameObject.transform.position;
        }
        if (_introStep == 2)
        {
            _forcePiece = true;
            _pieceToForce = new Vector2(3, 2);
            Border.transform.position = _playBoard.BoardObjects[3, 2].GameObject.transform.position;
            introText.text =
                "Good job soldier, we have enough to create a barack at the end of the turns. If we want soldiers we will need to pay them. Match the indicated yellow money token";
        }
        if (_introStep == 3)
        {
            _forcePiece = true;
            _pieceToForce = new Vector2(0, 5);
            Border.transform.position = _playBoard.BoardObjects[0, 5].GameObject.transform.position;
            introText.text =
                "Enemy incomming!!!!!, Every 3th turn we wil get an attack. Luckely he can not defeat yet. But better heal up. Move the indicated white health token";
        }
        if (_introStep == 4)
        {
            _forcePiece = true;
            _pieceToForce = new Vector2(7, 4);
            Border.transform.position = _playBoard.BoardObjects[7, 4].GameObject.transform.position;
            introText.text =
                "Took you long enough soldier. We have only 2 turns left for attack, but we are not ready. Let do something about that. Match the green turn token!!! On the double.";
        }
        if (_introStep == 5)
        {
            _forcePiece = true;
            _pieceToForce = new Vector2(7, 0);
            Border.transform.position = _playBoard.BoardObjects[7, 0].GameObject.transform.position;
            introText.text =
                "Next turn we will be attacked again. Get your defences up. Match the orange defence token. Defences can protect for 20 damage.";
        }
        if (_introStep == 6)
        {
            _forcePiece = true;
            _pieceToForce = new Vector2(3, 5);
            Border.transform.position = _playBoard.BoardObjects[3, 5].GameObject.transform.position;
            introText.text =
                "Our defences protected us. We are about tho attack ourselves. In the furture we will have tanks and commando's for now we need more soldiers. Get some more money.";
        }
        if (_introStep == 7)
        {
            Border.SetActive(false);
            _forcePiece = false;
            introText.text =
                "This is it the last turn, The enemy will throw all its forces at us after this turn. Don't worry we will use the resources we just got to protect ourselfes (enter to continue)";
        }

        if (_introStep == 8)
        {  
            Border.SetActive(true);
            Border.transform.position = _playBoard.BoardObjects[2, 4].GameObject.transform.position;
            _forcePiece = true;
            _pieceToForce = new Vector2(2, 4);
            introText.text =
                "But look we can have some more defence. match the defence token to make 5 match. matching more than 3 tokens will give a bonus. For example for defences it will give extra hitpoints.";
        }

        if (_introStep == 9)
        {
            Border.SetActive(false);
            introText.text =
                "As you can see matching black bombs will damage our health, SO DON'T DO IT !!! Now it is stime to attack so build your buildings, hire your soldiers and press attack";
            
            _forcePiece = false;
            _playBoard.Intro = false;
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
                return true;
            }
        }

        return false;
    }
}