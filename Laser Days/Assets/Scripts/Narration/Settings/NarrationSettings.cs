using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New NarrationSettings", menuName = "Narration Settings/Default", order = 52)]

public class NarrationSettings : ScriptableObject
{
    public bool letterByLetter = true;

    public bool animatedOpen;

    public bool animatedClose;

    public Sprite background;

    public string openAnimation;

    public string closeAnimation;

    public Vector2 position;

    public Vector2 scale;

    public Color backgroundRectangleColor;

    public Color decorationColor;

    public Color textColor;

    public Font textFace;

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
