using UnityEngine;

namespace CodingJar
{
    /// <summary>
    /// Denotes a field as read-only
    /// </summary>
    public class ReadonlyAttribute : PropertyAttribute {}

	public class BeginReadonlyAttribute : PropertyAttribute {}

	public class EndReadonlyAttribute : PropertyAttribute {}
}
