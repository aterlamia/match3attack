using System;
using UnityEngine;
using UnityEngine.UI;

public class MouseHandlerBoard : MonoBehaviour
{
    private bool _isDragging;
    private GameObject _followTemplate;

    public GameObject Border;
    public GameObject BorderTarget;
    public LayerMask LayerIdForTiles;
    public GameObject SelectedObject { get; set; }

    private Vector3 _oldPosition;

    public PlayBoard PlayBoard;

    private float flicker = 0.8f;
    private float flickerSpeed = 0.1f;

    private int _introStep = 0;
    public Text introText;

    private bool _forcePiece = false;

    private Vector2 _pieceToForce;
    private Vector2 _pieceToForceto;

    private Vector2 flickering;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleDrag();

        // Move to inouthandler

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (PlayBoard.Intro)
        {
            if ((Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonDown(0)) && !_forcePiece)
            {
                ProcessIntro();
            }
        }

        if (PlayBoard.TurnsLeft <= 0 || (PlayBoard.Intro && !_forcePiece))
        {
            return;
        }


        if (_forcePiece)
        {
            if (flicker >= 1.2f)
            {
                flickerSpeed = -0.1f;
            }
            else if (flicker <= 0.8f)
            {
                flickerSpeed = 0.1f;
            }

            flicker += (Time.deltaTime * flickerSpeed) * 10f;
            PlayBoard.BoardObjects[(int) _pieceToForce.x, (int) _pieceToForce.y].GameObject.transform.localScale =
                new Vector3(flicker, flicker, 1.0f);
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
                    PlayBoard.ApplyShiftBoard();
                    PlayBoard.ShowShiftBoard();
                    if (PlayBoard.Score() > 0)
                    {
                        if (_forcePiece)
                        {
                            _forcePiece = false;
                            ProcessIntro();
                        }

                        PlayBoard.DoTurn();
                    }
                }
                else
                {
                    PlayBoard.PlayNope();
                    SelectedObject.transform.position = _oldPosition;
                    PlayBoard.ResetShiftBoard();
                    PlayBoard.ShowShiftBoard();
                }

                Destroy(SelectedObject);
            }

            SelectedObject = null;
            _isDragging = false;
        }
    }


    private void resetFlicker()
    {
        PlayBoard.BoardObjects[(int) _pieceToForce.x, (int) _pieceToForce.y].GameObject.transform.localScale =
            new Vector3(1.0f, 1.0f, 1.0f);
    }

    private void HandleDrag()
    {
        if (_isDragging && SelectedObject != null)
        {
            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);

            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
            SelectedObject.transform.position = curPosition;

            int dropX = (int) Mathf.Floor(SelectedObject.transform.position.x + 0.5f);
            int dropY = (int) Mathf.Floor(SelectedObject.transform.position.y + 0.5f);

            PlayBoard.ShiftHorizontally((int) _oldPosition.x, (int) _oldPosition.y, dropX, dropY);
            PlayBoard.ShiftVertically((int) _oldPosition.x, (int) _oldPosition.y, dropX, dropY);
        }
    }

    private void ProcessIntro()
    {
        _introStep++;
        if (_introStep == 1)
        {
            _forcePiece = true;
            _pieceToForce = new Vector2(2, 6);
            _pieceToForceto = new Vector2(2, 4);
            introText.text =
                "To defeat the enemy you will need soldiers, for soldiers we will need baracks. To get bricks for a barack match 3 of the red tokens. Move the indicated one";
            Border.transform.position = PlayBoard.BoardObjects[2, 6].GameObject.transform.position;
            BorderTarget.transform.position = PlayBoard.BoardObjects[2, 4].GameObject.transform.position;
        }

        if (_introStep == 2)
        {
            resetFlicker();
            _forcePiece = true;
            _pieceToForce = new Vector2(3, 2);
            _pieceToForceto = new Vector2(5, 2);
            Border.transform.position = PlayBoard.BoardObjects[3, 2].GameObject.transform.position;
            BorderTarget.transform.position = PlayBoard.BoardObjects[5, 2].GameObject.transform.position;
            introText.text =
                "Good job soldier, we have enough to create a barack at the end of the turns. If we want soldiers we will need to pay them. Match the indicated yellow money token";
        }

        if (_introStep == 3)
        {
            resetFlicker();
            _forcePiece = true;
            _pieceToForce = new Vector2(0, 5);
            _pieceToForceto = new Vector2(1, 5);
            Border.transform.position = PlayBoard.BoardObjects[0, 5].GameObject.transform.position;
            BorderTarget.transform.position = PlayBoard.BoardObjects[1, 5].GameObject.transform.position;
            introText.text =
                "Enemy incomming!!!!!, Every 3th turn we wil get an attack. Luckely he can not defeat us yet. But better heal up. Move the indicated white health token";
        }

        if (_introStep == 4)
        {
            resetFlicker();
            _forcePiece = true;
            _pieceToForce = new Vector2(7, 4);
            _pieceToForceto = new Vector2(6, 4);
            Border.transform.position = PlayBoard.BoardObjects[7, 4].GameObject.transform.position;
            BorderTarget.transform.position = PlayBoard.BoardObjects[6, 4].GameObject.transform.position;
            introText.text =
                "Took you long enough soldier. We have only 2 turns left for attack, but we are not ready. Lets do something about that. Match the green turn token!!! On the double.";
        }

        if (_introStep == 5)
        {
            resetFlicker();
            _forcePiece = true;
            _pieceToForce = new Vector2(7, 0);
            _pieceToForceto = new Vector2(2, 0);
            Border.transform.position = PlayBoard.BoardObjects[7, 0].GameObject.transform.position;
            BorderTarget.transform.position = PlayBoard.BoardObjects[2, 0].GameObject.transform.position;
            introText.text =
                "Next turn we will be attacked again. Get your defences up. Match the orange defence token. Defences can protect for 20 damage.";
        }

        if (_introStep == 6)
        {
            resetFlicker();
            _forcePiece = true;
            _pieceToForce = new Vector2(3, 5);
            _pieceToForceto = new Vector2(4, 5);
            Border.transform.position = PlayBoard.BoardObjects[3, 5].GameObject.transform.position;
            BorderTarget.transform.position = PlayBoard.BoardObjects[4, 5].GameObject.transform.position;
            introText.text =
                "Our defences protected us. We are about to attack ourselves. In the future we will have tanks and commando's for now we need more soldiers. Get some more money.";
        }

        if (_introStep == 7)
        {
            resetFlicker();
            Border.SetActive(false);
            BorderTarget.SetActive(false);
            _forcePiece = false;
            introText.text =
                "This is it the last turn, The enemy will throw all its forces at us after this turn. Don't worry we will use the resources we just got to protect ourselves (enter to continue)";
        }

        if (_introStep == 8)
        {
            resetFlicker();
            Border.SetActive(true);
            BorderTarget.SetActive(true);
            Border.transform.position = PlayBoard.BoardObjects[2, 4].GameObject.transform.position;
            BorderTarget.transform.position = PlayBoard.BoardObjects[2, 2].GameObject.transform.position;
            _forcePiece = true;
            _pieceToForce = new Vector2(2, 4);
            _pieceToForceto = new Vector2(2, 2);
            introText.text =
                "But look we can have some more defence. match the defence token to make 5 match. matching more than 3 tokens will give a bonus. For example for defences it will give extra hitpoints.";
        }

        if (_introStep == 9)
        {
            resetFlicker();
            Border.SetActive(false);
            introText.text =
                "As you can see matching black bombs will damage our health, SO DON'T DO IT !!! Now it is stime to attack so build your buildings, hire your soldiers and press attack";

            _forcePiece = false;
            PlayBoard.Intro = false;
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

        if (_forcePiece && (Math.Abs(_pieceToForceto.x - dropX) > 0.5f ||
                            Math.Abs(_pieceToForceto.y - dropY) > 0.5f))
        {
            return false;
        }

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
            if (PlayBoard.ShiftBoard[dropX, y] != currentType)
            {
                matchVerticaly = 1;
                currentType = PlayBoard.ShiftBoard[dropX, y];
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
            if (PlayBoard.ShiftBoard[x, dropY] != currentType)
            {
                matchHorizontally = 1;
                currentType = PlayBoard.ShiftBoard[x, dropY];
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