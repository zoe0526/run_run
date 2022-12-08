using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace run_run
{
    public class CharcterController : MonoBehaviour
    {
        float input_move = 0f;
        [SerializeField]
        private float move_speed = 5f;

        [SerializeField]
        private float jump_speed = 5f;

        [SerializeField]
        private GameManager _game_manager;


        private List<GameObject> _triggered_obj_list;  //Ʈ���ŵǼ� �ߵ��� ������Ʈ ����Ʈ. ������ ������Ʈ�� ��� ���� ����۽� �ʱ�ȭ���־���Ѵ�.
        byte _jump_cnt = 2;
        bool _is_moving = false;
        bool _is_game_end = false;
        Vector3 move_velocity = Vector3.zero;


        private void Start()
        {
            _triggered_obj_list = new List<GameObject>();
            init_game();
            if (_game_manager == null)
                _game_manager = GetComponent<GameManager>();
            set_gamepad_jump();
        }

        #region ���� ���۽� �ʱ�ȭ���� 
        
        //ĳ���� ������� �ʱ�ȭ���� ����
        private void init_game()
        {
            _jump_cnt = 2;
            _is_moving = false;
            _is_game_end = false;
            init_character();
            init_objs();
        }

        private void init_character()
        {
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            transform.GetComponent<Animator>().Play("Idle", -1, 0f);
            transform.localScale= Vector3.one;
            transform.GetComponent<CircleCollider2D>().enabled = true;
        }

        //ĳ���� ������� Ʈ���ŉ�� ������Ʈ�� �����
        private void init_objs()
        {
            foreach (GameObject obj in _triggered_obj_list)
            {
                if(obj.tag== "Hiding_Spike_Trap")
                {
                    obj.transform.Find("spikes").gameObject.SetActive(false);   //���ܾ� �Ǵ� Ʈ������ spikes �ؿ� ��ġ���ش�.
                }
                else if(obj.tag== "Diamond" || obj.tag=="SavePoint" || obj.tag=="EndPoint")
                {
                    obj.SetActive(true);   //ȹ���ߴ� �����ܵ� �ٽ� ���ش�.
                }
            }
        }
        #endregion

        private void OnTriggerEnter2D(Collider2D collision)
        {
            _triggered_obj_list.Add(collision.gameObject);
            collision.gameObject.SetActive(false);
            if (collision.gameObject.CompareTag("Diamond"))
            {
                _game_manager.collect_diamond();
            }
            if (collision.gameObject.CompareTag("SavePoint"))
            {

            }
            if (collision.gameObject.CompareTag("EndPoint"))
            {
                StartCoroutine(game_win(0, 0));

            }
            if (collision.gameObject.CompareTag("Coin"))
            {
                StatManager.Instance.coin_value++;
                PlayFabManager.Instance.SaveCoin(StatManager.Instance.coin_value);
                _game_manager.update_coin(StatManager.Instance.coin_value);

            }
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                _jump_cnt = 2;
            }
            if (collision.gameObject.CompareTag("Spike_Trap") || collision.gameObject.CompareTag("Hiding_Spike_Trap") || collision.gameObject.CompareTag("Monster"))
            {
                if(collision.gameObject.CompareTag("Hiding_Spike_Trap"))
                {
                    collision.gameObject.transform.Find("spikes").gameObject.SetActive(true);
                    _triggered_obj_list.Add(collision.gameObject);
                }
                if(collision.gameObject.CompareTag("Monster"))
                {
                    Debug.Log("hit monster");
                    character_nuck_back();

                }    
                StatManager.Instance.character_life_cnt--;
                _game_manager.update_life(StatManager.Instance.character_life_cnt);
                if(StatManager.Instance.character_life_cnt<=0)
                {
                    StartCoroutine(charac_dead());
                }

            }
            if (collision.gameObject.CompareTag("Fall_Ground"))
            {
                StartCoroutine(charac_dead());

            }
        }

        //ĳ���� ��,�� �̵� ����

        private int move_dir = 0;
        private void character_move()
        {
            input_move = Input.GetAxisRaw("Horizontal");

            if (input_move != 0 || GamePadController.Instance.is_left_move || GamePadController.Instance.is_right_move)
            {
                if (input_move < 0 || GamePadController.Instance.is_left_move)
                {
                    move_dir = -1;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (input_move > 0 || GamePadController.Instance.is_right_move)
                {
                    move_dir = 1;
                    transform.localScale = Vector3.one;
                }
                else
                {
                    move_dir = 0;
                    Debug.Log("�߸��� �̵� ��ǲ ����");
                }

                _is_moving = true;
                move_velocity = new Vector3(move_dir * move_speed*Time.deltaTime, 0, 0);
                transform.position += move_velocity;
                transform.GetComponent<Animator>().Play("Run");
            }

            if ((input_move==0 && !GamePadController.Instance.is_left_move && !GamePadController.Instance.is_right_move)
                && _is_moving)
            {
                _is_moving = false;
                transform.localScale = Vector3.one;
                transform.GetComponent<Animator>().Play("Idle");
            }

        }
        //ĳ���� ���� ����
        private void character_jump()
        {
            if ((Input.GetButtonDown("Jump") ) && _jump_cnt>0)
            {
                transform.GetComponent<Animator>().Play("Jump",-1,0f);
                _jump_cnt--;
                //transform.position += new Vector3(0,jump_speed,0);
                transform.GetComponent<Rigidbody2D>().AddForce(jump_speed * transform.up, ForceMode2D.Impulse);
            }
        }
        //ĳ���� ���� ������ ��
        private void character_nuck_back()
        {
            transform.GetComponent<Animator>().Play("Blink", -1, 0f);
            //transform.position += new Vector3(0,jump_speed,0);
            transform.GetComponent<Rigidbody2D>().AddForce(Vector2.left*10f, ForceMode2D.Impulse);
        }
        void set_gamepad_jump()
        {
            GamePadController.Instance._jump_btn.onClick.AddListener(() => { 
            
                if(_jump_cnt>0)
                {
                    transform.GetComponent<Animator>().Play("Jump", -1, 0f);
                    _jump_cnt--;
                    //transform.position += new Vector3(0,jump_speed,0);
                    transform.GetComponent<Rigidbody2D>().AddForce(jump_speed * transform.up, ForceMode2D.Impulse);
                }
            });
        }

        private IEnumerator charac_dead()
        {
            _is_game_end = true;
            transform.localScale = new Vector3(1, -1, 1);
            transform.GetComponent<Animator>().Play("Idle", -1, 0f);
            transform.GetComponent<CircleCollider2D>().enabled = false;
            yield return new WaitForSeconds(1f);
            _game_manager.game_over(() => {
                init_game();
            });
        }
        private IEnumerator game_win(long coin_value, int medal_type)
        {
            _is_game_end = true;
            transform.GetComponent<Animator>().Play("Idle_Win", -1, 0f);
            yield return new WaitForSeconds(1f);
            _game_manager.game_win(coin_value,medal_type, () => {

                //�ϴ��� ���� �� �ʱ�ȭ��. ���� ���� �� �̵�����
                init_game();
            });
        }



        private void FixedUpdate()
        {
            if(!_is_game_end)
                character_move();
        }
        private void Update()
        {
            if (!_is_game_end)
                character_jump();

        }
    }

}


