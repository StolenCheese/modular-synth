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

    public Slider(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D trackSprite, Texture2D sliderSprite, Color col, String ParameterID,double trackScale=1,double sliderScale=1,bool vertical=false) : base(modulePos, parentModuleId, moduleLocalPos, sliderSprite, col, ParameterID,sliderScale,vertical)
    { 
        this.track = new Component(modulePos, parentModuleId, moduleLocalPos, trackSprite, col, ParameterID, trackScale,vertical);
        if(vertical){
            maxSliderOffset = track.height/2;
            minSliderOffset = -track.height/2;
        }else{
            maxSliderOffset = track.width/2;
            minSliderOffset = -track.width/2;
        }
        
    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 modulePos){
        track.UpdatePos(modulePos);
        this.modulePos = modulePos;
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
            if(vertical){
                SliderOffset = input.MousePosVector().Y + clickOffset - modulePos.Y - moduleLocalPos.Y;
                this.position.Y = SliderOffset + modulePos.Y + moduleLocalPos.Y;
            }else{
                SliderOffset = input.MousePosVector().X + clickOffset - modulePos.X - moduleLocalPos.X;
                this.position.X = SliderOffset + modulePos.X + moduleLocalPos.X;
            }
            
            if (input.LeftMouseClickUp()){
                dragging = false;
            }
            
        }else{
            this.position = modulePos + moduleLocalPos;
            if(vertical){
                this.position.Y += SliderOffset;  
            }else{
                this.position.X += SliderOffset;  
            }
                    
        }
    }
}

