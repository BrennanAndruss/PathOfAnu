using System.Collections.Generic;
using UnityEngine;

public class SpawnProjectiles : MonoBehaviour
{
    public GameObject firePoint; // point of spawn of projectiles 
    public List<GameObject> vfx = new List<GameObject> (); // list of prefab vfx 
    //public CrystalRotateMouse rotateToMouse; 
    //public PlayerCrystalController playerGem;

    public AudioSource sfxSource;
    public AudioClip attackSFX;
    

    private GameObject effectToSpawn; // prefab empty varible assigned 
    private float timeToFire = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        effectToSpawn = vfx[0];
    }

    // Update is called once per frame
    void Update()
    {
        // Followed tutorial which used old input system 2018. Fast Prototyping idc both workss. 
        // if (Input.GetMouseButton(0) && Time.time >= timeToFire) // change to new Unity Input System
        // {
        //     timeToFire = Time.time + 1 / effectToSpawn.GetComponent<ProjectileMove>().fireRate;
        //     SpawnVFX();
        // }
    }

    void SpawnVFX()
    {
        GameObject vfx;
        if (firePoint != null)
        {
            sfxSource.PlayOneShot(attackSFX); // Play Audio

            vfx = Instantiate(effectToSpawn, firePoint.transform.position, Quaternion.identity); // Spawn
            ProjectileMove move = vfx.GetComponent<ProjectileMove>(); // get move script
            // move.Init(playerGem.playersMaterial); dont need this anymore

            // if (rotateToMouse != null)
            // {
            //     vfx.transform.localRotation = rotateToMouse.GetRotation();
            //     vfx.transform.forward = rotateToMouse.getUpVector();
                
            // }
            
        } else
        {
            Debug.Log("No Fire Point!");
        }
    }
}
