import cv2
import time
#import Adafruit_BBIO.UART as UART
#import serial


class CharacterRecogniser:
    def __init__(self):
        self.cap = cv2.VideoCapture(0)
        self.cap.set(3, 500)
        self.cap.set(4, 720)

    def send_lights(self, player, ready):
        p = 1
        # UART.setup("UART1")
        # ser = serial.Serial(port="/dev/ttyO1", baudrate=9600)
        # ser.close()
        # ser.open()
        # time.sleep(1)
        #
        # if ser.isOpen():
        #     if player == 1:
        #         if ready == 1:
        #             ser.write("1R\n")
        #         elif ready == 0:
        #             ser.write("1N\n")
        #     elif player == 2:
        #         if ready == 1:
        #             ser.write("2R\n")
        #         elif ready == 0:
        #             ser.write("2N\n")
        # ser.close()

    def choose(self):
        ready1 = 0; ready2 = 0

        dk1 = 0; yoshi1 = 0; falcon1 = 0; fox1 = 0; mario1 = 0; kirby1 = 0
        ness1 = 0; pika1 = 0; jiggly1 = 0; link1 = 0; samus1 = 0; luigi1 = 0

        dk2 = 0; yoshi2 = 0; falcon2 = 0; fox2 = 0; mario2 = 0; kirby2 = 0
        ness2 = 0; pika2 = 0; jiggly2 = 0; link2 = 0; samus2 = 0; luigi2 = 0

        p1pick = 0; p2pick = 0
        p1 = 0; p2 = 0

        while (not ready1) or (not ready2):
            ret, img = self.cap.read()
            while not ret:
                ret, img = self.cap.read()

            gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
            ret, thr = cv2.threshold(gray, 50, 130, cv2.THRESH_BINARY)
            p1Thr = thr[:, 0:340]
            p2Thr = thr[:, 340:720]
            contours1, h = cv2.findContours(p1Thr, 1, 2)
            contours2, h = cv2.findContours(p2Thr, 1, 2)

            for cnt in contours1:
                approx = cv2.approxPolyDP(cnt, 0.01 * cv2.arcLength(cnt, True), True)
                area = cv2.contourArea(approx)

                if (area < 15000) and (area > 2000) and (len(approx) < 25):
                    #print area
                    #print len(approx)
                    # cv2.drawContours(img1, cnt, -1, (0, 255, 0), 5)

                    if (len(approx) >= 13) and (len(approx) <= 15) and (area > 3220) and (area < 3600):
                        print "DK"
                        cv2.drawContours(img, [cnt], 0, (255, 0, 0), -1)
                        dk1 += 1
                        p1 += 1
                    elif (len(approx) >= 11) and (len(approx) <= 16) and (area > 3350) and (area < 3800):
                        print "Yoshi"
                        cv2.drawContours(img, [cnt], 0, (0, 0, 255), -1)
                        yoshi1 += 1
                        p1 += 1
                    elif (len(approx) >= 20) and (len(approx) <= 24) and (area > 3500) and (area < 4000):
                        print "Falcon"
                        cv2.drawContours(img, [cnt], 0, (0, 100, 255), -1)
                        falcon1 += 1
                        p1 += 1
                    elif (len(approx) >= 11) and (len(approx) <= 12) and (area > 2200) and (area < 2500):
                        print "Pika"
                        cv2.drawContours(img, [cnt], 0, (100, 100, 255), -1)
                        pika1 += 1
                        p1 += 1
                    elif (len(approx) >= 6) and (len(approx) <= 8) and (area > 900) and (area < 1150):
                        print "Luigi"
                        cv2.drawContours(img, [cnt], 0, (100, 100, 100), -1)
                        luigi1 += 1
                        p1 += 1
                    elif (len(approx) >= 9) and (len(approx) <= 11) and (area > 2100) and (area < 2400):
                        print "Kirby"
                        cv2.drawContours(img, [cnt], 0, (100, 0, 255), -1)
                        kirby1 += 1
                        p1 += 1
                    elif (len(approx) >= 6) and (len(approx) <= 10) and (area > 1250) and (area < 2400):
                        print "Ness"
                        cv2.drawContours(img, [cnt], 0, (100, 150, 255), -1)
                        ness1 += 1
                        p1 += 1
                    elif (len(approx) >= 13) and (len(approx) <= 17) and (area > 5800) and (area < 6400):
                        print "Samus"
                        cv2.drawContours(img, [cnt], 0, (0, 255, 0), -1)
                        samus1 += 1
                        p1 += 1
                    elif (len(approx) >= 12) and (len(approx) <= 16) and (area > 1900) and (area < 2300):
                        print "Mario"
                        cv2.drawContours(img, [cnt], 0, (255, 255, 255), -1)
                        mario1 += 1
                        p1 += 1
                    elif (len(approx) >= 15) and (len(approx) <= 20) and (area > 2500) and (area < 2900):
                        print "Fox"
                        cv2.drawContours(img, [cnt], 0, (255, 0, 255), -1)
                        fox1 += 1
                        p1 += 1
                    elif (len(approx) >= 5) and (len(approx) <= 8) and (area > 2400) and (area < 2800):
                        print "Jiggly"
                        cv2.drawContours(img, [cnt], 0, (255, 100, 255), -1)
                        jiggly1 += 1
                        p1 += 1
                    elif (len(approx) >= 3) and (len(approx) <= 5) and (area > 3500) and (area < 3900):
                        print "Link"
                        cv2.drawContours(img, [cnt], 0, (0, 0, 0), -1)
                        link1 += 1
                        p1 += 1

            for cnt in contours2:
                approx = cv2.approxPolyDP(cnt, 0.01 * cv2.arcLength(cnt, True), True)
                area = cv2.contourArea(approx)

                if (area < 15000) and (area > 2000) and (len(approx) < 25):
                    # print area
                    # print len(approx)
                    # cv2.drawContours(img2, cnt, -1, (0, 255, 0), 3)

                    if (len(approx) >= 13) and (len(approx) <= 15) and (area > 3220) and (area < 3600):
                        print "DK"
                        cv2.drawContours(img, [cnt], 0, (255, 0, 0), -1)
                        dk2 += 1
                        p2 += 1
                    elif (len(approx) >= 11) and (len(approx) <= 16) and (area > 3350) and (area < 3800):
                        print "Yoshi"
                        cv2.drawContours(img, [cnt], 0, (0, 0, 255), -1)
                        yoshi2 += 1
                        p2 += 1
                    elif (len(approx) >= 20) and (len(approx) <= 24) and (area > 3500) and (area < 4000):
                        print "Falcon"
                        cv2.drawContours(img, [cnt], 0, (0, 100, 255), -1)
                        falcon2 += 1
                        p2 += 1
                    elif (len(approx) >= 11) and (len(approx) <= 12) and (area > 2200) and (area < 2500):
                        print "Pika"
                        cv2.drawContours(img, [cnt], 0, (100, 100, 255), -1)
                        pika2 += 1
                        p2 += 1
                    elif (len(approx) >= 6) and (len(approx) <= 8) and (area > 900) and (area < 1150):
                        print "Luigi"
                        cv2.drawContours(img, [cnt], 0, (100, 100, 100), -1)
                        luigi2 += 1
                        p2 += 1
                    elif (len(approx) >= 9) and (len(approx) <= 11) and (area > 2100) and (area < 2400):
                        print "Kirby"
                        cv2.drawContours(img, [cnt], 0, (100, 0, 255), -1)
                        kirby2 += 1
                        p2 += 1
                    elif (len(approx) >= 6) and (len(approx) <= 10) and (area > 1250) and (area < 2400):
                        print "Ness"
                        cv2.drawContours(img, [cnt], 0, (100, 150, 255), -1)
                        ness2 += 1
                        p2 += 1
                    elif (len(approx) >= 13) and (len(approx) <= 17) and (area > 5800) and (area < 6400):
                        print "Samus"
                        cv2.drawContours(img, [cnt], 0, (0, 255, 0), -1)
                        samus2 += 1
                        p2 += 1
                    elif (len(approx) >= 12) and (len(approx) <= 16) and (area > 1900) and (area < 2300):
                        print "Mario"
                        cv2.drawContours(img, [cnt], 0, (255, 255, 255), -1)
                        mario2 += 1
                        p2 += 1
                    elif (len(approx) >= 15) and (len(approx) <= 20) and (area > 2500) and (area < 2900):
                        print "Fox"
                        cv2.drawContours(img, [cnt], 0, (255, 0, 255), -1)
                        fox2 += 1
                        p2 += 1
                    elif (len(approx) >= 5) and (len(approx) <= 8) and (area > 2400) and (area < 2800):
                        print "Jiggly"
                        cv2.drawContours(img, [cnt], 0, (255, 100, 255), -1)
                        jiggly2 += 1
                        p2 += 1
                    elif (len(approx) >= 3) and (len(approx) <= 5) and (area > 3500) and (area < 3900):
                        print "Link"
                        cv2.drawContours(img, [cnt], 0, (0, 0, 0), -1)
                        link2 += 1
                        p2 += 1

            time.sleep(.1)

            if (p1 > 20) and (not ready1):
                ready1 = 1
                self.send_lights(1, 1)
                char = [luigi1, mario1, dk1, link1, samus1, falcon1, ness1, yoshi1, kirby1, fox1, pika1, jiggly1]
                maxval = char[0]
                index = 0
                for i in range(len(char)):
                    print i
                    if char[i] >= maxval:
                        maxval = char[i]
                        index = i

                if index == 0:
                    p1pick = "Luigi"
                elif index == 1:
                    p1pick = "Mario"
                elif index == 2:
                    p1pick = "DK"
                elif index == 3:
                    p1pick = "Link"
                elif index == 4:
                    p1pick = "Samus"
                elif index == 5:
                    p1pick = "Falcon"
                elif index == 6:
                    p1pick = "Ness"
                elif index == 7:
                    p1pick = "Yoshi"
                elif index == 8:
                    p1pick = "Kirby"
                elif index == 9:
                    p1pick = "Fox"
                elif index == 10:
                    p1pick = "Pika"
                elif index == 11:
                    p1pick = "Jiggly"
                print "P1: ", p1pick
                p1pick = index

            if (p2 > 20) and (not ready2):
                ready2 = 1
                self.send_lights(2, 1)
                char = [luigi2, mario2, dk2, link2, samus2, falcon2, ness2, yoshi2, kirby2, fox2, pika2, jiggly2]
                maxval = char[0]
                index = 0
                for i in range(len(char)):
                    print i
                    if char[i] >= maxval:
                        maxval = char[i]
                        index = i

                if index == 0:
                    p2pick = "Luigi"
                elif index == 1:
                    p2pick = "Mario"
                elif index == 2:
                    p2pick = "DK"
                elif index == 3:
                    p2pick = "Link"
                elif index == 4:
                    p2pick = "Samus"
                elif index == 5:
                    p2pick = "Falcon"
                elif index == 6:
                    p2pick = "Ness"
                elif index == 7:
                    p2pick = "Yoshi"
                elif index == 8:
                    p2pick = "Kirby"
                elif index == 9:
                    p2pick = "Fox"
                elif index == 10:
                    p2pick = "Pika"
                elif index == 11:
                    p2pick = "Jiggly"
                print "P2: ", p2pick
                p2pick = index

        self.cap.release()
        return p1pick, p2pick