using UnityEngine;

public class TouchController : MonoBehaviour
{
	public Board board;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began)
			{
				// Construct a ray from the current touch coordinates
				Ray ray = Camera.main.ScreenPointToRay(touch.position);
				foreach (Cell cell in board.grid)
				{
					// TODO Comprobar que celda se ha seleccionado
				}
			}
		}
	}
}
