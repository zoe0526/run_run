using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace run_run
{
    public enum PlayerAnim
    {
        Idle,
        Run,
        Jump,
        Idle_Win,
        Damage,
        Attack,
        Dead
    }
    public class CharcterController : MonoBehaviour
    {
        float input_move = 0f;
        [SerializeField]
        private float move_speed = 5f;

        [SerializeField]
        private float jump_speed = 5f;
        [SerializeField]
        private Collider2D _body_collider;
        [SerializeField]
        private Collider2D _foot_collider;

        private GameManager _game_manager;
        private Animator _player_anim;
        private Rigidbody2D _player_rigidbody;

        private List<GameObject> _triggered_obj_list;  //트리거되서 발동된 오브젝트 리스트. 삭제된 오브젝트일 경우 게임 재시작시 초기화해주어야한다.
        byte _jump_cnt = 2;
        bool _is_moving = false;
        bool _is_game_end = false;
        bool _is_squashing = false;
        bool _is_attacked = false;
        Vector3 move_velocity = Vector3.zero;
        private void Awake()
        {
            _triggered_obj_list = new List<GameObject>();
            if (_game_manager == null)
                _game_manager = FindObjectOfType<GameManager>();
            _player_anim = transform.GetComponent<Animator>();
            _player_rigidbody=transform.GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            init_game();
            //set_gamepad_jump();
            set_gamepad_attack();


        }

        #region 게임 시작시 초기화값들 
        
        //캐릭터 재생성시 초기화값들 셋팅
        private void init_game()
        {
            _game_manager._camera_controller.enabled = true;
            _jump_cnt = 2;
            _is_moving = false;
            _is_game_end = false;
            _is_squashing = false;
            _is_attacked = false;
            init_character();
            init_objs();
            StatManager.Instance.earn_coin = 0;
            StatManager.Instance.collected_diamond_cnt = 0;
        }

        private void init_character()
        {
            transform.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            _player_anim.Play(PlayerAnim.Idle.ToString());
            transform.localScale= Vector3.one;
            _body_collider.enabled = true;
            _foot_collider.enabled = true;
        }

        //캐릭터 재생성시 트리거됬던 오브젝트들 재셋팅
        private void init_objs()
        {
            foreach (GameObject obj in _triggered_obj_list)
            {
                if(obj.tag== "Hiding_Spike_Trap")
                {
                    obj.transform.Find("spikes").gameObject.SetActive(false);   //숨겨야 되는 트랩들은 spikes 밑에 배치해준다.
                }
                else// if(obj.tag== "Diamond" || obj.tag=="SavePoint" || obj.tag=="EndPoint")
                {
                    obj.SetActive(true);   //획득했던 아이콘들 다시 켜준다.
                }
            }
        }
        #endregion
       
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (_triggered_obj_list.Contains(collision.gameObject))
                return;
            if (collision.gameObject.CompareTag("Diamond"))
            {
                _triggered_obj_list.Add(collision.gameObject);
                collision.gameObject.SetActive(false);
                _game_manager.collect_diamond();
            }
            else if (collision.gameObject.CompareTag("SavePoint"))
            {

                _triggered_obj_list.Add(collision.gameObject);
                collision.gameObject.SetActive(false);
            }
            else if (collision.gameObject.CompareTag("EndPoint"))
            {
                _triggered_obj_list.Add(collision.gameObject);
                collision.gameObject.SetActive(false);
                StartCoroutine(game_win());

            }
            else if (collision.gameObject.CompareTag("Coin"))
            {
                _triggered_obj_list.Add(collision.gameObject);
                collision.gameObject.SetActive(false);
                StatManager.Instance.earn_coin++;
                StatManager.Instance.total_coin_value++;
                PlayFabManager.Instance.SaveCoin(StatManager.Instance.total_coin_value);
                _game_manager.update_coin(StatManager.Instance.total_coin_value);

            }
            else if(collision.gameObject.CompareTag("Heart"))
            {
                _triggered_obj_list.Add(collision.gameObject);
                collision.gameObject.SetActive(false);
                if (StatManager.Instance.character_life_cnt<StatManager._max_character_life_cnt)
                {
                    StatManager.Instance.character_life_cnt++;
                    Debug.Log("###########Heart");
                    _game_manager.update_life(StatManager.Instance.character_life_cnt);
                }
            }
            else if (collision.gameObject.CompareTag("Monster"))
            {
                //_game_manager.set_attack_ptcl(collision.gameObject.transform);
            }

        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_is_attacked)
                return;
            if (collision.gameObject.CompareTag("Ground"))
            {
                _jump_cnt = 2;
                _player_rigidbody.velocity = Vector2.zero;
            }
            if (collision.gameObject.CompareTag("Spike_Trap") || collision.gameObject.CompareTag("Hiding_Spike_Trap"))
            {
                if(collision.gameObject.CompareTag("Hiding_Spike_Trap"))
                {
                    collision.gameObject.transform.Find("spikes").gameObject.SetActive(true);
                    _triggered_obj_list.Add(collision.gameObject);
                    StartCoroutine( steped_spike());
                }


            }
            if (collision.gameObject.CompareTag("Fall_Ground"))
            {
                StartCoroutine(charac_dead());

            }
        }

        //캐릭터 좌,우 이동 구현

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
                    Debug.Log("잘못된 이동 인풋 감지");
                }

                _is_moving = true;
                move_velocity = new Vector3(move_dir * move_speed*Time.deltaTime, 0, 0);
                transform.position += move_velocity;
                _player_anim.Play(PlayerAnim.Run.ToString());
            }

            if ((input_move==0 && !GamePadController.Instance.is_left_move && !GamePadController.Instance.is_right_move)
                && _is_moving)
            {
                _is_moving = false;
                transform.localScale = Vector3.one;
                _player_anim.Play(PlayerAnim.Idle.ToString());
            }

        }
        //캐릭터 점프 구현
        private void character_jump()
        {
            if (_jump_cnt > 0)
            {
                _player_anim.Play(PlayerAnim.Jump.ToString(), -1, 0f);
                _jump_cnt--;
                //transform.position += new Vector3(0,jump_speed,0);
                _player_rigidbody.AddForce(jump_speed * transform.up, ForceMode2D.Impulse);
            }
            
        }
        void set_gamepad_jump()
        {
            GamePadController.Instance._jump_btn.onClick.AddListener(() => {
                Debug.Log("#####jump_btn_clicked");
                character_jump();
            });
        }
        void set_gamepad_attack()
        {
            GamePadController.Instance._attack_btn.onClick.AddListener(() => {

                StartCoroutine(attacked_clicked_routine());
            });
        }
        //캐릭터 공격 당했을 때
        public void character_nuck_back()
        {
            set_is_attacked(true);
            StatManager.Instance.character_life_cnt--;
            Debug.Log("###########nuckback");
            _game_manager.update_life(StatManager.Instance.character_life_cnt);
            _player_anim.Play(PlayerAnim.Damage.ToString(), -1, 0f);

            if (StatManager.Instance.character_life_cnt <= 0)
            {
                StartCoroutine(charac_dead());
            }
            else
                _player_rigidbody.AddForce(Vector2.left * 10f, ForceMode2D.Impulse);
            
        }
        public IEnumerator steped_spike()
        {
            character_nuck_back();
            yield return new WaitForSeconds(.1f);
            set_is_attacked(false);
        }
        public IEnumerator attacked_clicked_routine()
        {
            _player_anim.Play(PlayerAnim.Attack.ToString(),-1,0f);
            yield return new WaitForSeconds(.5f);
            _game_manager.clear_ptcl_list();

        }
        public void squash_monster()
        {
            set_is_squashing(true);
            _player_rigidbody.AddForce(Vector2.up * 11f, ForceMode2D.Impulse);
            //transform.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 10f, ForceMode2D.Impulse);
        }
        public void set_is_squashing(bool state)
        {
            _is_squashing = state;
        }
        public void set_is_attacked(bool state)
        {
            _is_attacked = state;
        }
        public void set_attack_ptcl()
        {
            GameObject ptcl_obj = PoolAllocater.Instance.get_pool_obj("Eff_Pool");
            ptcl_obj.SetActive(true);
            ptcl_obj.transform.SetParent(_game_manager._ptcl_container);
            ptcl_obj.transform.localScale = Vector3.one*100f;
            ptcl_obj.transform.position = transform.position+Vector3.right;
            _game_manager._ptcl_list.Add(ptcl_obj);
        }


        private IEnumerator charac_dead()
        {
            _is_game_end = true;
            transform.localScale = new Vector3(1, -1, 1);
            _player_anim.Play(PlayerAnim.Dead.ToString());
            _body_collider.enabled = false;
            _foot_collider.enabled = false;
            _game_manager._camera_controller.enabled = false;
            yield return new WaitForSeconds(1f);
            _game_manager.game_over(() => {
                init_game();
            });
        }
        private IEnumerator game_win()
        {
            _is_game_end = true;
            _player_anim.Play(PlayerAnim.Idle_Win.ToString());
            yield return new WaitForSeconds(1f);
            _game_manager.game_win(() => {

                //일단은 현재 맵 초기화로. 추후 다음 맵 이동구현
                init_game();
            });
        }



        private void FixedUpdate()
        {
            if (_is_squashing || _is_attacked)
                return;
            if (!_is_game_end)
                character_move();
        }
        private void Update()
        {
            if (_is_squashing || _is_attacked || _is_game_end)
                return;
            if ((Input.GetKeyDown(KeyCode.Space)))
            {
                Debug.Log("####Space bar clicked");
                character_jump();
            }

        }
    }

}


