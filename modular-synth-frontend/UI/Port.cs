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

    //these sames are with respect to visual creation of wires by clicking on a port and
    //dragging, not related to the directionality of the audio signal
    private Wire outGoingWire;
    private Wire inComingWire;

    private bool wireMade = false;

    public Port(Vector2 modulePos, Vector2 moduleLocalPos, Texture2D sprite, Color col, String ParameterID,bool isInput, double scale=1) : base(modulePos, moduleLocalPos, sprite, col, ParameterID,scale)
    { 
        this.isInput = isInput;
        
    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 modulePos){
        this.modulePos = modulePos;
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
    }

    private Wire getDraggedWire(){
        foreach(Wire wire in Wire.wires){
            if (wire.isInteracting == true){
                return wire;
            }
        }
        return null;
    }

    //temporary fix to problem with adding wire to entity manager
    public override void Draw(SpriteBatch spriteBatch){
        base.Draw(spriteBatch);

        if(this.outGoingWire != null){
            this.outGoingWire.Draw(spriteBatch);
        }
    } 

    public override void Update(){    
        this.position = modulePos + moduleLocalPos; 
        if (boundingBox.Contains(input.MousePosition()))
        {
            this.isInteracting=true;

            if (input.LeftMouseClickDown()){

                dragging = true;
                clickOffset = position - input.MousePosVector();

                //Create/toggle visibility of wire if we click on the port
                if(!wireMade){

                    //Placeholder args here for now
                    this.outGoingWire = new Wire(modulePos,moduleLocalPos,sprite,Color.White,"",0.1);
                    
                    //For some reason this crashes the entity manager. TODO: find out why
                    //this.outGoingWire.addComponentToEtyMgr();
                    
                    
                    //We are using the isInteracting bool on wires to check if they are currently being dragged
                    outGoingWire.isInteracting = true;
                    
                } else{

                    this.outGoingWire.visible=true;

                }
            }  else if(input.LeftMouseClickUp()&&!dragging){
                //If we aren't dragging a wire then we may have a wire about to be connected to us
                this.inComingWire = getDraggedWire();
                if(this.inComingWire != null){
                    inComingWire.isConnected = true;
                    inComingWire.isInteracting = false;
                }
            
                }
        }else{
            this.isInteracting=false;
        }

        if (dragging){
            this.isInteracting=true;

            if(!this.outGoingWire.isConnected){
                this.outGoingWire.wireEndPosition = input.MousePosVector() + clickOffset;

                if (input.LeftMouseClickUp()){
                    this.outGoingWire.visible = false;
                    this.isInteracting=false;
                    dragging = false;
                }
            }
        }

        //update the position of the end of the wire if one is connected to us
        if(this.inComingWire!=null){
            this.inComingWire.wireEndPosition = this.position;

            //There is a chance that the port that the wire is being dragged from will make the wire
            //invisible just after the connected port sets the wire to connected
            if(!this.inComingWire.visible){
                this.inComingWire.visible=true;
            }
        }


        if(this.outGoingWire != null){
            this.outGoingWire.UpdatePos(this.modulePos);
 
            //temporary fix to problem with adding wire to entity manager
            this.outGoingWire.Update();
        }
    }
}