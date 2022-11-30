using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class ComplexPolygon
{

    public RegionType type;

    public List<Vector2f> vertices { get; private set; }
    public List<LineSegment> edges { get; private set; }
    public List<Polygon> polygons { get; private set; }
    public Rectf bounds { get; private set; }

    public ComplexPolygon()
    {
        vertices = new List<Vector2f>();
        edges = new List<LineSegment>();
        polygons = new List<Polygon>();
    }

    public bool IsInside(Vector2f point)
    {
        foreach(Polygon poly in polygons)
        {
            if(poly.isPointInside(point)) { return true; }
        }
        return false;
    }
    public void AddPolygon(Polygon poly)
    {
        if(polygons.Contains(poly)) { return; }
        polygons.Add(poly);
        //Add verts
        foreach(Vector2f vert in poly.vertices)
        {
            if(!vertices.Contains(vert))
            {
                vertices.Add(vert);
            }
        }
        //Add edges
        foreach(LineSegment ls in poly.edges)
        {
            if(edges.Contains(ls))
            {
                edges.Remove(ls);
            }
            else
            {
                edges.Add(ls);
            }
        }
        UpdateBounds();
    }
    public bool IsNeighbor(Polygon poly)
    {
        foreach(Polygon child in polygons)
        {
            if(child.IsNeighbor(poly))
            {
                return true;
            }
        }
        return false;
    }
    public LineSegment getNearestEdge(Vector2f point)
    {
        float dist = float.MaxValue;
        LineSegment nearest = null;

        foreach(LineSegment edge in edges)
        {
            float edgeDistance = Polygon.DistanceToSegment(point, edge);
            if(edgeDistance < dist)
            {
                dist = edgeDistance;
                nearest = edge;
            }
        }

        return nearest;
    }

    private void UpdateBounds()
    {
        float minX = float.MaxValue, maxX = float.MinValue, minY = float.MaxValue, maxY = float.MinValue;
        foreach (Vector2f v in vertices)
        {
            if (v.x < minX) { minX = v.x; }
            if (v.x > maxX) { maxX = v.x; }
            if (v.y < minY) { minY = v.y; }
            if (v.y > maxY) { maxY = v.y; }
        }
        bounds = new Rectf(minX, minY, maxX - minX, maxY - minY);
    }
}
