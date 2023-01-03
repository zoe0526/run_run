using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace run_run
{
    public class YellowCactus : MonsterController
    {
        public override void Awake()
        {
            base.Awake();
        }

        bool is_attacking = false;
        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);
            if (is_attacking)
                return;
            if (collision.collider.CompareTag("Player_foot") || collision.collider.CompareTag("Player"))
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
