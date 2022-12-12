using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldTerrainMesher : MonoBehaviour
{

    public GameObject Region;

    private List<RegionObject> regions;
    private World world;

    public void GenerateMesh()
    {
        world = WorldObjectGenerator.worldObject;

        if(regions == null)
        {
            regions = new List<RegionObject>();
        }

        for (int i = 0; i < regions.Count; i++)
        {
            Destroy(regions[i].gameObject);
        }

        regions.Clear();

        foreach(Region reg in world.regions)
        {

            GameObject regionMesh = Instantiate(Region, this.gameObject.transform);

            RegionObject reg_obj = regionMesh.GetComponent<RegionObject>();

            regions.Add(reg_obj);

            reg_obj.region = reg;

            List<Vector3> verts = new List<Vector3>();
            foreach(Vector2f vert in reg.getPolygon().vertices)
            {
                verts.Add(new Vector3(vert.x, 0, vert.y));
            }

            regionMesh.GetComponent<MeshFilter>().mesh = HullMesher.BuildPolygon(verts.ToArray());
        }
    }

    public void UpdateMaterials(Layer layer, Material material)
    {
        foreach(RegionObject reg in regions)
        {
            reg.setLayer(layer, material);
        }
    }
}
