using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using SynthAPI;
using modular_synth_frontend.API;
namespace modular_synth_frontend.UI;
using Newtonsoft.Json;
using System.IO;
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

    public Module(Texture2D sprite, Vector2 pos, string TEMPmoduleType="") : base(sprite, pos)
    {
        width = 8;

        this.ModuleId = modules++;

        switch (TEMPmoduleType){
            case "":
                function = "sin-ar";

                components.Add(new Slider(pos, ModuleId, new Vector2(this.sprite.Width/2,250),Slider.rail1,Slider.slider2,Color.White,"add",0.7,0.7));
                components.Add(new Slider(pos, ModuleId, new Vector2(this.sprite.Width/2-50,this.sprite.Height/2-50),Slider.rail1,Slider.slider2,Color.White,"freq",0.7,0.7,true));
                components.Add(new Dial(pos, ModuleId, new Vector2(this.sprite.Width/2+50,this.sprite.Height/2-50),Dial.indicator1,Dial.dial1,Color.White,"mul",0.7,0.7));
                
                components.Add(new Port(pos, ModuleId, new Vector2(this.sprite.Width/2+50,250),Port.port1,Color.White,"freq",true));
                components.Add(new Port(pos, ModuleId, new Vector2(this.sprite.Width/2-50,300),Port.port1,Color.White,"add",true));
                components.Add(new Port(pos, ModuleId, new Vector2(this.sprite.Width/2+50,300),Port.port1,Color.White,"out",false));
                break;

            case "sliders":
                function = "sin-ar";

                components.Add(new Slider(pos, ModuleId, new Vector2(this.sprite.Width/2,150),Slider.rail1,Slider.slider2,Color.White,"add",0.7,0.7));
                components.Add(new Slider(pos, ModuleId, new Vector2(this.sprite.Width/2,200),Slider.rail1,Slider.slider2,Color.White,"add",0.7,0.7));
                components.Add(new Slider(pos, ModuleId, new Vector2(this.sprite.Width/2,250),Slider.rail1,Slider.slider2,Color.White,"add",0.7,0.7));
                break;
        }

        addToEtyMgr();

        API.API.createSection(this);
        sendInitialComponentValsToServer();
    }
    public Module(Texture2D sprite, Vector2 pos, string uiSecDefFile, GraphicsDevice graphicsDevice) : base(sprite, pos)
    {
        this.ModuleId = modules++;
        //this is in SectionDefFile but I can't seem to import it for some reason
        string jsonCombinedFile = File.ReadAllText(uiSecDefFile);
        Dictionary<string, Dictionary<string, string>> UISecDefDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonCombinedFile);
        foreach (KeyValuePair<string, Dictionary<string, string>> component in UISecDefDict)
        {
            Dictionary<string, string> newComp = component.Value;
            if (newComp.TryGetValue("componentType", out string ComponentType))
            {
                if (ComponentType == "slider")
                {
                    string xPosString = newComp.TryGetValue("xPos", out string compXPos) ? compXPos : "error";                            //get xPosition
                    string yPosString = newComp.TryGetValue("yPos", out string compYPos) ? compYPos : "error";                            //get yPosition            
                    string colString = newComp.TryGetValue("col", out string compCol) ? compCol : "error";                                //get Colour
                    string parameterIDString = newComp.TryGetValue("parameterID", out string compParameterID) ? compParameterID : "error";//get ParameterID
                    string trackScaleString = newComp.TryGetValue("trackScale", out string compTrackScale) ? compTrackScale : "1";      //get trackScale, if not default to 1
                    string sliderScaleString = newComp.TryGetValue("sliderScale", out string compSliderScale) ? compSliderScale : "1";   //get trackScale, if not default to 1
                    string isVerticalString = newComp.TryGetValue("isVertical", out string compIsVertical) ? compIsVertical : "false";  //get trackScale, if not default to false
                    string trackSpriteString = newComp.TryGetValue("trackSprite", out string compTrackSprite) ? compTrackSprite : "error";
                    string sliderSpriteString = newComp.TryGetValue("sliderSprite", out string compSliderSprite) ? compSliderSprite : "error";

                    if (xPosString == "error" || yPosString == "error" || colString == "error" || parameterIDString == "error" || sliderSpriteString == "error" || trackSpriteString == "error")
                    {
                        Console.WriteLine("error making slider");
                        //TODO: Handle error
                    }

                    Vector2 moduleLocalPos = new Vector2(int.Parse(xPosString), int.Parse(yPosString));    //combine, xypos into vector
                    Color col = (Color)new System.Drawing.ColorConverter().ConvertFromString(colString);   //convert colour to colour type
                    double trackScale = double.Parse(trackScaleString);
                    double sliderScale = double.Parse(sliderScaleString);
                    bool isVertical = bool.Parse(isVerticalString);
                    Texture2D trackSprite = Texture2D.FromFile(graphicsDevice, trackSpriteString);
                    Texture2D sliderSprite = Texture2D.FromFile(graphicsDevice, sliderSpriteString);

                    components.Add(new Slider(pos, ModuleId, moduleLocalPos, trackSprite, sliderSprite, col, parameterIDString, trackScale, sliderScale, isVertical));
                }
                else if (ComponentType == "dial")
                {
                    string xPosString = newComp.TryGetValue("xPos", out string compXPos) ? compXPos : "error";                                            //get xPosition
                    string yPosString = newComp.TryGetValue("yPos", out string compYPos) ? compYPos : "error";                                            //get yPosition
                    string colString = newComp.TryGetValue("col", out string compCol) ? compCol : "error";                                                //get Colour
                    string parameterIDString = newComp.TryGetValue("parameterID", out string compParameterID) ? compParameterID : "error";                //get ParameterID
                    string staticPartScaleString = newComp.TryGetValue("staticPartScale", out string compstaticPartScale) ? compstaticPartScale : "1";//get staticPartScale
                    string dialScaleString = newComp.TryGetValue("dialScale", out string compdialScale) ? compdialScale : "1";                        //get dialScale
                    string staticPartSpriteString = newComp.TryGetValue("staticPartSprite", out string compStaticPartSprite) ? compStaticPartSprite : "error";
                    string dialSpriteString = newComp.TryGetValue("dialSprite", out string compDialSprite) ? compDialSprite : "error";

                    if (xPosString == "error" || yPosString == "error" || colString == "error" || parameterIDString == "error" || dialSpriteString == "error" || staticPartSpriteString == "error")
                    {
                        Console.WriteLine("error making dial");
                        //TODO: Handle error
                    }

                    Vector2 moduleLocalPos = new Vector2(int.Parse(xPosString), int.Parse(yPosString));    //combine, xypos into vector
                    Color col = (Color)new System.Drawing.ColorConverter().ConvertFromString(colString);   //convert colour to colour type
                    double staticPartScale = double.Parse(staticPartScaleString);
                    double dialScale = double.Parse(dialScaleString);
                    Texture2D staticPartSprite = Texture2D.FromFile(graphicsDevice, staticPartSpriteString);
                    Texture2D dialSprite = Texture2D.FromFile(graphicsDevice, dialSpriteString);

                    components.Add(new Dial(pos, ModuleId, moduleLocalPos, staticPartSprite, dialSprite, col, parameterIDString, staticPartScale, dialScale));
                }
                else if (ComponentType == "port")
                {
                    string xPosString = newComp.TryGetValue("xPos", out string compXPos) ? compXPos : "error";                                            //get xPosition
                    string yPosString = newComp.TryGetValue("yPos", out string compYPos) ? compYPos : "error";                                            //get yPosition
                    string colString = newComp.TryGetValue("col", out string compCol) ? compCol : "error";                                                //get Colour
                    string parameterIDString = newComp.TryGetValue("parameterID", out string compParameterID) ? compParameterID : "error";                //get ParameterID
                    string isInputString = newComp.TryGetValue("isInput", out string compIsInput) ? compIsInput : "error";                                //get isInput
                    string scaleString = newComp.TryGetValue("scale", out string compScale) ? compScale : "1";                                            //get Scale
                    string spriteString = newComp.TryGetValue("sprite", out string compSprite) ? compSprite : "error";

                    if (xPosString == "error" || yPosString == "error" || colString == "error" || parameterIDString == "error" || spriteString == "error")
                    {
                        Console.WriteLine("error making port");
                        //TODO: Handle error
                    }

                    Vector2 moduleLocalPos = new Vector2(int.Parse(xPosString), int.Parse(yPosString));    //combine, xypos into vector
                    Color col = (Color)new System.Drawing.ColorConverter().ConvertFromString(colString);   //convert colour to colour type
                    double scale = double.Parse(scaleString);
                    Texture2D spritePort = Texture2D.FromFile(graphicsDevice, spriteString);
                    bool isInput = bool.Parse(isInputString);

                    components.Add(new Port(pos, ModuleId, moduleLocalPos, spritePort, col, parameterIDString, isInput, scale));
                }
                else if (ComponentType == "button")
                {
                    //TODO: add button creation
                }
            }
            else
            {
                //Error, do not know component type
            }
        }


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

    private void sendInitialComponentValsToServer()
    {
        foreach (Component c in components)
        {
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
