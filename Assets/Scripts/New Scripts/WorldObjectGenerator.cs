using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using csDelaunay;

[RequireComponent(typeof(Renderer), typeof(WorldVisualizer))]
public class WorldObjectGenerator : MonoBehaviour
{

    //World Generation Options
    public int RegionCount;
    public Vector2Int worldSize;

    //Precipitation
    public float maxPrecipitation;
    public float precipitationFalloffPercent;
    public float mountainPrecipitationBoostPercent;

    //World Noise Options
    public int Octaves;
    public int LloydIterations;
    public float NoiseScale;

    public float oceanLandThreshold;
    public float landMountainThreshold;

    private Voronoi voronoi;
    private World world;

    private WorldVisualizer worldVisualizer;

    // Start is called before the first frame update
    void Start()
    {
        //Create normalized voronoi object and scale to WorldSize
        List<Vector2f> points = CreateRandomPoints(RegionCount);
        Rectf bounds = new Rectf(0, 0, worldSize.x, worldSize.y);
        voronoi = new Voronoi(points, bounds, LloydIterations);
        worldVisualizer = GetComponent<WorldVisualizer>();

        world = new World();
        world.worldSize = worldSize;
        world.maxPrecipitation = maxPrecipitation;

        CreateTerrainRegions(world);
        CreateTerrainRegionNeighbors(world);
        CreatePrecipitationMap(world);

        Debug.Log(world.regions.Count);

        worldVisualizer.DrawWorld(world);

        Debug.Log("DONE!");
    }

    ////////////////////////////////////////////////////////////////

    private List<Vector2f> CreateRandomPoints(int count)
    {
        List<Vector2f> points = new List<Vector2f>();
        for (int i = 0; i < count; i++)
        {
            points.Add(new Vector2f(Random.Range(0, worldSize.x), Random.Range(0, worldSize.y)));
        }

        return points;
    }

    private void CreateTerrainRegions(World world)
    {
        Debug.Log("Creating Map Structure...");
        Vector2f offset = new Vector2f(Random.Range(-100f, 100f), Random.Range(-100f, 100f));
        foreach (Vector2f site in voronoi.SiteCoords())
        {
            Debug.Log("Created Region @ : " + site);
            Region reg = new Region(site);
            float alt = Mathf.PerlinNoise(site.x * NoiseScale + offset.x, site.y * NoiseScale + offset.y);
            reg.setRegionType(alt > landMountainThreshold ? RegionType.MOUNTAIN :
                (alt > oceanLandThreshold ? RegionType.LAND : RegionType.OCEAN));
            reg.setPolygon(new Polygon(voronoi.Region(site)));

            world.regions.Add(reg);
        }
    }

    private void CreateTerrainRegionNeighbors(World world)
    {
        Debug.Log("Establishing Neighbors...");

        foreach (Region region in world.regions)
        {
            Debug.Log("Established Neighbors for Region @ : " + region);

            List<Vector2f> neighborSites = voronoi.NeighborSitesForSite(region.getSite());
            region.AddNeighbors(world.regions.FindAll(tr => neighborSites.Contains(tr.getSite())));
        }
    }

    private void CreatePrecipitationMap(World world)
    {

        foreach(Region ocean in world.regions.FindAll(reg => reg.getRegionType() == RegionType.OCEAN))
        {
            ocean.precipitation = maxPrecipitation;
        }

        world.beaches = world.regions.FindAll(reg => reg.getRegionType() == RegionType.LAND && reg.getNeighbors().FindAll(neighbor => neighbor.getRegionType() == RegionType.OCEAN).Count > 0);

        Queue<Region> region_stack_1 = new Queue<Region>();
        Queue<Region> region_stack_2 = new Queue<Region>();

        Queue<Region>[] stacks = new Queue<Region>[] { region_stack_1, region_stack_2 };

        foreach(Region beach in world.beaches)
        {
            region_stack_1.Enqueue(beach);
        }

        int stack_index = 0;
        float currPrecipitation = maxPrecipitation;


        while(stacks[stack_index % 2].Count > 0)
        {
            while (stacks[stack_index % 2].Count > 0)
            {
                Region currRegion = stacks[stack_index % 2].Dequeue();

                currRegion.precipitation = currPrecipitation;

                if (currRegion.getRegionType() == RegionType.MOUNTAIN)
                {
                    currRegion.precipitation += mountainPrecipitationBoostPercent * maxPrecipitation;
                }

                foreach (Region neighbor in currRegion.getNeighbors())
                {
                    if (neighbor.precipitation == 0f && neighbor.getRegionType() != RegionType.OCEAN)
                    {
                        stacks[(stack_index + 1) % 2].Enqueue(neighbor);
                    }
                }
            }

            currPrecipitation -= precipitationFalloffPercent * maxPrecipitation;
            stack_index++;
        }
    }
}
