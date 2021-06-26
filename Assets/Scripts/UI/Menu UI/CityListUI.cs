using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CityListUI : MonoBehaviour {

    public CameraController cameraController;
    public GameObject content;
    public GameObject cityPrefab;
    public GameObject cityListManager;
    public GameObject cityListMenu;
    public Button cityUIButton;
    public Button xButton;
    public UnitInfo unitInfo;

    public List<CityListUIObject> cityListUIObjects = new List<CityListUIObject>();
    public List<CityListUISettings> cityListUISettings = new List<CityListUISettings>();

    private float cityListManagerZ;

    void Start() {
        cityUIButton.onClick.AddListener(ShowMenu);
        xButton.onClick.AddListener(CloseMenu);
        GameManager.Instance.nextPlayerDelegate += NextPlayer;
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

        for (int i = 0; i < GameManager.Instance.gameInfo.numberOfPlayers; i++) {
            cityListUISettings.Add(new CityListUISettings(Vector2.zero, false));
        }
        cityListManagerZ = cityListManager.transform.position.z;
    }

    void Update() {
        UpdateCityList();
    }

    public void UpdateCityList() {
        Player currentPlayer = GameManager.Instance.GetCurrentPlayer();
        if (currentPlayer != null) {
            List<City> cities = currentPlayer.playerCities;
            
            for (int i = 0; i < cityListUIObjects.Count; i++) {
                cityListUIObjects[i].button.onClick.RemoveAllListeners();
                if (i < cities.Count) {
                    cityListUIObjects[i].gameObject.SetActive(true);
                    cityListUIObjects[i].cityNameText.text = cities[i].cityName;
                    cityListUIObjects[i].unitImage.sprite = unitInfo.allUnits[cities[i].currentIndex].unitIcon;
                    cityListUIObjects[i].turnsToCreateText.text = cities[i].turnsLeft.ToString();
                    int tempNum = i;
                    cityListUIObjects[i].button.onClick.AddListener(() => {FocusCity(tempNum);});
                } else {
                    cityListUIObjects[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void FocusCity(int i) {
        cameraController.Focus(GridUtilities.TileToWorldPos(GameManager.Instance.GetCurrentPlayer().playerCities[i].pos), true);
    }

    public void ShowMenu() {
        cityUIButton.interactable = false;
        cityListMenu.SetActive(true);
    }

    public void CloseMenu() {
        cityUIButton.interactable = true;
        cityListMenu.SetActive(false);
    }

    public void NextPlayer() {
        int currentPlayerIndex = GameManager.Instance.currentPlayerIndex - 1;
        if (currentPlayerIndex == 0) {
            cityListUISettings[cityListUISettings.Count - 1] = new CityListUISettings(new Vector2(cityListManager.transform.localPosition.x, cityListManager.transform.localPosition.y), cityListMenu.activeInHierarchy);
        } else {
            cityListUISettings[currentPlayerIndex - 1] = new CityListUISettings(new Vector2(cityListManager.transform.localPosition.x, cityListManager.transform.localPosition.y), cityListMenu.activeInHierarchy);
        }

        CityListUISettings currentCityListUISettings = cityListUISettings[currentPlayerIndex];

        cityListManager.transform.localPosition = new Vector3(currentCityListUISettings.position.x, currentCityListUISettings.position.y, cityListManagerZ);
        if (currentCityListUISettings.isEnabled) {
            ShowMenu();
        } else {
            CloseMenu();
        }
    }

    [System.Serializable]
    public struct CityListUISettings {
        public Vector2 position;
        public bool isEnabled;

        public CityListUISettings(Vector2 position, bool isEnabled) {
            this.position = position;
            this.isEnabled = isEnabled;
        }
    }
}
