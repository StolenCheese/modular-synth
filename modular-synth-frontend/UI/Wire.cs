using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
public class Wire : Component
{
    //This is required by ports to get the wire that is about to be connected to them
    public static List<Wire> wires = new List<Wire>();
    private InputManager input = InputManager.GetInstance();
    public Vector2 endPosition;
    Vector2 wireLine;
    int spriteNum = 10;
    Rectangle node;
    public bool isConnected = false;
    public Vector2 Position {private get{return position;} set {this.position = value;}}
    public Port inputPort;
    public Port outputPort;

    public Wire(Vector2 modulePos, Vector2 moduleLocalPos, Texture2D sprite, Color col, String ParameterID, double scale=1) : base(modulePos, moduleLocalPos, sprite, col, ParameterID,scale)
    { 
        //set to true as wire is made when someone clicks on a port
        this.visible = true;

        //TODO: use an alternative? as this seems quite inefficient
        Wire.wires.Add(this);

        
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
    }

    //unique Draw method required to draw wire sprite from start to end positions
    public override void Draw(SpriteBatch spriteBatch)
    {
        wireLine = endPosition - this.position;

        //quick and easy way to get a number of sprites to render that is suitably proportional to line length
        spriteNum = MathHelper.Max((int)Math.Abs(wireLine.X),(int)Math.Abs(wireLine.Y));

        for(int i=0;i<spriteNum+1;i++){
            node = new Rectangle((int)(position.X+i*(wireLine.X/spriteNum)),(int)(position.Y+i*(wireLine.Y/spriteNum)), width, height);

            spriteBatch.Draw(sprite, node,null, colour,0,new Vector2(this.sprite.Width/2,this.sprite.Height/2),SpriteEffects.None,0);
        }       
        //For reding hitbox
        //spriteBatch.Draw(Slider.slider1, boundingBox,colour);

        //Using position instead of rect due to strange behaviour
        
    }


    public override void Update(){  
        
    }
}
