using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World
{
    public List<Zone> zones;
    public List<Region> regions;
    public List<Region> beaches;

    //World Properties
    public Vector2Int worldSize;
    public float maxPrecipitation;
    public float maxTemperature;
    public float poleTemp;
    public float poleDistance;

    public World()
    {
        zones = new List<Zone>();
        regions = new List<Region>();
        beaches = new List<Region>();
    }
}
