#pragma once
#include "scene.h"

class Game : public Scene
{
public:
	
	Game();
	int theColor=2;
	int p = 2;




	float xGap = 1.0;
	float yGap = 1.0;




	glm::vec2 center = glm::vec2(0, 0);


	glm::vec2 realCoordinates = glm::vec2(0, 0);



	void setCenter(float x,float y) {
		center = glm::vec2(x, y);
		realCoordinates += (center*xGap);
		printcoordinates(xGap);

	}
	void printcoordinates(float pixelWidth);

	void printPixelWidth(float pixelWidth);




	glm::vec4 getCoordSize() {
		return glm::vec4(realCoordinates.x- xGap, realCoordinates.x + xGap, realCoordinates.y - yGap, realCoordinates.y + yGap);

		//return glm::vec4(theMinusX, thePlusX , theMinusY , thePlusY );
	}

	void squeezeCoordSize(float yoffset) {
		if (yoffset > 0) {
			


			xGap /= 2;
			yGap /= 2;

			printPixelWidth(xGap);



		}
		else
		{



			xGap *= 2;
			yGap *= 2;

			printPixelWidth(xGap);

		}
	}



	void increaseP() {
		p += 1;
	}
	void decreaseP() {
		p > 2 ? --p : (1);
	}

	int getP() {
		return p;
	}


	int getColor() {
		return theColor;
	}
	void setColor(int theColor) {
		this->theColor = theColor;
	}
	Game(float angle,float relationWH,float near, float far);
	void Init();
	void Update(const glm::mat4 &MVP,const glm::mat4 &Model,const int  shaderIndx);
	void ControlPointUpdate();
	void WhenRotate();
	void WhenTranslate();
	void Motion();
	~Game(void);
};

