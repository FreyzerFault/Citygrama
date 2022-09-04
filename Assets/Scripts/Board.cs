using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

public class Board : MonoBehaviour
{
    // Grid [X][Y] ColxRow

    public List<Cell> grid = new List<Cell>();

    public int width = 10;
    public int height = 10;

    public void Awake()
    {
        grid = GetComponentsInChildren<Cell>().ToList();
        
    }

    

    public void RevealBoard()
    {
        for (int i = 0; i < width*height; i++)
        {
            RevealCell(i);
        }
    }

    // Road -> false, Building -> true
    public bool RevealCell(int i)
    {
        grid[i].Reveal();
        
        return grid[i].value == Cell.CellValue.Building;
    }

    public bool RevealCell(int x, int y)
    {
        return RevealCell(x + y * width);
    }    
    public bool RevealCell(Vector2Int coord)
    {
        return RevealCell(coord.x + coord.y * width);
    }

    // Lista de numeros para dar pistas en los margenes
    public List<int> GetRowHint(int row)
    {
        List<int> rowList = new List<int>();
        int cont = 0;

        for (int col = 0; col < width; col++)
        {
            Cell.CellValue cellValue = GetCell(col,row).value;
            
            // Si el estado es un edificio acumulamos 1
            if (cellValue == Cell.CellValue.Building)
            {
                cont++;
            }
            else
            {
                if (cont > 0)
                {
                    // Añade un numero a la lista
                    rowList.Add(cont);
                    
                    // Reset contador
                    cont = 0;                    
                }
            }
        }
        
        // Si la ultima celda es un edificio añadimos el contador
        if (cont > 0)
        {
            // Añade un numero a la lista
            rowList.Add(cont);             
        }
        
        return rowList;
    }
    
    public List<int> GetColHint(int col)
    {
        List<int> colList = new List<int>();
        int cont = 0;

        for (int row = 0; row < height; row++)
        {
            Cell.CellValue cellValue = GetCell(col,row).value;
            
            // Si el estado es un edificio acumulamos 1
            if (cellValue == Cell.CellValue.Building)
            {
                cont++;
            }
            else
            {
                if (cont > 0)
                {
                    // Añade un numero a la lista
                    colList.Add(cont);
                    
                    // Reset contador
                    cont = 0;                    
                }
            }
        }
        
        // Si la ultima celda es un edificio añadimos el contador
        if (cont > 0)
        {
            // Añade un numero a la lista
            colList.Add(cont);             
        }
        
        return colList;
    }
    
    public Cell GetCell(int x, int y)
    {
        return grid[x + y * width];
    }
    public Cell GetCell(Vector2Int coords)
    {
        return grid[coords.x + coords.y * width];
    }

    public void Reset()
    {
        grid.ForEach((cell) => cell.Reset());
    }
}
