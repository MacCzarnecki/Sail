using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class BoatController : MonoBehaviour
{
    // Start is called before the first frame update
    private bool isOnABeach = false;
    public Camera mainCamera;

    private Vector3 cameraPosition;

    private float cameraHeight;
    void Start()
    {
        cameraHeight = mainCamera.transform.position.y;
        cameraPosition = mainCamera.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.instance._canDock = CheckTerrainInFront();
        if(Input.GetKey(KeyCode.UpArrow))
            gameObject.transform.position += transform.forward * Time.deltaTime * 5f;
        if(Input.GetKey(KeyCode.RightArrow))
            gameObject.transform.rotation *= Quaternion.Euler(new Vector3(0f, 2f, 0f));
        if(Input.GetKey(KeyCode.LeftArrow))
            gameObject.transform.rotation *= Quaternion.Euler(new Vector3(0f, -2f, 0f));

        mainCamera.transform.position += (transform.position + cameraPosition - mainCamera.transform.position) / 20f;
        cameraPosition.y = cameraHeight;

    }

    void OnCollisionStay(Collision collision)
    {
        //Output the Collider's GameObject's name
        if(collision.collider is TerrainCollider)
        {
            // //GetComponent<Rigidbody>().AddForce(collision.contacts[0].impulse, ForceMode.Impulse);
            Terrain terrain = collision.collider.GameObject().GetComponent<Terrain>();
            Vector3 hit = collision.collider.ClosestPointOnBounds(transform.position);
            transform.position = hit;


            float normalizedX = (transform.position.x - terrain.GetPosition().x) / terrain.terrainData.size.x;
            float normalizedY = (transform.position.z - terrain.GetPosition().z) / terrain.terrainData.size.z;
            Vector3 normal = terrain.terrainData.GetInterpolatedNormal(normalizedX, normalizedY);
            float gradient = terrain.terrainData.GetSteepness(normalizedX, normalizedY); 
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.Cross(transform.right, normal), normal), 0.2f);    
            isOnABeach = true;
        }
    }

    public bool CheckTerrainInFront()
    {
        Ray[] rays = {new Ray(transform.position, (transform.forward + (Vector3.down / 3f)) * 3f),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 3f)) * 3f + transform.right),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 3f)) * 3f - transform.right),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 5f)) * 3f),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 5f)) * 3f + transform.right),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 5f)) * 3f - transform.right)};
        foreach(Ray ray in rays)
            Debug.DrawRay(ray.origin, transform.position);

        foreach(Ray ray in rays)
            if(Physics.Raycast(ray, 3f, LayerMask.GetMask("Terrain")))
                return true;

        return false;

    }
    
    void OnCollisionExit(Collision collision)
    {
        isOnABeach = false;
    }

    public void Bounce(List<Octave> octaves)
    {
        if(isOnABeach)
        {
            return;
        }
        Vector3[] vertices = new Vector3[8];
        
        vertices[0] = transform.position + new Vector3(-1f, 0, -1f);
        vertices[1] = transform.position + new Vector3(-1f, 0, 0);
        vertices[2] = transform.position + new Vector3(-1f, 0, 1f);
        vertices[3] = transform.position + new Vector3(0, 0, 1f);
        vertices[4] = transform.position + new Vector3(1f, 0, 1f);
        vertices[5] = transform.position + new Vector3(1f, 0, 0);
        vertices[6] = transform.position + new Vector3(1, 0, -1f);
        vertices[7] = transform.position + new Vector3(0, 0, -1f);

        Vector3 normal = Vector3.zero;


        for(int i = 0; i < 8; i++)
        {
            float height = 0f;
            foreach(Octave o in octaves)
            {
                var dir = o.direction.normalized;

                height += Mathf.Cos((vertices[i].x 
                                * dir.x + vertices[i].z 
                                    * dir.y + Time.time * o.speed) / o.length) 
                                    * o.scale;
            }
            vertices[i] += Vector3.up * height;
        }
        float y = 0f;
        foreach(Octave o in octaves)
            {
                var dir = o.direction.normalized;

                y += Mathf.Cos((transform.position.x 
                                * dir.x + transform.position.z 
                                    * dir.y + Time.time * o.speed) / o.length) 
                                    * o.scale;
            }
        transform.position = new Vector3(transform.position.x, y , transform.position.z);

        normal += Vector3.Cross(vertices[0] - transform.position, vertices[1] - transform.position);
        normal += Vector3.Cross(vertices[1] - transform.position, vertices[2] - transform.position);
        normal += Vector3.Cross(vertices[2] - transform.position, vertices[3] - transform.position);
        normal += Vector3.Cross(vertices[3] - transform.position, vertices[4] - transform.position);
        normal += Vector3.Cross(vertices[4] - transform.position, vertices[5] - transform.position);
        normal += Vector3.Cross(vertices[5] - transform.position, vertices[6] - transform.position);
        normal += Vector3.Cross(vertices[6] - transform.position, vertices[7] - transform.position);
        normal += Vector3.Cross(vertices[7] - transform.position, vertices[0] - transform.position);
        normal.Normalize();

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.Cross(transform.right, normal), normal), 0.1f);

        transform.position += new Vector3(normal.x, 0f, normal.z) / 10f;
    }
}
