using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MLTD.GenericSystem
{
    public class PC_Manager : MonoBehaviour
    {
        async void Start()
        {
    #if UNITY_STANDALONE_WIN 
            await LoadDisplayManagerAsync();
    #endif
        }

        async Task LoadDisplayManagerAsync()
        {
            var handle = Addressables.LoadAssetAsync<GameObject>("Windows/ResolutionManager");
            //Debug.Log("Handle Status: "+handle.Status);
            //Debug.Log("Handle Operation Exeption: "+handle.OperationException);
            var prefab = await handle.Task; //Waits until the loading operation is finished
            if (prefab != null)
                Instantiate(prefab, transform);
            else 
            Debug.Log("ResolutionManager is not found");
        }

    }

}

