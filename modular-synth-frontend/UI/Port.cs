using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using modular_synth_frontend.API;

namespace modular_synth_frontend.UI;
public class Port : Component
{
    
    public static Texture2D port1;

    public static List<Port> ports = new List<Port>();
    private InputManager input = InputManager.GetInstance();

    private bool isInput;
    private Vector2 clickOffset;

    public bool dragging = false;

    //this should not be necessary. For some reason the parameterID is read as null when set in Components
    public new string parameterID;
    public Port portConnectedFrom;
    public Port portConnectedTo;
    public Wire wire;
    public Vector2 Position {get{return position;} private set {this.position = value;}}



    public Port(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D sprite, Color col, string ParamID,bool isInput, double scale=1) : base(modulePos, parentModuleId, moduleLocalPos, sprite, col, ParamID,scale)
    { 
        this.isInput = isInput;

        this.wire = new Wire(modulePos,parentModuleId,moduleLocalPos,sprite,Color.White,"",0.2);

        //this should not be necessary. For some reason the parameterID is read as null when set in Components
        this.parameterID = ParamID;
        Console.WriteLine(this.parameterID);

        ports.Add(this);
        
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
    }

    public override void UpdatePos(Vector2 modulePos){
        this.modulePos = modulePos;
    }


    //temporary fix to problem with adding wire to entity manager
    public override void Draw(SpriteBatch spriteBatch){
        base.Draw(spriteBatch);
        if(portConnectedTo!=null||dragging){
            wire.Draw(spriteBatch);
        }
    } 

    private Port getInteractingPort(){
        foreach(Port port in ports){
            if(port.isInteracting&&port.parentModuleId!=this.parentModuleId){
                return port;
            }
        }
        return null;
    }

    private bool portIsConnected(Port p){
        if((portConnectedFrom!=null&&portConnectedFrom==p)||(portConnectedTo!=null&&portConnectedTo==p)){
            return true;
        } else {
            return false;
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

            //If we aren't dragging a wire then we may have a wire about to be connected to us
            } else if(input.LeftMouseClickUp()&&!dragging){
                Port portToConnect = getInteractingPort();
                if(portToConnect!=null){
                    portToConnect.dragging = false;

                    //to stop output to output connections TODO: add backend validation check here
                    if(!(this.isInput&&portToConnect.isInput)&&(this.isInput||portToConnect.isInput)&&!portIsConnected(portToConnect)){
                        Console.WriteLine("making port connection");

                        //remove old connection
                        if(portToConnect.portConnectedTo!=null){
                            API.API.unlinkPorts(portToConnect,portToConnect.portConnectedTo);
                        }

                        if(API.API.linkPorts(portToConnect,this)){
                            portToConnect.portConnectedTo = this;
                            this.portConnectedFrom = portToConnect;

                        } else if(portToConnect.portConnectedTo!=null){
                            //linking rejected by backend. restore
                            API.API.linkPorts(portToConnect,portToConnect.portConnectedTo);
                        }
                       
                        
                    }
                }                 
            }
        }else{
            this.isInteracting=false;
        }
        wire.Position=this.Position;
        if(dragging){
            this.isInteracting=true;
            this.wire.endPosition = input.MousePosVector() + clickOffset;

            //hide wire if not over a port
            if(input.LeftMouseClickUp()&&getInteractingPort()==null){
                this.portConnectedTo = null;
                dragging = false;
            }
        } else {
            if(portConnectedTo!=null){
                this.wire.endPosition = portConnectedTo.Position;
            } 
        }
    }
}