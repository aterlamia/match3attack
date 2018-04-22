using UnityEngine;

public class UnitMover : MonoBehaviour
{
    public Transform target;
    public PlayBoard board;
    public float speed;

    private bool attack = false;

    public int Hitpoints = 10;

    public int Damage = 9;

    public bool playerUnit = false;

    // Use this for initialization
    void Start()
    {
    }

    public void Attack()
    {
        attack = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (attack == false)
        {
            return;
        }

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);

        if (transform.position == target.position)
        {
            Destroy(gameObject);
            if (playerUnit == false)
            {
                board.DamageHq(Damage);
            }
            else
            {
                board.DamageEnemy(Damage);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log(coll.gameObject.tag);
        if (playerUnit == false)
        {
            if (coll.gameObject.tag == "playerborder")
            {
                var turret = board.User.getActiveTurret();

                if (turret != null)
                {
                    if (turret.HitPoints == 10)
                    {
                        board.User.DestoryTurret();
                    }

                    else if (turret.HitPoints > 10)
                    {
                        turret.HitPoints -= 10;
                    }


                    Destroy(gameObject);
                }
            }
        }
    }
}