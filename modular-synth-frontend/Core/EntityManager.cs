using System.Collections.Generic;
namespace modular_synth_frontend.Core;
public static class EntityManager{
    public static List<Entity> entities;

    public static void Update(){
        foreach(Entity entity in entities)
        {
            entity.Update();
        }
    }

    public static void FixedUpdate(){
        foreach (Entity entity in entities)
        {
            entity.FixedUpdate();
        }
    }
}