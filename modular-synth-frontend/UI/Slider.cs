using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
internal class Slider : Component
{
    public static Texture2D rail1;
    public static Texture2D slider1;
    public static Texture2D slider2;
    Component track;
    private bool dragging;
    private float clickOffset = 0;
    private float sliderOffset = 0;

    //TODO: make slider offsets based on StationaryComponent
    private int maxSliderOffset=130;
    private int minSliderOffset=0;
    private InputManager input = InputManager.GetInstance();

    public Slider(Vector2 pos, Vector2 localPos, Texture2D trackSprite, Texture2D sliderSprite, Color col, String ParameterID,double trackScale=1,double sliderScale=1) : base(pos, localPos, sliderSprite, col, ParameterID, trackScale)
    { 
        this.parentpos = pos;
        this.localPos = localPos;
        this.track = new Component(pos, localPos, trackSprite, col, ParameterID, sliderScale);
    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 pos){
        track.UpdatePos(pos);
        this.parentpos = pos;
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        track.addComponentToEtyMgr();
        EntityManager.entities.Add(this);
    }

    private float SliderOffset{
        get {return sliderOffset;}
        set {this.sliderOffset = MathHelper.Clamp(value,minSliderOffset,maxSliderOffset);}
    }
    public override void Update(){     
        if (boundingBox.Contains(input.MousePosition()))
        {
            this.isInteracting=true;
            if (input.LeftMouseClickDown())
            {
                dragging = true;
                clickOffset = position.X - input.MousePosVector().X;
            }
        }else{
            this.isInteracting=false;
        }
        if (dragging)
        {
            this.isInteracting=true;
            SliderOffset = input.MousePosVector().X + clickOffset - parentpos.X - localPos.X;
            this.position.X = SliderOffset + parentpos.X + localPos.X;
            if (input.LeftMouseClickUp()){
                dragging = false;
            }
            
        }else{
            this.position = parentpos + localPos;
            this.position.X += SliderOffset;          
        }
    }
}

