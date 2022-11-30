using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

[RequireComponent(typeof(Renderer))]
public class WorldGenerator : MonoBehaviour
{
    /*
    public Vector2Int WorldSize;
    public int RegionCount;
    public int LloydIterations;
    public Color lowAltitude;
    public Color middleAltitude;
    public Color highAltitude;
    public float noiseScale;

    [Range(0, 1)]
    public float oceanLandThreshold;
    [Range(0, 1)]
    public float landMountainThreshold;
    public float mountainHeightScale;
    public float oceanDepthScale;

    private Voronoi voronoi;
    private Vector2f offset;
    List<Vector2f> originalSites;
    List<Vector2f> relaxedSites;
    Rectf bounds;
    Texture2D tx;
    Dictionary<Vector2f, Region> regions;
    Dictionary<Polygon, List<Region>> polygons;

    float AveArea;

    private void Start()
    {
        GenerateAndRender();
    }

    public void GenerateAndRender()
    {
        originalSites = CreateRandomPoint();
        bounds = new Rectf(-1, -1, WorldSize.x + 1, WorldSize.y + 1);
        voronoi = new Voronoi(originalSites, bounds, LloydIterations);
        relaxedSites = voronoi.SiteCoords();
        tx = new Texture2D(WorldSize.x, WorldSize.y);
        AveArea = (WorldSize.x * WorldSize.y) / RegionCount;
        offset = new Vector2f(Random.Range(-100, 100), Random.Range(-100, 100));
        AssetManager.setRegionMap(voronoi);

        GenerateRegions();
        GenerateAltitudeMap();
        ApplyTexture();
    }

    private void GenerateRegions()
    {

        regions = new Dictionary<Vector2f, Region>();

        foreach (Site site in voronoi.SitesIndexedByLocation.Values)
        {
            Region reg = new Region(site);
            regions.Add(site.Coord, reg);
        }
    }

    private void GenerateAltitudeMap()
    {
        //Create a very low frequency noise map to define the general shape of the world
        
        foreach (Region region in regions.Values)
        {
            Vector2f position = region.getSite().Coord;
            float alt = Mathf.PerlinNoise(position.x * noiseScale + offset.x, position.y * noiseScale + offset.y);
            RegionType rt = alt > landMountainThreshold ? RegionType.MOUNTAIN :
                (alt > oceanLandThreshold ? RegionType.LAND : RegionType.OCEAN);
            region.setRegionType(rt);
        }

        MergeTerrainRegions();
    }

    public void DrawBorders()
    {
        foreach (Edge edge in voronoi.Edges)
        {
            //if the edge doesn't have clippedEnds, if was not within the bounds, dont draw it
            if (edge.ClippedEnds == null) continue;

            DrawLine(edge.ClippedEnds[LR.LEFT], edge.ClippedEnds[LR.RIGHT], tx, Color.black);
        }
    }

    public void ApplyTexture()
    {
        tx.Apply();
        GetComponent<Renderer>().material.mainTexture = tx;
    }

    private List<Vector2f> CreateRandomPoint()
    {
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < RegionCount; i++)
        {
            points.Add(new Vector2f(Random.Range(0, WorldSize.x), Random.Range(0, WorldSize.y)));
        }

        return points;
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

    private void DrawPolygon(Polygon p, Color col)
    {
        foreach(LineSegment ls in p.edges)
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

    private void Mountain(Polygon p)
    {
        Rectf bbox = p.getBoundingBox();

        for (int y = (int)bbox.bottom; y > bbox.top; y--)
        {
            for (int x = (int)bbox.left; x < bbox.right; x++)
            {
                if (p.isPointInside(new Vector2f(x, y)))
                {
                    Vector2f point = new Vector2f(x, y);
                    LineSegment nearest = p.getNearestEdge(point);
                    float dist = p.DistanceToSegment(point, nearest);
                    tx.SetPixel(x, y, Color.LerpUnclamped(middleAltitude, highAltitude, dist / (AveArea / mountainHeightScale)));
                }
            }
        }
    }

    private void Land(Polygon p)
    {
        Rectf bbox = p.getBoundingBox();

        for (int y = (int)bbox.bottom; y > bbox.top; y--)
        {
            for (int x = (int)bbox.left; x < bbox.right; x++)
            {
                if (p.isPointInside(new Vector2f(x, y)))
                {
                    tx.SetPixel(x, y, middleAltitude);
                }
            }
        }
    }

    private void Ocean(Polygon p)
    {
        Rectf bbox = p.getBoundingBox();

        for (int y = (int)bbox.bottom; y > bbox.top; y--)
        {
            for (int x = (int)bbox.left; x < bbox.right; x++)
            {
                if (p.isPointInside(new Vector2f(x, y)))
                {
                    Vector2f point = new Vector2f(x, y);
                    LineSegment nearest = p.getNearestEdge(point);
                    float dist = p.DistanceToSegment(point, nearest);
                    tx.SetPixel(x, y, Color.Lerp(middleAltitude, lowAltitude, dist / (AveArea / oceanDepthScale)));
                }
            }
        }
    }

    private Polygon MergePolygons(Polygon p1, Polygon p2)
    {
        List<LineSegment> all_edges = p1.edges;
        foreach(LineSegment ls in p2.edges)
        {
            if(all_edges.Contains(ls))
            {
                all_edges.Remove(ls);
            }
            else
            {
                all_edges.Add(ls);
            }
        }

        List<Vector2f> all_verts = p1.vertices;
        all_verts.AddRange(p2.vertices);
        all_verts = all_verts.Distinct().ToList<Vector2f>();

        Polygon merged = new Polygon(all_verts, all_edges);

        return merged;
    }

    private List<Region> getNeighborRegions(Region region)
    {
        List<Region> neighbors = new List<Region>();
        List<Vector2f> neighbor_sites = region.getNeighbors();
        foreach(Vector2f site in neighbor_sites)
        {
            neighbors.Add(regions[site]);
        }
        return neighbors;
    }

    private void MergeTerrainRegions()
    {
        Polygon merged = BFS(regions.Values.First());
        Mountain(merged);
    }

    private Polygon BFS(Region region)
    {
        Stack<Region> region_queue = new Stack<Region>();
        region_queue.Push(region);
        RegionType type = region.getRegionType();
        Polygon base_gon = new Polygon(region.getPolygon().vertices);

        while(region_queue.Count > 0)
        {
            Region curr_region = region_queue.Pop();
            
            foreach(Region neighbor in getNeighborRegions(curr_region))
            {
                if(!neighbor.merged)
                {
                    if (neighbor.getRegionType() == type && Polygon.AreNeighbors(curr_region.getPolygon(), neighbor.getPolygon()))
                    {
                        neighbor.merged = true;
                        base_gon = MergePolygons(base_gon, neighbor.getPolygon());
                        region_queue.Push(neighbor);
                    }
                }
            }
        }

        return base_gon;
    }
    */
}
