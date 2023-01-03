using run_run;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
 
enum monster_type_enum
{
    Penguin,
    YellowCaptus
}
public class MonsterStatManager : MonoBehaviour
{
    public static MonsterStatManager Instance;
    private Dictionary<string, Monster> _monster_info_list;
    public Dictionary<string, Monster> monster_info_list
    {
        get { return _monster_info_list; }
    }

    [SerializeField]
    private Transform _monster_parent_pos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        create_monsters();
    }
    public void create_monsters()
    {
        _monster_info_list = new Dictionary<string, Monster>();
        monster_info_list.Add("Monster_1",new Monster() { _name = monster_type_enum.Penguin.ToString(), _mons_obj =null,_move_speed =10, _pase_time=3,_monster_life=1, _start_direction=1, _is_alive = true,_start_pos=new Vector2(340,0) });
        monster_info_list.Add("Monster_2", new Monster() { _name = monster_type_enum.YellowCaptus.ToString(), _mons_obj =null,_move_speed = 10, _pase_time = 3, _monster_life = 3, _start_direction = -1, _is_alive = true, _start_pos = new Vector2(1565, 0) });
        monster_info_list.Add("Monster_3", new Monster() { _name = monster_type_enum.Penguin.ToString(), _mons_obj = null, _move_speed = 10, _pase_time = 3, _monster_life = 1, _start_direction = 1, _is_alive = true, _start_pos = new Vector2(2750, 300) });


        foreach (var monster in monster_info_list)
        {
            MonsterController mons = PoolAllocater.Instance.get_pool_obj(monster.Value._name+"_Pool").GetComponent<MonsterController>();
            mons.gameObject.transform.SetParent(_monster_parent_pos);
            mons.gameObject.SetActive(true);
            mons.gameObject.transform.localScale = Vector3.one;
            mons.gameObject.transform.localPosition = monster.Value._start_pos;
            mons.gameObject.name = monster.Key;
            mons.set_monster_info(monster.Value, monster.Key);
            mons.init_monster();
            monster.Value._mons_obj = mons;
        }
    }
   
    public void set_monster_dead(string name)
    {
        monster_info_list[name]._mons_obj.Return();
        monster_info_list[name]._mons_obj = null;
    }
    public void restore_monster()
    {
        foreach(var monster in monster_info_list)
        {
            if (monster.Value._mons_obj == null)
            {
                MonsterController mons = PoolAllocater.Instance.get_pool_obj(monster.Value._name + "_Pool").GetComponent<MonsterController>();
                mons.gameObject.transform.SetParent(_monster_parent_pos);
                mons.gameObject.SetActive(true);
                mons.gameObject.transform.localScale = Vector3.one;
                mons.gameObject.transform.localPosition = monster.Value._start_pos;
                mons.gameObject.name = monster.Key;
                mons.set_monster_info(monster.Value, monster.Key);
                mons.init_monster();
                monster.Value._mons_obj = mons;
            }
            else
                monster.Value._mons_obj.set_collider_on();
        }
    }
}



public class Monster
{
    public string _name;
    public MonsterController _mons_obj;
    public float _move_speed;
    public float _pase_time;
    public byte _monster_life;
    public int _start_direction;
    public bool _is_alive;
    public Vector2 _start_pos;
}


