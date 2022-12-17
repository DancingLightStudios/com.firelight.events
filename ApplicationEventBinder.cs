namespace Firelight.Events
{
    public delegate void OnAwakeHandler();
    public delegate void OnTickHandler(float deltaTime);
    public delegate void OnFocusGainedHandler();
    public delegate void OnFocusLostHandler();
    public delegate void OnQuittingHandler();
    
    /// <summary>
    /// The event binder allows for the container application to relay lifetime calls into the library for services that
    /// require them. This can be used for Unity or other applications.
    /// </summary>
    public static class ApplicationEventBinder
    {
        public static event OnAwakeHandler OnAwake;
        public static event OnTickHandler OnTick;
        public static event OnFocusGainedHandler OnFocusGained;
        public static event OnFocusLostHandler OnFocusLost;
        public static event OnQuittingHandler OnQuitting;

        public static void Awake()
        {
            OnAwake?.Invoke();
        }

        /// <summary>
        /// This method should be called by the container application when a heartbeat occurs in the container
        /// application. For example, if the container application is Unity, this method should be invoked on the Update
        /// lifetime call of a MonoBehaviour.
        /// </summary>
        /// <param name="deltaTime">The amount of time since the last Tick call was invoked</param>
        public static void Tick(float deltaTime)
        {
            OnTick?.Invoke(deltaTime);
        }

        /// <summary>
        /// This method should be called by the container application when the user focuses the application
        /// </summary>
        public static void FocusGained()
        {
            OnFocusGained?.Invoke();
        }

        /// <summary>
        /// This method should be called by the container application when the user un-focuses the application
        /// </summary>
        public static void FocusLost()
        {
            OnFocusLost?.Invoke();
        }

        /// <summary>
        /// This method should be called by the container application when it begins the quitting process
        /// </summary>
        public static void Quitting()
        {
            OnQuitting?.Invoke();
        }
    }
}