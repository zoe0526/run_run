using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public  class PoolObjectInfo
{
    public string _obj_name;    //prefab name +"_Pool"
    public GameObject _prefab;
    public int _min_cnt;
    public int _max_cnt;    //최대 풀 크기. 초과해도 오브젝트 생성은 되나, 사용 후 폐기된다.
}



public class PoolManager : MonoBehaviour
{
    private Transform _root_pos;    //오브젝트 풀들 생성할 위치
    private Dictionary<string, ObjectPool> _pool;


    //pool 생성 위치를 지정해준다.
    public void set_parent(Transform parent_root)
    {
        _root_pos = parent_root;
        _pool = new Dictionary<string, ObjectPool>();
    }

    public void install_pool(string pool_name, GameObject prefab ,int min_pool_num, int max_pool_num)
    {
        if (_pool.ContainsKey(pool_name))
            return;

        GameObject pool_container = new GameObject();
        pool_container.transform.SetParent(_root_pos);
        pool_container.transform.localScale = Vector3.one;
        pool_container.transform.localPosition = Vector3.zero;
        pool_container.name = pool_name;

        _pool.Add(pool_name, pool_container.AddComponent<ObjectPool>());
        _pool[pool_name].Initialize_pool(prefab,min_pool_num,max_pool_num);

    }

    //풀링된 오브젝트들을 삭제한다.
    public void uninstall_pool(string pool_name)
    {
        if (!_pool.ContainsKey(pool_name) || _pool == null)
            return;

        _pool[pool_name].uninstall_pool();


        _pool.Remove(pool_name);
    }

    public GameObject get_pool_obj(string pool_name)
    {
        GameObject p_obj = null;
        if(!_pool.ContainsKey(pool_name))
        {
            Debug.LogWarning("no pool available");
            return null;
        }


        p_obj = _pool[pool_name].get_pool_obj();

        return p_obj;

    }

    public void on_Click_btn()
    {

    }
}
