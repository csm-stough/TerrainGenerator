using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Season { SPRING, SUMMER, AUTUMN, WINTER };

public class WorldTimeUpdater : MonoBehaviour
{

    public int year;
    public Season season;

    [Range(0.5f, 5f)]
    public float timeStep;
    private float currTime;

    private World world;

    // Start is called before the first frame update
    void Start()
    {
        world = WorldObjectGenerator.worldObject;
    }

    // Update is called once per frame
    void Update()
    {

        world = WorldObjectGenerator.worldObject;


        if (currTime < timeStep)
        {
            currTime += Time.deltaTime;

            if(currTime > timeStep)
            {
                AdvanceTime();
                currTime -= timeStep;
            }
        }
    }

    private void AdvanceTime()
    {
        season = (Season)(((int)season + 1) % 4);

        if((int)season == 0)
        {
            year++;
        }

        UpdateRegions();
    }

    private void UpdateRegions()
    {
        foreach(Region reg in world.regions)
        {
            float sin_value = Mathf.Sin((int)season * (2 * Mathf.PI) / 4f);

            reg.temperature = world.maxTemperature - (reg.getSite().y / world.worldSize.y) * world.maxTemperature;
            reg.temperature += sin_value * 10f;
            reg.temperature *= (reg.moisture / world.maxMoisture);
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
