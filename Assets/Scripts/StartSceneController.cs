using UnityEngine;

using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private Button onePlayerButton;
    [SerializeField] private Button twoPlayerButton;
    // [SerializeField] private GameObject onePlayerMashroom;
    // [SerializeField] private GameObject twoPlayerMashroom;
    private PlayerInputActions _playerInputActions;


    private void Start()
    {
        // Create a new PlayerInputActions object
        _playerInputActions = new PlayerInputActions();
        // Set the initial selected button
        EventSystem.current.SetSelectedGameObject(onePlayerButton.gameObject);
        // Assign button listeners
        onePlayerButton.onClick.AddListener(() => StartGame(1));
        twoPlayerButton.onClick.AddListener(() => StartGame(2));
    }

    private void Update()
    {
        var input = _playerInputActions.UI.Move.ReadValue<Vector2>().y;
        // Navigate to the next button when the Down Arrow key is pressed
        if (input > 0)
        {
            // var vector3 = littleMashroom.gameObject.transform.position;
            // vector3.y -= 1f;
            // littleMashroom.gameObject.transform.position = vector3;
            // onePlayerMashroom.gameObject.SetActive(false);
            // twoPlayerMashroom.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(twoPlayerButton.gameObject);
        }
        // Navigate to the previous button when the Up Arrow key is pressed
        else if (input < 0)
        {
            // onePlayerMashroom.gameObject.SetActive(true);
            // twoPlayerMashroom.gameObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(onePlayerButton.gameObject);
        }
        
        // Select the button when the Enter key is pressed
        if (_playerInputActions.Player.Jump.triggered)
        {
            EventSystem.current.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
        }
    }
    
    private void StartGame(int playerCount)
    {
        // Store player count (can be used in the game scene)
        PlayerPrefs.SetInt("PlayerCount", playerCount);

        // Load the Game Start screen first
        SceneManager.LoadScene("Loading Screen");
    }
}
