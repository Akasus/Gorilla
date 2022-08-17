using System;
using System.Collections;
using System.Collections.Generic;
using Scene;
using UnityEngine;
using UnityEngine.UI;

namespace Scene
{
    public class IngameStatsManager : MonoBehaviour
    {

        public static IngameStatsManager Main;
        
        
        private Text _leftPlayerName;
        private Text _rightPlayerName;
        private Text _leftPlayerShoots;
        private Text _rightPlayerShoots;
        private Text _pastTime;

        private int _leftShoots = 0;
        private int _rightShoots = 0;
        
        // Start is called before the first frame update
        void Start()
        {
            Main = this;
            InitComponents();
            _leftPlayerName.text = GameSettings.LeftPlayerName;
            _rightPlayerName.text = GameSettings.RightPlayerName;
        }

        // Update is called once per frame
        void Update()
        {
            var time = TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
            _pastTime.text = time.Minutes.ToString("D2") + " : " + time.Seconds.ToString("D2");
        }



        void InitComponents()
        {
            _leftPlayerName = transform.Find("LeftPlayerName").GetComponent<Text>();
            _rightPlayerName = transform.Find("RightPlayerName").GetComponent<Text>();
            _leftPlayerShoots = transform.Find("LeftPlayerShoots").GetComponent<Text>();
            _rightPlayerShoots = transform.Find("RightPlayerShoots").GetComponent<Text>();
            _pastTime = transform.Find("PastTimeVarText").GetComponent<Text>();
        }


        public void IncreaseLeftShoots()
        {
            _leftShoots++;
            _leftPlayerShoots.text = "Shoots: " + _leftShoots.ToString();
        }
        public void IncreaseRightShoots()
        {
            _rightShoots++;
            _rightPlayerShoots.text = "Shoots: " + _rightShoots.ToString();
        }
    }
}
