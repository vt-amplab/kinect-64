#include <string>

#include "stdafx.h"
#include "Tracker.h"
#include "windows.h"

#include "opencv2\highgui\highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"

using namespace cv;
using namespace System;

std::string cvwin("match");

Tracker::Tracker():
	m_screenWidth(1920), m_screenHeight(1080)

{
	m_p1 = cvLoadImage("pics\\p1.png");
	m_p2 = cvLoadImage("pics\\p2.png");
	m_first = cvLoadImage("pics\\first.png");
	m_second = cvLoadImage("pics\\second.png");
	// namedWindow(cvwin);

}

Tracker::~Tracker(){

}

void Tracker::getScreenGrab(cv::Mat& pic, cv::Rect& r){
	HDC hScreen = GetDC(NULL);

	HDC hdcMem = CreateCompatibleDC (hScreen);
	HBITMAP hBitmap = CreateCompatibleBitmap(hScreen, m_screenWidth, m_screenHeight);
	HGDIOBJ hOld = SelectObject(hdcMem, hBitmap);

	BitBlt(hdcMem, 0, 0, r.width, r.height, hScreen, r.x, r.y, SRCCOPY);
	SelectObject(hdcMem, hOld);

	BITMAPINFOHEADER bmi = {0};
	bmi.biSize = sizeof(BITMAPINFOHEADER);
	bmi.biPlanes = 1;
	bmi.biBitCount = 24;
	bmi.biWidth = r.width;
	bmi.biHeight = -r.height;
	bmi.biCompression = BI_RGB;
	bmi.biSizeImage =  3 * r.width * r.height;


	GetDIBits(hdcMem, hBitmap, 0, r.height, pic.data, (BITMAPINFO*)&bmi, DIB_RGB_COLORS);

	ReleaseDC(NULL,hScreen);
	DeleteDC(hdcMem);
	DeleteObject(hBitmap);
}

void Tracker::showScreen(Mat& dst, int x, int y, int width, int height){

	dst = cv::Mat(height,width, CV_8UC3);
	cv::Rect r(x,y,width,height);
	getScreenGrab(dst,r);
}


Point2f Tracker::cmpPics(cv::Mat& scene, cv::Mat& obj, int& score){
	/// Source image to display
	Mat img_display, result;
	obj.copyTo( img_display );

	/// Create the result matrix
	int result_cols =  scene.cols - obj.cols + 1;
	int result_rows = scene.rows - obj.rows + 1;

	result.create( result_cols, result_rows, CV_32FC1 );

	/// Do the Matching and Normalize
	int match_method = match_method;
	matchTemplate( scene, obj, result, match_method );
	normalize( result, result, 0, 100, NORM_MINMAX, -1, Mat() );

	/// Localizing the best match with minMaxLoc
	double minVal; double maxVal; Point minLoc; Point maxLoc;
	Point matchLoc;
	score = cv::mean(result)[0];
	minMaxLoc( result, &minVal, &maxVal, &minLoc, &maxLoc, Mat() );

	/// For SQDIFF and SQDIFF_NORMED, the best matches are lower values. For all the other methods, the higher the better
	if( match_method  == CV_TM_SQDIFF || match_method == CV_TM_SQDIFF_NORMED )
	{ matchLoc = minLoc; }
	else
	{ matchLoc = maxLoc; }

	/// Show me what you got
	//rectangle( img_display, matchLoc, Point( matchLoc.x + scene.cols , matchLoc.y + scene.rows ), Scalar::all(0), 2, 8, 0 );
	//rectangle( result, matchLoc, Point( matchLoc.x + scene.cols , matchLoc.y + scene.rows ), Scalar::all(0), 2, 8, 0 );

	// Console::WriteLine("found at "+matchLoc.x + ", "+matchLoc.y);
	Console::WriteLine("with score "+score);
	//rectangle(scene, Point(matchLoc.x, matchLoc.y), Point(matchLoc.x + obj.cols, matchLoc.y+ obj.rows), Scalar(123, 50, 200), 2);
	//imshow(cvwin,scene);
	//waitKey(2);

	return matchLoc;
}

int Tracker::checkGameOver(){
	cv::Mat scene(960, 1280, CV_8UC3);
	cv::Rect r(0, 0, 1280, 960);
	getScreenGrab(scene,r);
	int score;
	Point2f first = cmpPics(scene, m_first, score);
	static unsigned int hist = 0;
	if (score < 20 && score > 0){
		hist |= 1;
		hist = hist << 1;
		if (hist < 0xf){
			return -1;
		}
		Console::WriteLine("GAME OVER");
		Point2f P1 = cmpPics(scene, m_p1, score);
		Point2f P2 = cmpPics(scene, m_p2, score);
		Point2f diff1 = P1 - first;
		Point2f diff2 = P2 - first;
		int d1 = cv::sqrt(diff1.x*diff1.x + diff1.y*diff1.y);
		int d2 = cv::sqrt(diff2.x*diff2.x + diff2.y*diff2.y);
		if (d1 < d2) return 0;
		return 1;
	}else{
		hist = 0;
	}
	return -1;
}

