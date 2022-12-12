using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public List<Zone> zones;
    public List<Region> regions;

    //World Properties
    public Vector2Int worldSize;
    public float maxMoisture;
    public float maxTemperature;
    public float poleTemp;
    public float poleDistance;

    public World()
    {
        zones = new List<Zone>();
        regions = new List<Region>();
    }
}
