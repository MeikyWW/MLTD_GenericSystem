using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MLTD.GenericSystem
{
    public class DisplayManager : MonoBehaviour
    {
        public static DisplayManager Instance;

        Resolution[] resolutions;

        [Header("Debug (Readonly)")]
        [SerializeField] int currentResolutionIndex = 0;   // index in Screen.resolutions
        [SerializeField] int currentScreenModeIndex = 0;   // 0 = Fullscreen, 1 = Borderless, 2 = Windowed

        private Coroutine _borderlessRoutine;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
        }

        void Start()
        {
            resolutions = Screen.resolutions;
            foreach (var res in resolutions)

            LoadDisplaySettings();
        }

        // ---------------- CORE APPLY METHOD ----------------

        private void ApplyDisplay()
        {
            if (resolutions == null || resolutions.Length == 0)
            {
                return;
            }

            currentResolutionIndex = Mathf.Clamp(currentResolutionIndex, 0, resolutions.Length - 1);
            currentScreenModeIndex = Mathf.Clamp(currentScreenModeIndex, 0, 2);

            Resolution res = resolutions[currentResolutionIndex];

            // stop any pending borderless apply
            if (_borderlessRoutine != null)
            {
                StopCoroutine(_borderlessRoutine);
                _borderlessRoutine = null;
            }

            // Decide fullscreen mode Unity should use
            FullScreenMode fsMode;
            switch (currentScreenModeIndex)
            {
                case 0: // Fullscreen (exclusive)
                    fsMode = FullScreenMode.ExclusiveFullScreen;
                    break;

                case 1: // Borderless (small window, no title bar) -> still WINDOWED from Unity's POV
                case 2: // Windowed
                default:
                    fsMode = FullScreenMode.Windowed;
                    break;
            }

    #if UNITY_STANDALONE_WIN
            // Whenever we're NOT in borderless mode, make sure borders are restored
            if (currentScreenModeIndex != 1)
            {
                WindowController.RestoreWindowBorder();
            }
    #endif

            // Apply resolution + fullscreen mode via Unity
            Screen.SetResolution(
                res.width,
                res.height,
                fsMode,
                res.refreshRateRatio
            );

            // For exclusive fullscreen, ensure the boolean matches
            Screen.fullScreen = (currentScreenModeIndex == 0);

    #if UNITY_STANDALONE_WIN
            // If we're in borderless mode, apply native borderless AFTER Unity resizes the window
            if (currentScreenModeIndex == 1)
            {
                _borderlessRoutine = StartCoroutine(ApplyBorderlessWindow(res.width, res.height));
            }
    #endif
        }

    #if UNITY_STANDALONE_WIN
        private System.Collections.IEnumerator ApplyBorderlessWindow(int width, int height)
        {
            // Give Unity time to recreate/resize the OS window
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(0.05f);

            // Only apply if we're still in borderless mode
            if (currentScreenModeIndex == 1)
            {
                WindowController.SetBorderless(width, height);
            }

            _borderlessRoutine = null;
        }
    #endif

        // ---------------- PUBLIC API ----------------

        public void SetResolution(int index)
        {
            currentResolutionIndex = Mathf.Clamp(index, 0, resolutions.Length - 1);
            ApplyDisplay();
        }

        // 0 = Fullscreen, 1 = Borderless, 2 = Windowed
        public void SetScreenMode(int mode)
        {
            currentScreenModeIndex = Mathf.Clamp(mode, 0, 2);
            ApplyDisplay();
        }

        public int GetCurrentResolutionIndex() => currentResolutionIndex;
        public int GetCurrentScreenModeIndex() => currentScreenModeIndex;

        // ---------------- UI BINDINGS ----------------

        public void InitializeResolutionDropdown(TMP_Dropdown dropdown)
        {
            dropdown.ClearOptions();
            List<string> options = new List<string>();

            // Highest resolution first
            for (int i = resolutions.Length - 1; i >= 0; i--)
            {
                options.Add($"{resolutions[i].width} x {resolutions[i].height}");
            }

            dropdown.AddOptions(options);

            int reversedIndex = (resolutions.Length - 1) - currentResolutionIndex;
            dropdown.value = Mathf.Clamp(reversedIndex, 0, options.Count - 1);
            dropdown.RefreshShownValue();

            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener((reversedSelectedIndex) =>
            {
                int realIndex = (resolutions.Length - 1) - reversedSelectedIndex;
                SetResolution(realIndex);
                SaveDisplaySettings();
            });
        }

        public void InitializeScreenModeDropdown(TMP_Dropdown dropdown)
        {
            dropdown.ClearOptions();

            List<string> options = new()
            {
                "Fullscreen",
                "Borderless",
                "Windowed"
            };

            dropdown.AddOptions(options);

            dropdown.value = Mathf.Clamp(currentScreenModeIndex, 0, options.Count - 1);
            dropdown.RefreshShownValue();

            dropdown.onValueChanged.RemoveAllListeners();
            dropdown.onValueChanged.AddListener((modeIndex) =>
            {
                SetScreenMode(modeIndex);
                SaveDisplaySettings();
            });
        }

        // ---------------- SAVE / LOAD ----------------

        public void SaveDisplaySettings()
        {
            var savedDisplay = new SavedDisplaySettings
            {
                resolutionIndex = currentResolutionIndex,
                screenModeIndex = currentScreenModeIndex
            };

            string json = JsonUtility.ToJson(savedDisplay);
            PlayerPrefs.SetString("DisplaySettings", json);
            PlayerPrefs.Save();
        }

        public void LoadDisplaySettings()
        {
            if (!PlayerPrefs.HasKey("DisplaySettings"))
            {
                // Default: current desktop resolution & fullscreen
                var current = Screen.currentResolution;

                int idx = 0;
                for (int i = 0; i < resolutions.Length; i++)
                {
                    if (resolutions[i].width == current.width &&
                        resolutions[i].height == current.height)
                    {
                        idx = i;
                        break;
                    }
                }

                currentResolutionIndex = idx;
                currentScreenModeIndex = 0; // Fullscreen by default
                ApplyDisplay();
                return;
            }

            string json = PlayerPrefs.GetString("DisplaySettings");
            var loaded = JsonUtility.FromJson<SavedDisplaySettings>(json);

            currentResolutionIndex = Mathf.Clamp(loaded.resolutionIndex, 0, resolutions.Length - 1);
            currentScreenModeIndex = Mathf.Clamp(loaded.screenModeIndex, 0, 2);

            ApplyDisplay();
        }
    }

    [Serializable]
    public class SavedDisplaySettings
    {
        public int resolutionIndex;
        public int screenModeIndex;
    }



    public class WindowController : MonoBehaviour
    {
    #if UNITY_STANDALONE_WIN

        const int GWL_STYLE = -16;
        const int WS_BORDER = 0x00800000;
        const int WS_DLGFRAME = 0x00400000;
        const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern System.IntPtr GetActiveWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int SetWindowLong(System.IntPtr hWnd, int nIndex, int dwNewLong);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern int GetWindowLong(System.IntPtr hWnd, int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter,
            int x, int y, int cx, int cy, uint uFlags);

        const uint SWP_FRAMECHANGED = 0x0020;
        const uint SWP_SHOWWINDOW = 0x0040;
        const int SWP_NOMOVE = 0x0002;
        const int SWP_NOSIZE = 0x0001;
        const int SWP_NOZORDER = 0x0004;

        public static void SetBorderless(int width, int height)
        {
            var window = GetActiveWindow();
            if (window == System.IntPtr.Zero) return;

            int style = GetWindowLong(window, GWL_STYLE);
            style &= ~WS_CAPTION; // remove caption/border bits
            SetWindowLong(window, GWL_STYLE, style);

            // Keep it simple: top-left at (0,0). You can center later if you want.
            SetWindowPos(window, System.IntPtr.Zero, 0, 0, width, height,
                SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_NOZORDER | SWP_NOMOVE);
        }

        public static void RestoreWindowBorder()
        {
            var window = GetActiveWindow();
            if (window == System.IntPtr.Zero) return;

            int style = GetWindowLong(window, GWL_STYLE);
            style |= WS_CAPTION; // restore caption/border bits
            SetWindowLong(window, GWL_STYLE, style);

            SetWindowPos(window, System.IntPtr.Zero, 0, 0, 0, 0,
                SWP_FRAMECHANGED | SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER);
        }
    #endif
    }
}