using csDelaunay;
using System;
using System.Collections.Generic;

public enum RegionType { LAND, OCEAN, MOUNTAIN };

public class Region
{
    private RegionType regionType;
    Voronoi map;
    private Site site;
    private Polygon polygon;

    public Region(Site site)
    {
        map = AssetManager.regionMap;
        this.site = site;
        polygon = new Polygon(map.Region(site.Coord));
    }

    public void setRegionType(RegionType rt)
    {
        this.regionType = rt;
    }

    public RegionType getRegionType()
    {
        return regionType;
    }

    public Site getSite()
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

    public List<Vector2f> getNeighbors()
    {
        return map.NeighborSitesForSite(site.Coord);
    }
}
