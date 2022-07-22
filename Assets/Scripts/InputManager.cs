using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public PlayerInputs Input { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            Input = new PlayerInputs();
            Input.Enable();
        }
    }
}
