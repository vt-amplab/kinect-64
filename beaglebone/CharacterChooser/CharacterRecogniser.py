import numpy as np
import cv2
import time


class CharacterRecogniser:
    def __init__(self):
        self.cap = cv2.VideoCapture(0)

    def choose(self):
        self.cap.set(3, 500)
        self.cap.set(4, 720)

        ready = 0
        p1 = 0
        p2 = 0
        r1 = 0
        b1 = 0
        s1 = 0
        t1 = 0
        o1 = 0
        n1 = 0
        r2 = 0
        b2 = 0
        s2 = 0
        t2 = 0
        o2 = 0
        n2 = 0
        p1pick = 0
        p2pick = 0

        while not ready:
            ret = 0
            while not ret:
                ret, img = self.cap.read()

            img1 = img[:, 0:340]
            img2 = img[:, 340:720]
            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            cv2.imshow('gray', gray)
            ret, thr = cv2.threshold(gray, 50 + 3*10, 130, cv2.THRESH_BINARY)
            p1Thr = thr[:, 0:340]
            p2Thr = thr[:, 340:720]
            contours1, h = cv2.findContours(p1Thr, 1, 2)
            contours2, h = cv2.findContours(p2Thr, 1, 2)

            for cnt in contours1:
                approx = cv2.approxPolyDP(cnt, 0.01 * cv2.arcLength(cnt, True), True)
                area = cv2.contourArea(approx)

                if (area < 15000) and (area > 2000) and (len(approx) < 17):
                    print area
                    print len(approx)
                    cv2.drawContours(img1, cnt, -1, (0, 255, 0), 5)

                    if (len(approx) >= 6) and (len(approx) <= 8) and (area > 3700) and (area < 4100):
                        r1 += 1
                        p1 += 1
                        # print "right angle 1"
                        cv2.drawContours(img1, [cnt], 0, (255, 0, 0), -1) #blue
                    elif (len(approx) >= 10) and (len(approx) <=15) and (area > 3700) and (area < 4250):
                        b1 += 1
                        p1 += 1
                        # print "bolt 1"
                        cv2.drawContours(img1, [cnt], 0, (102, 102, 0), -1) #dark green
                    elif (len(approx) >= 4) and (len(approx) <= 5) and (area > 8200) and (area < 9100):
                        s1 += 1
                        p1 += 1
                        # print "square 1"
                        cv2.drawContours(img1, [cnt], 0, (0, 0, 255), -1) #red
                    elif (len(approx) >= 3) and (len(approx) <= 4) and (area > 3200) and (area < 3700):
                        t1 += 1
                        p1 += 1
                        # print "triangle 1"
                        cv2.drawContours(img1, [cnt], 0, (255, 255, 0), -1) #teal
                    elif (len(approx) >= 8) and (len(approx) <= 10) and (area > 8400) and (area < 8750):
                        o1 += 1
                        p1 += 1
                        # print "octagon 1"
                        cv2.drawContours(img1, [cnt], 0, (0, 255, 255), -1) #yellow
                    elif (len(approx) >= 5) and (len(approx) <= 6) and (area > 9500) and (area < 9800):
                        n1 += 1
                        p1 += 1
                        # print "notch 1"
                        cv2.drawContours(img, [cnt], 0, (50, 102, 50), -1) #blue green

            for cnt in contours2:
                approx = cv2.approxPolyDP(cnt, 0.01 * cv2.arcLength(cnt, True), True)
                area = cv2.contourArea(approx)

                if (area < 15000) and (area > 2000) and (len(approx) < 17):
                    print area
                    print len(approx)
                    cv2.drawContours(img2, cnt, -1, (0, 255, 0), 3)

                    if (len(approx) >= 6) and (len(approx) <= 8) and (area > 3700) and (area < 4100):
                        r2 += 1
                        p2 += 1
                        # print "right angle 2"
                        cv2.drawContours(img2, [cnt], 0, (255, 0, 0), -1) #blue
                    elif (len(approx) >= 10) and (len(approx) <=15) and (area > 3700) and (area < 4250):
                        b2 += 1
                        p2 += 1
                        # print "bolt 2"
                        cv2.drawContours(img2, [cnt], 0, (102, 102, 0), -1) #dark green
                    elif (len(approx) >= 4) and (len(approx) <= 5) and (area > 8200) and (area < 9100):
                        s2 += 1
                        p2 += 1
                        # print "square 2"
                        cv2.drawContours(img2, [cnt], 0, (0, 0, 255), -1) #red
                    elif (len(approx) >= 3) and (len(approx) <= 4) and (area > 3200) and (area < 3700):
                        t2 += 1
                        p2 += 1
                        # print "triangle 2"
                        cv2.drawContours(img2, [cnt], 0, (255, 255, 0), -1) #teal
                    elif (len(approx) >= 8) and (len(approx) <= 10) and (area > 8400) and (area < 8750):
                        o2 += 1
                        p2 += 1
                        # print "octagon 2"
                        cv2.drawContours(img2, [cnt], 0, (0, 255, 255), -1) #yellow
                    elif (len(approx) >= 5) and (len(approx) <= 6) and (area > 9500) and (area < 9800):
                        n2 += 1
                        p2 += 1
                        # print "notch 2"
                        cv2.drawContours(img2, [cnt], 0, (50, 102, 50), -1) #blue green

            time.sleep(.1)

            if (p1 > 20) and (p2 > 20):
                ready = 1
                if (r1 > b1) and (r1 > s1) and (r1 > t1) and (r1 > o1) and (r1 > n1):
                    p1pick = 1
                elif (b1 > r1) and (b1 > s1) and (b1 > t1) and (b1 > o1) and (b1 > n1):
                    p1pick = 2
                elif (s1 > r1) and (s1 > b1) and (s1 > t1) and (s1 > o1) and (s1 > n1):
                    p1pick = 3
                elif (t1 > r1) and (t1 > b1) and (t1 > s1) and (t1 > o1) and (t1 > n1):
                    p1pick = 4
                elif (o1 > r1) and (o1 > b1) and (o1 > s1) and (o1 > t1) and (o1 > n1):
                    p1pick = 5
                elif (n1 > r1) and (n1 > b1) and (n1 > s1) and (n1 > t1) and (n1 > o1):
                    p1pick = 6

                if (r2 > b2) and (r2 > s2) and (r2 > t2) and (r2 > o2) and (r2 > n2):
                    p2pick = 1
                elif (b2 > r2) and (b2 > s2) and (b2 > t2) and (b2 > o2) and (b2 > n2):
                    p2pick = 2
                elif (s2 > r2) and (s2 > b2) and (s2 > t2) and (s2 > o2) and (s2 > n2):
                    p2pick = 3
                elif (t2 > r2) and (t2 > b2) and (t2 > s2) and (t2 > o2) and (t2 > n2):
                    p2pick = 4
                elif (o2 > r2) and (o2 > b2) and (o2 > s2) and (o2 > t2) and (o2 > n2):
                    p2pick = 5
                elif (n2 > r2) and (n2 > b2) and (n2 > s2) and (n2 > t2) and (n2 > o2):
                    p2pick = 6

        self.cap.release()
        return p1pick, p2pick