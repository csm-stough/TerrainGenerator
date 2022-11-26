using csDelaunay;
using System;
using System.Collections.Generic;

public class Region
{

    Voronoi map;
    private Site site;
    private Polygon polygon;

    public Region(Site site)
    {
        map = AssetManager.regionMap;
        this.site = site;
        polygon = new Polygon(map.Region(site.Coord));
        UnityEngine.Debug.Log("Region created!");
    }
}
