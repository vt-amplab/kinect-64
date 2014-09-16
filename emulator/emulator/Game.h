#pragma once
#include "player.h"
#include "TcpSocket.h"

using SmashBros::Player;

namespace SmashBros{

	typedef enum{
		SettingUp=1,
		PickChar,
		PickMap,
		Fighting,
		GameOverP1,
		GameOverP2
	} State;

	class Game{
	public:
		Game(State m = SettingUp);
		~Game();

		void begin();
		int pickCharacter(Character m, int p);
		int attack(Attack a, Direction d,  int p);
		int pickMap(Map a, int p);
		void win(int p);
		void test();

		inline int getState();
		inline void setState(State m);

	private:
		Player m_p1;
		Player m_p2;
		State m_state;
	
	};

	int Game::getState(){ return m_state; }
	void Game::setState(State m){ m_state = m; }

}