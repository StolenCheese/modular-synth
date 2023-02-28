using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using modular_synth_frontend.UI;
using System;
namespace modular_synth_frontend.Core;
public class EntityManager{

    public static List<Entity> entities = new List<Entity>();

    public static void Update(){
        foreach(Entity entity in entities)
        {
            if (entity.enabled)
            {
                entity.Update();
            }
        }
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach(Entity entity in entities)
        {
            if (entity.visible)// omitted entity.GetType()!=typeof(Wire) check as we dont add wires to entity manager
            {
                entity.Draw(spriteBatch);
            }

        //render wires at the front by calling this last
        }
        foreach(Wire wire in Wire.wires){
            if (wire.visible){wire.Draw(spriteBatch);}
        }
    }
}