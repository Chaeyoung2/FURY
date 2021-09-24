using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
  
    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;

    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;
    VRAnimatorController ani;
    // Start is called before the first frame update
    void Start()
    {
        tryInit();
        ani = GameObject.Find("playerch").GetComponent<VRAnimatorController>();
    }

    void tryInit()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        foreach (var item in devices)
        {
            Debug.Log(item.name + item.characteristics);
        }
        
        if (devices.Count > 0)
        {
            targetDevice = devices[0];
            GameObject prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            if (prefab)
            {

                Debug.LogError("제대로 찾았습니다");
                spawnedController = Instantiate(prefab, transform);
            }
            else
            {
                Debug.LogError("모델못찾았슴");
                spawnedController = Instantiate(controllerPrefabs[0], transform);
            }
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            handAnimator = spawnedHandModel.GetComponent<Animator>();
            if (handAnimator == null)
            {
                Debug.Log("핸드에니메이터없음");
            }
        }
        
    }
    void updateHandAni()
    {
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
            if(triggerValue > 0.01) { }
   //         Debug.Log("현재 " + targetDevice.name + "에서 트리거가 눌림" +triggerValue );
          //  if(targetDevice.name == "Oculus Touch Controller - Left")
           // {
           //     //왼쪽눌렸으니 왼쪽 에니메이션값주기
            //    Debug.Log("전달해주기!");

           //    ani.LhandTrigger = triggerValue;
          //  }
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
            
        }
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }
    }
    // Update is called once per frame
    void Update()
    {

     

        if (!targetDevice.isValid)
        {
            tryInit();
            Debug.Log("타겟디바이스가없어서 다시 이니셜라이즈하는중");
        }
        else
        {
            if (showController)
            {

                spawnedHandModel.SetActive(false);
                spawnedController.SetActive(true);
            }
            else
            {
                spawnedHandModel.SetActive(true);
                spawnedController.SetActive(false);
                updateHandAni();
                //업데이트 핸드에니메이션

            }
        }
        
    }
}
