#include "stdafx.h"
#include "player.h"
#include "windows.h"

using SmashBros::Player;

Player::Player(char up, char down, char left, char right, 
				char a, char b, char l, char r, char z, char s):
m_upKey(up), m_downKey(down), m_leftKey(left), m_rightKey(right), 
m_A(a), m_B(b), m_L(l), m_R(r), m_Z(z), m_S(s)
{

}

Player::~Player(){
}

void Player::up(int dur){
	if (dur)tapKey(m_upKey, dur);
}
void Player::down(int dur){
	if (dur)tapKey(m_downKey, dur);
}
void Player::left(int dur){
	if (dur)tapKey(m_leftKey, dur);
}
void Player::right(int dur){
	if (dur)tapKey(m_rightKey, dur);
}

void Player::center(){
	up(500);
	left(800);
}
void Player::select(int delay){
	tapKey(m_A,delay);
}
void Player::back(int delay){
	tapKey(m_B,delay);
}
void Player::start(){
	tapKey(VK_RETURN);
}
void Player::reset(){
	start();
	char buf[5] = {m_A,m_B,m_Z,m_R,0};
	tapKey(buf);
}

void Player::enterVersus(){
	// leave character selection
	Sleep(200);
	back();
	Sleep(200);
	up(650);
	right(700);
	left(50);
	select();

	// go back to start
	for (int i=0; i<3; i++){
		back(300);
		Sleep(100);
	}


	// to main menu
	start();

	// versus
	Sleep(1000);
	down(15);
	Sleep(1000);
	select();

	// select stock and start
	Sleep(400);
	down();
	for(int i=0; i <5; i++)
		left();
	right();
	up();
	select();
	Sleep(2000);
}

void Player::attack(SmashBros::Attack attack, SmashBros::Direction direction){
	char move[3] = {0,0,0};
	switch(direction){
		case NoMove:
		break;
		case MoveUp:
			move[0] = m_upKey;
		break;
		case MoveRight:
			move[0] = m_rightKey;
		break;
		case MoveLeft:
			move[0] = m_leftKey;
		break;
		case MoveDown:
			move[0] = m_downKey;
		break;
	}
	switch(attack){
		case Punch:
			move[1] = m_A;
		break;
		case Special:
			move[1] = m_B;
		break;
		case Taunt:
			move[1] = m_L;
		break;
		case Grab:
			move[1] = m_R;
		break;
		case Block:
			move[1] = m_Z;
		break;
	}

	if (move[0]) tapKey(move);
	else  tapKey(move+1);

}

void Player::pickCharacter(SmashBros::Character c){
	Sleep(400);
	back();
	Sleep(400);
	center();
	down(125);
	right(50);
	int charBlock = 120;
	int hfactor = ((int) c);
	int vfactor = hfactor/6;

	right(charBlock * ((hfactor % 6) ));
	down(charBlock * vfactor);
	select();

}

void Player::pickMap(SmashBros::Map m){
	Sleep(1400);
	int num = (int) m;
	if (num > 4){
		num -= 4;
		down();
		Sleep(200);
	}
	for (int i=0; i<num; i++){
		right(60);
		Sleep(200);
	}
	start();

}