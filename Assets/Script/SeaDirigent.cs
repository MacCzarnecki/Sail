using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using System;

public class SeaDirigent : MonoBehaviour
{
    // Start is called before the first frame update


    public List<Octave> octaves;

    public GameObject waves;

    public GameObject boat;

    private Vector2 offset = new Vector2(0f, 0f);

    GameObject[] squares = new GameObject[9];

    public bool automaticNormals = true;

    void Start()
    {
        for(int i = 0; i < 9; i++)
        {
            squares[i] = Instantiate(waves);
            squares[i].GetComponent<Waves>().offset = new Vector2(i / 3 - 1, i % 3 - 1);
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var square in squares)
        {
            square.GetComponent<Waves>().UpdateMesh(octaves);

            square.GetComponent<Waves>().automaticNormals = automaticNormals;
            if(square.GetComponent<Waves>().IsInBounds(boat.transform.position))
            {
                if(offset != square.GetComponent<Waves>().offset)
                {
                    for(int i = 0; i < 9; i++)
                    {
                        if((square.GetComponent<Waves>().offset - squares[i].GetComponent<Waves>().offset).magnitude > 1.8f)
                        {
                            var newOffset = offset - squares[i].GetComponent<Waves>().offset + square.GetComponent<Waves>().offset;
                            Destroy(squares[i]);
                            squares[i] = Instantiate(waves);
                            squares[i].GetComponent<Waves>().offset = newOffset;
                        }
                    }
                    offset = square.GetComponent<Waves>().offset;
                }
            }
        }

        boat.GetComponent<BoatController>().Bounce(octaves);
    }
}
