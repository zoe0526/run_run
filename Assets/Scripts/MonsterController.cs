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
    float pase_time=0f;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(transform.position.x>collision.gameObject.transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else
                transform.localScale = new Vector3(1, 1, 1);
            StartCoroutine(monster_attack_sequence());
        }

    }
    IEnumerator monster_attack_sequence()
    {
        transform.GetComponent<Animator>().Play("Attack", -1, 0f);
        StopCoroutine(move_coroutine);
        move_coroutine = null;
        yield return new WaitForSeconds(.1f);
        transform.localScale = new Vector3(move_dir, 1, 1);
        move_coroutine = move_routine();
        StartCoroutine(move_coroutine);

    }

    IEnumerator move_coroutine;
    // Start is called before the first frame update
    void Start()
    {
        move_dir = 1;
        //set_monster_stat();
        transform.localScale = Vector3.one;
        move_coroutine = move_routine();
        StartCoroutine(move_coroutine);
        pase_time = _pase_time;
    }

    IEnumerator move_routine()
    {
        transform.GetComponent<Animator>().Play("Move", -1, 0f);
        while (true)
        {
            transform.localPosition += new Vector3(move_dir*_move_speed, 0, 0);
            yield return new WaitForSeconds(.1f);
            pase_time -= .1f;
            if(pase_time <= 0)
            {
                if(move_dir==1)
                    move_dir = -1;
                else
                    move_dir = 1;
                transform.localScale = new Vector3(move_dir, 1, 1);
                pase_time = _pase_time;
            }

        }
    }
}
