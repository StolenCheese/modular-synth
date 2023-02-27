using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace modular_synth_frontend.UI;
internal class Wire : Component
{
    private InputManager input = InputManager.GetInstance();
    public Vector2 wireEndPosition;
    Vector2 wireLine;
    int spriteNum = 10;
    Rectangle node;

    public Wire(Vector2 modulePos, Vector2 moduleLocalPos, Texture2D sprite, Color col, String ParameterID, double scale=1) : base(modulePos, moduleLocalPos, sprite, col, ParameterID,scale)
    { 
        this.visible = false;
    }

    //We want the module that this component belongs to to give the component its coordinates
    public override void UpdatePos(Vector2 modulePos){
        this.modulePos = modulePos;
    }

    //order is important!
    public override void addComponentToEtyMgr(){
        EntityManager.entities.Add(this);
    }

    //unique Draw method required to draw wire sprite from start to end positions
    public override void Draw(SpriteBatch spriteBatch)
    {
        wireLine = wireEndPosition - this.position;

        //quick and easy way to get a number of sprites to render that is suitably proportional to line length
        spriteNum = MathHelper.Max((int)Math.Abs(wireLine.X),(int)Math.Abs(wireLine.Y));

        for(int i=0;i<spriteNum+1;i++){
            node = new Rectangle((int)(position.X+i*(wireLine.X/spriteNum)),(int)(position.Y+i*(wireLine.Y/spriteNum)), width, height);
            spriteBatch.Draw(sprite, node,null, colour);
        }       
        //For reding hitbox
        //spriteBatch.Draw(Slider.slider1, boundingBox,colour);

        //Using position instead of rect due to strange behaviour
        
    }


    public override void Update(){    
        this.position = modulePos + moduleLocalPos; 
    }
}
