#pragma once
#include "Keyboard.h"
#include "windows.h"

namespace SmashBros{

typedef enum{
	Luigi=0,
	Mario,
	DonkeyKong,
	Link,
	Samus,
	CFalcon,
	Ness,
	Yoshi,
	Kirby,
	Fox,
	Pikachu,
	Jigglypuff
} Character;

typedef enum{
	NoAttack=0,
	Punch,
	Special,
	Taunt, 
	Grab,
	Block
} Attack;

typedef enum{
	MCharacter=0,
	MAttack,
	MMap, 
} Mode;

typedef enum{
	NoMove=0,
	MoveUp,
	MoveDown,
	MoveLeft,
	MoveRight
} Direction;

typedef enum{
	PeachCastle=0,
	Jungle,
	HyruleCastle,
	Zebes, 
	Mushroom,
	YoshiIsland,
	DreamLand,
	SectorZ,
	SaffronCity,
	Random
} Map;



class Player : public Keyboard{
public:
	Player(char up='w', char down='s', char left='a', char right='d', 
				char a='x', char b='c', char l='z', char r='v', char z='b', char s=VK_RETURN);
	~Player();

	virtual void up(int dur=25);
	virtual void down(int dur=25);
	virtual void left(int dur=25);
	virtual void right(int dur=25);

	virtual void attack(Attack attack, Direction direction=NoMove);

	virtual void pickCharacter(Character c);
	virtual void pickMap(Map m);

	virtual void select(int delay=0);
	virtual void back(int delay=0);

	virtual void start();

	virtual void reset();
	virtual void enterVersus();

	virtual void center();

	inline bool hasPicked();

	inline void resetPick();

private:

	char m_upKey, m_leftKey, m_rightKey, m_downKey;

	char m_A, m_B, m_L, m_R, m_Z, m_S;

	bool m_picked;

	
};
bool Player::hasPicked(){return m_picked;}
void Player::resetPick(){m_picked = false;}
}