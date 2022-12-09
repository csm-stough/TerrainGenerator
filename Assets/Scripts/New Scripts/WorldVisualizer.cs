using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;
using UnityEngine.UI;

public class WorldVisualizer : MonoBehaviour
{

    private Texture2D precip_tx;
    private Texture2D temp_tx;
    private Texture2D zone_tx;
    private Texture2D regionType_tx;

    public RawImage Precipitation;
    public RawImage Temperature;
    public RawImage Regions;
    public RawImage Zones;

    public void DrawWorld(World world)
    {

        precip_tx = new Texture2D(world.worldSize.x, world.worldSize.y);
        temp_tx = new Texture2D(world.worldSize.x, world.worldSize.y);
        zone_tx = new Texture2D(world.worldSize.x, world.worldSize.y);
        regionType_tx = new Texture2D(world.worldSize.x, world.worldSize.y);

        DrawRegionTypes(world);
        DrawPrecipitation(world);
        DrawZones(world);

        ApplyTexture();
    }

    private void DrawRegionTypes(World world)
    {
        foreach(Region reg in world.regions)
        {
            Color reg_color = reg.getRegionType() == RegionType.OCEAN ? Color.blue : reg.getRegionType() == RegionType.LAND ? Color.green : Color.gray;
            ShadePolygon(regionType_tx, reg.getPolygon(), reg_color);
        }
    }

    private void DrawZones(World world)
    {
        foreach(Zone zone in world.zones)
        {
            Color zone_col = zone.regions[0].getRegionType() == RegionType.OCEAN ? Color.blue : zone.regions[0].getRegionType() == RegionType.LAND ? Color.green : Color.gray;
            ShadeCPoly(zone_tx, zone.region_area, zone_col);
        }
    }

    private void DrawPrecipitation(World world)
    {
        foreach(Region reg in world.regions)
        {
            ShadePolygon(precip_tx, reg.getPolygon(), Color.Lerp(Color.red, Color.blue, reg.precipitation / (float)world.maxPrecipitation));
        }
    }

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
        precip_tx.Apply();
        temp_tx.Apply();
        regionType_tx.Apply();
        zone_tx.Apply();

        Precipitation.texture = precip_tx;
        Temperature.texture = temp_tx;
        Regions.texture = regionType_tx;
        Zones.texture = zone_tx;
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
