using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class BoatController : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera mainCamera;

    private Vector3 cameraPosition;

    private float cameraHeight;

    public Material waterMaterial;

    private GameManager gameManager;
    void Start()
    {
        gameManager = GameManager.instance;
        cameraHeight = mainCamera.transform.position.y;
        cameraPosition = mainCamera.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Bounce();

        if(GetComponent<Rigidbody>().velocity.magnitude != 0f)
        {
            GetComponent<Rigidbody>().AddForce(-GetComponent<Rigidbody>().velocity, ForceMode.Force);
        }
        gameManager._canDock = CheckTerrainInFront();

        if(gameManager._canDock && Input.GetKeyDown("e"))
        {
            Debug.Log("Try to dock");
        }

        if(Input.GetKey(KeyCode.UpArrow))
            gameObject.transform.position += transform.forward * Time.deltaTime * 5f;
        if(Input.GetKey(KeyCode.RightArrow))
            gameObject.transform.rotation *= Quaternion.Euler(new Vector3(0f, 2f, 0f));
        if(Input.GetKey(KeyCode.LeftArrow))
            gameObject.transform.rotation *= Quaternion.Euler(new Vector3(0f, -2f, 0f));

        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, transform.position + cameraPosition, 0.2f);
    }

    public bool CheckTerrainInFront()
    {
        Ray[] rays = {new Ray(transform.position, (transform.forward + (Vector3.down / 3f)) * 3f),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 3f)) * 3f + transform.right),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 3f)) * 3f - transform.right),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 5f)) * 3f),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 5f)) * 3f + transform.right),
                    new Ray(transform.position, (transform.forward + (Vector3.down / 5f)) * 3f - transform.right),
                    new Ray(transform.position, Vector3.down)};

        bool groundDetected = false;

        foreach(Ray ray in rays)
            if(Physics.Raycast(ray, 3f, LayerMask.GetMask("Terrain")))
            {
                GetComponent<Rigidbody>().AddForce(-ray.direction, ForceMode.Acceleration);
                Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red);
                groundDetected = true;
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 3f, Color.green);
            }

        return groundDetected;
    }
    
    private void Dock()
    {

    }

    Vector3 GerstnerWave (
			Vector4 wave, Vector3 p, ref Vector3 tangent, ref Vector3 binormal) {
		    float steepness = wave.z;
		    float wavelength = wave.w;
		    float k = 2 * Mathf.PI / wavelength;
			float c = Mathf.Sqrt(9.8f / k);
			Vector2 d = new Vector2(wave.x, wave.y);
            d.Normalize();
			float f = k * (Vector2.Dot(d, new Vector2(p.x, p.z)) - c * Time.timeSinceLevelLoad);
			float a = steepness / k;

			tangent += new Vector3(
				-d.x * d.x * (steepness * Mathf.Sin(f)),
				d.x * (steepness * Mathf.Cos(f)),
				-d.x * d.y * (steepness * Mathf.Sin(f))
			);
			binormal += new Vector3(
				-d.x * d.y * (steepness * Mathf.Sin(f)),
				d.y * (steepness * Mathf.Cos(f)),
				-d.y * d.y * (steepness * Mathf.Sin(f))
			);
			return new Vector3(
				d.x * (a * Mathf.Cos(f)),
				a * Mathf.Sin(f),
				d.y * (a * Mathf.Cos(f))
			);
		}

    public void Bounce()
    {
        List<Vector4> waves = new List<Vector4>();
        char iter = 'A';
        while(waterMaterial.HasProperty("_Wave" + iter))
        {
            waves.Add(waterMaterial.GetVector("_Wave" + iter));
            iter++;
        }

        Vector3 gridPoint = new Vector3(transform.position.x, 0f, transform.position.z);
		Vector3 tangent = new Vector3(1, 0, 0);
		Vector3 binormal = new Vector3(0, 0, 1);
		Vector3 p = gridPoint;
        foreach(Vector4 wave in waves)
            p += GerstnerWave(wave, gridPoint, ref tangent, ref binormal);

		Vector3 normal = Vector3.Cross(binormal, tangent).normalized;
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, p.y, transform.position.z), 0.05f);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(Vector3.Cross(transform.right, normal), normal), 0.006f);
    }
}
