using UnityEngine;

public class BuildingHealthScript : MonoBehaviour
{
    public float health = 5;
    private float damageTimer;
    public float maxDamageTimer = 0.001f;
    private FirstPersonController character;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        character = GetComponent<FirstPersonController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (health <= 0)
        {
            Destroy(gameObject);
        }



    }


}
