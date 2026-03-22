using System;
using System.Collections.Generic;
using SanyaBeerExtension;
using UnityEngine;
using UnityEngine.Pool;

public enum PoolType {
    MoneyNearCube,
    RolledMoney,
    Boost, 
    Trap,
    LocationObject,
    Props
}

public class ObjectPoolManager : MonoBehaviour {
    
    private GameObject _emptyHolder;
    private GameObject _moneyParent;
    private GameObject _boostParent;
    private GameObject _trapsParent;
    private GameObject _locationObject;

    
    private Dictionary<GameObject, ObjectPool<GameObject>> _objectPoolsDict;
    private Dictionary<GameObject, GameObject> _cloneToPrefabMap;

    
    
    private void Awake() {
        _objectPoolsDict = new();
        _cloneToPrefabMap = new();
        SetupEmpties();
    }

    
    public T Spawn<T>(GameObject prefab, Vector3 pos, PoolType poolType) where T : Component {
        if (!_objectPoolsDict.TryGetValue(prefab, out var pool)) {
            CreatePool(prefab);
            pool = _objectPoolsDict[prefab];
        }
        var obj = pool.Get();
        obj.transform.position = pos;
        _cloneToPrefabMap[obj] = prefab;
        obj.transform.SetParent(GetParent(poolType), false);
        
        
        obj.ActiveSelf();
        return obj.GetComponent<T>();
    }

    public void ReturnObjectToPool(GameObject obj, PoolType poolType) {
        if (_cloneToPrefabMap.TryGetValue(obj, out GameObject prefab)) {
            var parent = GetParent(poolType);
            if (obj.transform.parent != parent) {
                obj.transform.SetParent(parent);
            }

            if (_objectPoolsDict.TryGetValue(prefab, out var pool)) {
                pool.Release(obj);
            }
        }
        else {
            Debug.LogError("Обьект не найден при возвращении в пул");
        }
    }
    
    
    
    private Transform GetParent(PoolType poolType) {
        switch (poolType) {
            case PoolType.MoneyNearCube:
                return _moneyParent.transform;
            case PoolType.RolledMoney:
                return _moneyParent.transform;
            case PoolType.Boost :
                return _boostParent.transform;
            case PoolType.LocationObject :
                return _locationObject.transform;
            case PoolType.Props:
                return _locationObject.transform;
            default:
                return _trapsParent.transform;;
        }
    }
    
    
    private void SetupEmpties() {
        _emptyHolder = new GameObject("Object Pool");

        _moneyParent = new GameObject("Money Near Cube Pool");
        _moneyParent.transform.SetParent(_emptyHolder.transform);
        
        
        _boostParent = new GameObject("Boost pool");
        _boostParent.transform.SetParent(_emptyHolder.transform);
        
        _trapsParent = new GameObject("Traps Pool");
        _trapsParent.transform.SetParent(_emptyHolder.transform);
        
        _locationObject = new GameObject("Location Objects");
        _locationObject.transform.SetParent(_emptyHolder.transform);
    }

    private void CreatePool(GameObject prefab) {
        ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
            createFunc:() => CreateObject(prefab),
            actionOnGet: OnGetObject,
            actionOnRelease: OnRealeseObject,
            actionOnDestroy: OnDestroyObject
        );
        _objectPoolsDict.Add(prefab, newPool);
    }
    
    private GameObject CreateObject(GameObject prefab) {
        prefab.DisactiveSelf();
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        prefab.ActiveSelf();
        return obj;
    }
    
    
    private void OnGetObject(GameObject obj) {
        
    }
    
    private void OnRealeseObject(GameObject obj) {
        obj.DisactiveSelf();
    }
    
    private void OnDestroyObject(GameObject obj) {
        _cloneToPrefabMap.Remove(obj);
    }


    
   
    
    
}
