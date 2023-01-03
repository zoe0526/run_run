using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace run_run
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private PopupManager _popup_manager;
        [SerializeField]
        private TextMeshProUGUI _User_Account_ID_txt;
        [SerializeField]
        private Text _coin_value_txt;
        [SerializeField]
        private GameObject[] _diamond_UI_arr;   //3개
        [SerializeField]
        private GameObject[] _life_UI_arr;   //3개
        [SerializeField]
        public CameraController _camera_controller;
        [SerializeField]
        public Transform _ptcl_container;
        public List<GameObject> _ptcl_list;
        private void Start()
        {
            init_game();
            set_character_info();
            _ptcl_list = new List<GameObject>();
        }

        private void set_character_info()
        {
            _User_Account_ID_txt.text = StatManager.Instance.user_ID;
            _coin_value_txt.text = StatManager.Instance.total_coin_value.ToString();

        }
        public void clear_ptcl_list()
        {
            foreach (var item in _ptcl_list)
                item.GetComponent<PooledObj>().Return();
            _ptcl_list.Clear();
        }

        public void init_game()
        {
            StatManager.Instance.init_character_stat();
            MonsterStatManager.Instance.restore_monster();

            //수집 다이아몬드 갯수 초기화
            for (int i=0; i< _diamond_UI_arr.Length;i++)
            {
                _diamond_UI_arr[i].GetComponent<Image>().color = StatManager._dim_color;
            }
            //life 초기화
            Debug.Log("###########init");
            update_life(StatManager.Instance.character_life_cnt);
        }
        public void update_coin(long coin_value)
        {
            _coin_value_txt.text = coin_value.ToString();
        }
        public void update_life(int remain_life_cnt)
        {
            for (int i = 0; i < _life_UI_arr.Length; i++)
            {
                if(i<remain_life_cnt)
                    _life_UI_arr[i].SetActive(true);
                else
                    _life_UI_arr[i].SetActive(false);
            }
        }

        public void game_over(Action callback)
        {
            _popup_manager.show_game_over_popup(StatManager.Instance.earn_coin, () => {
               init_game();
                if(callback!=null)
                {
                    callback();
                    callback = null;
                }
            });
        }
        public void game_win(Action callback)
        {
            _popup_manager.show_game_win_popup(StatManager.Instance.earn_coin,StatManager.Instance.collected_diamond_cnt, () => {
                init_game();
                if (callback != null)
                {
                    callback();
                    callback = null;
                }
            });
        }
        public void collect_diamond()
        {
            if(StatManager.Instance.collected_diamond_cnt<StatManager._max_diamond_cnt)
            {
                _diamond_UI_arr[StatManager.Instance.collected_diamond_cnt].GetComponent<Image>().color = Color.white;
                StatManager.Instance.collected_diamond_cnt++;
            }


        }


    }

}
