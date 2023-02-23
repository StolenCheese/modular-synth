using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
internal class Component : Interactable
{
    protected Vector2 localPos;
    protected Vector2 parentpos;
    //TODO: make slider offsets based on StationaryComponent
    public bool isInteracting=false;

    public Component(Vector2 pos, Vector2 localPos, Texture2D baseSprite, Color col, String ParameterID,double scale=1) : base(baseSprite, pos+localPos, col,scale)
    { 
        this.parentpos = pos;
        this.localPos = localPos;
    }

    //We want the module that this component belongs to to give the component its coordinates
    public virtual void UpdatePos(Vector2 pos){
        this.parentpos = pos;
    }
    //order is important!
    public virtual void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
    }
    public override void Update(){
        this.position = parentpos + localPos;
    }
}

