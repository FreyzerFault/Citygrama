using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;
using Image = UnityEngine.UI.Image;

public class Cell : MonoBehaviour
{
    public enum CellValue
    {
        Road,
        Building
    };

    public enum CellState
    {
        Hidden, 
        Selected,
        Revealed
    };
    
    public CellValue value = CellValue.Road;
    public CellState state = CellState.Hidden;

    public int index;
    public Vector2Int coords;
    
    public Sprite defaultSprite;
    public Sprite selectedSprite;
    public Sprite roadSprite;
    public Sprite buildingSprite;

    public event Action MouseDownEvent;
    public event Action MouseUpEvent;

    private void Awake()
    {
        GetComponent<UnityEngine.UI.Image>().sprite = defaultSprite;
    }

	public void OnMouseDown()
    {
        if (Input.GetMouseButton(0))
            MouseDownEvent.Invoke();
    }
    
    public void OnMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
            MouseUpEvent.Invoke();
    }

    public void Select()
    {
        state = CellState.Selected;
        GetComponent<Image>().sprite = selectedSprite;

        Color lightWhite = Color.white;
        lightWhite.a = 0.5f;
        GetComponent<Image>().color = lightWhite;
    }
    
    public void Reveal()
    {
        Color lightWhite = Color.white;
        lightWhite.a = 1;
        GetComponent<Image>().color = lightWhite;
        
        state = CellState.Revealed;
        switch (value)
        {
            case CellValue.Road:
                GetComponent<Image>().sprite = roadSprite;
                break;
            case CellValue.Building:
                GetComponent<Image>().sprite = buildingSprite;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Reset()
    {
        state = CellState.Hidden;
        GetComponent<Image>().sprite = defaultSprite;
        Color lightWhite = Color.white;
        lightWhite.a = 1;
        GetComponent<Image>().color = lightWhite;
    }
}
