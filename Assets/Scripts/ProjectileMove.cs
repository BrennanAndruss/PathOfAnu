using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;

    public AudioClip crashSFX;

    // Parameter Variables 
    //public Material projectileMaterial;

    // For instanitation to have real-time data on a prefab
    public void Init(Material m) 
    {
        //projectileMaterial = m; // defining parameters 
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Note: make sure top of heirachy child has the duration of the longest start life particle system so it plays out. 
    void Start()
    {
        if (muzzlePrefab != null)
        {
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;

            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>(); //hold the particle system of muzzle
            if (psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration); // destory when particle system is done. 
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (speed != 0)
        {
            transform.position += transform.forward * (speed * Time.deltaTime);

            // Safety Check to Disable Objects
            if (transform.position.y <= -100){
                Destroy(gameObject);
            }
        }
        else
        {
            Debug.Log("No Speed Defined!");
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("HIT: " + collision.gameObject.name);

        speed = 0;
        ContactPoint contact = collision.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if (hitPrefab != null)
        {
            //AudioManager.Instance.PlaySFX(crashSFX);
            var hitVFX = Instantiate(hitPrefab, pos, rot);

            var psVFX = hitVFX.GetComponent<ParticleSystem>(); //hold the particle system of muzzle
            
            if (psVFX != null)
            {
                Destroy(hitVFX, psVFX.main.duration); // destory when particle system is done. 
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }
        // Delete Projectile
        Destroy(gameObject);
        
        // Collision Handling 
        // if (collision.gameObject.CompareTag("Crystal")) {
        //     CrystalController crystal = collision.gameObject.GetComponent<CrystalController>();
        //     if (crystal != null && crystal.myMaterial == projectileMaterial)
        //     {
        //         // Update Score and Streak
        //         //ScoreManager.Instance.addScore(1);
        //         //ScoreManager.Instance.addStreak();
        //     }
        //     else {
        //         // if player hits the wrong crystal material, 
        //         // penalize by reseting streak => benefits
        //         //ScoreManager.Instance.resetStreak(); // line 95?
        //     }
        //     // Get rid of crystal no matter what
        //     Destroy(collision.gameObject);
        // }
    }
    
}
