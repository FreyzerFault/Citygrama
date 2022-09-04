using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;

public class RoadTile : Tile
{
    public List<Tile> neighbours;

    // RoadX -> X is the number of road connections to sides
    public GameObject[] roads0;
    public GameObject[] roads1;
    public GameObject[] roads2;
    public GameObject[] roads2Curve; // Curvas
    public GameObject[] roads3;
    public GameObject[] roads4;

    private GameObject road;

    protected override void Awake()
    {
        base.Awake();
        neighbours = new List<Tile> {null, null, null, null};
    }

    public override void Reveal()
    {
        // Si ya hay una carretera no hacemos nada
        if (transform.childCount > 1)
            return;
        
        UpdateConnections();
        
        // Actualizar los vecinos para conectarlos
        foreach (Tile neighbour in neighbours)
        {
            if (neighbour is RoadTile roadTile && neighbour.State == Cell.CellState.Revealed)
            {
                roadTile.UpdateConnections();
            }
        }
    }
    
    public void UpdateConnections()
    {
        // Quitamos el anterior modelo
        ClearTile();
        
        List<bool> connections = new List<bool>();
        for (int i = 0; i < 4; i++)
        {
            connections.Add(
                neighbours[i] != null
                && neighbours[i].Value == Cell.CellValue.Road
                && neighbours[i].State == Cell.CellState.Revealed
            );
        }
        
        // Numero de conexiones -> lista Roads[Cont]
        int cont = 0;
        foreach (bool connection in connections)
        {
            if (connection) cont++;
        }

        switch (cont)
        {
            case 0:
                road = Instantiate(roads0[new Random().Next(roads0.Length)], transform);
                break;
            case 1:
                road = Instantiate(roads1[new Random().Next(roads1.Length)], transform);

                // Giramos hacia la conexion
                for (int i = 0; i < 4; i++)
                {
                    if (connections[i])
                    {
                        // TOP / BOTTOM -> Girar 90º
                        switch (i)
                        {
                            // TOP
                            case 0:
                                road.transform.Rotate(Vector3.up, 90);
                                break;
                            // BOTTOM
                            case 1:
                                road.transform.Rotate(Vector3.up, 270);
                                break;
                            // LEFT
                            case 2:
                                break;
                            // RIGHT
                            case 3:
                                road.transform.Rotate(Vector3.up, 180);
                                break;
                        }
                        break;
                    }
                }
                break;
            case 2:
                // RECTA vertical / horizontal
                if (connections[0] && connections[1] || connections[2] && connections[3])
                {
                    road = Instantiate(roads2[new Random().Next(0,roads2.Length)], transform);
                    
                    // Vertical -> Girar 90º
                    if (connections[0] && connections[1])
                        road.transform.Rotate(Vector3.up, 90);
                }
                // CURVAS 0,2 / 0,3 / 1,2 / 1,3
                else
                {
                    road = Instantiate(roads2Curve[new Random().Next(0, roads2Curve.Length)], transform);

                    // La giramos para conectarla bien
                    // UP LEFT
                    if (connections[0] && connections[2])
                        road.transform.Rotate(Vector3.up, 270);
                    // UP RIGHT
                    if (connections[0] && connections[3])
                        road.transform.Rotate(Vector3.up, 0f);
                    // DOWN RIGHT
                    if (connections[1] && connections[3])
                        road.transform.Rotate(Vector3.up, 90);
                    
                    // DOWN LEFT is by DEFAULT (1,2)
                    if (connections[1] && connections[2])
                        road.transform.Rotate(Vector3.up, 180);
                } 
                break;
            case 3:
                road = Instantiate(roads3[new Random().Next(roads3.Length)], transform);
                
                // Buscamos la direccion sin conexion:
                int noConnected = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (!connections[i])
                    {
                        noConnected = i;
                        break;
                    }
                }

                // Giramos dejando el lado que no conecta sin conectar
                switch (noConnected)
                {
                    // TOP
                    case 0:
                        road.transform.Rotate(Vector3.up, 180);
                        break;
                    // BOTTOM
                    case 1:
                        break;
                    // LEFT
                    case 2:
                        road.transform.Rotate(Vector3.up, 90);
                        break;
                    //RIGHT
                    case 3:
                        road.transform.Rotate(Vector3.up, 270);
                        break;
                }
                break;
            case 4:
                road = Instantiate(roads4[new Random().Next(roads4.Length)], transform);
                break;
        }
    }
    

}
