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

    GameObject[] squares = new GameObject[16];

    void Start()
    {
        squares[0] = Instantiate(waves);
        squares[0].GetComponent<Waves>().SetVerts(new Vector2(0, 0), boat.transform.position);
        /*for(int i = 0; i < 16; i++)
        {
            squares[i] = Instantiate(waves);
            squares[i].GetComponent<Waves>().SetVerts(new Vector2(i / 4 - 2, i % 4 - 1), boat.transform.position);
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        squares[0].GetComponent<Waves>().SetVerts(new Vector2(0, 0), boat.transform.position);
        /*foreach(var square in squares)
        {
            if(square.GetComponent<Waves>().IsInBounds(boat.transform.position))
            {
                if(offset != square.GetComponent<Waves>().offset)
                {
                    for(int i = 0; i < 16; i++)
                    {
                        int xOffset = (int)(square.GetComponent<Waves>().offset - squares[i].GetComponent<Waves>().offset).x;
                        int zOffset = (int)(square.GetComponent<Waves>().offset - squares[i].GetComponent<Waves>().offset).y;

                        if(xOffset > 2f)
                        {
                            var newOffset = squares[i].GetComponent<Waves>().offset + new Vector2(4f, 0f);
                            Destroy(squares[i]);
                            squares[i] = Instantiate(waves);
                            squares[i].GetComponent<Waves>().SetVerts(newOffset, boat.transform.position);
                        }
                        if(xOffset < -1f)
                        {
                            var newOffset = squares[i].GetComponent<Waves>().offset + new Vector2(-4f, 0f);
                            Destroy(squares[i]);
                            squares[i] = Instantiate(waves);
                            squares[i].GetComponent<Waves>().SetVerts(newOffset, boat.transform.position);
                        }
                        if(zOffset < -2f)
                        {
                            var newOffset = squares[i].GetComponent<Waves>().offset + new Vector2(0f, -4f);
                            Destroy(squares[i]);
                            squares[i] = Instantiate(waves);
                            squares[i].GetComponent<Waves>().SetVerts(newOffset, boat.transform.position);
                        }
                        if(zOffset > 1f)
                        {
                            var newOffset = squares[i].GetComponent<Waves>().offset + new Vector2(0f, 4f);
                            Destroy(squares[i]);
                            squares[i] = Instantiate(waves);
                            squares[i].GetComponent<Waves>().SetVerts(newOffset, boat.transform.position);
                        }
                    }
                    offset = square.GetComponent<Waves>().offset;
                }
            }
        }*/
    }
}
