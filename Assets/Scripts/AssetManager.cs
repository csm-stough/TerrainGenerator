using csDelaunay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AssetManager
{

    public static Voronoi regionMap { get; private set; }

    

    /////////////////////////////////////////////////////
    public static void setRegionMap(Voronoi regionMap)
    {
        AssetManager.regionMap = regionMap;
    }
}
