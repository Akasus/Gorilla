using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Menu
{
//This is usually in a Prefab and is controlled by the ArenaManager.

    public class MenuInGame : MonoBehaviour
    {
        private readonly string[] _optionsMenuTexts = {"Graphics", "Audio"};
        private string _clicked = "";

        private AudioSource _menuAudio;
        private int _optionsMenuPrevious;
        private int _optionsMenuSelected;

        private Vector2 _scrollPosition;
        private bool _showGui = false;
        private float _volumeMaster;
        private float _volumeMusic;
        private float _volumeEffects;

        private Rect _windowRect = new Rect(Screen.width / 4 - 200, Screen.height / 2 - 200, 400, 400);
        [SerializeField] private Texture2D background;
        [SerializeField] private Texture2D logo;
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private GUISkin guiSkin;

        [SerializeField] private AudioMixer generalAudioMixer;

        private void Start()
        {

            generalAudioMixer.GetFloat("volumeMaster", out _volumeMaster);
            generalAudioMixer.GetFloat("volumeMusic", out _volumeMusic);
            generalAudioMixer.GetFloat("volumeEffects", out _volumeEffects);
            _menuAudio = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (!Input.GetKeyDown(KeyCode.Escape)) return;
            _showGui = !_showGui;
            Time.timeScale = 1.0f;
        }


        private void OnGUI()
        {
            if (!_showGui) return;
            Time.timeScale = 0.0f;
            if (background != null)
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), background);
            if (logo != null && _clicked != "about")
                GUI.DrawTexture(new Rect(Screen.width / 2 - 100, 30, 200, 200), logo);

            GUI.skin = guiSkin;
            _windowRect = new Rect(Screen.width / 4 - 200, Screen.height / 2 - 200, 400, 400);
            switch (_clicked)
            {
                case "":
                    _windowRect = new Rect(Screen.currentResolution.width / 2 - 200,
                        Screen.currentResolution.height / 2 - 200, 400, 400);
                    _windowRect = GUI.Window(0, _windowRect, MenuFunc,"");
                    break;
                case "options":
                    _windowRect = new Rect(Screen.currentResolution.width / 2 - 200,
                        Screen.currentResolution.height / 2 - 200, 400, 400);
                    _windowRect = GUI.Window(1, _windowRect, OptionsFunc, "Options");
                    break;
            }

        }


        private void ResolutionFunc()
        {
            GUILayout.BeginArea(new Rect(10, 80, _windowRect.width - 20, 300));
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.Width(200), GUILayout.Height(250));

            foreach (var resolution in Screen.resolutions)
                if (GUILayout.Button(resolution.width + "X" + resolution.height))
                {
                    Screen.SetResolution(resolution.width, resolution.height, true);
                    _windowRect = new Rect(resolution.width / 4 - 200, resolution.height / 2 - 200, 400, 400);
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

                    GUILayout.Box("Volume", GUILayout.Height(30));
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
                    PlayerPrefs.SetFloat("gen_vol_master",_volumeMaster);
                    PlayerPrefs.SetFloat("gen_vol_music",_volumeMusic);
                    PlayerPrefs.SetFloat("gen_vol_effects",_volumeEffects);

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

            GUILayout.EndArea();
        }

        private void MenuFunc(int id)
        {
            if (GUILayout.Button("Back to Game"))
            {
                _menuAudio.PlayOneShot(buttonClick);
                _showGui = false;
                Time.timeScale = 1.0f;
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Options"))
            {
                _clicked = "options";
                _menuAudio.PlayOneShot(buttonClick);
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Main Menu"))
            {
                _menuAudio.PlayOneShot(buttonClick);
                SceneManager.LoadScene("MainMenu");
            }
            GUILayout.Space(20);
            if (!GUILayout.Button("Quit Game")) return;

            _menuAudio.PlayOneShot(buttonClick);
            Application.Quit();
        }
    }
}