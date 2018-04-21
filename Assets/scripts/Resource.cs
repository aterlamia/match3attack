using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class Resource
{
    private GameObject _object;
    private int _type;
    private bool _scored = false;

    public Resource(GameObject o, int type)
    {
        _object = o;
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
            _object.GetComponentInChildren<TextMesh>().text = value.ToString();
        }
    }


    public bool IsDisabled()
    {
        return ! _object.GetComponentInChildren<SpriteRenderer>().enabled;
    }

    public void Disable()
    {
        _object.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    public void Enable()
    {
        _object.GetComponentInChildren<SpriteRenderer>().enabled = true;
    }

    public void SetSprite(Sprite sprite)
    {
        var spriteRenderer = _object.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }
}