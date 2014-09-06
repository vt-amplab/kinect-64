#include "stdafx.h"
#include "Game.h"

using SmashBros::Game;
using SmashBros::Player;

using namespace System::Net::Sockets;
using namespace System::Text;
using namespace System::Threading;
using namespace System::Net;



Game::Game(SmashBros::State m):
	m_p1('w','s','a','d','x','c','z','v','b', VK_RETURN),
	m_p2('t','g','f','h','u','i','y','o','p', VK_BACK)
{
	Server^ ser = gcnew Server(this);
	m_state = m;
	if (m == SettingUp) begin();
}
Game::~Game(){
	
}

void Game::begin(){
	m_p1.reset();
	m_p1.enterVersus();
	m_state = PickChar;
}

int Game::pickCharacter(SmashBros::Character m, int p)
{
	if (m_state != PickChar) return m_state;

	if (!p) m_p1.pickCharacter(m); 
	else m_p2.pickCharacter(m);

	if (m_p2.hasPicked() && m_p1.hasPicked()){
		m_p1.resetPick();
		m_p2.resetPick();
		m_state = PickMap;
		m_p1.start();
	}

	return 0;
}
int Game::attack(SmashBros::Attack a, SmashBros::Direction d,  int p)
{
	if (m_state != Fighting) return m_state;

	if (!p) m_p1.attack(a,d); 
	else m_p2.attack(a,d);

	return 0;

}
int Game::pickMap(SmashBros::Map m, int p)
{
	if (m_state != PickMap) return m_state;
	m_p1.pickMap(m); 
	m_state = Fighting;

	return 0;
}
