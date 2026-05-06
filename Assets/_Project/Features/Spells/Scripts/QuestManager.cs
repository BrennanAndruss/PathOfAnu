using UnityEngine;

public class QuestManager : MonoBehaviour
{
    // Purpose Statement: Quest Manager oversees variables and schedules events to happen.
    [SerializeField] private int questpoint = 0; // quest: 0 (Tutorial), 1, 2, 3 

    
    // Quest 1: State Variables 
    [SerializeField] public GameObject[] growingAreas; // 3 areas
    [SerializeField] public int ruinesActivated = 0;
    [SerializeField] public int growingAreasHealed = 0;
    [SerializeField] public GameObject VirgoUI;
    // Quest 2: State Variables 

    // Quest 3: State Variables 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
