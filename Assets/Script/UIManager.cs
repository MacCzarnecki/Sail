using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static UIManager instance{get; private set;}

    [SerializeField]
    Image _dockButton;
	
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        _dockButton.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        _dockButton.enabled = GameManager.instance._canDock;
    }
}
