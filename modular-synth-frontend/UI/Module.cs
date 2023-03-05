using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using modular_synth_frontend.Core;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using SynthAPI;
using modular_synth_frontend.API;
using SectionDefTest;
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

    public Module(Texture2D sprite, Vector2 pos, string TEMPmoduleType="",int TEMP=0) : base(sprite, pos)
    {
        width = 8;

        this.ModuleId = modules++;

        switch (TEMPmoduleType){
            case "":
                function = "sin-ar";

                
                
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
    public Module(Vector2 pos, string uiDefFile, string secDefFile) : base(LoadSprite(uiDefFile),pos)
    {
        this.ModuleId = modules++;
        SectionDefTest.Program.combineSecUIDef(uiDefFile, secDefFile, "uiSecDefFile.json"); //combines UI and Sec Def
        var path = Path.GetFullPath("..\\..\\..\\..\\modular-synth-frontend\\SectionDef\\");
        //this is in SectionDefFile but I can't seem to import it for some reason
        string jsonCombinedFile = File.ReadAllText(path + "uiSecDefFile.json");
        Dictionary<string, Dictionary<string, string>> UISecDefDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonCombinedFile);

        foreach (KeyValuePair<string, Dictionary<string, string>> component in UISecDefDict)
        {
            Dictionary<string, string> newComp = component.Value;
            string ComponentType = newComp.TryGetValue("type", out string type) ? type : "error";

            if (component.Key == "moduleArgs") {//not a component. Placed here temporarily. This to get the synthdef function name
                this.function = newComp.TryGetValue("function", out string func) ? func : null;
                if (this.function == null) {
                    Console.WriteLine("Error parsing function type");
                }

                width = newComp.TryGetValue("width", out string val) ? int.Parse(val) : 8;

            }
            else{
                string xPosString = newComp.TryGetValue("xPos", out string compXPos) ? compXPos : "error";//get xPosition
                string yPosString = newComp.TryGetValue("yPos", out string compYPos) ? compYPos : "error";//get yPosition  
                Vector2 moduleLocalPos = new Vector2(parsePositionX(xPosString), parsePositionY(yPosString));
                string colString = newComp.TryGetValue("col", out string compCol) ? compCol : "255255255"; 
                //Color col = (Color)new System.Drawing.ColorConverter().ConvertFromString(colString); //convert colour to colour type
                var col = new Color(int.Parse(colString.Substring(0,3)),int.Parse(colString.Substring(3,3)),int.Parse(colString.Substring(6,3)));
                if (ComponentType == "slider")
                {                                                   
                    string parameterIDString = newComp.TryGetValue("parameterID", out string compParameterID) ? compParameterID : "error";//get ParameterID
                    string trackScaleString = newComp.TryGetValue("trackScale", out string compTrackScale) ? compTrackScale : "1";      //get trackScale, if not default to 1
                    string sliderScaleString = newComp.TryGetValue("sliderScale", out string compSliderScale) ? compSliderScale : "1";   //get trackScale, if not default to 1
                    string isVerticalString = newComp.TryGetValue("vertical", out string compIsVertical) ? compIsVertical : "false";  //get trackScale, if not default to false
                    string trackSpriteString = newComp.TryGetValue("trackSprite", out string compTrackSprite) ? compTrackSprite : "error";
                    string sliderSpriteString = newComp.TryGetValue("sliderSprite", out string compSliderSprite) ? compSliderSprite : "error";

                    if (xPosString == "error" || yPosString == "error" || colString == "error" || parameterIDString == "error" || sliderSpriteString == "error" || trackSpriteString == "error")
                    {
                        Console.WriteLine("error making slider");
                        //TODO: Handle error
                    }
                    
                    double trackScale = double.Parse(trackScaleString);
                    double sliderScale = double.Parse(sliderScaleString);
                    bool isVertical = bool.Parse(isVerticalString);
                    Texture2D trackSprite = ModularSynth.content.Load<Texture2D>(trackSpriteString);
                    Texture2D sliderSprite = ModularSynth.content.Load<Texture2D>(sliderSpriteString);

                    components.Add(new Slider(pos, ModuleId, moduleLocalPos, trackSprite, sliderSprite, col, parameterIDString, trackScale, sliderScale, isVertical));
                }
                else if (ComponentType == "dial")
                {                                                                              
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
                       //convert colour to colour type
                    double staticPartScale = double.Parse(staticPartScaleString);
                    double dialScale = double.Parse(dialScaleString);
                    Texture2D staticPartSprite = ModularSynth.content.Load<Texture2D>(staticPartSpriteString);
                    Texture2D dialSprite = ModularSynth.content.Load<Texture2D>(dialSpriteString);
                    components.Add(new Dial(pos, ModuleId, moduleLocalPos, staticPartSprite, dialSprite, col, parameterIDString, staticPartScale, dialScale));
                }
                else if (ComponentType == "port")
                {                                           
                    string parameterIDString = newComp.TryGetValue("parameterID", out string compParameterID) ? compParameterID : "error";                //get ParameterID
                    string isInputString = newComp.TryGetValue("isInput", out string compIsInput) ? compIsInput : "error";                                //get isInput
                    string scaleString = newComp.TryGetValue("scale", out string compScale) ? compScale : "1";                                            //get Scale
                    string spriteString = newComp.TryGetValue("sprite", out string compSprite) ? compSprite : "error";
                    double scale = double.Parse(scaleString);
                    bool isInput = bool.Parse(isInputString);
                    if(!isInput){
                        parameterIDString="out";
                    }

                    if (xPosString == "error" || yPosString == "error" || colString == "error" || parameterIDString == "error" || spriteString == "error")
                    {
                        Console.WriteLine("error making port");
                        //TODO: Handle error
                    }
                    
                    Texture2D spritePort = ModularSynth.content.Load<Texture2D>(spriteString);
                    

                    

                    components.Add(new Port(pos, ModuleId, moduleLocalPos, spritePort, col, parameterIDString, isInput, scale));
                }
                else if (ComponentType == "button")
                {
                    string parameterIDString = newComp.TryGetValue("parameterID", out string compParameterID) ? compParameterID : "error";
                    string scaleString = newComp.TryGetValue("scale", out string compScale) ? compScale : "1";
                    string spriteString = newComp.TryGetValue("sprite", out string compSprite) ? compSprite : "error";
                    double scale = double.Parse(scaleString);

                    if (xPosString == "error" || yPosString == "error" || colString == "error" || parameterIDString == "error" || spriteString == "error")
                    {
                        Console.WriteLine("error making button");
                        //TODO: Handle error
                    }

                    Texture2D spriteButton = ModularSynth.content.Load<Texture2D>(spriteString);


                    components.Add(new ButtonComponent(pos, ModuleId, moduleLocalPos, spriteButton, col, parameterIDString, scale));
                }            
                else
                {
                    Console.WriteLine("Error, do not know component type");
                }
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
    private int parsePositionX(string secDefString){
        Console.WriteLine("secDefString: " + secDefString);
        int offset=0;
        try{
            return int.Parse(secDefString);
        } catch{  
            switch(secDefString[0]){
                case 'm': offset = this.sprite.Width/2; break;
                case 'r': offset = this.sprite.Width; break;
                case 'l': offset = 0; break;
            }
            if(secDefString.Substring(1)!=""){
                return offset+int.Parse(secDefString.Substring(1));
            } else {
                return offset;
            } 
        }
    }
    private int parsePositionY(string secDefString){
        //Console.WriteLine("secDefString: " + secDefString);
        int offset=0;
        try{
            return int.Parse(secDefString);
        } catch{  
            switch(secDefString[0]){
                case 'm': offset = this.sprite.Height/2; break;
                case 'b': offset = this.sprite.Height; break;
                case 't': offset = 0; break;
            }
            if(secDefString.Substring(1)!=""){
                return offset+int.Parse(secDefString.Substring(1));
            } else {
                return offset;
            } 
        }
    }

    static private Texture2D LoadSprite(string uidefFilePath)
    {
        var path = Path.GetFullPath("..\\..\\..\\..\\modular-synth-frontend\\SectionDef\\");
        string jsonCombinedFile = File.ReadAllText(path + uidefFilePath + ".json");
        Dictionary<string, Dictionary<string, string>> UIDefDict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonCombinedFile);

        string img;

        if (UIDefDict.ContainsKey("moduleArgs"))
        {
            if (UIDefDict["moduleArgs"].ContainsKey("backgroundImage"))
            {
                img = UIDefDict["moduleArgs"]["backgroundImage"];
                return ModularSynth.content.Load<Texture2D>(img);
            }
        }

        return ModularSynth.content.Load<Texture2D>("module");
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
