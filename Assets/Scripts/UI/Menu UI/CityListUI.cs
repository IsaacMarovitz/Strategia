using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityListUI : MonoBehaviour {

    public GameObject content;
    public GameObject cityPrefab;
    public GameObject cityListMenu;
    public Button cityUIButton;
    public Button xButton;

    public List<City> cities = new List<City>();

    void Start() {
        cityUIButton.onClick.AddListener(ShowMenu);
        xButton.onClick.AddListener(CloseMenu);
    }

    void Update() {
        if (cityListMenu.activeSelf) {
            Player currentPlayer = GameManager.Instance.GetCurrentPlayer();
            foreach (var playerCity in currentPlayer.playerCities) {
                if (!cities.Contains(playerCity)) {
                    Debug.Log("Updating");
                    cities.Clear();
                    foreach (var city in currentPlayer.playerCities) {
                        cities.Add(city);
                    }
                    foreach (Transform child in content.transform) {
                        GameObject.Destroy(child.gameObject);
                    }
                    foreach (City city in cities) {
                        GameObject instantiatedObject = GameObject.Instantiate(cityPrefab);
                        instantiatedObject.transform.SetParent(content.transform);
                        instantiatedObject.transform.localScale = new Vector3(1, 1, 1);
                        instantiatedObject.GetComponent<CityListUIObject>().cityNameText.text = city.cityName;
                    }
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
