using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace modular_synth_frontend.Core;

//Superclass for all """objects""" in program
public abstract class Entity
{
    // <summary>
    // Checks if we run Update events on entity
    // </summary>
    public bool enabled = true;
    
    // <summary>
    // Checks if we run Draw events on entity
    // </summary>
    public bool visible = true;

    protected Entity() {}

    public abstract void Update();
    //public abstract void Destroy();
    public abstract void Draw(SpriteBatch spriteBatch);
    //public abstract void Draw(SpriteBatch spriteBatch, int width, int height);
}


//List of components the entity holds
//protected List<Component> componentList;

/*
protected void AddComponent(Component component){
    componentList.Add(component);
}

public void Update(){
    foreach(Component component in componentList){
        component.Update();
        Debug.WriteLine("aheem heem wheemper");
    }
}

public void FixedUpdate(){
    foreach(Component component in componentList){
        component.FixedUpdate();
    }
}
*/