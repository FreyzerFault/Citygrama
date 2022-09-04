using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public Cell cell;

    public Cell.CellValue Value { get => cell.value; set => cell.value = value; }
    public Cell.CellState State { get => cell.state; set => cell.state = value; }
    public Vector2Int Coords { get => cell.coords; set => cell.coords = value; }
    public int Index { get => cell.index; set => cell.index = value; }


    private GameObject land;

    protected virtual void Awake()
    {
        land = transform.GetChild(0).gameObject;
    }

    public abstract void Reveal();

    public void Select()
    {
        Color c = land.GetComponent<MeshRenderer>().material.color;
        c.b *= 2;
    }
    

    public void ClearTile()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject != land)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
