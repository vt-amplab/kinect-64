
#include "opencv2\core\core.hpp"

class Tracker{
public:
	Tracker();
	~Tracker();

	cv::Point2f cmpPics(cv::Mat& scene, cv::Mat& obj, int& score);

	void getScreenGrab(cv::Mat& pic, cv::Rect& r);
	void showScreen(cv::Mat& dst, int x, int y, int width, int height);
	int checkGameOver();

private:

	int m_screenWidth;
	int m_screenHeight;

	cv::Mat m_p1;
	cv::Mat m_p2;
	cv::Mat m_first;
	cv::Mat m_second;

};