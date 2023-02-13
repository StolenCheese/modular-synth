using Microsoft.Xna.Framework;

namespace modular_synth_frontend.Core;

public class Transform : Component {
    private Vector2 position; //relative to world space not screenspace is the idea here (0,9) for this program will be centre of main screen when completely static

    public Transform()
    {
        position = new Vector2(0, 0);
    }
    public Transform(Vector2 position)
    {
        this.position = position;
    }
    public Transform(float x, float y)
    {
        position = new Vector2(x, y);
    }
}