using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace modular_synth_frontend.UI;
internal class Module : Interactable
{
    private InputManager input = InputManager.GetInstance();
    private bool dragging;
    private Vector2 clickOffset;

    private List<Component> components = new List<Component>();

    public Module(Texture2D sprite) : base(sprite)
    { }

    public Module(Texture2D sprite, Vector2 pos,SpriteBatch spriteBatch) : base(sprite, pos)
    { 
        components.Add(new Slider(pos, new Vector2(this.sprite.Width/2,250),Slider.rail1,Slider.slider2,Color.White,"",0.7,0.7));
        components.Add(new Slider(pos, new Vector2(this.sprite.Width/2-50,this.sprite.Height/2-50),Slider.rail1,Slider.slider2,Color.White,"",0.7,0.7,true));
        components.Add(new Dial(pos, new Vector2(this.sprite.Width/2+50,this.sprite.Height/2-50),Dial.indicator1,Dial.dial1,Color.White,"",0.7,0.7));

        addToEtyMgr();
    }

    public void addToEtyMgr(){
        EntityManager.entities.Add(this);
        foreach(Component c in components){
            c.addComponentToEtyMgr();
        }
    }
    private void updateComponentPositions(){
    foreach(Component c in components){
        c.UpdatePos(position);
        }
    }

    private bool isHoveringOverComponent(){
        foreach(Component c in components){
            if(c.isInteracting) {
                return true;
            }
        }
        return false;
    }


    public override void Update()
    {
        if(!isHoveringOverComponent()){

            if (boundingBox.Contains(input.MousePosition()))
            {
                if (input.LeftMouseClickDown())
                {
                    dragging = true;
                    clickOffset = position - input.MousePosVector();
                }
            }

            if (dragging)
            {
                position = input.MousePosVector() + clickOffset;
                //TODO: turn red if invalid placement
                if (input.LeftMouseClickUp())
                {
                    dragging = false;
                    boundingBox = new Rectangle((int)position.X, (int)position.Y, sprite.Width, sprite.Height); //TODO: edit this to include collision box offset size instead of just sprite width + height
                }
            }
            
        }
        updateComponentPositions();
    }
       

    //set on spawn
    public void Drag()
    {
        dragging = true;
        clickOffset = position - input.MousePosVector();
    }
}