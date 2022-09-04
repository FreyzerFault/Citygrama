using UnityEngine;

public class BuildingTile : Tile
{
    public GameObject[] buildingModels;
    public GameObject[] pavements;
    
    private GameObject building;
    private GameObject pavement;

    public override void Reveal()
    {
        // Si ya hay un edificio, no hacemos nada
        if (transform.childCount > 1)
        {
            return;
        }
        
        // Elige un edificio entre todos
        building = Instantiate(buildingModels[new System.Random().Next(buildingModels.Length)], transform);
            
        // Rotacion random 
        building.transform.Rotate(Vector3.up, new System.Random().Next(0, 4) * 90);
        
        pavement = Instantiate(pavements[new System.Random().Next(pavements.Length)], transform);
    }
}
