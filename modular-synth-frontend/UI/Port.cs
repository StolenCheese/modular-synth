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
    public Port portConnectedFrom;

    //public List<Port> portConnectedTo;
    public Dictionary<Port, Wire> portsConnectedTo = new Dictionary<Port, Wire>();

    public Vector2 Position {get{return position;} private set {this.position = value;}}

    public Wire draggingWire;


    public Port(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D sprite, Color col, string ParamID,bool isInput, double scale=1) : base(modulePos, parentModuleId, moduleLocalPos, sprite, col, ParamID,scale)
    { 

        this.isInput = isInput;

        this.draggingWire = new Wire(modulePos,parentModuleId,moduleLocalPos,sprite,Color.White,"",0.2);

        ports.Add(this);
        
    }

    //if null returns first port it can find that is connected
    private Port portIsConnectedToMe(Port p = null){
        foreach(Port port in ports){
            if(port==p){
                return port;
            }
        }
        return null;
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
    } 

    private Port getInteractingPort(){
        foreach(Port port in ports){
            if(port.isInteracting&&port!=this){
                return port;
            }
        }
        return null;
    }

    private bool portOnSameModule(Port port){
        return port.parentModuleId==this.parentModuleId;
    }
    private bool portIsConnected(Port p){
        if((portConnectedFrom!=null&&portConnectedFrom==p)||(p.portConnectedFrom!=null&&p.portConnectedFrom==this)){
            return true;
        } else {
            return false;
        }
    }

    private bool removeConnectionFrom(Port portFrom=null){
    //remove portConnectedFrom if we arent given specific
    if(portFrom==null){
        portFrom = this.portConnectedFrom;
    }
    if(portFrom!=null){
        if(portFrom.portsConnectedTo.Count!=0&&portConnectedFrom==portFrom&&API.API.unlinkPorts(portFrom,this)){
            portConnectedFrom = null;
            portFrom.portsConnectedTo.Remove(this);
            return true;
        }
        else return false;
    } else {
        return false;
    }
    }
    private bool removeConnectionTo(Port portTo=null){
    if(portsConnectedTo.Count!=0){
        if(portTo==null){
        //random. Not the last port
        portTo = portsConnectedTo.ElementAt(portsConnectedTo.Count-1).Key;
        }
        if(portsConnectedTo.Count!=0&&portTo.portConnectedFrom==this&&API.API.unlinkPorts(this,portTo)){
            portTo.portConnectedFrom = null;
            portsConnectedTo.Remove(portTo);
            return true;
        }
        else return false;
    }
    else return false;
    }

    private bool connectTo(Port portTo=null){
        if(portTo!=null&&!portsConnectedTo.ContainsKey(portTo)){
            if(API.API.linkPorts(this,portTo)){
                portsConnectedTo[portTo] = new Wire(portTo.modulePos,portTo.parentModuleId,portTo.moduleLocalPos,Wire.orangewire,Color.White,"",0.2,true);
                portTo.portConnectedFrom = this;
                return true;
            }
        }
        return false;
    }
    private bool connectFrom(Port portFrom=null){
        if(portFrom!=null&&!portFrom.portsConnectedTo.ContainsKey(this)){
            if(API.API.linkPorts(portFrom,this)){
                portFrom.portsConnectedTo[this] = new Wire(modulePos,parentModuleId,moduleLocalPos,Wire.orangewire,Color.White,"",0.2,true);
                portConnectedFrom = portFrom;
                return true;
            }
        }
        return false;
    }


    public override void Update(){    
        this.position = modulePos + moduleLocalPos; 
        if (boundingBox.Contains(input.MousePosition()))
        {
            this.isInteracting=true;

            
            if (input.LeftMouseClickDown()){
                //removeConnectedTo(this);
                dragging = true;
                clickOffset = position - input.MousePosVector();

            //If we aren't dragging a wire then we may have a wire about to be connected to us
            } else if(input.LeftMouseClickUp()&&!dragging){
                Port portToConnectFrom = getInteractingPort();
                if(portToConnectFrom!=null){
                    portToConnectFrom.dragging = false;

                    if(portIsConnected(portToConnectFrom)){ //delete connections to same port
                        if(!removeConnectionFrom(portToConnectFrom)&&!removeConnectionTo(portToConnectFrom)){
                            Console.WriteLine("failed to remove connection by dragging to same connection");
                        }

                    //stop output to output connections and allow connections on the same module if they are inputs
                    } else if((this.isInput||portToConnectFrom.isInput) &&!(portOnSameModule(portToConnectFrom)&&(!this.isInput||!portToConnectFrom.isInput)) 
                    ){
                        Console.WriteLine("making port connection");

                        connectFrom(portToConnectFrom);
                    }
                }            
            //removing links with right click     
            } else if(input.RightMouseClickDown()&&!dragging){
                //try remove incoming connection. 
                if(!removeConnectionFrom()){
                    if(!removeConnectionTo()){
                        Console.WriteLine("no connection to remove");
                    }
                }
            }
        }else{
            this.isInteracting=false;
        }
        if(dragging){
            this.isInteracting=true;
            this.draggingWire.Position=Position;
            this.draggingWire.endPosition = input.MousePosVector() + clickOffset;

            //hide wire if not over a port
            if(input.LeftMouseClickUp()&&getInteractingPort()==null){
                dragging = false;
            }
        }
    }
}