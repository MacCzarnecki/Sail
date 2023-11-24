using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static GameManager instance{get; set;}

    public GameObject boat;
    public bool _canDock{get; set;}

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        _canDock = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_canDock && Input.GetKeyDown("e"))
        {
            Debug.Log("Try to dock");
        }
    }
}
