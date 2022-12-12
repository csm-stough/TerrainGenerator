using csDelaunay;
using System;
using System.Collections.Generic;

public enum RegionType { LAND, OCEAN, MOUNTAIN };

public class Region
{
    private RegionType regionType;
    private Vector2f site;
    private Polygon polygon;
    private List<Region> neighbors;

    //Region Weather Properties
    public float altitude;
    public float moisture;
    public float temperature;
    public Biome biome;

    public Region(Vector2f site)
    {
        this.site = site;
        neighbors = new List<Region>();
    }

    public void unload()
    {
        neighbors = null;
        polygon = null;
    }

    public void setRegionType(RegionType rt)
    {
        this.regionType = rt;
    }

    public RegionType getRegionType()
    {
        return regionType;
    }

    public Vector2f getSite()
    {
        return this.site;
    }

    public Polygon getPolygon()
    {
        return polygon;
    }

    public void setPolygon(Polygon p)
    {
        this.polygon = p;
    }

    public List<Region> getNeighbors()
    {
        return this.neighbors;
    }

    public void AddNeighbor(Region reg)
    {
        this.neighbors.Add(reg);
    }

    public void AddNeighbors(List<Region> regs)
    {
        this.neighbors.AddRange(regs);
    }
}
