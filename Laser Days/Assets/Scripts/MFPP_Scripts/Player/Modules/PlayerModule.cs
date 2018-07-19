using UnityEngine;

namespace MFPP
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#creating-a-module")]
    [RequireComponent(typeof(Player))]
    public abstract class PlayerModule : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Is this PlayerModule enabled?")]
        private new bool enabled = true;
        /// <summary>
        /// Is this <see cref="PlayerModule"/> enabled?
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        [SerializeField]
        [Tooltip("The execution order of this PlayerModule, lower means earlier execution.")]
        private int executionOrder = 0;
        /// <summary>
        /// The execution order of this <see cref="PlayerModule"/>, lower means earlier execution.
        /// </summary>
        public int ExecutionOrder
        {
            get { return executionOrder; }
            set { executionOrder = value; }
        }

        private Player player;
        /// <summary>
        /// The attached <see cref="global::Player"/> to this <see cref="PlayerModule"/>.
        /// </summary>
        protected Player Player
        {
            get { return player ?? (player = GetComponent<Player>()); }
        }

        /// <summary>
        /// The attached camera to this <see cref="Player"/>. Shortcut equivalent of "Player.Main.Camera"
        /// </summary>
        protected Camera Camera
        {
            get { return Player.Main.Camera; }
        }

        /// <summary>
        /// Used for initialization.
        /// </summary>
        public virtual void Initialize() { }
        /// <summary>
        /// Called right before updating the player.
        /// </summary>
        public virtual void BeforeUpdate() { }
        /// <summary>
        /// Called right after updating the player.
        /// </summary>
        public virtual void AfterUpdate() { }

        /// <summary>
        /// Similar to <see cref="Input.GetAxis(string)"/> but also taking into account <see cref="MFPP.Player.ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="axisName">The name of the axis.</param>
        /// <returns>The value of the virtual axis identified by <see cref="axisName"/>.</returns>
        protected float GetAxis(string axisName)
        {
            return Player.Controls.ControlsEnabled ? Input.GetAxis(axisName) : 0f;
        }
        /// <summary>
        /// Similar to <see cref="Input.GetAxisRaw(string)"/> but also taking into account <see cref="MFPP.Player.ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="axisName">The name of the axis.</param>
        /// <returns>The value of the virtual axis identified by <see cref="axisName"/>.</returns>
        protected float GetAxisRaw(string axisName)
        {
            return Player.Controls.ControlsEnabled ? Input.GetAxisRaw(axisName) : 0f;
        }
        /// <summary>
        /// Returns the current axis value, based on whetever we use <see cref="MFPP.Player.ControlSettings.RawInput"/> and also taking into account <see cref="MFPP.Player.ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="axisName">The axis name.</param>
        /// <returns>The value of the virtual axis identified by <see cref="axisName"/>.</returns>
        protected float GetCurrentAxis(string axisName)
        {
            return Player.Controls.RawInput ? GetAxisRaw(axisName) : GetAxis(axisName);
        }
        /// <summary>
        /// Similar to <see cref="Input.GetButton(string)"/> but also taking into account <see cref="MFPP.Player.ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="buttonName">The name of the axis.</param>
        /// <returns>Returns true while the virtual button identified by <see cref="buttonName"/> is held down.</returns>
        protected bool GetButton(string buttonName)
        {
            return Player.Controls.ControlsEnabled && Input.GetButton(buttonName);
        }
        /// <summary>
        /// Similar to <see cref="Input.GetButtonDown(string)"/> but also taking into account <see cref="MFPP.Player.ControlSettings.ControlsEnabled"/>.
        /// </summary>
        /// <param name="buttonName">The name of the axis.</param>
        /// <returns>Returns true during the frame the user pressed down the virtual button identified by <see cref="buttonName"/>.</returns>
        protected bool GetButtonDown(string buttonName)
        {
            return Player.Controls.ControlsEnabled && Input.GetButtonDown(buttonName);
        }
    }
}