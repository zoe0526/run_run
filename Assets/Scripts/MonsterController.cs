using run_run;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace run_run
{
    public class MonsterController : MonoBehaviour
    {
        private int move_dir = 1;
        private float pase_time = 0f;
        private byte _life_cnt = 0;
        private Monster _monster_info;
        private string _monster_key;
        private IEnumerator move_coroutine;
        private Animator _monster_animator;
        private Collider2D _monster_collider;
        private run_run.CharcterController _player;
        public void set_monster_info(Monster monster_info, string key)
        {
            _monster_info = monster_info;
            _monster_key = key;
        }
        public IEnumerator monster_check_killed_sequence(Action end_callback=null)
        {
            _player.squash_monster();
            _monster_collider.enabled = false;

            StopCoroutine(move_coroutine);
            move_coroutine = null;
            _life_cnt--;
            if(_life_cnt<=0)
            {
                _monster_animator.Play("Dead", -1, 0f);
                yield return new WaitForSeconds(.5f);
                MonsterStatManager.Instance.set_monster_dead(_monster_key);
            }
            else
            {
                _monster_animator.Play("Damage", -1, 0f);
                yield return new WaitForSeconds(.5f);
                _monster_collider.enabled = true;
                move_coroutine = move_routine();
                StartCoroutine(move_coroutine);
            }
            _player.set_is_squashing(false);
            if (end_callback != null)
            {
                end_callback();
                end_callback = null;
            }
        }

        public IEnumerator monster_attack_sequence()
        {
            _monster_animator.Play("Attack", -1, 0f);
            StopCoroutine(move_coroutine);
            move_coroutine = null;
            yield return new WaitForSeconds(.1f);
            transform.localScale = new Vector3(move_dir, 1, 1);
            move_coroutine = move_routine();
            StartCoroutine(move_coroutine);

        }
        private void Awake()
        {
            _player = FindObjectOfType<run_run.CharcterController>();
        }
        void Start()
        {
            move_dir = 1;
            _monster_animator = transform.GetComponent<Animator>();
            _monster_collider = transform.GetComponent<Collider2D>();
            transform.localScale = Vector3.one;
            init_monster();
            move_coroutine = move_routine();
            StartCoroutine(move_coroutine);
        }
        private void init_monster()
        {
            _monster_collider.enabled = true;
            pase_time = _monster_info._pase_time;
            transform.localPosition = _monster_info._start_pos;
            _life_cnt = _monster_info._monster_life;
            gameObject.SetActive(true);
        }

        IEnumerator move_routine()
        {
            _monster_animator.Play("Move", -1, 0f);
            while (true)
            {
                transform.localPosition += new Vector3(move_dir * _monster_info._move_speed, 0, 0);
                yield return new WaitForSeconds(.1f);
                pase_time -= .1f;
                if (pase_time <= 0)
                {
                    if (move_dir == 1)
                        move_dir = -1;
                    else
                        move_dir = 1;
                    transform.localScale = new Vector3(move_dir, 1, 1);
                    pase_time = _monster_info._pase_time;
                }

            }
        }

        bool checking_hit = false;
        public virtual void OnCollisionEnter2D(Collision2D collision)
        {
            if (checking_hit)
                return;
            if (collision.collider.CompareTag("Player_foot"))
            {
                Debug.Log("player_foot Á¢ÃË");
                checking_hit = true;
                StartCoroutine(monster_check_killed_sequence(() => {

                    checking_hit = false;
                }));
            }
            else if (collision.collider.CompareTag("Player"))
            {
                Debug.Log("player Á¢ÃË");
                checking_hit = false;
                _player.character_nuck_back();
                if (transform.position.x > collision.gameObject.transform.position.x)
                    transform.localScale = new Vector3(-1, 1, 1);
                else
                    transform.localScale = new Vector3(1, 1, 1);
                StartCoroutine(monster_attack_sequence());
            }

        }
    }


}
