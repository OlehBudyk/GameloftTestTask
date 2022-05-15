using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlateHandler : MonoBehaviour
{
    public GameObject gameHandler;

    public Sprite plate0, plate1, plate2, plate3, plate4, plate5, plate6, plate7, plate8, plate9, plate10, plate11, plate12, plate13, plate14, plate15;

    private int _x, _y;
    private Color _defaultColor = new Color(255, 255, 255, 255);
    private Color _selectedColor = new Color(100, 0, 0, 255);

    public void SetX(int x)
    {
        _x = x;
    }

    public void SetY(int y)
    {
        _y = y;
    }

    public int GetX()
    {
        return _x;
    }

    public int GetY()
    {
        return _y;
    }

    public void Activate()
    {
        gameHandler = GameObject.FindGameObjectWithTag("GameController");

        const int PlatesCount = 16;
        var sprites = new Sprite[PlatesCount] { plate0, plate1, plate2, plate3, plate4, plate5, plate6, plate7, plate8, plate9, plate10, plate11, plate12, plate13, plate14, plate15 };

        this.GetComponent<SpriteRenderer>().sprite = sprites[int.Parse(this.name)];

        this.transform.position = new Vector3(_x, _y);
    }

    void OnMouseUp()
    {
        this.GetComponent<SpriteRenderer>().color = _defaultColor;
        
        GameHandler handler = gameHandler.GetComponent<GameHandler>();
        handler.TryMove(this);
    }

    void OnMouseDown()
    {
        this.GetComponent<SpriteRenderer>().color = _selectedColor;

        GameHandler handler = gameHandler.GetComponent<GameHandler>();
        handler.GameStarted();
    }
}
