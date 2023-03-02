using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using SynthAPI;
using modular_synth_frontend.API;
namespace modular_synth_frontend.UI;
public class Module : Interactable
{
    private InputManager input = InputManager.GetInstance();
    private bool dragging=false;
    private Vector2 clickOffset;

    //this is used for api calls and ports to check if they are on the same module
    private static int modules = 0;
    public int ModuleId { get; private set; }

    private List<Component> components = new List<Component>();

    public SCSection scSection;

    public string function;


    public Module(Texture2D sprite) : base(sprite)
    { }

    public Module(Texture2D sprite, Vector2 pos) : base(sprite, pos)
    { 
        this.ModuleId = modules++;

        function = "sin-ar";

        components.Add(new Slider(pos, ModuleId, new Vector2(this.sprite.Width/2,250),Slider.rail1,Slider.slider2,Color.White,"add",0.7,0.7));
        //components.Add(new Slider(pos, ModuleId, new Vector2(this.sprite.Width/2-50,this.sprite.Height/2-50),Slider.rail1,Slider.slider2,Color.White,"freq",0.7,0.7,true));
        components.Add(new Dial(pos, ModuleId, new Vector2(this.sprite.Width/2+50,this.sprite.Height/2-50),Dial.indicator1,Dial.dial1,Color.White,"mul",0.7,0.7));
        
        components.Add(new Port(pos, ModuleId, new Vector2(this.sprite.Width/2-50,300),Port.port1,Color.White,"add",true));
        components.Add(new Port(pos, ModuleId, new Vector2(this.sprite.Width/2+50,300),Port.port1,Color.White,"out",false));

        addToEtyMgr();

        API.API.createSection(this);
        sendInitialComponentValsToServer();
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

    private bool isInteractingWithComponent(){
        foreach(Component c in components){
            if(c.isInteracting) {
                //Console.WriteLine(c.GetType());
                return true;
            }
        }
        return false;
    }

    private void sendInitialComponentValsToServer(){
        foreach(Component c in components){
            c.sendValToServer();
        }
    }

    public override void Update()
    {
        if(!isInteractingWithComponent()){

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