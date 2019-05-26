// FlisPlop.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "FlisPlop.h"
#include <random>
#include <iostream>
#include <Windows.h>
#include <ObjIdl.h>
#include <gdiplus.h>
#include <vector>
#include <fstream>
#include <istream>
#include <string>
#include <algorithm>
using namespace Gdiplus;
using namespace std;
#pragma comment (lib,"Gdiplus.lib")

#define MAX_LOADSTRING 100
#define WINDOW_WIDTH	816
#define WINDOW_HEIGHT	599
#define BALL_SIZE		15
#define GAP         	10
#define ROW_NUMBER      6
#define COLUMN_NUMBER   15

int time = 0;
int liczbapunkow = 0;
int sekundy = 0;
int minuty = 0;
int kierunek = 0; //0 - prawo, 1 - dol , 2 - lewo , 3 - gora
int kierunek2 = 0; //0 - prawo, 1 - dol , 2 - lewo , 3 - gora
int gamestate = 0; // 0 - start, 1 - active, 2 - pauza, 3 - wyniki
int whowon = 0; //0 - nie ma wynika, 1 - red, 2 - blue, 3 - draw
const int shootspeed = 30;
const int bulletsize = 16;
struct Paddle {
	HWND paddle_hwnd;
	int x;
	int y;
	int h;
	int w;
	int score =0;
	int health =100;
	int ammo =50;
};

struct Block {
	HWND block_hwnd;
	bool missed;
	bool collide = true;
	int x;
	int y;
	int h;
	int w;
};

struct Ball {
	HWND ball_hwnd;
	int h;
	int x;
	int y;
	int dx;
	int dy;
	int prize = 0;
};
struct Bullet {
	HWND hwnd;
	int x;
	int y;
	int dx;
	int dy;
	bool active = false;
	int kierunek = 0;
};

class Wynik{
public:
	int whowon;//0 - red // 1 - blue
	int score;
	Wynik(int whowon, int score) 
	{
		this->whowon = whowon;
		this->score = score;
	}
};	

vector<Wynik> Wyniki;
const int numofbullets = 50;
int bulletiter = 0;
Bullet Bullets[numofbullets];
Block TopOfWindow;
Paddle paddle;
Paddle paddle2;
HWND hMainWnd;
Ball ball;

bool gameover = false;
// Global Variables:
HINSTANCE hInst;                                // current instance
WCHAR szTitle[MAX_LOADSTRING];                  // The title bar text
WCHAR szWindowClass[MAX_LOADSTRING];            // the main window class name

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
ATOM                MyRegisterBlockClass(HINSTANCE hInstance);
ATOM				MyRegisterBallClass(HINSTANCE hInstance);
ATOM				MyRegisterPaddleClass(HINSTANCE hInstance);
ATOM				 MyRegisterBulletClass(HINSTANCE hInstance);
void MoveBall(void);
void checkcollision(void);
void BulletsAction(void);
void ResetGame(void);

void SaveToFile(void);
void ReadFromFile(void);
void UpdateData(Wynik w1, Wynik w2);
void Draw(HDC hdc);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
LRESULT CALLBACK    WndProc2(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);

void ResetGame(void)
{
	paddle.health = 100;
	paddle2.health = 100;
	paddle.ammo = 50;
	paddle2.ammo = 50;
	paddle.score = 0;
	paddle2.score = 0;
	minuty = 0;
	sekundy = 0;
}


BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
	{
		hInst = hInstance; // Store instance handle in our global variable

		HWND hWnd = CreateWindowW(szWindowClass, szTitle, (WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_MINIMIZEBOX | WS_CLIPCHILDREN),
			(GetSystemMetrics(SM_CXSCREEN) - 800) / 2, (GetSystemMetrics(SM_CYSCREEN) - 600) / 2, WINDOW_WIDTH, WINDOW_HEIGHT, nullptr, nullptr, hInstance, nullptr);
		hMainWnd = hWnd;
		if (!hWnd)
		{
			return FALSE;
		}
		TCHAR s[256];
		_stprintf_s(s, 256, _T("Tanks"));
		SetWindowText(hWnd, s);

		//SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) |
		   // WS_EX_LAYERED);
		//// Make this window 50% alpha
		   //SetLayeredWindowAttributes(hWnd, 0, (255 * 50) / 100, LWA_ALPHA);
		////
		ShowWindow(hWnd, nCmdShow);
		UpdateWindow(hWnd);
		RECT rect;
		GetClientRect(hWnd, &rect);

		paddle.w = 40; paddle.h = 40;
		paddle.x = 120;
		paddle.y = 120;
		/////////////////////////////
		paddle2.w = 40; paddle2.h = 40;
		paddle2.x = 320;
		paddle2.y = 120;
		////////////////////BALLS
		ball.x = 40; ball.y = 40;
		ball.h = 36;
		ball.dx = 5; ball.dy = 5;
		
		SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) |
			WS_EX_LAYERED);
		// Make this window 50% alpha
		SetLayeredWindowAttributes(hWnd, 0, (255 * 100) / 100, LWA_ALPHA);

		
	}
   ////////////////////////////////////////// INSTANTIATING BULLETS
	//Wyniki.push_back(Wynik(0, 20));
	//Wyniki.push_back(Wynik(1, 30));
	//SaveToFile();
	ReadFromFile();
	//UpdateData(Wynik(0, 40), Wynik(1, 50));
   return TRUE;
}

void SaveToFile(void)
{
	ofstream myfile("wyniki.txt",ios::trunc);
	for (std::vector<Wynik>::iterator it = Wyniki.begin(); it != Wyniki.end(); ++it) {
		myfile << it->whowon << " " << it->score << endl;
	}
	myfile.close();
}
void ReadFromFile(void) {
	ifstream myfile("wyniki.txt",ios::in);
	if (!myfile) return;
	string str;
	while (std::getline(myfile, str))
	{
		Wynik w(0, 0);
		w.whowon = stoi(str.substr(0, 1));
		w.score = stoi(str.substr(2, 4));
		Wyniki.push_back(w);
	}
	myfile.close();
}
void UpdateData(Wynik w1, Wynik w2) 
{
	Wyniki.push_back(w1);
	Wyniki.push_back(w2);
	sort(Wyniki.begin(), Wyniki.end(),
		[](const Wynik & a, const Wynik & b) -> bool
	{
		return a.score > b.score;
	});

}


void Draw(HDC hdc) 
{
	Bitmap bmptimer(L"timer.png");
	Bitmap bmpammo(L"ammo.png");
	Bitmap bmpbullet(L"bullet.png");
	Bitmap bmpcoin(L"coin.png");
	Bitmap bmpkit(L"emergency-kit.png");
	Bitmap bmpscore(L"score.png");
	Bitmap bmptankblue(L"tank_blue.png");
	Bitmap bmptankred(L"tank_red.png");

	Pen pen(Color(255, 255, 50, 50));
	SolidBrush brushred(Color(255, 255, 50, 50));
	SolidBrush brushgray(Color(255, 50, 50, 50));
	SolidBrush brushblue(Color(255, 50, 50, 255));

	Graphics gfx(hdc);
	Bitmap bmp(800, 600);

	Graphics* gf = Graphics::FromImage(&bmp);

	Color color = Color(255, 0, 0, 0);
	gf->Clear(color);

	//HBITMAP bm = LoadBitmap(hInst, MAKEINTRESOURCE(IDB_PNG1));
	gf->FillRectangle(&brushred, 0, 0, 330, 40);
	gf->FillRectangle(&brushgray, 330, 0, 140, 40);
	gf->FillRectangle(&brushblue, 470, 0, 330, 40);
	gf->DrawImage(&bmpkit, 4, 4,32, 32);
	gf->DrawImage(&bmpammo, 114, 4,32, 32);
	gf->DrawImage(&bmpscore, 224, 4,32, 32);
	gf->DrawImage(&bmpkit, 764, 4, 32, 32);
	gf->DrawImage(&bmpammo, 654, 4, 32, 32);
	gf->DrawImage(&bmpscore, 544, 4, 32, 32);
	gf->DrawImage(&bmptimer, 348, 4, 32, 32);

	switch (ball.prize) {
		case 0:
			gf->DrawImage(&bmpkit, ball.x, ball.y, 32, 32);
			break;
		case 1:
			gf->DrawImage(&bmpammo, ball.x, ball.y, 32, 32);
			break;
		case 2:
			gf->DrawImage(&bmpcoin, ball.x, ball.y, 32, 32);
			break;
	}
	gf->DrawImage(&bmptimer, 348, 4, 32, 32);
	
	switch (kierunek) {
	case 1:
		bmptankred.RotateFlip(Rotate90FlipNone);
		break;
	case 2:
		bmptankred.RotateFlip(Rotate180FlipNone);
		break;
	case 3:
		bmptankred.RotateFlip(Rotate270FlipNone);
		break;
	}
	
	gf->DrawImage(&bmptankred, paddle.x, paddle.y, 40, 40);
	switch (kierunek2) {
	case 1:
		bmptankblue.RotateFlip(Rotate90FlipNone);
		break;
	case 2:
		bmptankblue.RotateFlip(Rotate180FlipNone);
		break;
	case 3:
		bmptankblue.RotateFlip(Rotate270FlipNone);
		break;
	}
	gf->DrawImage(&bmptankblue, paddle2.x, paddle2.y, 40, 40);
	////////////////BULLETS
	for (size_t i = 0; i < numofbullets; i++)
	{
		if (Bullets[i].active)
		{
			switch (Bullets[i].kierunek) {
			case 1:
				bmpbullet.RotateFlip(Rotate90FlipNone);
				break;
			case 2:
				bmpbullet.RotateFlip(Rotate180FlipNone);
				break;
			case 3:
				bmpbullet.RotateFlip(Rotate270FlipNone);
				break;
			}
			gf->DrawImage(&bmpbullet, Bullets[i].x, Bullets[i].y,16,16);
			switch (Bullets[i].kierunek) {
			case 1:
				bmpbullet.RotateFlip(Rotate270FlipNone);
				break;
			case 2:
				bmpbullet.RotateFlip(Rotate180FlipNone);
				break;
			case 3:
				bmpbullet.RotateFlip(Rotate90FlipNone);
				break;
			}
		}
	}
	//gf.DrawImage()
	////////////////////// Logika
	  // Create a string.
	WCHAR string[100];
	// Initialize arguments.
	Font myFont(L"Arial", 16);
	RectF layoutRect(46, 10, .0f, 40.0f);
	StringFormat format;
	format.SetAlignment(StringAlignmentNear);
	SolidBrush whiteBrush(Color(255, 255, 255, 255));
	SolidBrush blackBrush(Color(255, 0, 0, 0));
	SolidBrush blackfade(Color(140, 0, 0, 0));

	_stprintf_s(string, 100, _T("%d"), paddle.health);
	gf->DrawString(string,-1,&myFont,layoutRect,&format,&whiteBrush);
	_stprintf_s(string, 100, _T("%d"), paddle.ammo);
	RectF layoutRect2(110 + 46, 10, .0f, 40.0f);
	gf->DrawString(string, -1, &myFont, layoutRect2, &format, &whiteBrush);
	_stprintf_s(string, 100, _T("%d"), paddle.score);
	RectF layoutRect3(220 + 46, 10, .0f, 40.0f);
	gf->DrawString(string, -1, &myFont, layoutRect3, &format, &whiteBrush);
	

	format.SetAlignment(StringAlignmentFar);
	_stprintf_s(string, 100, _T("%d"), paddle2.score);
	RectF layoutRect6(540, 10, .0f, 40.0f);
	gf->DrawString(string, -1, &myFont, layoutRect6, &format, &whiteBrush);
	_stprintf_s(string, 100, _T("%d"), paddle2.ammo);
	RectF layoutRect4(540+ 110, 10, .0f, 40.0f);
	gf->DrawString(string, -1, &myFont, layoutRect4, &format, &whiteBrush);
	_stprintf_s(string, 100, _T("%d"), paddle2.health);
	RectF layoutRect5(540 + 220, 10, .0f, 40.0f);
	gf->DrawString(string, -1, &myFont, layoutRect5, &format, &whiteBrush);

	if (sekundy < 10) {
		_stprintf_s(string, 100, _T("%d:0%d"), minuty, sekundy);
	}
	else
		_stprintf_s(string, 100, _T("%d:%d"), minuty, sekundy);
	RectF layoutRect7(440, 10, .0f, 40.0f);
	gf->DrawString(string, -1, &myFont, layoutRect7, &format, &blackBrush);
	if (gamestate != 1)gf->FillRectangle(&blackfade, 0, 0, 800, 600);
	/////////////////////// UI
	RectF layoutRect10(330, 210, 140, 40);
	RectF layoutRect8(330, 270, 140, 40);
	RectF layoutRect9(330, 330, 140, 40);
	POINT cursor;
	GetCursorPos(&cursor);
	ScreenToClient(hMainWnd, &cursor);
	format.SetAlignment(StringAlignmentCenter);
	switch (gamestate) {
	case 2:
		if (cursor.x > 330 && cursor.x < 470 && cursor.y < 240 && cursor.y>200)
			gf->FillRectangle(&brushred, 330, 200, 140, 40);
		else gf->FillRectangle(&brushgray, 330, 200, 140, 40);
		_stprintf_s(string, 100, _T("RETURN"));
		
		gf->DrawString(string, -1, &myFont, layoutRect10, &format, &blackBrush);
	case 0:
		if(cursor.x>330 && cursor.x < 470 && cursor.y<300 && cursor.y>260)
			gf->FillRectangle(&brushred, 330, 260, 140, 40);
			else gf->FillRectangle(&brushgray, 330, 260, 140, 40);
		if (cursor.x > 330 && cursor.x < 470 && cursor.y < 360 && cursor.y>320)
			gf->FillRectangle(&brushred, 330, 260 + 60, 140, 40);
			else gf->FillRectangle(&brushgray, 330, 260+60, 140, 40);
		_stprintf_s(string, 100, _T("NEW GAME"));
		
		gf->DrawString(string, -1, &myFont, layoutRect8, &format, &blackBrush);
		_stprintf_s(string, 100, _T("TOP 10"));
		
		gf->DrawString(string, -1, &myFont, layoutRect9, &format, &blackBrush);
		break;
	case 3:

		if (cursor.x > 330 && cursor.x < 470 && cursor.y < 550 && cursor.y>510)
			gf->FillRectangle(&brushred, 330, 510, 140, 40);
		else gf->FillRectangle(&brushgray, 330, 510, 140, 40);
		///////////
		_stprintf_s(string, 100, _T("RETURN"));
		RectF layoutRect13(330, 520, 140, 40.0f);
		gf->DrawString(string, -1, &myFont, layoutRect13, &format, &blackBrush);
		_stprintf_s(string, 100, _T("TOP 10"));
		RectF layoutRect14(330, 14, 140, 40.0f);
		gf->DrawString(string, -1, &myFont, layoutRect14, &format, &whiteBrush);
		//////////Wyniki
		for (std::vector<Wynik>::size_type i = 0; i != Wyniki.size() && i<10; i++) {
			gf->FillRectangle(&whiteBrush, 100, 55 + i*45, 600, 35);
			format.SetAlignment(StringAlignmentNear);
			if(Wyniki[i].whowon == 0)
			_stprintf_s(string, 100, _T("RED"));
			else _stprintf_s(string, 100, _T("BLUE"));
			RectF layoutRect15(110, 60+i*45, 140, 40.0f);
			gf->DrawString(string, -1, &myFont, layoutRect15, &format, &blackBrush);
			format.SetAlignment(StringAlignmentFar);
			_stprintf_s(string, 100, _T("%d"),Wyniki[i].score);
			RectF layoutRect16(620, 60 + i * 45, 70, 40.0f);
			gf->DrawString(string, -1, &myFont, layoutRect16, &format, &blackBrush);
		}
	}
	RectF layoutRect11(300, 100, 200, 40.0f);
	Font myFont1(L"Arial", 25);
	if(gamestate==0)
	switch (whowon) {
	case 1:
		_stprintf_s(string, 100, _T("RED WIN"));
		gf->DrawString(string, -1, &myFont1, layoutRect11, &format, &brushred);
		break;
	case 2:
		_stprintf_s(string, 100, _T("BLUE WIN"));
		gf->DrawString(string, -1, &myFont1, layoutRect11, &format, &brushblue);
		break;
	case 3:
		_stprintf_s(string, 100, _T("DRAW"));
		gf->DrawString(string, -1, &myFont1, layoutRect11, &format, &whiteBrush);
		break;
	}

	gfx.DrawImage(&bmp, 0, 0, 800, 600);
}

LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	PAINTSTRUCT ps;
	HDC hdc;
	HDC* hdc2;
    switch (message)
    {
	case WM_LBUTTONDOWN:
		if (gamestate == 1) break;
		POINT cursor;
		GetCursorPos(&cursor);
		ScreenToClient(hMainWnd, &cursor);
		if (cursor.x > 330 && cursor.x < 470) 
		{
			if (cursor.y < 240 && cursor.y>200) {
				if (gamestate == 2) gamestate = 1;
			}
			if (cursor.y < 300 && cursor.y>260) {
				if (gamestate != 3) {
					ResetGame();
					gamestate = 1; }
			}
			if (cursor.y < 360 && cursor.y>320) {
				if (gamestate !=3) gamestate = 3;
			}
			if (cursor.y < 550 && cursor.y>510) {
				if (gamestate == 3) if (sekundy == 0) gamestate = 0; else gamestate = 2;
			}
		}
		break;
	case WM_PAINT:
	{	
		hdc2 = new HDC();
		hdc = BeginPaint(hWnd, &ps);
		Draw(hdc);
		//BitBlt(hdc, 0, 0, 800, 600, hdc, 0, 0, DSTINVERT);
		EndPaint(hWnd, &ps);
		//UpdateWindow(hMainWnd);
		//DefWindowProc(hWnd,)
	}
	break;
    case WM_COMMAND: {}
        break;
	case WM_MOVE: 
		{
		SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) |
			WS_EX_LAYERED);
		// Make this window 50% alpha
		SetLayeredWindowAttributes(hWnd, 0, (255 * 50) / 100, LWA_ALPHA);
		//
		}
		break;
	case WM_ACTIVATE:
	{
		SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) |
			WS_EX_LAYERED);
		// Make this window 50% alpha
		SetLayeredWindowAttributes(hWnd, 0, (255 * 100) / 100, LWA_ALPHA);
		//
	}
	break;
	case WM_ERASEBKGND:
		return 1;
		break;
	case WM_NCMOUSEMOVE:
		{
		SetWindowLong(hWnd, GWL_EXSTYLE, GetWindowLong(hWnd, GWL_EXSTYLE) |
			WS_EX_LAYERED);
		// Make this window 50% alpha
		SetLayeredWindowAttributes(hWnd, 0, (255 * 100) / 100, LWA_ALPHA);
		//
		
		}
	break;
	case WM_KEYDOWN: 
		{
		if (wParam == VK_ESCAPE) {
			switch (gamestate) {
			case 1:
				gamestate = 2;
				break;
			case 2:
				gamestate = 1;
				break;
			case 3:
				if(sekundy == 0)
				gamestate = 0;
				else gamestate = 2;
				break;
			}
		}
		if (gamestate != 1) break;
		if (wParam == VK_SPACE) 
		{
			
			if (paddle.ammo <= 0) return 0;
			paddle.ammo--;
			time++;
			int i = bulletiter;
			bulletiter = (bulletiter + 1) % numofbullets;
			///////////////
			Bullets[i].active = true;
			int offset = (paddle.h - bulletsize) / 2;
			switch(kierunek) 
			{
			case 0:
				Bullets[i].x = paddle.x + paddle.w;
				Bullets[i].y = paddle.y + offset;
				Bullets[i].dx = shootspeed;
				Bullets[i].dy = 0;
				break;
			case 1:
				Bullets[i].x = paddle.x + offset;
				Bullets[i].y = paddle.y + paddle.h;
				Bullets[i].dy = shootspeed;
				Bullets[i].dx = 0;
				break;
			case 2:
				Bullets[i].x = paddle.x - bulletsize;
				Bullets[i].y = paddle.y + offset;
				Bullets[i].dx = -shootspeed;
				Bullets[i].dy = 0;
				break;
			case 3:
				Bullets[i].x = paddle.x + offset;
				Bullets[i].y = paddle.y - bulletsize;
				Bullets[i].dy = -shootspeed;
				Bullets[i].dx = 0;
				break;
			}
			Bullets[i].kierunek = kierunek;

		}
		if (wParam == VK_NUMPAD0)
		{
			if (paddle2.ammo <= 0) return 0;
			paddle2.ammo--;
			time++;
			int i = bulletiter;
			bulletiter = (bulletiter + 1) % numofbullets;
			///////////////
			Bullets[i].active = true;
			int offset = (paddle2.h - bulletsize) / 2;
			switch (kierunek2)
			{
			case 0:
				Bullets[i].x = paddle2.x + paddle2.w;
				Bullets[i].y = paddle2.y + offset;
				Bullets[i].dx = shootspeed;
				Bullets[i].dy = 0;
				break;
			case 1:
				Bullets[i].x = paddle2.x + offset;
				Bullets[i].y = paddle2.y + paddle2.h;
				Bullets[i].dy = shootspeed;
				Bullets[i].dx = 0;
				break;
			case 2:
				Bullets[i].x = paddle2.x - bulletsize;
				Bullets[i].y = paddle2.y + offset;
				Bullets[i].dx = -shootspeed;
				Bullets[i].dy = 0;
				break;
			case 3:
				Bullets[i].x = paddle2.x + offset;
				Bullets[i].y = paddle2.y - bulletsize;
				Bullets[i].dy = -shootspeed;
				Bullets[i].dx = 0;
				break;
			}
			Bullets[i].kierunek = kierunek2;

		}
		if (wParam == 0x57)
			{
			paddle.y -= 40;
			if (paddle.y < 40) paddle.y = 40;
			MoveWindow(paddle.paddle_hwnd, paddle.x, paddle.y, paddle.w, paddle.h, TRUE);
			kierunek = 3;
			}
		if (wParam == 0x53)
		{
			RECT rc;
			GetClientRect(hWnd, &rc);
			paddle.y += 40;
			if (paddle.y > rc.bottom-40) paddle.y = 520;
			MoveWindow(paddle.paddle_hwnd, paddle.x, paddle.y, paddle.w, paddle.h, TRUE);
			kierunek = 1;
		}
		if (wParam == 0x41)
		{
			paddle.x -= 40;
			if (paddle.x < 00) paddle.x = 0;
			MoveWindow(paddle.paddle_hwnd, paddle.x, paddle.y, paddle.w, paddle.h, TRUE);
			kierunek = 2;
		}
		if (wParam == 0x44)
		{
			kierunek = 0;
			RECT rc;
			GetClientRect(hWnd, &rc);
			paddle.x += 40;
			if (paddle.x > rc.right - 40) paddle.x = 760;
			MoveWindow(paddle.paddle_hwnd, paddle.x, paddle.y, paddle.w, paddle.h, TRUE);
		}

		if (wParam == VK_NUMPAD8)
		{
			paddle2.y -= 40;
			if (paddle2.y < 40) paddle2.y = 40;
			MoveWindow(paddle2.paddle_hwnd, paddle2.x, paddle2.y, paddle2.w, paddle2.h, TRUE);
			kierunek2 = 3;
		}
		if (wParam == VK_NUMPAD5)
		{
			RECT rc;
			GetClientRect(hWnd, &rc);
			paddle2.y += 40;
			if (paddle2.y > rc.bottom - 40) paddle2.y = 520;
			MoveWindow(paddle2.paddle_hwnd, paddle2.x, paddle2.y, paddle2.w, paddle2.h, TRUE);
			kierunek2 = 1;
		}
		if (wParam == VK_NUMPAD4)
		{
			paddle2.x -= 40;
			if (paddle2.x < 00) paddle2.x = 0;
			MoveWindow(paddle2.paddle_hwnd, paddle2.x, paddle2.y, paddle2.w, paddle2.h, TRUE);
			kierunek2 = 2;
		}
		if (wParam == VK_NUMPAD6)
		{
			kierunek2 = 0;
			RECT rc;
			GetClientRect(hWnd, &rc);
			paddle2.x += 40;
			if (paddle2.x > rc.right - 40) paddle2.x = 760;
			MoveWindow(paddle2.paddle_hwnd, paddle2.x, paddle2.y, paddle2.w, paddle2.h, TRUE);
		}
		
		//InvalidateWindow(hMainWnd)
		//RECT rect;
		//rect.left = 0;rect.right = 800; rect.top = 0;rect.bottom = 40;
		//InvalidateRect(hMainWnd, NULL, FALSE);

		//UpdateWindow(hMainWnd);
		}
		break;
    
	case WM_CREATE:
		SetTimer(hWnd, 1, 1000, NULL); 
		SetTimer(hWnd, 2, 14, NULL);
		SetTimer(hWnd, 3, 40, NULL);
		InvalidateRect(hMainWnd, NULL, FALSE);
		UpdateWindow(hMainWnd);
		break;
	case WM_TIMER:
		switch (wParam) {
		case 1:
			MoveBall();
			break;
		case 2:
			BulletsAction();
			checkcollision();
			break;
		case 3:
			InvalidateRect(hMainWnd, NULL, FALSE);
			break;
		}
		break;
	
    case WM_DESTROY:
		SaveToFile();
        PostQuitMessage(0);
        break;
    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}

void BulletsAction(void) 
{
	for (size_t i = 0; i < numofbullets; i++)
	{
		if (Bullets[i].active)
		{
			Bullets[i].x += Bullets[i].dx;
			Bullets[i].y += Bullets[i].dy;
			MoveWindow(Bullets[i].hwnd, Bullets[i].x, Bullets[i].y, bulletsize ,bulletsize, TRUE);
			UpdateWindow(Bullets[i].hwnd);
		}
	}
	//InvalidateRect(hMainWnd, NULL, FALSE);
	//UpdateWindow(hMainWnd);
}

void checkcollision(void)
{
	std::random_device dev;
	std::mt19937 rng(dev());
	std::uniform_int_distribution<std::mt19937::result_type> dist1(1, 18);
	std::random_device dev2;
	std::mt19937 rng2(dev2());
	std::uniform_int_distribution<std::mt19937::result_type> dist2(1, 12);
	//znajdzki
	{
		
		std::random_device dev3;
		std::mt19937 rng3(dev3());
		std::uniform_int_distribution<std::mt19937::result_type> dist3(0, 2);
		int x = ball.x + ball.h / 2;
		int y = ball.y + ball.h / 2;
		if (x > paddle.x && x < paddle.x + paddle.w && y > paddle.y && y < paddle.y + paddle.h)
		{
			ball.x = 40 * dist1(rng) + 2;
			ball.y = 40 * dist2(rng2) + 2;
			liczbapunkow++;
			switch (ball.prize) {
			case 0:
				paddle.health += 50;
				break;
			case 1:
				paddle.ammo += 20;
				break;
			case 2:
				paddle.score += 5;
				break;
			}

			ball.prize = dist3(rng3);
			InvalidateRect(hMainWnd, NULL, FALSE);
			UpdateWindow(hMainWnd);
		}
		if (x > paddle2.x && x < paddle2.x + paddle2.w && y > paddle2.y && y < paddle2.y + paddle2.h)
		{
			ball.x = 40 * dist1(rng) + 2;
			ball.y = 40 * dist2(rng2) + 2;
			liczbapunkow++;
			switch (ball.prize) {
			case 0:
				paddle2.health += 50;
				break;
			case 1:
				paddle2.ammo += 20;
				break;
			case 2:
				paddle2.score += 5;
				break;
			}

			ball.prize = dist3(rng3);
			InvalidateRect(hMainWnd, NULL, FALSE);
			UpdateWindow(hMainWnd);
		}
	}
	//MoveWindow(ball.ball_hwnd, ball.x, ball.y, ball.h, ball.h, TRUE);
	//
	for (int i = 0;i < numofbullets;i++)
	{
		int x = Bullets[i].x + bulletsize/2;
		int y = Bullets[i].y + bulletsize/2;
		if (x > paddle.x && x < paddle.x + paddle.w && y > paddle.y && y < paddle.y + paddle.h)
		{
			paddle.health -= 20;
			paddle2.score++;
			Bullets[i].x = MAXINT;
			if (paddle.health <= 0) {
				paddle.x = 40 * dist1(rng);
				paddle.y = 40 * dist2(rng2);
				paddle.health = 100;
				paddle2.score += 10;
			}
		}
		if (x > paddle2.x && x < paddle2.x + paddle2.w && y > paddle2.y && y < paddle2.y + paddle2.h)
		{
			paddle2.health -= 20;
			paddle.score++;
			Bullets[i].x = MAXINT;
			if (paddle2.health <= 0) {
				paddle2.x = 40 * dist1(rng);
				paddle2.y = 40 * dist2(rng2);
				paddle2.health = 100;
				paddle.score += 10;
			}
		}
	}


	//InvalidateRect(hMainWnd, NULL, TRUE);
	//UpdateWindow(hMainWnd);
	

}
	
void MoveBall(void)
{
	if (gamestate != 1) return;
	PAINTSTRUCT ps;
	HDC hdc;
	sekundy++;
	if (sekundy == 10) {
		minuty++;
		sekundy = 0;
		gamestate = 0;
		////////////Zbieranie wynikow
		if (paddle.score > paddle2.score) whowon = 1;
		else if (paddle.score < paddle2.score) whowon = 2;
		else whowon = 3;
		UpdateData(Wynik(0, paddle.score), Wynik(1, paddle2.score));
	}

	UpdateWindow(hMainWnd);

}

int APIENTRY wWinMain(_In_ HINSTANCE hInstance,
	_In_opt_ HINSTANCE hPrevInstance,
	_In_ LPWSTR    lpCmdLine,
	_In_ int       nCmdShow)
{
	UNREFERENCED_PARAMETER(hPrevInstance);
	UNREFERENCED_PARAMETER(lpCmdLine);

	Gdiplus::GdiplusStartupInput gdiplusStartupInput;
	ULONG_PTR gdiplusToken;
	Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, nullptr);
	// TODO: Place code here.

	// Initialize global strings
	LoadStringW(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
	LoadStringW(hInstance, IDC_FLISPLOP, szWindowClass, MAX_LOADSTRING);

	MyRegisterClass(hInstance);
	MyRegisterBlockClass(hInstance);
	MyRegisterPaddleClass(hInstance);
	MyRegisterBallClass(hInstance);
	MyRegisterBulletClass(hInstance);
	// Perform application initialization:
	if (!InitInstance(hInstance, nCmdShow))
	{
		return FALSE;
	}

	HACCEL hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(IDC_FLISPLOP));

	MSG msg;

	// Main message loop:
	while (GetMessage(&msg, nullptr, 0, 0))
	{
		if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
		{
			TranslateMessage(&msg);
			DispatchMessage(&msg);
		}
	}
	Gdiplus::GdiplusShutdown(gdiplusToken);
	return (int)msg.wParam;
}

ATOM MyRegisterClass(HINSTANCE hInstance)
{
	WNDCLASSEXW wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style = WS_OVERLAPPED | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;////CS_HREDRAW | CS_VREDRAW | WS_OVERLAPPED | WS_MINIMIZEBOX | WS_MAXIMIZEBOX ;
	wcex.lpfnWndProc = WndProc;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_FLISPLOP));
	wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(CreateSolidBrush(RGB(0, 0, 0)));
	wcex.lpszMenuName = MAKEINTRESOURCEW(IDC_FLISPLOP);
	wcex.lpszClassName = szWindowClass;
	wcex.hIconSm = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassExW(&wcex);
}

ATOM MyRegisterPaddleClass(HINSTANCE hInstance)
{
	WNDCLASSEXW wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style = 0;/// CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc = WndProc2;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_SMALL));
	wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(CreateSolidBrush(RGB(0, 255, 0)));
	wcex.lpszMenuName = MAKEINTRESOURCEW(IDI_SMALL);
	wcex.lpszClassName = L"PaddleWnd";
	wcex.hIconSm = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassExW(&wcex);
}

ATOM MyRegisterBallClass(HINSTANCE hInstance)
{
	WNDCLASSEXW wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style = 0;///CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc = WndProc2;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_SMALL));
	wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(CreateSolidBrush(RGB(255, 255, 0)));
	wcex.lpszMenuName = MAKEINTRESOURCEW(IDI_SMALL);
	wcex.lpszClassName = L"BallWnd";
	wcex.hIconSm = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassExW(&wcex);
}

ATOM MyRegisterBulletClass(HINSTANCE hInstance)
{
	WNDCLASSEXW wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style = 0;///CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc = WndProc2;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_SMALL));
	wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(CreateSolidBrush(RGB(255, 0, 0)));
	wcex.lpszMenuName = MAKEINTRESOURCEW(IDI_SMALL);
	wcex.lpszClassName = L"BulletWnd";
	wcex.hIconSm = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassExW(&wcex);
}


ATOM MyRegisterBlockClass(HINSTANCE hInstance)
{
	WNDCLASSEXW wcex;

	wcex.cbSize = sizeof(WNDCLASSEX);

	wcex.style = 0;///CS_HREDRAW | CS_VREDRAW;
	wcex.lpfnWndProc = WndProc2;
	wcex.cbClsExtra = 0;
	wcex.cbWndExtra = 0;
	wcex.hInstance = hInstance;
	wcex.hIcon = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_SMALL));
	wcex.hCursor = LoadCursor(nullptr, IDC_ARROW);
	wcex.hbrBackground = (HBRUSH)(CreateSolidBrush(RGB(50, 50, 50)));
	wcex.lpszMenuName = MAKEINTRESOURCEW(IDI_SMALL);
	wcex.lpszClassName = L"WallWnd";
	wcex.hIconSm = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

	return RegisterClassExW(&wcex);
}



// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
	UNREFERENCED_PARAMETER(lParam);
	switch (message)
	{
	case WM_INITDIALOG:
		return (INT_PTR)TRUE;

	case WM_COMMAND:
		if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
		{
			EndDialog(hDlg, LOWORD(wParam));
			return (INT_PTR)TRUE;
		}
		break;
	}
	return (INT_PTR)FALSE;
}

LRESULT CALLBACK WndProc2(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	switch (message)
	{
	case WM_NCHITTEST:
		return HTTRANSPARENT;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}

LRESULT CALLBACK WndProcBullet(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
	PAINTSTRUCT ps;
	HDC hdc;
	switch (message)
	{
	case WM_PAINT:
	{
		hdc = BeginPaint(hWnd, &ps);
		EndPaint(hWnd, &ps);
		UpdateWindow(hMainWnd);
		//DefWindowProc(hWnd,)
	}
	break;
	case WM_NCHITTEST:
		return HTTRANSPARENT;
	case WM_DESTROY:
		PostQuitMessage(0);
		break;
	default:
		return DefWindowProc(hWnd, message, wParam, lParam);
	}
	return 0;
}