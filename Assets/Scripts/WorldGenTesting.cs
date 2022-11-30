using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using System.Linq;

[RequireComponent(typeof(Renderer))]

public class WorldGenTesting : MonoBehaviour
{

    Texture2D tx;

    // Start is called before the first frame update
    void Start()
    {
        tx = new Texture2D(255, 255);

        ///////////TESTING CODE/////////////

        List<Vector2f> verts1 = new List<Vector2f>()
        {
            new Vector2f(50, 50),
            new Vector2f(100, 50),
            new Vector2f(50, 100)
        };

        List<Vector2f> verts2 = new List<Vector2f>()
        {
            new Vector2f(100, 50),
            new Vector2f(100, 100),
            new Vector2f(50, 100)
        };

        Polygon p1 = new Polygon(verts1);

        Polygon p2 = new Polygon(verts2);

        ComplexPolygon cpoly = new ComplexPolygon();
        cpoly.AddPolygon(p1);
        if (cpoly.IsNeighbor(p2)) { cpoly.AddPolygon(p2); }

        ShadeCPoly(cpoly, Color.blue);

        ////////////////////////////////////

        ApplyTexture();
    }

    ////////////////////////////////////////////////////////

    private void DrawCPoly(ComplexPolygon cpoly, Color col)
    {
        foreach(Polygon child in cpoly.getChildren())
        {
            DrawPolygon(child, col);
        }
    }
    private void ShadeCPoly(ComplexPolygon cpoly, Color col)
    {
        foreach (Polygon child in cpoly.getChildren())
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
