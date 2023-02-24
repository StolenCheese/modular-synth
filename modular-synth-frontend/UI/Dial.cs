using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
internal class Dial : Component
{
    public static Texture2D dial1;
    public static Texture2D indicator1;

    Component staticPart;
    private bool rotating;
    private Vector2 clickOffset;
    private double dialRotation = 0;
    private double lastRotation = 0;

    //TODO: make slider offsets based on StationaryComponent
    private double maxRotation;
    private double minRotation=-Math.PI;
    private double dialRotationOffset;
    private InputManager input = InputManager.GetInstance();

    public Dial(Vector2 pos, Vector2 moduleLocalPos, Texture2D staticPartSprite, Texture2D dialSprite, Color col, String ParameterID,double staticPartScale=1,double dialScale=1) : base(pos, moduleLocalPos, dialSprite, col, ParameterID,dialScale)
    { 
        this.modulePos = pos;
        this.moduleLocalPos = moduleLocalPos;
        this.staticPart = new Component(pos, moduleLocalPos, staticPartSprite, col, ParameterID, staticPartScale);

        maxRotation=(Math.PI);
    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 pos){
        staticPart.UpdatePos(pos);
        this.modulePos = pos;
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        staticPart.addComponentToEtyMgr();
        EntityManager.entities.Add(this);
    }

    private double DialRotation{
        get {return dialRotation;}
        set {this.dialRotation = MathHelper.Clamp((float)(value),(float)minRotation,(float)maxRotation);}
    }

    //returns an angle between 0 and 2pi with 0 at +ve y axis and increasing rotating clockwise (broken)
    private double getAngle(Vector2 v){
        if(v.Y<=0&&v.X>=0){
            return Math.PI/2-Math.Atan2(-v.Y,v.X);
        }else if(v.Y>=0&&v.X>=0){
            return Math.PI/2+Math.Atan2(v.Y,v.X);
        } else if(v.Y>=0&&v.X<=0){
            return Math.PI/2+Math.Atan2(v.Y,v.X);
        } else{
            return 5*Math.PI/2-Math.Atan2(-v.Y,v.X);
        }
    }

    public override void Update(){    
        this.position = modulePos + moduleLocalPos; 
        if (boundingBox.Contains(input.MousePosition()))
        {
            this.isInteracting=true;
            if (input.LeftMouseClickDown()){
                rotating = true;
                //Console.WriteLine(String.Format("{0},{1}",clickOffset.X,-clickOffset.Y));
                dialRotationOffset = getAngle(input.MousePosVector() - position);
                //Console.WriteLine(dialRotationOffset*180/Math.PI);
            }
        }else{
            this.isInteracting=false;
        }
        if (rotating){
            this.isInteracting=true;
            dialRotation = getAngle(input.MousePosVector() - position)- dialRotationOffset + lastRotation;
            Console.WriteLine(dialRotation*180/Math.PI);


            this.rotation = dialRotation;
            
            if (input.LeftMouseClickUp()){
                lastRotation = this.rotation;
                rotating = false;
            }
        }
    }
}