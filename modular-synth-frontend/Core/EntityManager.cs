using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
namespace modular_synth_frontend.Core;
public class EntityManager{

    public static List<Entity> entities = new List<Entity>();

    public static void Update(){
        foreach(Entity entity in entities)
        {
            entity.Update();
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach(Entity entity in entities)
        {
            entity.Draw(spriteBatch);
        }
    }
}