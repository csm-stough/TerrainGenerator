using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

[RequireComponent(typeof(Renderer))]
public class WorldGenerator : MonoBehaviour
{

    public Vector2Int WorldSize;
    public int RegionCount;
    public int LloydIterations;
    public Color c1;
    public Color c2;

    private Voronoi voronoi;
    List<Vector2f> originalSites;
    List<Vector2f> relaxedSites;
    Rectf bounds;
    Texture2D tx;
    Dictionary<Vector2f, Region> regions;

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

        AltitudeTest();
        ApplyTexture();
    }

    private void GenerateRegions()
    {

        regions = new Dictionary<Vector2f, Region>();

        foreach(Site site in voronoi.SitesIndexedByLocation.Values)
        {
            Region reg = new Region(site);
            regions.Add(site.Coord, reg);
        }
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

    private void DrawPolygon(Polygon p)
    {
        foreach(LineSegment ls in p.edges)
        {
            DrawLine(ls.p0, ls.p1, tx, Color.black);
        }
    }

    private void AltitudeTest()
    {
        List<Vector2f> vertices = voronoi.Region(voronoi.SiteCoords()[0]);
        Polygon polygon = new Polygon(vertices);

        for(int i = 1; i < RegionCount; i++)
        {
            List<Vector2f> verts = voronoi.Region(voronoi.SiteCoords()[i]);
            polygon = MergePolygons(polygon, new Polygon(verts));
        }

        DrawPolygon(polygon);
        Mountain(polygon);
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
                    tx.SetPixel(x, y, Color.Lerp(c1, c2, dist / (AveArea / 5f)));
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
        merged.AddChildren(p1.getChildren());
        merged.AddChildren(p2.getChildren());

        return merged;
    }
}
