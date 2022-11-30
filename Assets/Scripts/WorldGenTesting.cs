using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

[RequireComponent(typeof(Renderer))]

public class WorldGenTesting : MonoBehaviour
{

    Texture2D tx;

    public int regionCount;
    public Vector2Int worldSize;

    public Color HighAltitude;
    public Color MiddleAltitude;
    public Color LowAltitude;

    public float landMountainThreshold;
    public float oceanLandThreshold;

    void Start()
    {
        Vector2Int size = worldSize;
        tx = new Texture2D(size.x, size.y);
        tx.wrapMode = TextureWrapMode.Clamp;
        tx.filterMode = FilterMode.Bilinear;

        ///////////TESTING CODE/////////////

        List<Vector2f> points = CreateRandomPoint(regionCount, size);
        Rectf bounds = new Rectf(0, 0, size.x, size.y);
        Voronoi voronoi = new Voronoi(points, bounds, 3);

        List<TerrainRegion> terrainRegions = CreateTerrainRegions(voronoi);
        CreateTerrainRegionNeighbors(terrainRegions, voronoi);
        List<ComplexPolygon> terrainPolygons = MergeTerrain(terrainRegions);

        foreach(ComplexPolygon cpoly in terrainPolygons)
        {
            switch(cpoly.type)
            {
                case RegionType.OCEAN:
                    Ocean(cpoly);
                    break;
                case RegionType.LAND:
                    Land(cpoly);
                    break;
                case RegionType.MOUNTAIN:
                    Mountain(cpoly);
                    break;
            }
            DrawCPoly(cpoly, Color.black);
        }

        ////////////////////////////////////

        ApplyTexture();
    }

    class TerrainRegion
    {
        public RegionType type;
        public Polygon polygon;
        public Vector2f site;
        public List<TerrainRegion> neighbors;
    }

    private List<TerrainRegion> CreateTerrainRegions(Voronoi voronoi)
    {
        Vector2f offset = new Vector2f(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        List<TerrainRegion> terrainRegions = new List<TerrainRegion>();
        foreach(Vector2f site in voronoi.SiteCoords())
        {
            TerrainRegion tr = new TerrainRegion();
            float alt = Mathf.PerlinNoise(site.x * 0.01f + offset.x, site.y * 0.01f + offset.y);
            tr.type = alt > landMountainThreshold ? RegionType.MOUNTAIN :
                (alt > oceanLandThreshold ? RegionType.LAND : RegionType.OCEAN);
            tr.polygon = new Polygon(voronoi.Region(site));
            tr.site = site;
            terrainRegions.Add(tr);
        }
        return terrainRegions;
    }

    private void CreateTerrainRegionNeighbors(List<TerrainRegion> regions, Voronoi voronoi)
    {
        foreach(TerrainRegion region in regions)
        {
            region.neighbors = new List<TerrainRegion>();
            List<Vector2f> neighborSites = voronoi.NeighborSitesForSite(region.site);
            region.neighbors.AddRange(regions.FindAll(tr => neighborSites.Contains(tr.site)));
        }
    }

    private List<ComplexPolygon> MergeTerrain(List<TerrainRegion> regions)
    {
        List<ComplexPolygon> terrainPolygons = new List<ComplexPolygon>();

        if(regions.Count == 0) { return terrainPolygons; }

        Queue<TerrainRegion> terrainRegionQueue = new Queue<TerrainRegion>();

        while(regions.Count > 0)
        {
            TerrainRegion currRegion = regions[0];
            ComplexPolygon currPoly = new ComplexPolygon();

            currPoly.AddPolygon(currRegion.polygon);
            currPoly.type = currRegion.type;
            terrainRegionQueue.Enqueue(currRegion);
            RegionType type = currRegion.type;

            while (terrainRegionQueue.Count > 0)
            {
                currRegion = terrainRegionQueue.Dequeue();
                regions.Remove(currRegion);

                foreach (TerrainRegion neighbor in currRegion.neighbors)
                {
                    if(!currPoly.IsNeighbor(neighbor.polygon)) { continue; }
                    if (neighbor.type == type && regions.Contains(neighbor))
                    {
                        terrainRegionQueue.Enqueue(neighbor);
                        currPoly.AddPolygon(neighbor.polygon);
                    }
                }
            }

            terrainPolygons.Add(currPoly);
        }

        return terrainPolygons;
    }

    ////////////////////////////////////////////////////////

    private void Mountain(ComplexPolygon p)
    {
        Rectf bbox = p.bounds;

        for (int y = (int)bbox.bottom; y > bbox.top; y--)
        {
            for (int x = (int)bbox.left; x < bbox.right; x++)
            {
                if (p.IsInside(new Vector2f(x, y)))
                {
                    Vector2f point = new Vector2f(x, y);
                    LineSegment nearest = p.getNearestEdge(point);
                    float dist = Polygon.DistanceToSegment(point, nearest);
                    tx.SetPixel(x, y, Color.Lerp(MiddleAltitude, HighAltitude, dist / 40f));
                }
            }
        }
    }
    private void Land(ComplexPolygon p)
    {
        Rectf bbox = p.bounds;

        for (int y = (int)bbox.bottom; y > bbox.top; y--)
        {
            for (int x = (int)bbox.left; x < bbox.right; x++)
            {
                if (p.IsInside(new Vector2f(x, y)))
                {
                    tx.SetPixel(x, y, MiddleAltitude);
                }
            }
        }
    }
    private void Ocean(ComplexPolygon p)
    {
        Rectf bbox = p.bounds;

        for (int y = (int)bbox.bottom; y > bbox.top; y--)
        {
            for (int x = (int)bbox.left; x < bbox.right; x++)
            {
                if (p.IsInside(new Vector2f(x, y)))
                {
                    Vector2f point = new Vector2f(x, y);
                    LineSegment nearest = p.getNearestEdge(point);
                    float dist = Polygon.DistanceToSegment(point, nearest);
                    tx.SetPixel(x, y, Color.Lerp(MiddleAltitude, LowAltitude, dist / 60f));
                }
            }
        }
    }

    private List<Vector2f> CreateRandomPoint(int count, Vector2Int size)
    {
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < count; i++)
        {
            points.Add(new Vector2f(Random.Range(0, size.x), Random.Range(0, size.y)));
        }

        return points;
    }
    private void DrawCPoly(ComplexPolygon cpoly, Color col)
    {
        foreach(LineSegment edge in cpoly.edges)
        {
            DrawLine(edge.p0, edge.p1, tx, col);
        }
    }
    private void ShadeCPoly(ComplexPolygon cpoly, Color col)
    {
        foreach (Polygon child in cpoly.polygons)
        {
            ShadePolygon(child, col);
        }
    }
    private void DrawPolygon(Polygon p, Color col)
    {
        foreach (LineSegment ls in p.edges)
        {
            DrawLine(ls.p0, ls.p1, tx, col);
        }
    }
    private void ShadePolygon(Polygon p, Color col)
    {
        Rectf bbox = p.getBoundingBox();

        for (int y = (int)bbox.bottom; y > bbox.top; y--)
        {
            for (int x = (int)bbox.left; x < bbox.right; x++)
            {
                if (p.isPointInside(new Vector2f(x, y)))
                {
                    tx.SetPixel(x, y, col);
                }
            }
        }
    }
    public void ApplyTexture()
    {
        tx.Apply();
        GetComponent<Renderer>().material.mainTexture = tx;
    }
    private void DrawLine(Vector2f p0, Vector2f p1, Texture2D tx, Color c, int offset = 0)
    {
        int x0 = (int)p0.x;
        int y0 = (int)p0.y;
        int x1 = (int)p1.x;
        int y1 = (int)p1.y;

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tx.SetPixel(x0 + offset, y0 + offset, c);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
}
