using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// https://www.youtube.com/watch?v=9O7uqbEe-xc

// Object Pool Manager class -- list of object pools and associated functions to avoid
// instantiating a bunch of identical objects all the time for no reason

public class GameObjectPoolManager : MonoBehaviour
{

    private static GameObject _poolObjects;

    private void Awake(){
        _poolObjects = new GameObject("Pooled Objects");
    }

    // list of object pools
    public static List<ObjectPool> PoolList = new List<ObjectPool>();

    // replaces Instantiate() -- pulls an inactive object from a given pool. If no such pool
    // exists, create one. If no active object exists, Instantiate one.
    public static GameObject SpawnObject(GameObject spawn_target, Vector3 pos, Quaternion rot) {
        
        // Find the given pool. If it does not exist, create it and add it to PoolList
        ObjectPool pool = PoolList.Find(p => p.pool_name == spawn_target.name);

        if (pool == null) {
            pool = new ObjectPool() { pool_name = spawn_target.name };
            PoolList.Add(pool);
        }

        // search for an inactive object in the pool
        GameObject to_spawn = null;
        foreach (GameObject obj in pool.InactiveObjects) {
            if (obj != null) {
                to_spawn = obj;
                break;
            }
        }

        // if there are no inactive objects, instantiate a new one. Otherwise, set the transform and activate it.
        if (to_spawn == null) {
            to_spawn = Instantiate(spawn_target, pos, rot);
            to_spawn.transform.SetParent(_poolObjects.transform);
            to_spawn.SetActive(true);
        } else {
            to_spawn.transform.position = pos;
            to_spawn.transform.rotation = rot;
            to_spawn.SetActive(true);
            pool.InactiveObjects.Remove(to_spawn);
        }

        return to_spawn;
    }

    // replaces Destroy(), deactivates an object and returns it to the inactive pool
    public static bool Deactivate(GameObject obj) {
        ObjectPool pool = PoolList.Find(p => p.pool_name+ "(Clone)" == obj.name ); // newly instantiated objects will all be clones

        if (pool == null) {
            Debug.LogWarning("You're trying to deactivate an object with no pool:" + obj.name);
            return false;
        } else {
            obj.SetActive(false);
            pool.InactiveObjects.Add(obj);
            return true;
        }
    }

}


// Object Pool class that maintains a list of inactive objects that can be activated
// instead of instantiating new ones
public class ObjectPool {
    public string pool_name;
    public List<GameObject> InactiveObjects = new List<GameObject>();
}
