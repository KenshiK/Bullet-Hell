using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpaceship : Spaceship {

    private static PlayerSpaceship _instance;
    #region Singleton
    public static PlayerSpaceship Instance
    {
        set
        {
            _instance = value;
        }
        get
        {
            if (_instance == null)
            {
                Debug.LogError("[PlayerSpaceship] :: No instance of" + typeof(PlayerSpaceship));
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
}
