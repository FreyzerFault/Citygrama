using System;
using UnityEngine;

public class City : MonoBehaviour
{
    public bool rotation = false;
    private Vector3 rotCenter;
    public float rotSpeed = 10;

    private void Awake()
    {
        rotCenter = GetComponentInChildren<MeshRenderer>().bounds.center;
    }

    private void Update()
    {
        if (rotation)
            transform.RotateAround(rotCenter, transform.up, Time.deltaTime * rotSpeed);
    }

    public void Reset()
    {
        rotSpeed = 10;
        rotation = true;
    }
}
