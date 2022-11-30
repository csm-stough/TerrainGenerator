using csDelaunay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{
    public List<LineSegment> edges;
    public List<Vector2f> vertices;

    float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;

    public Polygon(List<Vector2f> vertices)
    {
        if (vertices.Count < 3)
        {
            UnityEngine.Debug.Log("Attempting to create a polygon with less than 3 vertices!");
            return;
        }
        this.vertices = vertices;
        this.edges = new List<LineSegment>();

        List<Vector2Int> indices = new List<Vector2Int>();

        indices.Add(new Vector2Int(0, 1));

        for (int v = 1; v < vertices.Count; v++)
        {
            indices.Add(new Vector2Int(v, (v + 1) % vertices.Count));
        }

        foreach (Vector2Int e in indices)
        {
            edges.Add(new LineSegment(vertices[e.x], vertices[e.y]));
        }

        CreateBounds();
    }

    public Polygon(List<Vector2f> vertices, List<LineSegment> edges)
    {
        this.vertices = vertices;
        this.edges = edges;

        CreateBounds();
    }

    private void CreateBounds()
    {
        foreach (Vector2f v in vertices)
        {
            if (v.x < minX) { minX = v.x; }
            if (v.x > maxX) { maxX = v.x; }
            if (v.y < minY) { minY = v.y; }
            if (v.y > maxY) { maxY = v.y; }
        }
    }

    public Rectf getBoundingBox()
    {
        return new Rectf(minX, minY, maxX - minX, maxY - minY);
    }

    public bool isPointInside(Vector2f p)
    {
        int counter = 0;
        int i;
        double xinters;
        Vector2f p1, p2;

        p1 = vertices[0];

        for (i = 1; i <= vertices.Count; i++)
        {
            p2 = vertices[i % vertices.Count];
            if (p.y > Mathf.Min(p1.y, p2.y))
            {
                if (p.y <= Mathf.Max(p1.y, p2.y))
                {
                    if (p.x <= Mathf.Max(p1.x, p2.x))
                    {
                        if (p1.y != p2.y)
                        {
                            xinters = (p.y - p1.y) * (p2.x - p1.x) / (p2.y - p1.y) + p1.x;
                            if (p1.x == p2.x || p.x <= xinters)
                            {
                                counter++;
                            }
                        }
                    }
                }
            }
            p1 = p2;
        }
        if (counter % 2 == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public static float DistanceToSegment(Vector2f p, LineSegment line)
    {
        float l2 = Vector2f.DistanceSquare(line.p0, line.p1);
        if (l2 == 0.0) { return Vector2f.Distance(line.p1, p); }
        float t = Mathf.Max(0, Mathf.Min(1, Vector2f.dot(p - line.p1, line.p0 - line.p1) / l2));
        Vector2f proj = line.p1 + (line.p0 - line.p1) * t;
        return Vector2f.Distance(p, proj);
    }

    public LineSegment getNearestEdge(Vector2f p)
    {

        LineSegment nearest = null;
        float dist = float.MaxValue;

        foreach (LineSegment ls in edges)
        {
            float d;
            if ((d = DistanceToSegment(p, ls)) < dist)
            {
                if (d < dist)
                {
                    nearest = ls;
                    dist = d;
                }
            }
        }

        return nearest;
    }

    public bool IsNeighbor(Polygon poly)
    {
        foreach(LineSegment ls in poly.edges)
        {
            if(edges.Contains(ls)) { return true; }
        }
        return false;
    }
}
