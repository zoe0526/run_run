using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;

namespace run_run
{
    public class GamePadController : MonoBehaviour
    {
        public static GamePadController Instance;

        public bool is_left_move = false;
        public bool is_right_move = false;
        public bool is_attack = false;
        [SerializeField]
        public Button _jump_btn;
        [SerializeField]
        private Button _attack_btn;

        public void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _attack_btn.onClick.AddListener(() => { is_attack = true; });
        }


        public void on_click_left_btn_down()
        {
            is_left_move = true;
        }
        public void on_click_left_btn_up()
        {
            is_left_move = false;
        }

        public void on_click_right_btn_down()
        {
            is_right_move = true;
        }
        public void on_click_right_btn_up()
        {
            is_right_move = false;
        }

    }

}