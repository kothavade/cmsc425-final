using UnityEngine;
using UnityEngine.U2D;

public class GravLauncherHandler : MonoBehaviour
{

    private PublicMover Player;
    public AmmoTracker Weapon;
    public GameObject Bullet;
    public GameObject GravField;
    public GameObject AltBuller;
    public GameObject AntiGravField;
    public Transform ProjectileSpawner;

    void Start(){
        Player = GameObject.Find("Player").GetComponent<PublicMover>();
        // ShotgunPellet = GameObject.Find("ShotgunPellet");
        // ProjectileSpawner = GameObject.Find("GravLauncherProjectileSpawner").transform;
        Weapon.onEnemyHitEffect = (float x) => {};
        Weapon.onHitEffect = (Vector3 pos) => {
            print("Hit!");
            print("Spawning" + GravField+ " at "+gameObject.transform.position);
            GameObjectPoolManager.SpawnObject(GravField, pos, Quaternion.identity);
        };
    }

    // Update is called once per frame
    void Update()
    {
        // shoots the gun
        if (Input.GetKeyDown(KeyCode.Mouse0) /*&& Weapon.getAmmo() > 0*/) {
            Fire();
        }
        
    }


    // consolidates all functions called upon firing
    void Fire(){
        //Weapon.fireAmmo(1);
        GameObjectPoolManager.SpawnObject(Bullet, ProjectileSpawner.position, ProjectileSpawner.rotation);
    }
}
