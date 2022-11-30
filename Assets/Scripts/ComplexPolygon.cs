using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

public class ComplexPolygon
{

    private List<Vector2f> vertices;
    private List<LineSegment> edges;
    private List<Polygon> polygons;

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

    public List<Polygon> getChildren()
    {
        return polygons;
    }
}
