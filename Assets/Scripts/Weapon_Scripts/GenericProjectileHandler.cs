using Unity.VisualScripting;
using UnityEngine;

public class GenericProjectileHandler : MonoBehaviour
{
    Rigidbody rb;
    public float projectile_speed;
    public float max_lifetime;
    public int damage;
    public AmmoTracker SourceWeapon;
    private float lifetime;

    void Start() {
        lifetime = max_lifetime;
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    // Update is called once per frame

    void OnEnable(){
        lifetime = max_lifetime;
    }
    void Update()
    {
        lifetime = lifetime - Time.deltaTime;
        if(lifetime <= 0f){
            lifetime = max_lifetime;
            GameObjectPoolManager.Deactivate(gameObject);
        }
    }

    void FixedUpdate(){
        rb.MovePosition(transform.position + transform.forward * projectile_speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision col){
        int enemy_hp = -2;
        Enemy enemy = col.gameObject.GetComponent<Enemy>();
        if (enemy != null) {
            enemy_hp = enemy.TakeDamage(damage);
            print(gameObject.name + " hit " + enemy.name + " for " + enemy_hp +" health left!");
        }
        SourceWeapon.onHit(enemy_hp);
        GameObjectPoolManager.Deactivate(gameObject);
    }
}
