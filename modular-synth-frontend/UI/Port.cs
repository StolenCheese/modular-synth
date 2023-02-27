using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
internal class Port : Component
{
    public static Texture2D port1;
    private InputManager input = InputManager.GetInstance();

    private bool isInput;
    private Vector2 clickOffset;
    private bool dragging = false;
    private Wire wire;

    public Port(Vector2 modulePos, Vector2 moduleLocalPos, Texture2D sprite, Color col, String ParameterID,bool isInput, double scale=1) : base(modulePos, moduleLocalPos, sprite, col, ParameterID,scale)
    { 
        this.isInput = isInput;
        wire = new Wire(modulePos,moduleLocalPos,sprite,col,"",0.1);
    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 modulePos){
        this.modulePos = modulePos;
        this.wire.UpdatePos(modulePos);
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
        EntityManager.entities.Add(this.wire);
    }

    private void linkWireToPortPos(){

    }

    public override void Update(){    
        this.position = modulePos + moduleLocalPos; 
        if (boundingBox.Contains(input.MousePosition()))
        {
            this.isInteracting=true;
            if (input.LeftMouseClickDown()){
                this.wire.visible = true;
                dragging = true;
                clickOffset = position - input.MousePosVector();
            }
        }else{
            this.isInteracting=false;
        }
        if (dragging){
            this.isInteracting=true;
            wire.wireEndPosition = input.MousePosVector() + clickOffset;
            Console.WriteLine(wire.wireEndPosition);

            if (input.LeftMouseClickUp()){
                this.wire.visible = false;
                this.isInteracting=false;
                dragging = false;
            }
        }
    }
}