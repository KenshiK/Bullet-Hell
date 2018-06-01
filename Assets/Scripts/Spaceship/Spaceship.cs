using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Spaceship : MonoBehaviour {

    #region DataNotToBeUsed
    [SerializeField] private int _health;
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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ApplyDamage(int damages)
    {
        Health -= damages;
    }

    protected void Death()
    {
        Destroy(this.gameObject);
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
