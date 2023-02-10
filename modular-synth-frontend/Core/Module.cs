namespace modular_synth_frontend.Core;
public class Module : Entity{
    public Module(){
        base.AddComponent(new Transform());
        base.AddComponent(new SpriteRenderer());
    }
}