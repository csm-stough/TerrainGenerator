using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using UnityEngine.UI;

public class WorldVisualizer : MonoBehaviour
{

    public TMPro.TMP_Dropdown dropdown;

    public List<Material> Materials;

    private WorldTerrainMesher mesher;

    private void Start()
    {
        mesher = GetComponent<WorldTerrainMesher>();
    }

    public void OnLayerSelection()
    {
        mesher.UpdateMaterials((Layer)dropdown.value, Materials[dropdown.value]);
    }

    /**OLD CODE

    /*
    private void DrawCPoly(Texture2D tx, ComplexPolygon cpoly, Color col)
    {
        foreach (LineSegment edge in cpoly.edges)
        {
            DrawLine(edge.p0, edge.p1, tx, col);
        }
    }

    private void ShadeCPoly(Texture2D tx, ComplexPolygon cpoly, Color col)
    {
        foreach (Polygon child in cpoly.polygons)
        {
            ShadePolygon(tx, child, col);
        }
    }

    private void DrawPolygon(Texture2D tx, Polygon p, Color col)
    {
        foreach (LineSegment ls in p.edges)
        {
            DrawLine(ls.p0, ls.p1, tx, col);
        }
    }

    private void ShadePolygon(Texture2D tx, Polygon p, Color col)
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
        moist_tx.Apply();
        temp_tx.Apply();
        regionType_tx.Apply();
        zone_tx.Apply();
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
    */

}
