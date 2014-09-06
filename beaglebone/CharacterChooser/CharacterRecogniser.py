import numpy as np
import cv2

# img = cv2.imread('polygon.png')
# gray = cv2.imread('polygon.png',0)

cap = cv2.VideoCapture(0)
cap.set(3, 640)
cap.set(4, 480)

for i in range(100):
    ret, pic = cap.read()
    if ret:
        break

img = pic[1:460,170:530]

gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

ret, th1 = cv2.threshold(gray, 100, 110, cv2.THRESH_BINARY)
# ret, th2 = cv2.threshold(gray, 100, 120, cv2.THRESH_BINARY)
# ret, th3 = cv2.threshold(gray, 100, 130, cv2.THRESH_BINARY)
# ret, th4 = cv2.threshold(gray, 100, 140, cv2.THRESH_BINARY)
# ret, th5 = cv2.threshold(gray, 100, 150, cv2.THRESH_BINARY)
# ret, th6 = cv2.threshold(gray, 100, 160, cv2.THRESH_BINARY)
# ret, th7 = cv2.threshold(gray, 100, 170, cv2.THRESH_BINARY)
# ret, th8 = cv2.threshold(gray, 100, 180, cv2.THRESH_BINARY)
# ret, th9 = cv2.threshold(gray, 100, 190, cv2.THRESH_BINARY)

cv2.imshow('thresh1', th1)
# cv2.imshow('thresh2', th2)
# cv2.imshow('thresh3', th3)
# cv2.imshow('thresh4', th4)
# cv2.imshow('thresh5', th5)
# cv2.imshow('thresh6', th6)
# cv2.imshow('thresh7', th7)
# cv2.imshow('thresh8', th8)
# cv2.imshow('thresh9', th9)

#
# contours, h = cv2.findContours(th1, 1, 2)
#
# for cnt in contours:
#     approx = cv2.approxPolyDP(cnt, 0.01 * cv2.arcLength(cnt, True), True)
#     area = cv2.contourArea(approx)
#     print area
#     print len(approx)
#
#     if (area < 150000) and (area > 2000):
#         if len(approx) == 5:
#             print "pentagon"
#             cv2.drawContours(img, [cnt], 0, 255, -1)
#         elif len(approx) == 3:
#             print "triangle"
#             cv2.drawContours(img, [cnt], 0, (0, 255, 0), -1)
#         elif len(approx) == 4:
#             print "square"
#             cv2.drawContours(img, [cnt], 0, (0, 0, 255), -1)
#         elif len(approx) == 9:
#             print "half-circle"
#             cv2.drawContours(img, [cnt], 0, (255, 255, 0), -1)
#         elif len(approx) > 12:
#             print "circle"
#             cv2.drawContours(img, [cnt], 0, (0, 255, 255), -1)
#
# cv2.imshow('img', img)
cv2.waitKey(0)
cv2.destroyAllWindows()