using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledObj : MonoBehaviour
{
    private ObjectPool _pool;
    private Transform _my_transform;
    protected string _obj_name;
    public void Return()
    {
        try
        {
            _pool.return_to_pool(this);
        }
        catch
        {
            Destroy(gameObject);
        }

    }
    public void Reset(Transform parent_pos, ObjectPool pool=null)
    {
        if(pool!=null)
        {
            _pool = pool;
            if (_my_transform == null)
                _my_transform = transform;
        }
        _my_transform.SetParent(parent_pos);
        _my_transform.localPosition = Vector3.zero;
        _my_transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }

    public void set_name(string name)
    {
        _obj_name = name;
    }
}


public class ObjectPool : MonoBehaviour
{
    private GameObject _prefab_obj;
    private int _min_cnt;
    private int _max_cnt;
    private Transform _my_transform;

    public Queue<PooledObj> _obj_queue;

    public void Initialize_pool(GameObject prefab, int min_cnt,int max_cnt)
    {
        _my_transform=transform;
        _prefab_obj = prefab;
        _min_cnt = min_cnt;
        _max_cnt = max_cnt;
        _obj_queue = new Queue<PooledObj>();
        GameObject g_obj;
        PooledObj p_obj;
        for(int i=0; i<min_cnt;i++)
        {
            g_obj = Instantiate(prefab);
            p_obj = g_obj.GetComponent<PooledObj>();
            if (p_obj == null)
                p_obj = g_obj.AddComponent<PooledObj>();

            p_obj.set_name(prefab.name);

            p_obj.Reset(_my_transform, this);
            _obj_queue.Enqueue(p_obj);
        }

    }



    public void uninstall_pool()
    {
        if (_obj_queue == null || gameObject==null)
            return;

        PooledObj p_obj;
        while(_obj_queue.Count>0)
        {
            p_obj = _obj_queue.Dequeue();
            Destroy(p_obj);
        }
        _obj_queue = null;

        Destroy(gameObject);
    }


    //max큐보다 많이 생성된 오브젝트들은 제거한다.
    public void return_to_pool(PooledObj obj)
    {
        if(_obj_queue.Count<_max_cnt)
        {
            obj.Reset(_my_transform);
            _obj_queue.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
        else
        {
            Destroy(obj);
        }
    }


    public GameObject get_pool_obj()
    {
        GameObject g_obj;
        PooledObj p_obj;
        if(_obj_queue.Count>0)
        {
            p_obj = _obj_queue.Dequeue();
            if (p_obj != null)
                return p_obj.gameObject;

        }   //미리 풀링된 오브젝트가 있을 경우

        //풀링된 오브젝트가 없을경우 추가 생성해준다.

        g_obj = Instantiate(_prefab_obj);

        p_obj = g_obj.GetComponent<PooledObj>();
        if (p_obj == null)
            p_obj= g_obj.AddComponent<PooledObj>();
        p_obj.set_name(_prefab_obj.name);
        p_obj.Reset(_my_transform,this);
        return p_obj.gameObject;

    }
}
