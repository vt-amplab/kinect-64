import cv2

cap = cv2.VideoCapture(0)
cap.set(3, 640)
cap.set(4, 480)

ret, img = cap.read()
while not ret:
    ret, img = cap.read()

gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)
cv2.imshow('gray', gray)

i = 0

while True:
    key = cv2.waitKey(0)

    if key == ord('u'):
        i += 1
    elif key == ord('d'):
        i -= 1
    elif key == ord('g'):
        print (50 + i*10)
        break

    ret, thr = cv2.threshold(gray, 50 + i*10, 130, cv2.THRESH_BINARY)
    cv2.imshow('thresh', thr)
