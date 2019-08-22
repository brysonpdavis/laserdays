using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New NarrationSettings", menuName = "Narration Settings/Default", order = 52)]

public class NarrationSettings : ScriptableObject
{
    public bool letterByLetter = true;

    public bool animatedOpen;

    public bool animatedClose;

    public Image background;

    public Animation openAnimation;

    public Animation closeAnimation;

    public TextAsset text;

    public Vector2 position;

    public Vector2 scale;

    protected INarrationActor _actor;

    public virtual void OnActivate()
    {
        if (_actor != null)
            _actor.OnNarrationActivate();
    }

    public virtual void OnDeactivate()
    {
        if (_actor != null)
            _actor.OnNarrationDeactivate();
    }

    public virtual void OnNext()
    {
        if (_actor != null)
            _actor.NextNarration();
    }
}
