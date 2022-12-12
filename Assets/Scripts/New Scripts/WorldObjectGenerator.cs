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
    [Range(10f, 100f)]
    public float maxMoisture;
    [Range(0.01f, 1f)]
    public float moistureFalloffPercent;
    [Range(0f, 1f)]
    public float mountainMoistureBoostPercent;

    public float maxTemperature;

    //World Noise Options
    public int Octaves;
    public int LloydIterations;
    public float NoiseScale;

    public float oceanLandThreshold;
    public float landMountainThreshold;

    private Voronoi voronoi;
    private World world;

    private WorldVisualizer worldVisualizer;
    private WorldTerrainMesher worldMesher;

    public static World worldObject;


    // Start is called before the first frame update
    void Start()
    {
        Generate();
    }

    ////////////////////////////////////////////////////////////////

    public void Generate()
    {

        if(world != null)
            UnloadWorld();

        //Create normalized voronoi object and scale to WorldSize
        List<Vector2f> points = CreateRandomPoints(RegionCount);
        Rectf bounds = new Rectf(0, 0, worldSize.x, worldSize.y);
        voronoi = new Voronoi(points, bounds, LloydIterations);

        worldVisualizer = GetComponent<WorldVisualizer>();
        worldMesher = GetComponent<WorldTerrainMesher>();

        world = new World();
        world.worldSize = worldSize;
        world.maxMoisture = maxMoisture;
        world.maxTemperature = maxTemperature;

        CreateTerrainRegions(world);
        CreateTerrainRegionNeighbors(world);
        CreatePrecipitationMap(world);
        CreateTemperatureMap(world);
        CreateBiomeMap(world);

        WorldObjectGenerator.worldObject = this.world;
        
        worldMesher.GenerateMesh();
               
    }

    private void UnloadWorld()
    {
        for(int reg = 0; reg < world.regions.Count; reg++)
        {
            world.regions[reg].unload();
            world.regions[reg] = null;
        }

        world.regions = null;
        world.zones = null;
    }

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
            reg.altitude = alt;

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

        foreach (Region ocean in world.regions.FindAll(reg => reg.getRegionType() == RegionType.OCEAN))
        {
            ocean.moisture = maxMoisture;
        }

        List<Region> beaches = world.regions.FindAll(reg => reg.getRegionType() == RegionType.LAND && reg.getNeighbors().FindAll(neighbor => neighbor.getRegionType() == RegionType.OCEAN).Count > 0);

        Queue<Region> region_stack_1 = new Queue<Region>();
        Queue<Region> region_stack_2 = new Queue<Region>();

        Queue<Region>[] stacks = new Queue<Region>[] { region_stack_1, region_stack_2 };

        foreach (Region beach in beaches)
        {
            region_stack_1.Enqueue(beach);
        }

        int stack_index = 0;
        float currPrecipitation = maxMoisture * 0.75f;


        while (stacks[stack_index % 2].Count > 0)
        {
            while (stacks[stack_index % 2].Count > 0)
            {
                Region currRegion = stacks[stack_index % 2].Dequeue();

                currRegion.moisture = currPrecipitation;

                if (currRegion.getRegionType() == RegionType.MOUNTAIN)
                {
                    currRegion.moisture += mountainMoistureBoostPercent * maxMoisture;
                }

                foreach (Region neighbor in currRegion.getNeighbors())
                {
                    if (neighbor.moisture == 0f && neighbor.getRegionType() != RegionType.OCEAN)
                    {
                        stacks[(stack_index + 1) % 2].Enqueue(neighbor);
                    }
                }
            }

            currPrecipitation -= moistureFalloffPercent * maxMoisture;
            stack_index++;
        }
    }

    void CreateTemperatureMap(World world)
    {
        foreach (Region reg in world.regions)
        {
            reg.temperature = world.maxTemperature - (reg.getSite().y / world.worldSize.y) * world.maxTemperature;
            reg.temperature *= ((reg.moisture * 0.5f) / maxMoisture);
        }
    }

    void CreateBiomeMap(World world)
    {
        foreach (Region reg in world.regions)
        {
            reg.biome = determineBiome(reg);
        }
    }

    private Biome determineBiome(Region region)
    {
        if (region.getRegionType() == RegionType.OCEAN)
        {
            return Biome.OCEAN;
        }
        if (region.getRegionType() == RegionType.MOUNTAIN)
        {
            return Biome.MOUNTAINS;
        }
        if (region.temperature > world.maxTemperature * 0.75f && region.moisture < world.maxMoisture * 0.45f)
        {
            return Biome.DESERT;
        }

        if (region.moisture > world.maxMoisture * 0.25f && (region.temperature > world.maxTemperature * 0.25f && region.temperature < world.maxTemperature * 0.65f) && region.altitude > 0.45f)
        {
            return Biome.FOREST;
        }

        return Biome.PLAINS;
    }
}

