using UnityEngine;
using UnityEngine.U2D;

public class ShotgunHandler : MonoBehaviour
{

    private PublicMover Player;
    private AmmoTracker Weapon;
    public GameObject ShotgunPellet;
    private Transform ProjectileSpawner;
    public float spread;
    public float recoil_intensity;
    private bool hasTouchedGround = false;
    public AudioClip fireSound; 

    void Start(){
        Player = GameObject.Find("Player").GetComponent<PublicMover>();
        Weapon = GameObject.Find("Shotgun").GetComponent<AmmoTracker>();
        // ShotgunPellet = GameObject.Find("ShotgunPellet");
        ProjectileSpawner = GameObject.Find("ShotgunProjectileSpawner").transform;
        Weapon.onHitEffect = (int a) => {if (a == -1) { Weapon.reload(1); }};
    }

    // Update is called once per frame
    void Update()
    {
        // shoots the gun
        if (Input.GetKeyDown(KeyCode.Mouse0) && Weapon.getAmmo() > 0) {
            Fire();
        }

        // checks if the player has touched the ground
        if (Player.isGrounded()){
            hasTouchedGround = true;
        }

        /*  
        *   The shotgun is intended to also act as the player's double jump, so
        *   it can't be reloaded until the player has touched the ground.
        *
        *   Once non-instant reload animations are added, consider reloading
        *   automatically on landing.
        */

        // Reloads the gun if the player has touched the ground since they last fired a shot
        if (Input.GetKeyDown(KeyCode.R) && hasTouchedGround) {
            Weapon.reload();
        }
    }


    // consolidates all functions called upon firing
    void Fire()
    {
        HandleFireEffects();
        
        Weapon.fireAmmo(1);
        Debug.Log("Spawning Pellets");
        for ( int i = 0; i < 8; i++)
        {
            GameObject pellet = GameObjectPoolManager.SpawnObject(ShotgunPellet, ProjectileSpawner.position, 
                ProjectileSpawner.rotation * Quaternion.Euler(Random.Range(-spread, spread),
                Random.Range(-spread, spread), Random.Range(-spread, spread)));
    
            Debug.Log($"Pellet {i} spawned: {pellet != null}, Position: {pellet?.transform.position}");
            hasTouchedGround = false;
        }

    }

    void HandleFireEffects()
    {
        ApplyRecoil();
        if (fireSound != null)
        {
            AudioSource.PlayClipAtPoint(fireSound, transform.position);
        }
    }
    // applies the recoil force of the shotgun
    void ApplyRecoil(){
        Player.ApplyImpulse(-transform.parent.forward, recoil_intensity);
    }
}
