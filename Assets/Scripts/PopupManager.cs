using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace run_run
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject _game_over_popup;
        [SerializeField]
        private GameObject _game_win_popup;

        [SerializeField]
        private GameObject[] _game_win_diamond_arr;
        [SerializeField]
        private TextMeshProUGUI _game_win_coin_value;
        [SerializeField]
        private Image _game_medal; // 결과에 따라 동, 은 ,금
        [SerializeField]
        private Sprite[] _medal_spr_arr;    //동, 은 ,금

        Action start_callback;
        public void show_game_over_popup(Action callback)
        {
            start_callback = callback;
            _game_over_popup.SetActive(true);
            _game_over_popup.transform.Find("Scale/BG/Button").GetComponent<Button>().interactable = true;
        }

        public void on_click_game_over()
        {
            _game_over_popup.transform.Find("Scale/BG/Button").GetComponent<Button>().interactable = false;
            _game_over_popup.SetActive(false);


            if (start_callback != null)
            {
                start_callback();
                start_callback = null;
            }

        }
        public void show_game_win_popup(long win_value, int medal_type,Action callback) //medal_type : 0,1,2
        {
            start_callback = callback;
            _game_win_popup.SetActive(true);
            _game_medal.sprite = _medal_spr_arr[medal_type];
            _game_win_coin_value.text = win_value.ToString();

            for (int i=0; i<_game_win_diamond_arr.Length;i++)
            {
                if (i < StatManager.Instance.collected_diamond_cnt)
                    _game_win_diamond_arr[i].GetComponent<Image>().color = Color.white;
                else
                    _game_win_diamond_arr[i].GetComponent<Image>().color = StatManager._dim_color;

            }
            _game_win_popup.transform.Find("Scale/Inner_obj/Button").GetComponent<Button>().interactable = true;

        }
        public void on_click_win_collect()
        {
            _game_win_popup.transform.Find("Scale/Inner_obj/Button").GetComponent<Button>().interactable = false;
            _game_win_popup.SetActive(false);


            if (start_callback != null)
            {
                start_callback();
                start_callback = null;
            }


        }
        public void show_collected_save_point()
        {

        }
    }

}