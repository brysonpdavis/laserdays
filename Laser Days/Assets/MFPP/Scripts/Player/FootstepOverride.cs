using UnityEngine;

namespace MFPP
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#footstep-override")]
    public class FootstepOverride : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The override footstep data name to use.")]
        private string footstepOverrideName = "Default";
        /// <summary>
        /// The override footstep data name to use.
        /// </summary>
        public string FootstepOverrideName
        {
            get { return footstepOverrideName; }
            set { footstepOverrideName = value; }
        }
    }
}