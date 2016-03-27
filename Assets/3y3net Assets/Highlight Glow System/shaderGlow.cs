using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class shaderGlow : MonoBehaviour {

	public bool useNormal=false;
	public enum allowedModes {onMouseEnter, alwaysOn, userCallFunctions};
	public enum labelModes {onMouseEnter, whenGlowIsOn};
	public allowedModes glowMode;
	public bool flashing=false; //Object will flash glow
	[Range (0.5f,4.0f)]
	public float flashSpeed=1f; //Flash speed
	public bool noOcclusion=false; //Show glow when object is occluded
	public bool scaleGlow = true; //If object is very sharp (nor squared) this help to get better glow
	[Range (0.15f,3.0f)]
	public float glowIntensity=1f; //Glow intensity on screen of the object
	[Range (0.5f,2.0f)]
	public float glowOpacity=1f; //Glow opacity on screen of the object
	public Color glowColor = Color.red; //Glow color of the object

	public String labelToDisplay="LABEL";	//Text to show
	public labelModes labelMode;
	public Color labelColor=Color.white;	//Text color
	public bool outlined=true;				//Set true for display outlined text
	public Color outlineColor=Color.black;	//Outline text color
	public Font textFont = null;			//Text font
	public int fontSize=12;					//Text size

	private float clipGlow=0.04f; //Min real allowed glow
	private float maxGlow=0.25f; //Max real allowed glow
	private float lastGlow=0f; //Last glow based on distance. Incrase or decrease glow based on distance to crete the constant glow effect
	private float lastOpacity=0f; //Last glow based on distance. Incrase or decrease glow based on distance to crete the constant glow effect
	private float clipDistance=1.0f; //Min distance. Closer the glow does not decrease
	private float maxDistance=10f; //Max distance. Further the glow does not increase
	private List<Shader> originalObjetsShader;
	private Shader highightShaderVisible = null;	//Shader used to glow object
	private Shader highightShaderHidden = null;	//Shader used to glow object
	private bool highlighted=false;
	private float flashPosition=1f;
	private bool flashDirectionUp = true;
	private GUIStyle style;
	private bool showLabel=false;
	private bool isMobile=false;

	void Awake() {
		//Grab the glow shader
		if (useNormal) {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleNormal");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenNormal");
		} 
		else {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisible");
			highightShaderHidden = Shader.Find ("3y3net/GlowHidden");
		}
		#if UNITY_IPHONE
		isMobile=true;
		if (useNormal) {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleNormalMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenNormalMobile");
		} 
		else {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenMobile");
		}
		#endif
		
		#if UNITY_ANDROID
		isMobile=true;
		if (useNormal) {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleNormalMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenNormalMobile");
		} 
		else {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenMobile");
		}
		#endif
		
		#if UNITY_BLACKBERRY
		isMobile=true;
		if (useNormal) {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleNormalMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenNormalMobile");
		} 
		else {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenMobile");
		}
		#endif
		
		#if UNITY_WP8
		isMobile=true;
		if (useNormal) {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleNormalMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenNormalMobile");
		} 
		else {
			highightShaderVisible = Shader.Find ("3y3net/GlowVisibleMobile");
			highightShaderHidden = Shader.Find ("3y3net/GlowHiddenMobile");
		}
		#endif
	}

	// Use this for initialization
	void Start () {

		//Stores all shaders of the object and children objects
		Component[] renderers;
		renderers = GetComponentsInChildren<Renderer>();
		originalObjetsShader = new List<Shader> ();
		
		foreach (Renderer singleRenderer in renderers)
		foreach (Material singleMaterial in singleRenderer.materials) {
			originalObjetsShader.Add(singleMaterial.shader);
		}

		style = new GUIStyle();
		style.normal.textColor = labelColor;  
		style.alignment = TextAnchor.UpperCenter ;
		style.wordWrap = true;
		style.fontSize = fontSize;
		if(textFont!=null)
			style.font=textFont;
		else
			style.font= (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
	}

	void OnMouseEnter() {
		if (!labelToDisplay.Equals ("") && labelMode==labelModes.onMouseEnter)
			showLabel=true;
		if(highlighted)
			return;
		if(glowMode == allowedModes.onMouseEnter)
			lightOn ();
	}
	
	void OnMouseExit() {
		if (labelMode==labelModes.onMouseEnter)
			showLabel=false;
		if (!highlighted)
			return;
		if(glowMode == allowedModes.onMouseEnter)
			lightOff ();
	}

	//draw text of a specified color, with a specified outline color
	void DrawOutline(Rect position, String text, GUIStyle theStyle, Color outColor, Color inColor){
		var backupStyle = theStyle;
		theStyle.normal.textColor = outColor;
		position.x--;
		GUI.Label(position, text, style);
		position.x +=2;
		GUI.Label(position, text, style);
		position.x--;
		position.y--;
		GUI.Label(position, text, style);
		position.y +=2;
		GUI.Label(position, text, style);
		position.y--;
		theStyle.normal.textColor = inColor;
		GUI.Label(position, text, style);
		theStyle = backupStyle;
	}
	
	void OnGUI () {
		if (!showLabel)
			return;
		float x, y;
		if(labelMode==labelModes.onMouseEnter) {
			x = Event.current.mousePosition.x-149;
			y = Event.current.mousePosition.y-20;
		}
		else {
			Vector3 pos=Camera.main.WorldToScreenPoint(GetComponent<Renderer>().bounds.center);
			x=pos.x-150;
			y=Screen.height-pos.y;
		}
		if (outlined)
			DrawOutline (new Rect(x,y,300,60), labelToDisplay, style, outlineColor, labelColor);
		else
			GUI.Label ( new Rect(x,y,300,60), labelToDisplay, style);
	}

	Color scaleFactor (Bounds meshfilter) {
		//Vector3 size=meshfilter.mesh.bounds.size;
		//Vector3 scale=meshfilter.transform.localScale;
		//Vector3 sizeScale = new Vector3(size.x*scale.x, size.y*scale.y, size.y*scale.z);
		Vector3 sizeScale = meshfilter.size;
		float maxSize = (sizeScale.x>sizeScale.y)?sizeScale.x:sizeScale.y;
		maxSize = (maxSize>sizeScale.z)?maxSize:sizeScale.z;
		sizeScale.x=maxSize/sizeScale.x; sizeScale.y=maxSize/sizeScale.y; sizeScale.z=maxSize/sizeScale.z; 
		maxSize = (sizeScale.x>sizeScale.y)?sizeScale.x:sizeScale.y;
		maxSize = (maxSize>sizeScale.z)?maxSize:sizeScale.z;
		sizeScale/=maxSize; 
		return  new Color(sizeScale.x, sizeScale.y, sizeScale.z, 1f);

	}

	public void lightOn() {
		if (!labelToDisplay.Equals ("") && labelMode==labelModes.whenGlowIsOn)
			showLabel=true;
		Renderer[] renderers;
		renderers = GetComponentsInChildren<Renderer>();
		foreach (Renderer singleRenderer in renderers) {
			foreach (Material singleMaterial in singleRenderer.materials) {
				if(noOcclusion)
					singleMaterial.shader=highightShaderVisible;
				else
					singleMaterial.shader=highightShaderHidden;
				singleMaterial.SetColor ("_GlowColor", glowColor);
				if(scaleGlow && !useNormal)
					singleMaterial.SetColor ("_ScaleTrick", scaleFactor(singleRenderer.bounds));
				else
					singleMaterial.SetColor ("_ScaleTrick", new Color(1f,1f,1f,1f));
			}
		}
		highlighted = true;
	}

	public void lightOff() {
		if (labelMode==labelModes.whenGlowIsOn)
			showLabel=false;
		int i=0;
		Component[] renderers;
		renderers = GetComponentsInChildren<Renderer>();
		
		foreach (Renderer singleRenderer in renderers)
		foreach (Material singleMaterial in singleRenderer.materials) {
			singleMaterial.shader=originalObjetsShader[i++];
		}
		highlighted = false;
	}

	// Update is called once per frame
	void Update () {
		if(glowMode == allowedModes.alwaysOn && !highlighted)
			lightOn();

		if(highlighted) {
			float distance = Vector3.Distance (gameObject.transform.position, Camera.main.transform.position);
			distance = Mathf.Clamp (distance, clipDistance, maxDistance);
			distance -= clipDistance;
			float glow = clipGlow+(distance * (maxGlow - clipGlow) / ((maxDistance) - (clipDistance)));
			
			if(flashing) {
				if(flashDirectionUp) {
					flashPosition+=flashSpeed*Time.deltaTime;
					if(flashPosition>2f) {
						flashDirectionUp=false;
						flashPosition=2.0f;
					}
				}
				else {
					flashPosition-=flashSpeed*Time.deltaTime;
					if(flashPosition<0.5f) {
						flashDirectionUp=true;
						flashPosition=0.5f;
					}
				}
				glow*=flashPosition;
			}
			else
				glow *=glowIntensity;
			
			if(useNormal && !isMobile) {
				glow=glowIntensity/(8f* Mathf.Sqrt(distance));
			}
			//Debug.Log("Glow "+glow+" Distance "+distance);

			if(glow!=lastGlow) {
				Component[] renderers;
				renderers = GetComponentsInChildren<Renderer>();
				foreach (Renderer singleRenderer in renderers)
				foreach (Material singleMaterial in singleRenderer.materials) {
					singleMaterial.SetFloat ("_Outline", glow);
				}
				lastGlow=glow;
			}
			
			if(glowOpacity!=lastOpacity) {
				Component[] renderers;
				renderers = GetComponentsInChildren<Renderer>();
				foreach (Renderer singleRenderer in renderers)
				foreach (Material singleMaterial in singleRenderer.materials) {
					singleMaterial.SetFloat ("_Opacity", glowOpacity);
				}
				lastOpacity=glowOpacity;
			}
		}
	}
}
