using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Nonogram : MonoBehaviour
{
    public enum Side { Top, Bottom, Left, Right, None };
    
    public Board board;
    public TileMap tileMap;
    public GameObject colHintPanel;
    public GameObject rowHintPanel;
    public City city;

    public List<Sprite> numSprites;
    public GameObject numPrefab;

    public GameObject BuildingTilePrefab;
    public GameObject RoadTilePrefab;

    // Board Generation
    public int cellBuildingProbability = 50;
    
    // FAIL Score
    public int fails;
    public Text failsText;


    public Vector2Int selectedBegin;
    public Vector2Int selectedEnd;
    public Side selectedSide = Side.None;
    
    public Stack<Vector2Int> selectedCells = new Stack<Vector2Int>();

    public int Width { get => board.width; private set => board.width = value; }
    public int Height { get => board.height; private set => board.height = value; }

    private void Awake()
    {
        // Score inversa de fallos
        fails = 0;
        failsText.text = "0";
    }

    private void Start()
    {
        // Inicializacion del Tablero
        InitializeRandomBoard();
        InitializeCityTileMap();
        
        // Set Column and Row Number Hints
        SetHints();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Toggle menuToggle = GetComponent<Toggle>();
            menuToggle.isOn = !menuToggle.isOn;
        }
        
        if (Input.GetKeyDown(KeyCode.F12))
        {
            RevealBoard();
        }
    }
    
    public void MenuToggle(bool isOn)
    {
        Time.timeScale = isOn ? 0 : 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Reset()
    {
        fails = 0;
        failsText.text = "0";

		tileMap.Clear();

		InitializeRandomBoard();
		SetHints();

		board.Reset();
        tileMap.Reset();
        city.Reset();
        
        selectedCells.Clear();
    }

    public void RevealBoard()
    {
        board.RevealBoard();
        tileMap.tiles.ForEach((tile) => tile.Reveal());

        StartCoroutine(EndingRoutine());
    }

    public void InitializeRandomBoard()
    {
        // Probabilidad acumulada, va aumentando o disminuyendo conforme salen casillas llenas
        int accumulativeProbability = 0;

        // Reduce la probabilidad cuando pasa de 4
		Boolean reduceProb = false;
        int reduceProbThreshold = 5;

        // Casillas llenas acumuladas
        int prevBuildingAccumulative = 0;

		Cell.CellValue prevValue = Cell.CellValue.Road;

		for (int i = 0; i < Width * Height; i++)
        {
            Cell cell = board.grid[i];

			// Valor Random (X% empty)

			cell.value = new Random().Next(0, 100) < (cellBuildingProbability + accumulativeProbability) ? Cell.CellValue.Building : Cell.CellValue.Road;
            prevValue = cell.value;

            if (cell.value == Cell.CellValue.Building) // Casilla llena
			{
				prevBuildingAccumulative++;
				if (prevBuildingAccumulative < reduceProbThreshold) accumulativeProbability += 10;
				else accumulativeProbability -= 10;
            }
            else // Casilla vacia
			{
                accumulativeProbability = 0;
				prevBuildingAccumulative = 0;
				reduceProb = false;
			}

            // Estado oculto al empezar
            cell.state = Cell.CellState.Hidden; 
            
            // Posicion en el tablero
            cell.index = i;
            cell.coords = new Vector2Int(i % Width, i / Width);

            // Create Tiles
            GameObject tileObj = Instantiate(cell.value == Cell.CellValue.Building ? BuildingTilePrefab : RoadTilePrefab, tileMap.transform);
            Tile tile = tileObj.GetComponent<Tile>();
            tileMap.tiles.Add(tile);
            tile.transform.localPosition = new Vector3(cell.coords.x, 0,  -cell.coords.y) * 2;
            tile.cell = cell;
            
            // EVENTOS
            var index = i;
            
            board.grid[i].MouseDownEvent += () => {
                Cell thisCell = board.grid[index];
                Vector2Int coord = thisCell.coords;
                
                // Primer click
                if (selectedCells.Count == 0)
                {
                    selectedBegin = coord;
                    selectedEnd = coord;
                    SelectCell(coord);
                    return;
                }
                
                // Segunda celda
                if (selectedCells.Count == 1)
                {
                    selectedEnd = coord;
                    SelectCell(coord);

                    if (selectedEnd.x > selectedBegin.x)
                        selectedSide = Side.Right;
                    if (selectedEnd.x < selectedBegin.x)
                        selectedSide = Side.Left;
                    if (selectedEnd.y > selectedBegin.y)
                        selectedSide = Side.Bottom;
                    if (selectedEnd.y < selectedBegin.y)
                        selectedSide = Side.Top;
                    
                    return;
                }

                // Vuelve al inicio
                if (selectedBegin == coord)
                {
                    DeselectCell();
                    return;
                }

                switch (selectedSide)
                {
                    case Side.Right:
                        if (coord.x > selectedEnd.x)
                            SelectCell(selectedEnd.x + 1, selectedEnd.y);
                        if (thisCell.coords.x < selectedEnd.x)
                            DeselectCell();
                        
                        selectedEnd = new Vector2Int(coord.x, selectedBegin.y);
                        break;
                    case Side.Left:
                        if (coord.x < selectedEnd.x)
                            SelectCell(selectedEnd.x - 1, selectedEnd.y);
                        if (thisCell.coords.x > selectedEnd.x)
                            DeselectCell();
                        
                        selectedEnd = new Vector2Int(coord.x, selectedBegin.y);
                        break;
                    case Side.Top:
                        if (coord.y < selectedEnd.y)
                            SelectCell(selectedEnd.x, selectedEnd.y - 1);
                        if (thisCell.coords.y > selectedEnd.y)
                            DeselectCell();
                        
                        selectedEnd = new Vector2Int(selectedBegin.x, coord.y);
                        break;
                    case Side.Bottom:
                        if (coord.y > selectedEnd.y)
                            SelectCell(selectedEnd.x, selectedEnd.y + 1);
                        if (thisCell.coords.y < selectedEnd.y)
                            DeselectCell();
                        
                        selectedEnd = new Vector2Int(selectedBegin.x, coord.y);
                        break;
                }
            };
            
            board.grid[i].MouseUpEvent += () =>
            {
                StartCoroutine(RevealSelectedRoutine());
            };
        }
    }

    private IEnumerator RevealSelectedRoutine()
    {
        List<Vector2Int> reversed = selectedCells.ToArray().Reverse().ToList();
        foreach (Vector2Int selectedCellCoord in reversed)
        {
            // Revelar las celdas seleccionadas
            bool fail = RevealCell(selectedCellCoord);
            // Suma un fallo por cada casilla de carretera
            if (fail)
            {
                AddFail();
                foreach (Vector2Int coord in reversed)
                {
                    if (board.GetCell(coord).state == Cell.CellState.Selected)
                        board.GetCell(coord).Reset();
                }
                break;
            }

        }
        selectedCells.Clear();

        yield return null;
    }

    private void SelectCell(int x, int y)
    {
        SelectCell(new Vector2Int(x,y));
    }
    private void SelectCell(Vector2Int coords)
    {
        Cell cell = board.GetCell(coords);
        if (cell != null && cell.state == Cell.CellState.Hidden)
        {
            cell.Select();
            selectedCells.Push(coords);
        }
    }
    
    private void DeselectCell()
    {
       Vector2Int coords = selectedCells.Pop();
       board.GetCell(coords).Reset();
    }

    // Road -> false, Building -> true
    private bool RevealCell(Vector2Int coord)
    {
        bool fail = !board.RevealCell(coord);
                        
        // Update City Tiles
        tileMap.RevealTile(coord);

        if (CompletedCol(coord.x))
        {
            RevealColumn(coord.x);
        }
        if (CompletedRow(coord.y))
        {
            RevealRow(coord.y);
        }
        
        
        if (EndCondition())
        {
            StopAllCoroutines();
            StartCoroutine(EndingRoutine());
            return true;
        }

        return fail;
    }


    public void RevealColumn(int x)
    {
        for (int y = 0; y < Height; y++)
        {
            if (board.GetCell(x,y).state != Cell.CellState.Revealed)
            {
                RevealCell(new Vector2Int(x,y));
            }
        }
    }

    public void RevealRow(int y)
    {
        
        for (int x = 0; x < Width; x++)
        {
            if (board.GetCell(x,y).state != Cell.CellState.Revealed)
            {
                RevealCell(new Vector2Int(x,y));
            }
        }
    }

    public bool CompletedCol(int x)
    {
        for (int y = 0; y < Height; y++)
        {
            if (board.GetCell(x,y).value == Cell.CellValue.Building && board.GetCell(x,y).state != Cell.CellState.Revealed)
            {
                return false;
            }
        }

        return true;
    }
    
    public bool CompletedRow(int y)
    {
        for (int x = 0; x < Width; x++)
        {
            if (board.GetCell(x,y).value == Cell.CellValue.Building && board.GetCell(x,y).state != Cell.CellState.Revealed)
            {
                return false;
            }
        }

        return true;
    }

    public void InitializeCityTileMap()
    {
        tileMap.width = Width;
        tileMap.height = Height;
    }

    public void AddFail()
    {
        fails++;
        failsText.text = fails.ToString();
    }

    public bool EndCondition()
    {
        // Si algun edificio no esta revelado no ha acabado el juego
        foreach (Cell cell in board.grid)
        {
            if (cell.value == Cell.CellValue.Building && cell.state != Cell.CellState.Revealed)
            {
                return false;
            }
        }

        return true;
    }
    
    
    public IEnumerator EndingRoutine()
    {
        city.rotation = true;
        city.rotSpeed = 50;
        
        yield return new WaitForSeconds(6);
        
        Reset();
        yield return null;
    }
    
    public void SetHints()
    {
        // Columns
        for (int i = 0; i < colHintPanel.transform.childCount; i++)
        {
            Transform colHint = colHintPanel.transform.GetChild(i);
            foreach (Transform numChild in colHint.transform)
            {
                Destroy(numChild.gameObject);
            }
            
            List<int> nums = board.GetColHint(i);

            foreach (int num in nums)
            {
                GameObject numObj = Instantiate(numPrefab, colHint.transform);
                numObj.GetComponent<Image>().sprite = numSprites[num];
            }
        }
        
        // Rows
        for (int i = 0; i < rowHintPanel.transform.childCount; i++)
        {
            Transform rowHint = rowHintPanel.transform.GetChild(i);
            foreach (Transform numChild in rowHint.transform)
            {
                Destroy(numChild.gameObject);
            }
            
            List<int> nums = board.GetRowHint(i);

            foreach (int num in nums)
            {
                GameObject numObj = Instantiate(numPrefab, rowHint.transform);
                numObj.GetComponent<Image>().sprite = numSprites[num];
            }
        }
    }
}
