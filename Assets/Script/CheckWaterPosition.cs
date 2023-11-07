using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckWaterPosition : MonoBehaviour
{
    private Terrain terrain;

    public GameObject boat;
    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        terrain.GetComponent<TerrainCollider>().isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(terrain.SampleHeight(boat.transform.position));
    }
}
