using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace run_run
{
    public class StatManager : MonoBehaviour
    {
        public static int _max_character_life_cnt = 3;
        public static int _max_diamond_cnt = 3;
        public static Vector3 _character_init_pos = Vector3.zero;
        public static Color32 _dim_color = new Color32(255, 255, 255, 80);

        public static StatManager Instance;
        private int _character_life_cnt;
        public int character_life_cnt
        {
            get { return _character_life_cnt; }
            set { _character_life_cnt = value; }
        }

        private int _collected_diamond_cnt;
        public int collected_diamond_cnt
        {
            get { return _collected_diamond_cnt; }
            set { _collected_diamond_cnt = value; }
        }
        private long _coin_value;
        public long coin_value
        {
            get { return _coin_value; }
            set { _coin_value = value; }
        }
        private string _user_ID;
        public string user_ID
        {
            get { return _user_ID; }
        }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        public void set_character_ID(string ID)
        {
            _user_ID = ID;
        }
        public void set_character_coin(string coin)
        {
            _coin_value = long.Parse(coin);
        }

        public void init_character_stat()
        {
            _character_life_cnt = _max_character_life_cnt;
            _collected_diamond_cnt = 0;
        }

    }

}