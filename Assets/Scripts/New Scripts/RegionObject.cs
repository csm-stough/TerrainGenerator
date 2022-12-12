using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Layer { TYPE, MOIST, TEMP, BIOME};

public class RegionObject : MonoBehaviour
{

    public float altitude;
    public float temperature;
    public float moisture;
    public Region region;

    MeshRenderer renderer;

    private Layer layer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        altitude = region.altitude;
        temperature = region.temperature;
        moisture = region.moisture;

        switch(layer)
        {
            case Layer.TYPE:
                UpdateType();
                break;
            case Layer.TEMP:
                UpdateTemperature();
                break;
            case Layer.MOIST:
                UpdateMoisture();
                break;
            case Layer.BIOME:
                UpdateBiome();
                break;
        }
    }

    public void setLayer(Layer layer, Material material)
    {
        this.layer = layer;
        renderer.material = material;
    }

    private void UpdateType()
    {
        renderer.material.SetFloat("_Altitude", altitude);
    }

    private void UpdateTemperature()
    {
        renderer.material.SetFloat("_Temperature", temperature);
    }

    private void UpdateMoisture()
    {
        renderer.material.SetFloat("_Moisture", moisture);
    }

    private void UpdateBiome()
    {

    }
}
