using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    [SerializeField] private Queue<GameObject> pool = new Queue<GameObject>();

    public GameObject GetObject()
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.transform.position = Vector3.zero;
            obj.transform.rotation = Quaternion.identity;
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab);
        
        
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.position = Vector3.zero;
        obj.transform.rotation = Quaternion.identity;
        pool.Enqueue(obj);
    }
}
