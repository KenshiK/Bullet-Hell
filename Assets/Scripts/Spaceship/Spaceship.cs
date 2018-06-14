using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spaceship : MonoBehaviour {

    #region DataNotToBeUsed
    [SerializeField] private int _health;

    protected Collider col;
    
    protected Vehicle vehicle;
    #endregion

    #region UsableData
    public int Health {
        get { return _health; }
        set {
            _health = Mathf.Max(0, value);
            if(Health == 0)
                Death();
        }
    }
    #endregion

    // Use this for initialization
    void Start () {
        col = GetComponent<Collider>();
        vehicle = GetComponentInChildren<Vehicle>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyDamage(int damages)
    {
        //commenté pour les tests de performances et collisions
        //Health -= damages; 
    }

    protected void Death()
    {
        Destroy(this.gameObject);
    }

    public Collider GetCollider()
    {
        return col;
    }

    public Vehicle GetVehicle()
    {
        return vehicle;
    }
}

/*[CustomEditor(typeof(Spaceship))]
public class SpaceshipEditor : Editor
{

    Spaceship spaceship;

    public void OnEnable()
    {
        spaceship = target as Spaceship;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Health"));
        spaceship.Health = EditorGUILayout.IntField(spaceship.Health);
        EditorGUILayout.EndHorizontal();
    }
}*/
