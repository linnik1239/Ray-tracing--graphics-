#version 130 

 
uniform float theMinusX;
uniform float thePlusX;
uniform float theMinusY;
uniform float thePlusY;
 
uniform int allysing;  // In case that we want to make allysing or not.
 
 
uniform vec4 eye;
uniform vec4 ambient;
uniform vec4[20] objects;
uniform vec4[20] reflectiveObjects;
uniform vec4[20] transparantObjects;
uniform vec4[20] objColors;
uniform vec4[10] lightsDirection;
uniform vec4[10] lightsIntensity;
uniform vec4[10] lightPosition;
uniform ivec4 sizes; //{number of objects , number of lights , width, hight}  

in vec3 position1;
in vec2 texCoord0;

#define ANY_YET_OBJECT_TYPE -1
#define REFLECTIVE_OBJECT_TYPE 1
#define REGULAR_OBJECT_TYPE 2

float intersection(vec3 sourcePoint,vec3 V,vec4 object)
{    // Function wich return minimal t for intersection, in case of no intersection
     // with any object -1.0 will be returned.

    vec3 P0 = sourcePoint;
	V = normalize(V);
    if(object.w>0){	 // In case of sphere
	    vec3 O = object.xyz;	
        float a= 1;
	    float b = 2*dot(V,P0-O);
     	float c = pow(length(P0-O),2)-pow(object.w,2);
   	        
    	float theDet = (pow(b,2)-(4*a*c));
        if(theDet>=0){
	         return (-b-sqrt(theDet))/(2*a);
  	    }
        return -1.0;
	}else{  // in case of plain.
      vec3 Q0 = vec3(0,0,-object.w/object.z);
	  vec3 N = object.xyz;
	  if(dot(N,V)==0.0) return -1.0;
	  float t=dot(N,Q0-P0)/dot(N,V);
	  return t;
	
	}
    
}



int lightPositionIndex=0;

vec3 giveminimalTAndObjectIndex(vec3 position,vec3 direction,int notObject,int notObjectType){
// Function which return vec3 which consists from x part which is minimal t for intersection
// y part which is the object minimal index for intersection and z part which give the object type (regular,reflective and transparent).
// In case of no intersection negative numbers will be returned.

	float minimalT = -1.0;
	int objectMinimalTIndex = -1;
	
	int minimalObjectType=ANY_YET_OBJECT_TYPE;

    for(int i=0;i<sizes.x;++i){ // Running on regular objects
	    if((i==notObject&& notObjectType==2)){
		   continue;
		}
	    float localT = intersection(position,direction,objects[i]);		
		
		if(localT>=0 && (objectMinimalTIndex==-1 || localT<minimalT)){
		    minimalT = localT;
			objectMinimalTIndex=i;
			minimalObjectType=2;
		}
	}
	
	
	if(notObjectType==2){
	     return vec3(minimalT,objectMinimalTIndex,minimalObjectType);
	}
	
	for(int i=0;i<sizes.z;++i){ // Running on reflective objects.
	    if((i==notObject&& notObjectType==REFLECTIVE_OBJECT_TYPE)){
		   continue;
		}
		
	    float localT = intersection(position,direction,reflectiveObjects[i]);		
		
		if(localT>=0 && (objectMinimalTIndex==-1 || localT<minimalT)){
		    minimalT = localT;
			objectMinimalTIndex=i;
			minimalObjectType=REFLECTIVE_OBJECT_TYPE;
		}

	}
	
	
	return vec3(minimalT,objectMinimalTIndex,minimalObjectType);



}


vec3 colorCalc( vec3 intersectionPoint,vec2 texCoordNormalized)
{   //Function which calculate colour for each pixel.
	

	vec3 V = vec3(texCoordNormalized,0) - vec3(eye);
	V = normalize(V);
	vec3 P0 = eye.xyz;
	
	
	float minimalT ;
	int objectMinimalTIndex ;
	int minimalObjectType ;
	
	
	float minimallLightTObject0 ;
	int objectMinimalTLightIndex ;
	

	int numRecursion=5;   //Number for recursion iteration in case of reflective object.
	vec3 val;
	
	
	vec3 P;
	
	vec3 currentPoint = P0;
	vec3 currentDirection = V;
	
	int currentObj = -1;
	int notObjectType = ANY_YET_OBJECT_TYPE;
	do{
	
	  val = giveminimalTAndObjectIndex(currentPoint,currentDirection,currentObj,notObjectType);
	  minimalT = val[0];
	  objectMinimalTIndex = int(val[1]);
	  minimalObjectType = int(val[2]);
	  numRecursion--;
	  P = currentPoint+ currentDirection*minimalT;
	  if(minimalObjectType==REFLECTIVE_OBJECT_TYPE){  // Instead of recursion iteration for reflective object. 
	      currentPoint= currentPoint+ currentDirection*minimalT;
		  vec3 currentN = vec3(reflectiveObjects[objectMinimalTIndex].x,
		  reflectiveObjects[objectMinimalTIndex].y,
		  reflectiveObjects[objectMinimalTIndex].z);
		  currentN = normalize(currentN);
		  vec3 refVectorL = V-2*currentN*(dot(V,currentN)); 
		  refVectorL = normalize(refVectorL);
	      currentDirection =refVectorL;
		  currentObj = objectMinimalTIndex;
		  notObjectType=REFLECTIVE_OBJECT_TYPE;
	  
	  }
	  
	}while(numRecursion>=0  && minimalObjectType==REFLECTIVE_OBJECT_TYPE);  
	  
	
	if(objectMinimalTIndex==-1 ){ // In case what ray didn't hit any object.
	   return vec3(0.0,0.0,0.0);
	}	
	vec4 choisenObject ;
	vec4 choisenObjectColor;
	choisenObject = objects[objectMinimalTIndex];
	choisenObjectColor = objColors[objectMinimalTIndex+sizes.z];
	
	//if(notObjectType==REGULAR_OBJECT_TYPE || notObjectType==-1){
    //   choisenObject = objects[objectMinimalTIndex];
	//   choisenObjectColor = objColors[objectMinimalTIndex+sizes.z];
	//}else{
	//      vec4 choisenObject = objects[objectMinimalTIndex];
	//      vec4 choisenObjectColor = objColors[objectMinimalTIndex];	
	//}
	if(choisenObject.w>=0){ // I case of sphere object.

			vec3 inSigma = vec3(0,0,0);
			for(int j=0;j<sizes.y;++j){
				vec3 lightsDirectionImproviesd; 

				vec3 O = choisenObject.xyz;
              				
				if(lightsDirection[j].w==1.0){
				  
				  lightsDirectionImproviesd =-P+lightPosition[lightPositionIndex++].xyz;
				  lightsDirection[j].xyz;
				  if(dot(normalize(-lightsDirectionImproviesd),
				  normalize(lightsDirection[j].xyz)) 
				  < lightPosition[lightPositionIndex].w){
				  
                      continue;				  
				  }
				  
				  
			    }else{
				   lightsDirectionImproviesd =lightsDirection[j].xyz;
			    }

				vec3 L = lightsDirectionImproviesd;
				vec3 N = P - O;
				L = normalize(L);
				N = normalize(N);
				vec3 refVectorL = L-2*N*(dot(L,N)); 
				refVectorL = normalize(refVectorL);				
				float cosAngle = dot(L,N);
				  
				float cosAlpha = dot(-refVectorL ,N);
				if(cosAlpha<0){
				   cosAlpha=0;
				}				  
				vec3 Is = vec3(0.7,0.7,0.7)*pow(cosAlpha,choisenObjectColor.w);

				if(lightsDirection[j].w==0)
		           val = giveminimalTAndObjectIndex(P,-L,objectMinimalTIndex,REGULAR_OBJECT_TYPE);
		        else
			       val = giveminimalTAndObjectIndex(P,L,objectMinimalTIndex,REGULAR_OBJECT_TYPE);			
	            minimallLightTObject0= int(val[0]);
	            objectMinimalTLightIndex = int(val[1]);
				int objectMinimalTLightType = int(val[2]);
				
				
				//if(objectMinimalTLightType==1)  // To change
				//   return vec3(1,0,0);				 
                float distanceFromLightSource = 0.0;  				 
				if(objectMinimalTLightIndex >=0 && (objectMinimalTLightType==REGULAR_OBJECT_TYPE || objectMinimalTLightType==-1) ){
				    if(lightsDirection[j].w==0.0){
			           continue;
			         }
			  
			         float lengthMinRangeObject = length(minimallLightTObject0*L);
			   
			         distanceFromLightSource = length(lightPosition[lightPositionIndex-1].xyz-P);
			   
			         if(lengthMinRangeObject <distanceFromLightSource )
			            continue;
 
				}
				 
				vec3 currentColor = choisenObjectColor.xyz*cosAngle +Is;
				currentColor.x*= lightsIntensity[j].x/(1+distanceFromLightSource);
				currentColor.y*= lightsIntensity[j].y/(1+distanceFromLightSource);
				currentColor.z*= lightsIntensity[j].z/(1+distanceFromLightSource);
				inSigma+=currentColor;
	        } 
			vec3 outSigma = vec3(choisenObjectColor.x*ambient.x,
			choisenObjectColor.y*ambient.y,
			choisenObjectColor.z*ambient.z);
	             			 
	        return (outSigma  + inSigma);
	

	}else{  // Plain object
		vec3 inSigma = vec3(0,0,0);
		for(int j=0;j< sizes.y;++j){		
			vec3 lightsDirectionImproviesd; 
	    	if(lightsDirection[j].w==1.0){				  
				  lightsDirectionImproviesd =-P+lightPosition[lightPositionIndex].xyz;
				  lightsDirection[j].xyz;
				  if(dot(normalize(-lightsDirectionImproviesd),
				  normalize(lightsDirection[j].xyz)) 
				  < lightPosition[lightPositionIndex].w){			  
                         continue;				  
				  }				  
			}else{
				  lightsDirectionImproviesd =lightsDirection[j].xyz;  // Here are the problems.  
			}		    
			vec3 L = lightsDirectionImproviesd;
			vec3 N =  choisenObject.xyz;
			L = normalize(L);
			N = normalize(N);

			vec3 refVectorL = L-2*N*(dot(L,N)); 
			refVectorL = normalize(refVectorL);	
			float cosAngle = dot(L,N);				  
			float cosAlpha = dot(-refVectorL ,N);
			if(cosAlpha<0){
			   cosAlpha=0;
			}				  
			vec3 Is = vec3(0.7,0.7,0.7)*pow(cosAlpha,choisenObjectColor.w); //***
			if(lightsDirection[j].w==0)
		         val = giveminimalTAndObjectIndex(P,-lightsDirectionImproviesd,objectMinimalTIndex,REGULAR_OBJECT_TYPE);
		    else
			    val = giveminimalTAndObjectIndex(P,lightsDirectionImproviesd,objectMinimalTIndex,REGULAR_OBJECT_TYPE);
	        minimallLightTObject0= int(val[0]);

	        objectMinimalTLightIndex = int(val[1]);
			int objectMinimalTLightType = int(val[2]);
			

			
			float distanceFromLightSource=0.0;
			if(objectMinimalTLightIndex>=0 && (objectMinimalTLightType==REGULAR_OBJECT_TYPE || objectMinimalTLightType==-1) ){
			   if(lightsDirection[j].w==0){
			      continue;
			   }
			   float lengthMinRangeObject = length(minimallLightTObject0*L);
			   
			   distanceFromLightSource = length(lightPosition[lightPositionIndex-1].xyz-P);
			   
			   if(lengthMinRangeObject <distanceFromLightSource )
			        continue;	   
			}
			vec3 currentColor = choisenObject.xyz*cosAngle +Is;
			currentColor.x*= lightsIntensity[j].x/(1.0+distanceFromLightSource);
			currentColor.y*= lightsIntensity[j].y/(1.0+distanceFromLightSource);
			currentColor.z*= lightsIntensity[j].z/(1.0+distanceFromLightSource);
			inSigma+=currentColor;
		}
        vec3 theColor= vec3(0.2,0.2,0.3);
		float duplicateOut=1.0;
		if(P.x * P.y <0){
		
          if((mod(int(1.5*P.x),2)) == (mod(int(1.5*P.y),2))){
		  		      theColor= vec3(0.5,0.6,0.8);
					  duplicateOut=0.5;
	      }
		}
		else if((mod(int(1.5*P.x),2)) != (mod(int(1.5*P.y),2))) 
		{
		          theColor= vec3(0.5,0.6,0.8);
				  duplicateOut=0.5;
		}
		 vec3 outSigma = vec3(choisenObjectColor.x*ambient.x,
			choisenObjectColor.y*ambient.y,
			choisenObjectColor.z*ambient.z);
	     if(inSigma.x ==0  && inSigma.y==0 && inSigma.z ==0)
              theColor = vec3(0,0,0);	 	 
		 return (theColor+duplicateOut*outSigma + inSigma);
	}
	  
}

void main()
{    
    float pixdelStep = 1/800.0;
    vec2 texCoordNormalized = vec2(theMinusX + texCoord0.x * (thePlusX -theMinusX),   theMinusY + texCoord0.y * (thePlusY-theMinusY) );
    
	if(allysing!=0){   // As default it doesn't make allysion but it can be changed on CPU on game.cpp
	   vec3 sumVolors= vec3(0,0,0);
	   for(int i=-1;i<=1;++i){

	       for(int j=-1;j<=1;++j){
		      
			         float curY = texCoordNormalized.y+i*pixdelStep;
		     		  float curX = texCoordNormalized.x+j*pixdelStep;
		   		     if(curX <-1.0||curX>1.0 || curY <-1.0||curY>1.0)
		                 continue;
		             sumVolors+= colorCalc(eye.xyz,vec2(curX,curY));
		     
	      	}
	
	   }
	
	   gl_FragColor = vec4(sumVolors/9.0,1);              
    } else{
	
        gl_FragColor = vec4(colorCalc(eye.xyz,texCoordNormalized),1);  
    }	
}
 