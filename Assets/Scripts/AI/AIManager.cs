using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour {

    #region Singleton
    private static AIManager _instance;
    public static AIManager Instance { 
        set
        {
            _instance = value;
        }
        get
        {
            if(_instance == null)
            {
                Debug.LogError("[AIManager] :: No instance of" + typeof(AIManager));
                return null;
            }
            return _instance;
        }
    }

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [SerializeField] private SteeringSettings _steeringSettings;

    private Dictionary<string, float> steeringDictionary;


    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public SteeringSettings SteeringSettings
    {
        get
        {
            return _steeringSettings;
        }
    }
}
