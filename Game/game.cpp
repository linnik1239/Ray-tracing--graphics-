#include "game.h"
#include <iostream>
#include <stdio.h>
#include <stdlib.h>

#include <fstream>
#include <iterator>
#include <vector>
#include <glm/gtc/matrix_transform.hpp>

using namespace std;
char filechar[30];

static void printMat(const glm::mat4 mat)
{
	std::cout << " matrix:" << std::endl;
	for (int i = 0; i < 4; i++)
	{
		for (int j = 0; j < 4; j++)
			std::cout << mat[j][i] << " ";
		std::cout << std::endl;
	}
}

Game::Game() : Scene()
{
}

Game::Game(float angle, float relationWH, float near, float far) : Scene(angle, relationWH, near, far)
{
}

void Game::Init()
{

	cout << "Choose text file" << endl;
	cout << "1. scene.txt" << endl;
	cout << "2. scene1.txt" << endl;
	cout << "3. scene3.txt" << endl;
	cout << "4. scene4.txt" << endl;
	cout << "5. scene5.txt" << endl;

	int choise;
	cin >> choise;

	switch (choise) {
	case 1: strcpy(filechar, "scene.txt"); break;
	case 2: strcpy(filechar, "scene1.txt"); break;
	case 3: strcpy(filechar, "scene3.txt"); break;
	case 4: strcpy(filechar, "scene4.txt"); break;
	case 5: strcpy(filechar, "scene5.txt"); break;
	default: strcpy(filechar, "scene.txt"); break;
	}

	/*FILE* fp;
	char buffer[255];

	char neededBuffer[255];

	fp = fopen("scene.txt", "r");

	while (fgets(buffer, 255, (FILE*)fp)) {
	printf("%s", buffer);
	char letter;
	float x, y, z, w;
	sscanf(buffer,"%c %f %f %f %f",&letter,&x,&y,&z,&w);
	if (buffer[0] == 'o') {
	strcpy(neededBuffer,buffer);
	break;
	}
	}

	fclose(fp);*/


	unsigned int texIDs[1] = { 0 };
	AddShader("../res/shaders/pickingShader");

	bool for2D = true;
	if (for2D) {
		texIDs[0] = 2;
		AddShader("../res/shaders/basicShader3");
	}
	else {
		AddShader("../res/shaders/basicShader2");
	}

	//AddTexture("../res/textures/box0.bmp",true);
	AddTexture("../res/textures/pal.png", for2D);//1D
	AddTexture("../res/textures/box0.bmp", true);
	AddTexture("../res/textures/blankShader.png", true);


	AddMaterial(texIDs, 1);
	AddShape(Plane, -1, TRIANGLES);

	pickedShape = 0;

	SetShapeMaterial(0, 0);
	MoveCamera(0, zTranslate, 10);
	pickedShape = -1;

	//ReadPixel(); //uncomment when you are reading from the z-buffer
}

void Game::printcoordinates(float pixelWidth) {
	std::cout << "Center x = " << center.x << " y=" << center.y << std::endl;
	std::cout << "Real coordinates: x = " << realCoordinates.x << " y=" << realCoordinates.y << std::endl;

	std::cout << "Pixel with =  " << pixelWidth << std::endl;


}


void Game::printPixelWidth(float pixelWidth) {
	std::cout << "Pixel with =  " << pixelWidth << std::endl;

}


void Game::Update(const glm::mat4 &MVP, const glm::mat4 &Model, const int  shaderIndx)
{





	Shader *s = shaders[shaderIndx];
	int r = ((pickedShape + 1) & 0x000000FF) >> 0;
	int g = ((pickedShape + 1) & 0x0000FF00) >> 8;
	int b = ((pickedShape + 1) & 0x00FF0000) >> 16;






	//int squareNum = pow(2, colorNum)-1;
	//float additionToColor = (double)this->getColor() / 10 ;




	textures[materials[shapes[pickedShape]->GetMaterial()]->GetTexture(0)]->Bind(0);
	s->Bind();
	s->SetUniform4f("texCoords", 11.0, 8.0, 177.0, 1.0);
	s->SetUniformMat4f("MVP", MVP);
	s->SetUniformMat4f("Normal", Model);




	glm::vec4 coordData = getCoordSize();

	s->SetUniform1f("theMinusX", coordData.x);
	s->SetUniform1f("thePlusX", coordData.y);
	s->SetUniform1f("theMinusY", coordData.z);
	s->SetUniform1f("thePlusY", coordData.w);

	s->SetUniform1i("allysing",0);



	FILE* fp;
	char buffer[255];

	char neededBuffer[255];


	fp = fopen(filechar, "r");

	glm::ivec4 sizes = glm::ivec4(0, 0, 0, 0);

	std::vector<glm::vec4> objectList;
	std::vector<glm::vec4> reflectiveObjectList;
	std::vector<glm::vec4> transparantObjectList;
	std::vector<glm::vec4> objectColorList;
	std::vector<glm::vec4> directionLightList;
	std::vector<glm::vec4> positionLightList;
	std::vector<glm::vec4> intensityLightList;

	while (fgets(buffer, 255, (FILE*)fp)) {
		char letter;
		float x, y, z, w;
		sscanf(buffer, "%c %f %f %f %f", &letter, &x, &y, &z, &w);
		switch (letter) {
		case 'e':
			s->SetUniform4f("eye", x, y, z, w);
			break;
		case 'a':
			s->SetUniform4f("ambient", x, y, z, w);
			break;
		case 'o':
			++sizes.x;
			objectList.push_back(glm::vec4(x, y, z, w));
			//s->SetUniform4f("objects", x, y, z, w);
			break;
		case 'r':
			reflectiveObjectList.push_back(glm::vec4(x, y, z, w));
			++sizes.z;
			break;
		case 't':
			transparantObjectList.push_back(glm::vec4(x, y, z, w));
			++sizes.w;
			break;
		case 'c':
			objectColorList.push_back(glm::vec4(x, y, z, w));
			//s->SetUniform4f("objColors", x, y, z, w);
			break;
		case 'd':
			directionLightList.push_back(glm::vec4(x, y, z, w));
			//s->SetUniform4f("lightsDirection", x, y, z, w);
			++sizes.y;
			break;
		case 'p':

			positionLightList.push_back(glm::vec4(x, y, z, w));
			//s->SetUniform4f("lightPosition", x, y, z, w);
			break;
		case 'i':
			intensityLightList.push_back(glm::vec4(x, y, z, w));
			//s->SetUniform4f("lightsIntensity", x, y, z, w);
			break;



		default: break;
		}
	}
	s->SetUniform4i("sizes", objectList.size(), directionLightList.size(), sizes.z, sizes.w);
	fclose(fp);
	//}
	glm::vec4 *objects = new  glm::vec4[objectList.size()];
	glm::vec4 *reflectiveObjects = new  glm::vec4[reflectiveObjectList.size()];
	glm::vec4 *transparantObjects = new  glm::vec4[transparantObjectList.size()];


	glm::vec4 *objectsColors = new  glm::vec4[objectColorList.size()];
	glm::vec4 *positions = new  glm::vec4[positionLightList.size()];
	glm::vec4 *directions = new  glm::vec4[directionLightList.size()];
	glm::vec4 *intensities = new  glm::vec4[intensityLightList.size()];

	int k = 0;
	for (auto i = objectList.begin(); i != objectList.end(); ++i) {
		objects[k] = *i;
		++k;
	}
	s->SetUniform4fv("objects", objects, objectList.size());



	k = 0;
	for (auto i = reflectiveObjectList.begin(); i != reflectiveObjectList.end(); ++i) {
		reflectiveObjects[k] = *i;
		++k;
	}
	s->SetUniform4fv("reflectiveObjects", reflectiveObjects, reflectiveObjectList.size());

	k = 0;
	for (auto i = transparantObjectList.begin(); i != transparantObjectList.end(); ++i) {
		transparantObjects[k] = *i;
		++k;
	}
	s->SetUniform4fv("transparantObjects", transparantObjects, transparantObjectList.size());

	k = 0;
	for (auto i = objectColorList.begin(); i != objectColorList.end(); ++i) {
		objectsColors[k] = *i;
		++k;
	}
	s->SetUniform4fv("objColors", objectsColors, objectColorList.size());


	k = 0;
	for (auto i = positionLightList.begin(); i != positionLightList.end(); ++i) {
		positions[k] = *i;
		++k;
	}
	s->SetUniform4fv("lightPosition", positions, positionLightList.size());



	k = 0;
	for (auto i = directionLightList.begin(); i != directionLightList.end(); ++i) {
		directions[k] = *i;
		++k;
	}
	s->SetUniform4fv("lightsDirection", directions, directionLightList.size());


	k = 0;
	for (auto i = intensityLightList.begin(); i != intensityLightList.end(); ++i) {
		intensities[k] = *i;
		++k;
	}
	s->SetUniform4fv("lightsIntensity", intensities, intensityLightList.size());







	delete objects;
	delete reflectiveObjects;
	delete transparantObjects;
	delete objectsColors;
	delete positions;
	delete directions;
	delete intensities;



	s->SetUniform4f("lightDirection", 0.0f, 0.0f, -1.0f, 0.0f);
	if (shaderIndx == 0)
		s->SetUniform4f("lightColor", r / 255.0f, g / 255.0f, b / 255.0f, 1.0f);    
	else {
		s->SetUniform4f("lightColor", (float)255 / 255, (float)1 / 255,
			(float)1 / 255, 1.0f);
	}
	//s->SetUniform4f("lightColor",0.7f+ additionToColor,0.8f+ additionToColor,
	//	additionToColor+0.1f,1.0f);

	s->Unbind();
}

void Game::WhenRotate()
{
}

void Game::WhenTranslate()
{
}

void Game::Motion()
{
	if (isActive)
	{
	}
}

Game::~Game(void)
{
}
