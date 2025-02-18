using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FishingManager : MonoBehaviour
{
    [SerializeField] RectTransform fish;
    [SerializeField] RectTransform reel;
    [SerializeField] Slider stamina;

    private PlayerInventory playerInventory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)) {
            Debug.Log("클릭 감지");
            fish.anchoredPosition += new Vector2(0f, 1f);
            reel.Rotate(0f, 0f, -200f * Time.deltaTime);
            stamina.value -= Time.deltaTime;
        }
        else {
            fish.anchoredPosition -= new Vector2(0f, 0.2f);
            stamina.value += Time.deltaTime * 0.5f;
        }

        fish.anchoredPosition = new Vector2(fish.anchoredPosition.x, Mathf.Clamp(fish.anchoredPosition.y, -280f, 260f));

        if(fish.anchoredPosition.y >= 260f) {
            playerInventory.GetFish(1);
            gameObject.SetActive(false);
            EventManager.Instance.EndFishing();
        }
    }

    public void ResetStatus(PlayerInventory _playerInventory)
    {
        playerInventory = _playerInventory;
        fish.anchoredPosition = new Vector2(0f, -280f);
        reel.rotation = Quaternion.Euler(0f, 0f, 0f);
        stamina.maxValue = 10f;
        stamina.value = 10f;
    }
}
