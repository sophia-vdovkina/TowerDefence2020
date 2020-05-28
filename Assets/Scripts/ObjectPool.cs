using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    /// <summary>
    /// Массив префабов, которые используются для создания объектов в мире
    /// </summary>
    [SerializeField]
    private GameObject[] objectPrefabs;

    private List<GameObject> pooledObjects = new List<GameObject>();

    /// <summary>
    /// Получает объект из пулa
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetObject(string type)
    {
        foreach (GameObject go in pooledObjects)
        {
            if (go.name == type && !go.activeInHierarchy)
            {
                go.SetActive(true);
                return go;
            }
        }
        // если в пуле не было нужного объекта
        for (int i = 0; i < objectPrefabs.Length; i++)
        {
            // если у нас есть префаб для создания этого объекта
            if (objectPrefabs[i].name == type)
            {
                GameObject newObject = Instantiate(objectPrefabs[i]);
                newObject.name = type;
                pooledObjects.Add(newObject);
                return newObject;
            }
        }
        return null;
    }

    public void ReleaseObject(GameObject gameObject)
    {
        gameObject.SetActive(false);
    }
}
