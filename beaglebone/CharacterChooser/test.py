import numpy as np
import cv2

cap = cv2.VideoCapture(0)
cap.set(3, 500)
cap.set(4, 720)

ret, img = cap.read()
while not ret:
    ret, img = cap.read()

gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
cv2.imshow('gray', gray)

ret, thr = cv2.threshold(gray, 50 + 3*10, 130, cv2.THRESH_BINARY)
cv2.imshow('thresh', thr)

contours1, h = cv2.findContours(thr, 1, 1)

for cnt in contours1:
    approx = cv2.approxPolyDP(cnt, 0.01 * cv2.arcLength(cnt, True), True)
    area = cv2.contourArea(approx)

    if (area < 5000) and (area > 2000) and (len(approx) < 25):
        print area
        print len(approx)
        cv2.drawContours(img, cnt, -1, (0, 255, 0), 5)

        if (len(approx) >= 13) and (len(approx) <= 15) and (area > 3350) and (area < 3600):
            print "DK"
            cv2.drawContours(img, [cnt], 0, (255, 0, 0), -1)
        elif (len(approx) >= 11) and (len(approx) <= 16) and (area > 3350) and (area < 3800):
            print "Yoshi"
            cv2.drawContours(img, [cnt], 0, (0, 0, 255), -1)
        elif (len(approx) >= 20) and (len(approx) <= 24) and (area > 3600) and (area < 4000):
            print "Falcon"
            cv2.drawContours(img, [cnt], 0, (0, 100, 255), -1)
        elif (len(approx) >= 10) and (len(approx) <= 12) and (area > 2200) and (area < 2500):
            print "Pika"
            cv2.drawContours(img, [cnt], 0, (100, 100, 255), -1)


cv2.imshow('contour', img)
cap.release()

cv2.waitKey(0)