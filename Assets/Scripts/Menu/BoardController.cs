using System.Runtime.CompilerServices;
using Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Menu
{
    public class BoardController : MonoBehaviour
    {
        private Text _currentRound;
        private GameObject _drawText;

        //These down here are not changing, they are ever connected in the same way! Not like variables above this text.

        private Button _goButton;
        private GameObject _leftLoseText;

        //Variable text of the Score & matchBoard
        //This section is for both of the board types, and they are changeable via "GetMatchBoardValues()" & GetScoreBoardValues().
        private Text _leftPlayer;
        private Text _leftPlayerWin;

        private GameObject _leftWinText;
        private Text _maxRounds;
        private Button _nextButton;
        private GameObject _nextButtonObj;
        private GameObject _rightLoseText;
        private Text _rightPlayer;
        private Text _rightPlayerWin;
        private GameObject _rightWinText;
        private Button _toMainMenuButton;
        private GameObject _toMainMenuButtonObj;

        //Stores the Boards
        [SerializeField] private GameObject matchBoard;
        [SerializeField] private GameObject scoreBoard;
        [SerializeField] private GameObject winBoard;

        [SerializeField] private int maxLevels = 1;

        // Start is called before the first frame update
        private void Start()
        {
            //Initialize the level and freeze it till the player hits the "GO!" button
            Time.timeScale = 0;
            scoreBoard.SetActive(false);
            GetMatchBoardValues();
            SetCurrentValues();


            //This section searches for the specific buttons on the boards and make them readable after "Start()".
            _nextButtonObj = scoreBoard.transform.Find("Next").gameObject;
            _toMainMenuButtonObj = winBoard.transform.Find("ToMainMenu").gameObject;
            _leftWinText = winBoard.transform.Find("LeftWinText").gameObject;
            _leftLoseText = winBoard.transform.Find("LeftLoseText").gameObject;
            _rightWinText = winBoard.transform.Find("RightWinText").gameObject;
            _rightLoseText = winBoard.transform.Find("RightLoseText").gameObject;
            _drawText = winBoard.transform.Find("DrawText").gameObject;

            _goButton = matchBoard.transform.Find("Go").GetComponent<Button>();
            _nextButton = _nextButtonObj.GetComponent<Button>();
            _toMainMenuButton = _toMainMenuButtonObj.GetComponent<Button>();

            //Add functions from this behavior as listeners to the buttons on the board
            _goButton.onClick.AddListener(GoButtonOnClick);
            _nextButton.onClick.AddListener(NextButtonOnClick);
            _toMainMenuButton.onClick.AddListener(ToMainMenuButtonOnClick);

            //Tells the GameSettings that I'm the main board controller in this scene.
            GameSettings.SetBoardController(this);

            matchBoard.SetActive(true);
        }

        // Update is called once per frame
        private void Update()
        {
            //Changes the Board when last round has been reached
            if (!GameSettings.IsLastRound()) return;
            {
                _nextButtonObj.SetActive(false);
                _toMainMenuButtonObj.SetActive(true);
            }
        }


        private void GetMatchBoardValues()
        {
            _leftPlayer = matchBoard.transform.Find("LeftPlayer").GetComponent<Text>();
            _rightPlayer = matchBoard.transform.Find("RightPlayer").GetComponent<Text>();
            _currentRound = matchBoard.transform.Find("CurrentRound").GetComponent<Text>();
            _maxRounds = matchBoard.transform.Find("MaxRounds").GetComponent<Text>();
        }

        private void GetScoreBoardValues()
        {
            _leftPlayer = scoreBoard.transform.Find("LeftPlayer").GetComponent<Text>();
            _rightPlayer = scoreBoard.transform.Find("RightPlayer").GetComponent<Text>();
            _currentRound = scoreBoard.transform.Find("CurrentRound").GetComponent<Text>();
            _maxRounds = scoreBoard.transform.Find("MaxRounds").GetComponent<Text>();
            _leftPlayerWin = scoreBoard.transform.Find("LeftPlayerWin").GetComponent<Text>();
            _rightPlayerWin = scoreBoard.transform.Find("RightPlayerWin").GetComponent<Text>();
        }

        private void GetWinBoardValues()
        {
            _leftPlayer = winBoard.transform.Find("LeftPlayer").GetComponent<Text>();
            _rightPlayer = winBoard.transform.Find("RightPlayer").GetComponent<Text>();
            _maxRounds = winBoard.transform.Find("MaxRounds").GetComponent<Text>();
            _leftPlayerWin = winBoard.transform.Find("LeftPlayerWin").GetComponent<Text>();
            _rightPlayerWin = winBoard.transform.Find("RightPlayerWin").GetComponent<Text>();
        }

        /// <summary>
        ///     Will be called when the "_goButton" is clicked.
        /// </summary>
        private void GoButtonOnClick()
        {
            //This function is a listener and Event triggered! Caution
            matchBoard.SetActive(false);
            ArenaInterface.InitFirstStrike();
            Time.timeScale = 1;
        }

        /// <summary>
        ///     Will be called when the "_nextButton" is clicked.
        /// </summary>
        private void NextButtonOnClick()
        {
            // This function is a listener and Event triggered! Caution
            var arena = "Arena" + Random.Range(0, maxLevels);
            Debug.Log(arena);
            SceneManager.LoadScene(arena);
            GameSettings.IncreaseRounds();
        }

        /// <summary>
        ///     Will be called when the "_toMainMenuButton" is clicked.
        /// </summary>
        private void ToMainMenuButtonOnClick()
        {
            // This function is a Listener and Event triggered! Caution
            GameSettings.Reset();
            SceneManager.LoadScene("MainMenu");
        }


        /// <summary>
        ///     Sets the specific values from the "GameSettings".
        /// </summary>
        private void SetCurrentValues()
        {
            // Works for both the "scoreBoard" and the "matchBoard".
            _leftPlayer.text = GameSettings.LeftPlayerName;
            _rightPlayer.text = GameSettings.RightPlayerName;
            _currentRound.text = GameSettings.currentRound.ToString();
            _maxRounds.text = GameSettings.maxRounds.ToString();
        }


        public void ShowScoreBoard()
        {
            if (GameSettings.currentRound < GameSettings.maxRounds)
            {
                //Caution it gets triggered outside of this Object, by the "ProjectileCollisionManager"!
                GetScoreBoardValues();
                SetCurrentValues();
                _leftPlayerWin.text = GameSettings.leftPlayerWin.ToString();
                _rightPlayerWin.text = GameSettings.rightPlayerWin.ToString();
                scoreBoard.SetActive(true);
            }
            else
            {
                ShowWinBoard();
            }
        }

        public void ShowWinBoard()
        {
            GetWinBoardValues();
            SetCurrentValues();
            _leftPlayerWin.text = GameSettings.leftPlayerWin.ToString();
            _rightPlayerWin.text = GameSettings.rightPlayerWin.ToString();
            if (GameSettings.leftPlayerWin > GameSettings.rightPlayerWin)
            {
                _leftWinText.SetActive(true);
                _leftLoseText.SetActive(false);
                _rightWinText.SetActive(false);
                _rightLoseText.SetActive(true);
                _drawText.SetActive(false);
            }
            else if (GameSettings.leftPlayerWin < GameSettings.rightPlayerWin)
            {
                _leftWinText.SetActive(false);
                _leftLoseText.SetActive(true);
                _rightWinText.SetActive(true);
                _rightLoseText.SetActive(false);
                _drawText.SetActive(false);
            }
            else if (GameSettings.leftPlayerWin == GameSettings.rightPlayerWin)
            {
                Debug.Log("Draw");
                _leftWinText.SetActive(false);
                _leftLoseText.SetActive(false);
                _rightWinText.SetActive(false);
                _rightLoseText.SetActive(false);
                _drawText.SetActive(true);
            }

            winBoard.SetActive(true);
        }
    }
}