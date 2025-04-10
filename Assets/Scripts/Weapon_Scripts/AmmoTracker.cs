using UnityEngine;

public class AmmoTracker : MonoBehaviour
{

    public int max_ammo;
    private int curr_ammo;
    public delegate void OnHitEffect(int dmg);
    public OnHitEffect onHitEffect;
    void Start(){
        curr_ammo = max_ammo;
    }

    public void onHit(int x) {
        onHitEffect(x);
    }

    public int getAmmo(){
        return curr_ammo;
    }

    // reduces ammo by a given amount, or by whatever is left. Returns the amount of ammo fired.
    public int fireAmmo(int amount){
        int ammo_fired = curr_ammo;
        if (curr_ammo > amount) {
            ammo_fired = amount;
        } 
        curr_ammo -= ammo_fired;
        return ammo_fired;
    }

    // sets curr_ammo to max_ammo
    public void reload(){
        curr_ammo = max_ammo;
    }

    // overloads reload(). Increases curr_ammo by a given amount, up to max_ammo.
    public void reload(int amnt){
        curr_ammo = Mathf.Min(curr_ammo + amnt, max_ammo);
    }
}
