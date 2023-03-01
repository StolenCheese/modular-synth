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

    public static List<Port> ports = new List<Port>();
    private InputManager input = InputManager.GetInstance();

    private bool isInput;
    private Vector2 clickOffset;

    private bool dragging = false;

    //these are with respect to visual creation of wires by clicking on a port and
    //dragging, not related to the directionality of the audio signal
    private Wire outGoingWire;
    private Wire inComingWire;
    public int parentModuleId;

    public Port(Vector2 modulePos, Vector2 moduleLocalPos, Texture2D sprite, Color col, String ParameterID,bool isInput, int parentModuleId, double scale=1) : base(modulePos, moduleLocalPos, sprite, col, ParameterID,scale)
    { 
        this.isInput = isInput;

        this.parentModuleId = parentModuleId;

        ports.Add(this);
        
    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 modulePos){
        this.modulePos = modulePos;
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
    }


    //temporary fix to problem with adding wire to entity manager
    public override void Draw(SpriteBatch spriteBatch){
        base.Draw(spriteBatch);
    } 

    private bool PortIsInteracting(){
        foreach(Port port in ports){
            if(port.isInteracting&&port!=this){
                return true;
            }
        }
        return false;
    }

    public bool portIsModuleLocal(Port port){
        if(this.parentModuleId == port.parentModuleId){
            return true;
        } else {
            return false;
        }
    }

    private bool portIsConnected(){
        return incomingWireIsConnected()||outgoingWireIsConnected();
    }
    private bool outgoingWireIsConnected(){
         if(this.outGoingWire!=null&&this.outGoingWire.isConnected==true){
            return true;
        } else{
            return false;
        }
    }
    private bool incomingWireIsConnected(){
         if(this.inComingWire!=null&&this.inComingWire.isConnected==true){
            return true;
        }else{
            return false;
        }
    }

    private void deleteOldWire(){
        
    }

    private void createOutgoingWire(){

        Console.WriteLine("outgoing is connected: {0}",outgoingWireIsConnected());

        //Check that we are allowed to drag a wire from us

        if(this.outGoingWire==null){
            //Placeholder args here for now
            this.outGoingWire = new Wire(modulePos,moduleLocalPos,sprite,Color.White,"",0.1);

            //For some reason this crashes the entity manager. TODO: find out why
            //this.outGoingWire.addComponentToEtyMgr();

        } else{
            this.outGoingWire.visible = true;
        }

        //We are using the isInteracting bool on wires to check if they are currently being dragged
        this.outGoingWire.inputPort = this;
        this.outGoingWire.isInteracting = true;
            

    }
        

    private Wire getDraggedWire(){
        foreach(Wire wire in Wire.wires){
            if (wire.isInteracting == true){
                return wire;
            }
        }
        Console.WriteLine("No wire");
        return null;
    }

    public override void Update(){    
        this.position = modulePos + moduleLocalPos; 
        if (boundingBox.Contains(input.MousePosition()))
        {
            this.isInteracting=true;

            
            if (input.LeftMouseClickDown()){
  
                dragging = true;
                clickOffset = position - input.MousePosVector();

                //Create/toggle visibility of wire
                 if(!outgoingWireIsConnected()){
                    createOutgoingWire();
                 } else {
                    //move wire 
                    this.outGoingWire.isInteracting = true;

                 }

                //If we aren't dragging a wire then we may have a wire about to be connected to us
            }  else if(input.LeftMouseClickUp()&&!dragging&&(this.isInput||!portIsConnected())){

                //Console.WriteLine("test");
                Wire incomingWiretoLink = getDraggedWire();
                Console.WriteLine("incomingWiretoLink!=null-{0}",incomingWiretoLink!=null);
                if(incomingWiretoLink!=null){    

                    Console.WriteLine("!portIsModuleLocal-{0}",!portIsModuleLocal(incomingWiretoLink.inputPort));

                    Console.WriteLine("!incomingWiretoLink.inputPort.dragging-{0}",incomingWiretoLink.inputPort.dragging);


                    //make sure we arent connecting to a module local port
                    if(!incomingWireIsConnected()&&(this.isInput||incomingWiretoLink.inputPort.isInput)&&!portIsModuleLocal(incomingWiretoLink.inputPort)&&(incomingWiretoLink.outputPort==null||incomingWiretoLink.outputPort.dragging)){
                        inComingWire = incomingWiretoLink;
                        inComingWire.isConnected = true;
                        inComingWire.isInteracting = false;
                        inComingWire.outputPort = this;
                    
                    //case when wire is trying to make us the inputport
                    } else if(!outgoingWireIsConnected()&&(this.isInput||incomingWiretoLink.outputPort.isInput)&&!portIsModuleLocal(incomingWiretoLink.inputPort)&&incomingWiretoLink.inputPort.dragging){
                        inComingWire = incomingWiretoLink;
                        inComingWire.isConnected = true;
                        inComingWire.isInteracting = false;
                        inComingWire.inputPort = this;
                    }
                    else {
                        incomingWiretoLink.visible = false;
                        incomingWiretoLink.isInteracting = false;
                    }
                }
            }
        }else{
            this.isInteracting=false;
        }

        if (dragging){
            //Console.WriteLine("drag");
            this.isInteracting=true;
            if(this.outGoingWire!=null){

                Console.WriteLine(outGoingWire.inputPort==this);

                //check if we are moving a connected wire or making a new connection
                if(outgoingWireIsConnected()&&(outGoingWire.inputPort==this)){
                    this.outGoingWire.updatePos(input.MousePosVector() + clickOffset);   
                }else if(incomingWireIsConnected()&&inComingWire.outputPort==this){
                    this.inComingWire.wireEndPosition = input.MousePosVector() + clickOffset;
                } else {
                    this.outGoingWire.wireEndPosition = input.MousePosVector() + clickOffset;
                }
                //     (A n !B) u (!A n B)

                if (input.LeftMouseClickUp()){

                    dragging = false;

                    //disable wire when we click up, else let port being hovered over handle wire
                    if(!PortIsInteracting()){
                        this.outGoingWire.visible = false;
                        this.outGoingWire.isInteracting = false;
                        this.isInteracting=false;
                    }
                    
                    
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
            if(!dragging){
                this.outGoingWire.UpdateModulePos(this.modulePos);
                this.outGoingWire.Update();
            }
            //temporary fix to problem with adding wire to entity manager
            
        }
    }
}