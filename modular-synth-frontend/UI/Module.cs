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
    private Grid grid = Grid.GetInstance();

    private int width; //in tiles - used for collision code and marking tiles as occupied

    private bool dragging;
    private bool invalidPos = false;
    private Vector2 originalPosition;
    private Vector2 clickOffset;

    //this is used for api calls and ports to check if they are on the same module
    private static int modules = 0;
    public int ModuleId { get; private set; }

    private List<Component> components = new List<Component>();

    public SCSection scSection;

    public string function;


    public Module(Texture2D sprite) : base(sprite)
    {
        width = 8; //TODO: this is temp
    }

    public Module(Texture2D sprite, Vector2 pos) : base(sprite, pos)
    {
        width = 8;

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

    public override void Update()
    {
        if(!isInteractingWithComponent()){

        if (boundingBox.Contains(input.MousePosition()))
        {
            if (input.LeftMouseClickDown())
            {
                dragging = true;
                grid.DeOccupyTiles(width, GetPosition());
                originalPosition = GetPosition();
                clickOffset = GetPosition() - input.MousePosVector();
            }

            if(input.RightMouseClickDown())
            {
                //EntityManager.entities.Remove(this);
                //God I hope that makes this garbage collect and we don't have a memory leak TODO: Check that lol
                //TODO: once merged update how entity manager actually works such that can alter list
            }
        }
      }

        if (dragging)
        {
            SetPosition(input.MousePosVector() + clickOffset);

            Vector2 TopLeftCorner = grid.GetNearestRightEdgeTileSnap(new Vector2(boundingBox.Left, boundingBox.Top));

            if(grid.AreTilesOccupied(TopLeftCorner, width))
            {
                invalidPos= true;
            }
            else
            {
                invalidPos= false;
            }



            if (invalidPos)
            {
                colour = Color.Red;
            }
            else
            {
                colour = Color.White;
            }

            if (input.LeftMouseClickUp())
            {
                dragging = false;
                if (invalidPos)
                {
                    //TODO: if menu still open then delete module
                    SetPosition(originalPosition);
                    grid.OccupyTiles(width, GetPosition());
                    colour = Color.White;
                    invalidPos = false;
                }
                else
                {
                    //Vector2 TopRightCorner = grid.GetNearestLeftEdgeTileSnap(new Vector2(boundingBox.Right, boundingBox.Top));
                    //if (Math.Abs((position - TopLeftCorner).X) < Math.Abs((new Vector2(boundingBox.Right,position.Y) - TopRightCorner).X)) //TODO: either remove or fix this :(

                    SetPosition(TopLeftCorner);
                    grid.OccupyTiles(width,GetPosition());

                }
            }
        }
        updateComponentPositions();
    }


    public int GetWidth()
    {
        return width;
    }

    //set on spawn
    public void Drag()
    {
        dragging = true;
        originalPosition = GetPosition();
        clickOffset = GetPosition() - input.MousePosVector();
    }
}
