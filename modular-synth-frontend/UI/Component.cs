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
    public static Texture2D rail1;
    public static Texture2D slider1;
    public static Texture2D slider2;

    private Vector2 localPos;
    private Vector2 parentpos;
    private bool dragging;
    private float clickOffset = 0;
    private float sliderOffset = 0;

    //TODO: make slider offsets based on StationaryComponent
    private int maxSliderOffset=130;
    private int minSliderOffset=0;
    public bool isInteracting=false;
    private InputManager input = InputManager.GetInstance();
    private bool isMovable;

    public Component(Vector2 pos, Vector2 localPos, Texture2D baseSprite, Color col, String ParameterID,double scale=1,bool isMovable=false) : base(baseSprite, pos, col,scale)
    { 
        this.parentpos = pos;
        this.localPos = localPos;
        this.isMovable = isMovable;
    }

    //We want the module that this component belongs to to give the component its coordinates
    public void UpdatePos(Vector2 pos){
        this.parentpos = pos;
    }

    //order is important!
    public void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
    }

    private float SliderOffset{
        get {return sliderOffset;}
        set {this.sliderOffset = MathHelper.Clamp(value,minSliderOffset,maxSliderOffset);}
    }
    public override void Update(){
        if(isMovable){       
            //Console.WriteLine(SliderOffset); 
            if (boundingBox.Contains(input.MousePosition()))
            {
                isInteracting=true;
                if (input.LeftMouseClickDown())
                {
                    dragging = true;
                    clickOffset = position.X - input.MousePosVector().X;
                }
            }else{
                isInteracting=false;
            }
            if (dragging)
            {
                isInteracting=true;
                SliderOffset = input.MousePosVector().X + clickOffset - parentpos.X - localPos.X;
                this.position.X = SliderOffset + parentpos.X + localPos.X;
                if (input.LeftMouseClickUp()){
                    dragging = false;
                }
                
            }else{
                this.position = parentpos + localPos;
                this.position.X += SliderOffset;          
            }
            
        }else{
            this.position = parentpos + localPos;
        }
    }


}

