"""Global thresholding segmentations"""
import math

def getAperturePosition(x, y, imgSize, i, j, filterSize):
    pixelPosX = x + j - filterSize / 2
    pixelPosY = y + i - filterSize / 2
    if pixelPosX<0:
        pixelPosX+=filterSize / 2
    if pixelPosY<0:
        pixelPosY+=filterSize / 2
    if pixelPosX>imgSize[0] or pixelPosY>imgSize[1]:
        return -1, -1
    if pixelPosX==imgSize[0]:
        pixelPosX=imgSize[0] - (filterSize / 2)-1
    if pixelPosY==imgSize[1]:
        pixelPosY=imgSize[1]-(filterSize / 2)-1
    return pixelPosX, pixelPosY


# Bersen's method
def bernsen(img, imgSize, filterSize):
    pixels = img.load()
    for x in range(imgSize[0]):
        for y in range(imgSize[1]):
            minValue = 255
            maxValue = 0
            for i in range(filterSize):
                for j in range(filterSize):
                    pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize) 
                    if pixelPosX == -1 or pixelPosY == -1:
                        continue                  
                    if pixels[pixelPosX,pixelPosY] > maxValue:
                        maxValue = pixels[pixelPosX,pixelPosY]
                    if pixels[pixelPosX,pixelPosY] < minValue:
                        minValue = pixels[pixelPosX,pixelPosY]
            avg = (minValue + maxValue) / 2
            for i in range(filterSize):
                for j in range(filterSize):
                    pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
                    if pixelPosX == -1 or pixelPosY == -1:
                        continue
                    if pixels[pixelPosX,pixelPosY] < avg:
                        pixels[pixelPosX,pixelPosY] = 0
                    else:
                        pixels[pixelPosX,pixelPosY] = 255
    img.show()

# Niblack`s method
def niblack(img, imgSize, filterSize):
    k = 0.2
    pixels = img.load()
    for x in range(imgSize[0]):
        for y in range(imgSize[1]):
            minValue = 255
            maxValue = 0
            for i in range(filterSize):
                for j in range(filterSize):
                    pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
                    if pixelPosX == -1 or pixelPosY == -1:
                        continue                  
                    if pixels[pixelPosX,pixelPosY] > maxValue:
                        maxValue = pixels[pixelPosX,pixelPosY]
                    if pixels[pixelPosX,pixelPosY] < minValue:
                        minValue = pixels[pixelPosX,pixelPosY]
            avg = (minValue + maxValue) / 2
            RMS = 0
            differenceSum = 0
            for i in range(filterSize):
                for j in range(filterSize):
                    pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
                    if pixelPosX == -1 or pixelPosY == -1:
                        continue
                    differenceSum = (pixels[pixelPosX,pixelPosY] - avg) ** 2                    
            RMS += math.sqrt(differenceSum/filterSize)
            # value = int(avg + k*RMS)
            # for i in range(filterSize):
            #     for j in range(filterSize):
            #         pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
            #         if pixelPosX == -1 or pixelPosY == -1:
            #             continue
            #         pixels[pixelPosX,pixelPosY] = value
            value = int(avg + k*RMS)
            for i in range(filterSize):
                for j in range(filterSize):
                    pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
                    if pixelPosX == -1 or pixelPosY == -1:
                        continue
                    if pixels[pixelPosX,pixelPosY] < value:
                        pixels[pixelPosX,pixelPosY] = 0
                    else:
                        pixels[pixelPosX,pixelPosY] = 255
    img.show()
