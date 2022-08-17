using Scene;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Menu
{
    public class Menu : MonoBehaviour
    {
        private readonly string[] _optionsMenuTexts = {"Graphics", "Audio"};

        private string _clicked = "", _messageDisplayOnAbout = "About \n ";
        private int _maxRounds = 3;
        private AudioSource _menuAudio;
        private int _optionsMenuPrevious;
        private int _optionsMenuSelected;
        private Vector2 _scrollPosition;
        private float _volumeMaster;
        private float _volumeMusic;
        private float _volumeEffects;
        private Rect _windowRect = new Rect(Screen.width / 4 - 200, Screen.height / 2 - 200, 400, 400);
        [SerializeField] private string[] aboutTextLines = new string[0];
        [SerializeField] private Texture2D background;
        [SerializeField] private AudioClip buttonClick;

        [SerializeField] private AudioClip buttonToggle;
        [SerializeField] private bool dragWindow;
        [SerializeField] private GUISkin guiSkin;
        [SerializeField] private Texture2D logo;
        [SerializeField] private AudioMixer generalAudioMixer;

        [SerializeField] private int maxLevels = 1;


        private void Start()
        {
            _volumeMaster = PlayerPrefs.GetFloat("gen_vol_master");
            _volumeMusic = PlayerPrefs.GetFloat("gen_vol_music");
            _volumeEffects = PlayerPrefs.GetFloat("gen_vol_effects");
            generalAudioMixer.SetFloat("volumeMaster", _volumeMaster);
            generalAudioMixer.SetFloat("volumeMusic", _volumeMusic);
            generalAudioMixer.SetFloat("volumeEffects", _volumeEffects);


            _menuAudio = GetComponent<AudioSource>();

            foreach (var aboutTextLine in aboutTextLines) _messageDisplayOnAbout += aboutTextLine + " \n ";

            _messageDisplayOnAbout += "Press Esc To Go Back";
        }

        private void OnGUI()
        {
            if (background != null)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
            if (logo != null && _clicked != "about")
                GUI.DrawTexture(new Rect(Screen.width / 2 - 100, 30, 200, 200), logo);

            GUI.skin = guiSkin;
            _windowRect = new Rect(Screen.width / 4 - 200, Screen.height / 2 - 200, 400, 400);
            if (_clicked == "")
                _windowRect = GUI.Window(0, _windowRect, MenuFunc, "");
            else if (_clicked == "options")
                _windowRect = GUI.Window(1, _windowRect, OptionsFunc, "Options");
            else if (_clicked == "about")
                GUI.Box(new Rect(0, 0, Screen.width, Screen.height), _messageDisplayOnAbout);
            else if (_clicked == "GameSettings") _windowRect = GUI.Window(3, _windowRect, GameFunc, "Game Settings");
        }

        private void GameFunc(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginArea(new Rect(20, 40, 150, 100));
            GUILayout.Label("Left Player");

            GameSettings.LeftPlayerName = GUILayout.TextField(GameSettings.LeftPlayerName);

            if (GameSettings.leftControlIsPlayer)
            {
                if (GUILayout.Button("Player"))
                {
                    GameSettings.SetLeftControl(false);
                    _menuAudio.PlayOneShot(buttonToggle);
                }
            }
            else
            {
                if (GUILayout.Button("NPC"))
                {
                    GameSettings.SetLeftControl(true);
                    _menuAudio.PlayOneShot(buttonToggle);
                }
            }

            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(230, 40, 150, 100));
            GUILayout.Label("Right Player");

            GameSettings.RightPlayerName = GUILayout.TextField(GameSettings.RightPlayerName);

            if (GameSettings.rightControlIsPlayer)
            {
                if (GUILayout.Button("Player"))
                {
                    GameSettings.SetRightControl(false);
                    _menuAudio.PlayOneShot(buttonToggle);
                }
            }
            else
            {
                if (GUILayout.Button("NPC"))
                {
                    GameSettings.SetRightControl(true);
                    _menuAudio.PlayOneShot(buttonToggle);
                }
            }

            GUILayout.EndArea();
            GUILayout.BeginArea(new Rect(20, 150, 150, 100));
            GUILayout.Label("Rounds: " + GameSettings.maxRounds);
            _maxRounds = (int) GUILayout.HorizontalSlider(_maxRounds, 1, 100);
            //_maxRounds = int.Parse(GUILayout.TextField(_maxRounds.ToString(),2));

            if (_maxRounds != GameSettings.maxRounds) GameSettings.SetMaxRounds(_maxRounds);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(20, 330, 150, 50));
            if (GUILayout.Button("Back"))
            {
                _clicked = "";
                _menuAudio.PlayOneShot(buttonClick);
            }

            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(230, 330, 150, 50));
            if (GUILayout.Button("Start"))
            {
                GameSettings.Reset();
                var arena = "Arena" + Random.Range(0, maxLevels);
                Debug.Log(arena);
                SceneManager.LoadScene(arena);
                _menuAudio.PlayOneShot(buttonClick);
            }

            GUILayout.EndArea();
        }

        private void ResolutionFunc()
        {
            GUILayout.BeginArea(new Rect(10, 80, _windowRect.width - 20, 300));
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(200), GUILayout.Height(250));

            foreach (var resolution in Screen.resolutions)
                if (GUILayout.Button(resolution.width + "X" + resolution.height))
                {
                    Screen.SetResolution(resolution.width, resolution.height, true);
                    _windowRect = new Rect(Screen.width / 4 - 200, Screen.height / 2 - 200, 400, 400);
                    _menuAudio.PlayOneShot(buttonClick);
                    
                }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void OptionsFunc(int id)
        {
            GUILayout.BeginArea(new Rect(10, 40, _windowRect.width - 20, 30));
            _optionsMenuSelected = GUILayout.Toolbar(_optionsMenuSelected, _optionsMenuTexts);
            GUILayout.EndArea();

            switch (_optionsMenuSelected)
            {
                case 0:

                    ResolutionFunc();

                    break;
                case 1:
                    GUILayout.BeginArea(new Rect(10, 90, _windowRect.width - 20, 200));
                    
                    GUILayout.Box("Volume",GUILayout.Height(30));
                    GUILayout.Label("Master");
                    _volumeMaster = GUILayout.HorizontalSlider(_volumeMaster, -80.0f, 10.0f);
                    GUILayout.Label("Music");
                    _volumeMusic = GUILayout.HorizontalSlider(_volumeMusic, -80.0f, 10.0f);
                    GUILayout.Label("Effects");
                    _volumeEffects = GUILayout.HorizontalSlider(_volumeEffects, -80.0f, 10.0f);
                    //AudioListener.volume = _volumeMaster;
                    generalAudioMixer.SetFloat("volumeMaster", _volumeMaster);
                    generalAudioMixer.SetFloat("volumeMusic", _volumeMusic);
                    generalAudioMixer.SetFloat("volumeEffects", _volumeEffects);
                    PlayerPrefs.SetFloat("gen_vol_master", _volumeMaster);
                    PlayerPrefs.SetFloat("gen_vol_music", _volumeMusic);
                    PlayerPrefs.SetFloat("gen_vol_effects", _volumeEffects);

                    GUILayout.EndArea();
                    break;
            }

            if (_optionsMenuSelected != _optionsMenuPrevious)
            {
                _menuAudio.PlayOneShot(buttonClick);
                _optionsMenuPrevious = _optionsMenuSelected;
            }

            GUILayout.BeginArea(new Rect(10, _windowRect.height - 40, 80, 30));

            if (GUILayout.Button("Back"))
            {
                _clicked = "";
                _menuAudio.PlayOneShot(buttonClick);
            }

            if (dragWindow)
                GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.EndArea();
        }

        private void MenuFunc(int id)
        {
            if (GUILayout.Button("Play Game",GUILayout.Height(60)))
            {
                _clicked = "GameSettings";
                _menuAudio.PlayOneShot(buttonClick);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Options", GUILayout.Height(60)))
            {
                _clicked = "options";
                _menuAudio.PlayOneShot(buttonClick);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("About", GUILayout.Height(60)))
            {
                _clicked = "about";
                _menuAudio.PlayOneShot(buttonClick);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Quit Game", GUILayout.Height(60)))
            {
                Application.Quit();
                _menuAudio.PlayOneShot(buttonClick);
            }

            if (dragWindow)
                GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }

        private void Update()
        {
            if (_clicked == "about" && Input.GetKey(KeyCode.Escape))
                _clicked = "";
        }
    }
}