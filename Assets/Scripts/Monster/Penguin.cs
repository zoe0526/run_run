using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace run_run
{
    public class Penguin : MonsterController
    {
        public override void Awake()
        {
            base.Awake();
        }

        bool checking_hit = false;
        bool is_attacking = false;
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            if (checking_hit || is_attacking)
                return;
            if (collision.collider.CompareTag("Player_foot"))
            {
                checking_hit = true;
                got_squashed(() => {
                    checking_hit = false;
                
                });
            }
            else if (collision.collider.CompareTag("Player"))
            {
                is_attacking = true;
                set_monster_attack(() => {
                    is_attacking = false;

                });
            }
        }

        public override void OnTriggerEnter2D(Collider2D collision)
        {
            base.OnTriggerEnter2D(collision);
        }
    }


}
