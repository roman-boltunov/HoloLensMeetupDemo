using HoloToolkit.Unity;

public class GamepadClientSingleton : Singleton<GamepadClientSingleton>
{
    #region Public Properties

    public Gamepad_Client GamepadClient { get; set; }

    #endregion

    #region Other Methods

    // Use this for initialization
    private void Start()
    {
        this.GamepadClient = this.GetComponent<Gamepad_Client>();
    }

    #endregion
}