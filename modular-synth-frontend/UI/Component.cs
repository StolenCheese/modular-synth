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
    private Vector2 localPos;
    private Vector2 parentpos;
    private float lastPos;
    private bool dragging;
    private int clickOffset = 0;
    private float sliderOffset = 0;
    private int maxSliderOffset=50;
    private int minSliderOffset=-50;
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
    public override void Update(){
        if(isMovable){        
            Console.WriteLine(this.position.X-(2*sliderOffset));
            if (boundingBox.Contains(input.MousePosition()))
            {
                isInteracting=true;
                if (input.LeftMouseClickDown())
                {
                    lastPos=this.position.X;
                    dragging = true;
                    clickOffset = (int)position.X - (int)input.MousePosVector().X;

                    //Console.WriteLine(sliderOffset);
                }
            }else{
                isInteracting=false;
                
            }
            if (dragging)
            {
                //TODO: turn red if invalid placement
                if (input.LeftMouseClickUp())
                {
                    //Console.WriteLine(sliderOffset);
                    dragging = false;
                }
                isInteracting=true;
                this.position.X = (int)input.MousePosVector().X + clickOffset;
                sliderOffset = this.position.X - lastPos;

            }else{
                dragging = false;
                this.position = parentpos + localPos;
                this.position.X += sliderOffset;    
            }
            
        }else{
            this.position = parentpos + localPos;
        }
    }


}

