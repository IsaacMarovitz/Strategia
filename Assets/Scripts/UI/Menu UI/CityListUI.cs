using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityListUI : MonoBehaviour {

    public GameObject content;
    public GameObject cityPrefab;
    public GameObject cityListMenu;
    public Button cityUIButton;
    public Button xButton;

    public List<CityListUIObject> cityListUIObjects = new List<CityListUIObject>();

    void Start() {
        cityUIButton.onClick.AddListener(ShowMenu);
        xButton.onClick.AddListener(CloseMenu);
        int numberOfCities = GameManager.Instance.grid.cityTiles.Count;
        for (int i = 0; i < numberOfCities; i++) {
            GameObject instantiatedObject = GameObject.Instantiate(cityPrefab);
            instantiatedObject.transform.SetParent(content.transform);
            instantiatedObject.transform.localScale = Vector3.one;
            instantiatedObject.transform.localPosition = Vector3.zero;
            instantiatedObject.transform.localRotation = Quaternion.identity;
            instantiatedObject.transform.name = $"City List UI Object {i+1}";
            cityListUIObjects.Add(instantiatedObject.GetComponent<CityListUIObject>());
        }
    }

    void Update() {
        UpdateCityList();
    }

    public void UpdateCityList() {
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer();
        if (currentPlayer != null) {
            List<City> cities = currentPlayer.playerCities;
            
            for (int i = 0; i < cityListUIObjects.Count; i++) {
                if (i < cities.Count) {
                    cityListUIObjects[i].gameObject.SetActive(true);
                    cityListUIObjects[i].cityNameText.text = cities[i].cityName;
                } else {
                    cityListUIObjects[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowMenu() {
        cityUIButton.interactable = false;
        cityListMenu.SetActive(true);
    }

    public void CloseMenu() {
        cityUIButton.interactable = true;
        cityListMenu.SetActive(false);
    }
}
