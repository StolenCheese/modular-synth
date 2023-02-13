namespace modular_synth_frontend.Core;
public class Module : Entity{
    Transform t = new Transform();
    SpriteRenderer sprite = new SpriteRenderer();

    public Module(){
        base.AddComponent(t);
        base.AddComponent(sprite);
    }
}