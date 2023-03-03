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
internal class Dial : Component
{
    public static Texture2D dial1;
    public static Texture2D indicator1;
    Component staticPart;
    private bool rotating;
    private double dialRotation = 0;
    private double lastRotation = 0;
    private double r;
    private double lastVal;
    private double maxRotation=2*Math.PI;
    private double minRotation=0;
    private double dialRotationOffset;
    private InputManager input = InputManager.GetInstance();

    public Dial(Vector2 modulePos, int parentModuleId, Vector2 moduleLocalPos, Texture2D staticPartSprite, Texture2D dialSprite, Color col, string parameterID,double staticPartScale=1,double dialScale=1) : base(modulePos, parentModuleId, moduleLocalPos, dialSprite, col, parameterID,dialScale)
    { 
        this.staticPart = new Component(modulePos, parentModuleId, moduleLocalPos, staticPartSprite, col, parameterID, staticPartScale);
        this.rotation = minRotation;
        this.lastRotation = minRotation;
        this.dialRotation = minRotation;

    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 modulePos){
        staticPart.UpdatePos(modulePos);
        this.modulePos = modulePos;
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        staticPart.addComponentToEtyMgr();
        EntityManager.entities.Add(this);
    }

    public override void sendValToServer(){
        if(rotation>maxRotation||rotation<minRotation){
            Console.WriteLine("Rotation is greater than max allowed!! skipping send");
        } else {
            double svRange = maxValueForServer-minValueForServer;
            double thisRange = maxRotation-minRotation;
            //translate value relative to this range to value relative to server range
            //Console.WriteLine($"SliderOffset:{SliderOffset},svRange:{svRange},thisRange:{thisRange},minValueForServer:{minValueForServer},minSliderOffset:{minSliderOffset}");
            float val = (float)((rotation-minRotation)*svRange/thisRange+minValueForServer);
            if(val!=lastSentVal){
                API.API.setValue(this.parentModuleId, this.parameterID, val);
                lastSentVal=val;
            }
        }
    }

    private double DialRotation{
        get {return dialRotation;}
        set {
            //clamp value to a range between 0 and 2pi
            if(value<0){
                value +=2*Math.PI;
            }else if(value>2*Math.PI){
                value -= 2*Math.PI;
            }

            //Console.WriteLine($"value:{value}");

            double deltaR = value - lastVal;

            //Console.WriteLine($"deltaR:{deltaR},lastVal<maxRotation:{lastVal<maxRotation},value>maxRotation:{value>maxRotation}");

            //check we are not trying to go past limits. delta can only be this big when we get to limits at rotation wrap border
            if(deltaR<-3*maxRotation/4||(lastVal<maxRotation&&value>maxRotation)){
                    dialRotation = maxRotation;
                } else if(deltaR>3*maxRotation/4||(lastVal>minRotation&&value<minRotation)){
                    dialRotation = minRotation;
                }
            //ignore unreasonably high changes in deltaR in a frame or no changes at all ()
            if(deltaR!=0&&Math.Abs(deltaR)<Math.PI/7){
                
                if(deltaR<0){
                    //clamp probably not necessary but left for extra safety
                    dialRotation = MathHelper.Max((float)(value),(float)minRotation);
                } else if(deltaR>0){
                    
                    
                    dialRotation = MathHelper.Min((float)(value),(float)maxRotation);
                
                }
                lastVal = value;
            }
            
            //Console.WriteLine("value:{0},dialRotation:{1},deltaR:{2}",value*180/Math.PI,dialRotation*180/Math.PI,deltaR*180/Math.PI);
        }
    }

    //returns an angle between 0 and 2pi with 0 at +ve y axis and increasing rotating clockwise (broken)
    private double getAngle(Vector2 v){
        int rotations = 0;
        if(v.Y<=0&&v.X>=0){
            r = Math.PI/2-Math.Atan2(-v.Y,v.X);
        }else if(v.Y>=0&&v.X>=0){
            r =  Math.PI/2+Math.Atan2(v.Y,v.X);
        } else if(v.Y>=0&&v.X<=0){
            r =  Math.PI/2+Math.Atan2(v.Y,v.X);
        } else{
            r =  5*Math.PI/2-Math.Atan2(-v.Y,v.X);
        }
        if (r < 0) { r += 360f*(rotations+1);}
        return r;
    }

    public override void Update(){ 
        this.position = modulePos + moduleLocalPos; 
        if (boundingBox.Contains(input.MousePosition()))
        {
            this.isInteracting=true;
            if (input.LeftMouseClickDown()){
                rotating = true;
                dialRotationOffset = getAngle(input.MousePosVector() - position);
            } else if(input.MouseWheelUp()){
                Console.WriteLine("MouseWheelUp");
                DialRotation += Math.PI/8;

                this.rotation = DialRotation;
                sendValToServer();
                lastRotation = this.rotation;
            } else if(input.MouseWheelDown()){
                Console.WriteLine("MouseWheelDown");
                DialRotation -= Math.PI/8;

                this.rotation = DialRotation;
                sendValToServer();
                lastRotation = this.rotation;
            }
        }else{
            this.isInteracting=false;
        }
        if (rotating){
            this.isInteracting=true;
            DialRotation = getAngle(input.MousePosVector() - position)- dialRotationOffset+ lastRotation;
            //Console.WriteLine(String.Format("DialRotation:{0},dialRotationOffset:{1},lastRotation:{2},clkWiseLock:{3},aClkWiseLock:{4}",DialRotation*180/Math.PI,dialRotationOffset*180/Math.PI,lastRotation*180/Math.PI,clkWiseLock,aClkWiseLock));
            this.rotation = DialRotation;
          
            if (input.LeftMouseClickUp()){
                this.isInteracting=false;
                lastRotation = this.rotation;
                rotating = false;
            }
        //only make an api call when we are rotating
        sendValToServer();
        }
    }
}