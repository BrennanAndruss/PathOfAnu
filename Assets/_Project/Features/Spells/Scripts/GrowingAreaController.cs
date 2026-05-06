using UnityEngine;

public class GrowingAreaController : MonoBehaviour
{
    // Purpose Statement: Controls geometry instances, vfx, and counting variables [...] 
    // aswell managing collision detection / interactables

    [SerializeField] public GameObject[] growingAreas; // 3 areas-- get list of emmebdded children 
    [SerializeField] public GameObject growingSpell; // 3 growing vfx areas

    //[SerializeField] public SpellType spellType; // each ruine has a dedicated spell 
    [SerializeField] public int healedAmount = 0;
    public int instanceLength = 0; 


    [SerializeField] public bool healed = false; 
    [SerializeField] public bool activated = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // step = growingAreas.Length() / 3; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Virgo")) // need magic type reference
        //{
            for (int i = 0 ; i < instanceLength ; i ++)
            {
                growingAreas[i].gameObject.SetActive(true);
            }
            
        //}
    }


}
