using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Resource
{
    private GameObject _object;
    private int _type;
    private bool _scored = false;

    public Resource(GameObject o, int type)
    {
        GameObject = o;
        Type = type;
    }

    public bool Scored
    {
        get { return _scored; }
        set { _scored = value; }
    }

    public int Type
    {
        get { return _type; }
        set
        {
            _type = value;
            GameObject.GetComponentInChildren<TextMesh>().text = value.ToString();
        }
    }

    public GameObject GameObject
    {
        get { return _object; }
        set { _object = value; }
    }


    public bool IsDisabled()
    {
        return ! GameObject.GetComponentInChildren<SpriteRenderer>().enabled;
    }

    public void Disable()
    {
        GameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    public void Enable()
    {
        GameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void SetSprite(Sprite sprite)
    {
        var spriteRenderer = GameObject.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}