using run_run;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.StandaloneInputModule;


[Serializable]
public class MonsterStat : MonoBehaviour
{
    public string _monster_name = String.Empty;
    public float _move_speed=10f;
    public float _pase_time=2f;
    public float _monster_life=3f;

    public void set_monster_stat(string name,float speed, float pase_time, float life)
    {
        this._monster_name = name;
        this._move_speed = speed;
        this._pase_time = pase_time;
        this._monster_life = life;

    }
}
public class MonsterController : MonsterStat
{
    int move_dir = 1;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            transform.GetComponent<Animator>().Play("Attack", -1, 0f);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Animator>().Play("Move", -1, 0f);
        move_dir = 1;
        //set_monster_stat();
        transform.localScale = Vector3.one;
        StartCoroutine(move_routine());
    }

    IEnumerator move_routine()
    {
        while (true)
        {
            transform.localPosition += new Vector3(move_dir*_move_speed, 0, 0);
            yield return new WaitForSeconds(.1f);
            _pase_time -= .1f;
            if(_pase_time <= 0)
            {
                if(move_dir==1)
                    move_dir = -1;
                else
                    move_dir = 1;
                transform.localScale = new Vector3(move_dir, 1, 1);
                _pase_time = 2f;
            }

        }
    }
}
