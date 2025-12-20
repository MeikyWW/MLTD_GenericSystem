using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MLTD.GenericSystem
{
    public class BootManager : MonoBehaviour
    {
        [SerializeField] float progress;
        
        [SerializeField] Slider progressBar;

        [SerializeField] UnityEvent onBootCompleted;
        [SerializeField] Canvas canvasBoot;

        void Start()
        {
            StartCoroutine(BootFlow());
        }

        IEnumerator BootFlow()
        {
            yield return WaitForManagers(0f, 0.25f, 0.6f);
            yield return LoadSaveData(0.25f, 0.5f, 0.6f);

            // Later:
            // yield return InitAddressables(0.5f, 1f);

            SetProgress(1f);

            // 🔹 HOLD at full
            yield return new WaitForSeconds(1f); // 0.5–1 sec feels good

            onBootCompleted.Invoke();
            canvasBoot.gameObject.SetActive(false);

            Debug.Log("Boot is finished");
        }

        IEnumerator WaitForManagers(float start, float end, float minDuration)
        {
            float timer = 0f;

            while (!GlobalGameManager.Instance.SystemsReady || timer < minDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(timer / minDuration);
                SetProgress(Mathf.Lerp(start, end, t));

                yield return null;
            }

            SetProgress(end);
            Debug.Log("All Managers are loaded");
        }

        IEnumerator LoadSaveData(float start, float end, float minDuration)
        {
            SystemSaveManager.Instance.Load();

            float timer = 0f;

            while (timer < minDuration)
            {
                timer += Time.deltaTime;

                float t = Mathf.Clamp01(timer / minDuration);
                SetProgress(Mathf.Lerp(start, end, t));

                yield return null;
            }

            SetProgress(end);
            Debug.Log("Save Data is loaded");
        }

        // IEnumerator InitAddressables(float start, float end)
        // {
        //     var handle = Addressables.InitializeAsync();

        //     while (!handle.IsDone)
        //     {
        //         float p = Mathf.Lerp(start, end, handle.PercentComplete);
        //         SetProgress(p);
        //         yield return null;
        //     }

        //     SetProgress(end);
        // }

        public void SetProgress(float value)
        {
            progressBar.value = value;
        }
    }
}