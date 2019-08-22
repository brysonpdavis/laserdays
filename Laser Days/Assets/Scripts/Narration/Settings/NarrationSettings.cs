using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New NarrationSettings", menuName = "Narration Settings/Default", order = 52)]

public class NarrationSettings : ScriptableObject
{
    public bool letterByLetter = true;

    public bool animatedOpen;

    public bool animatedClose;

    public Image background;

    public string openAnimation;

    public string closeAnimation;

    public TextAsset text;

    public Vector2 position;

    public Vector2 scale;

    public INarrationActor actor;

    public virtual void OnActivate()
    {
        if (actor != null)
            actor.OnNarrationActivate();
    }

    public virtual void OnDeactivate()
    {
        if (actor != null)
            actor.OnNarrationDeactivate();
    }

    public virtual void OnNext()
    {
        if (actor != null)
            actor.NextNarration();
    }
}
