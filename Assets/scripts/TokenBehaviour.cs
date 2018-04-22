using UnityEngine;

public class TokenBehaviour : MonoBehaviour
{
    public float speed;

    private bool shift = false;

    private Vector3 realPosition;

    // Use this for initialization
    void Start()
    {
        realPosition = transform.position;
    }

    public void Shift()
    {
        Debug.Log(realPosition.y);
        transform.position = new Vector3(realPosition.x, realPosition.y + 1, realPosition.z);
        shift = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (shift == false)
        {
            return;
        }

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, realPosition, step);

        if (transform.position == realPosition)
        {
            shift = false;
        }
    }
}