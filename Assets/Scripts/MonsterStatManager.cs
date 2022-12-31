using run_run;
using System.Collections;
using System.Collections.Generic;
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
    private GameObject[] _monster_prefab_list;
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
        monster_info_list.Add("Monster_1",new Monster() { _name = monster_type_enum.Penguin.ToString(), _mons_obj =null,_move_speed =10, _pase_time=3,_monster_life=1,_is_alive=true,_start_pos=new Vector2(340,0) });
        monster_info_list.Add("Monster_2", new Monster() { _name = monster_type_enum.YellowCaptus.ToString(), _mons_obj =null,_move_speed = 10, _pase_time = 3, _monster_life = 3, _is_alive = true, _start_pos = new Vector2(1290, 0) });
        monster_info_list.Add("Monster_3", new Monster() { _name = monster_type_enum.Penguin.ToString(), _mons_obj = null, _move_speed = 10, _pase_time = 3, _monster_life = 1, _is_alive = true, _start_pos = new Vector2(2835, 300) });


        foreach (var monster in monster_info_list)
        {
            for(int i=0; i<_monster_prefab_list.Length;i++)
            {
                if (_monster_prefab_list[i].name==monster.Value._name)
                {
                    GameObject mons= Instantiate(_monster_prefab_list[i]) as GameObject;
                    mons.transform.SetParent(_monster_parent_pos);
                    mons.transform.localScale = Vector3.one;
                    mons.transform.localPosition = monster.Value._start_pos;
                    mons.name=monster.Key;
                    mons.GetComponent<MonsterController>().set_monster_info(monster.Value , monster.Key);
                    monster.Value._mons_obj = mons;

                }
            }
        }
    }
    public void set_monster_dead(string name)
    {
        Destroy(monster_info_list[name]._mons_obj);
        monster_info_list.Remove(name);
    }
}



public class Monster
{
    public string _name;
    public GameObject _mons_obj;
    public float _move_speed;
    public float _pase_time;
    public byte _monster_life;
    public bool _is_alive;
    public Vector2 _start_pos;
}


