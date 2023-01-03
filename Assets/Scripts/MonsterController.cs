using run_run;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace run_run
{
    public enum MonsterAnim
    {
        Idle,
        Move,
        Jump,
        Dead,
        Damage,
        Attack,
        Squashed
    }
    public class MonsterController : PooledObj
    {
        private int start_dir = 1;   //1 : 왼쪽부터 시작 , -1 : 오른쪽부터 시작
        private int move_dir = 1;
        private float pase_time = 0f;
        private byte _life_cnt = 0;
        private Monster _monster_info;
        private string _monster_key;
        private IEnumerator move_coroutine;
        private Animator _monster_animator;
        private Collider2D _monster_collider;
        public run_run.CharcterController _player;
       
        
        public void set_monster_info(Monster monster_info, string key)
        {
            _monster_info = monster_info;
            _monster_key = key;
        }
        public void got_squashed(Action callback)
        {
            _player.squash_monster();
            StartCoroutine(monster_check_killed_sequence(MonsterAnim.Dead.ToString(), () => {
                _player.set_is_squashing(false);
                if(callback!=null)
                {
                    callback();
                    callback = null;
                }
            }));
        }
        public void got_stabbed(Action callback)
        {
            StartCoroutine(monster_check_killed_sequence(MonsterAnim.Damage.ToString(), () => {
                
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
            }));
        }
        public IEnumerator monster_check_killed_sequence(string anim_name, Action end_callback=null)
        {
            _monster_collider.enabled = false;

            StopCoroutine(move_coroutine);
            move_coroutine = null;
            _life_cnt--;
            if(_life_cnt<=0)
            {
                _monster_animator.Play(MonsterAnim.Dead.ToString(), -1, 0f);
                yield return new WaitForSeconds(.5f);
                MonsterStatManager.Instance.set_monster_dead(_monster_key);
            }
            else
            {
                _monster_animator.Play(anim_name ,- 1, 0f);
                yield return new WaitForSeconds(.5f);
                _monster_collider.enabled = true;
                move_coroutine = move_routine();
                StartCoroutine(move_coroutine);
            }

            if (end_callback != null)
            {
                end_callback();
                end_callback = null;
            }
        }
   

        private IEnumerator monster_attack_sequence(Action end_callback=null)
        {
            _monster_animator.Play(MonsterAnim.Attack.ToString(), -1, 0f);
            StopCoroutine(move_coroutine);
            move_coroutine = null;
            yield return new WaitForSeconds(.5f);
            transform.localScale = new Vector3(move_dir, 1, 1);
            move_coroutine = move_routine();
            StartCoroutine(move_coroutine);
            if(end_callback!=null)
            {
                end_callback();
                end_callback = null;
            }

        }
        public virtual void Awake()
        {
            _monster_animator = transform.GetComponent<Animator>();
            _monster_collider = transform.GetComponent<Collider2D>();
        }
        void Start()
        {
            _player = FindObjectOfType<run_run.CharcterController>();
            
        }
        public void init_monster()
        {
            start_dir = _monster_info._start_direction;
            move_dir = 1;
            transform.localScale = Vector3.one;
            _monster_collider.enabled = true;
            pase_time = _monster_info._pase_time;
            transform.localPosition = _monster_info._start_pos;
            _life_cnt = _monster_info._monster_life;
            gameObject.SetActive(true);
            move_coroutine = move_routine();
            StartCoroutine(move_coroutine);

        }
        public void set_collider_on()
        {
            _monster_collider.enabled = true;
        }

        IEnumerator move_routine()
        {
            _monster_animator.Play(MonsterAnim.Move.ToString(), -1, 0f);
            while (true)
            {
                transform.localPosition += new Vector3(move_dir * _monster_info._move_speed *start_dir, 0, 0);
                yield return new WaitForSeconds(.1f);
                pase_time -= .1f;
                if (pase_time <= 0)
                {
                    move_dir *= -1;
                    transform.localScale = new Vector3(move_dir, 1, 1);
                    pase_time = _monster_info._pase_time;
                }

            }
        }

        public virtual void OnCollisionEnter2D(Collision2D collision)
        {


        }
        bool is_attacked = false;
        public virtual void OnTriggerEnter2D(Collider2D collision)
        {
            if (is_attacked)
                return;
            if (collision.CompareTag("Sword"))
            {
                is_attacked = true;
                _player.set_attack_ptcl();
                got_stabbed(() => {
                    is_attacked = false;
                });

            }
        }

        public void set_monster_attack(Action attack_end) 
        {
            _player.character_nuck_back();
            if (transform.position.x < _player.transform.position.x)
                transform.localScale = new Vector3(start_dir, 1, 1);
            else
                transform.localScale = new Vector3(-1*start_dir, 1, 1);
            StartCoroutine(monster_attack_sequence(() => {
                _player.set_is_attacked(false);
                attack_end();
            }));
        }
    }


}
