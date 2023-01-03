using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolAllocater : PoolManager
{
    private static PoolAllocater instance;
    public static  PoolAllocater Instance
    { get { return instance; } }


    [SerializeField]
    public PoolObjectInfo[] _pool_obj_arr;

    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance);

        set_parent(transform);
    }
    private void Start()
    {
        install_pools();
    }
    private void  install_pools()
    {
        if (_pool_obj_arr == null)
            return;
        for(int i=0; i<_pool_obj_arr.Length;i++)
        {
            if (_pool_obj_arr[i]._prefab == null)   //생성할 오브젝트가 할당되지 않았을 경우 리턴
                return;

            install_pool(_pool_obj_arr[i]._obj_name,_pool_obj_arr[i]._prefab,_pool_obj_arr[i]._min_cnt,_pool_obj_arr[i]._max_cnt);
        }
        
    }

    private void OnDestroy()
    {
        instance = null;
        if(_pool_obj_arr.Length>0)
        {
            for(int i=0; i<_pool_obj_arr.Length;i++)
            {
                uninstall_pool(_pool_obj_arr[i]._obj_name);
            }
        }
        
    }

}
