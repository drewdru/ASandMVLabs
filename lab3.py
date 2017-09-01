import sys, os
import json
import random
import math
import argparse

from PIL import Image
# import numpy

from imageSegmentation import histogram

def main(file):
    img = Image.open(file)
    size = img.size
    img = img.convert(mode='L')
    img.show()
    histogram.histogramSegmentation(img, size, 'otsu', 100)
    img.show()
    cutObjectsFromImage(img, size)


def getAperturePosition(x, y, imgSize, i, j, filterSize):
    pixelPosX = x + i - 1
    pixelPosY = y + j - 1
    if pixelPosX < 0:
        pixelPosX += 1
    if pixelPosY < 0:
        pixelPosY += 1
    if pixelPosX > imgSize[0] or pixelPosY > imgSize[1]:
        return -1, -1
    if pixelPosX == imgSize[0]:
        pixelPosX = imgSize[0] - 2
    if pixelPosY == imgSize[1]:
        pixelPosY = imgSize[1] - 2
    return pixelPosX, pixelPosY


def findPointsInApperture(pixels, x, y, imgSize, filterSize, img, imageFigures):
    whiteColor = 255
    pointList = [getAperturePosition(x, y, imgSize, 0, 0, filterSize)]
    # for i in range(filterSize):
    #     for j in range(filterSize):
    pointTemp = []
    i = j = 0
    img2 = img.convert(mode='RGB')
    pixels2 = img2.load()
    goBack = True
    lastX = []
    lastY = []
    while True:
        if i >= filterSize:
            i = 0
            if goBack >= len(lastX):
                break
            else:
                goBack += 1
                x = lastX[-goBack]
                y = lastY[-goBack]
        while True:
            if j >= filterSize:
                j = 0
                break
            pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
            if pixelPosX == -1 or pixelPosY == -1:
                j += 1
                continue
            if (pixelPosX, pixelPosY) in pointList:
                j += 1
                continue
            if pixels[pixelPosX, pixelPosY] == whiteColor:
                pixels2[pixelPosX, pixelPosY] = (255, 0, 0)
                pointList.append((pixelPosX, pixelPosY))
                x = pixelPosX
                y = pixelPosY
                lastX.append(x)
                lastY.append(y)
                i = j = 0
                # wasApertureCenterList = wasApertureCenterList + findPointsInApperture(
                #         pixels, x, y, imgSize, filterSize, wasApertureCenterList)
            j += 1
        i += 1
    img2.show()
    return pointList

def cutObjectsFromImage(img, imgSize):
    pixels = img.load()
    filterSize = 3
    objectID = 0
    imageFigures = {
        'figures': []
    }
    x = 0
    y = 0
    whiteColor = 255
    apertureTempX = 0
    apertureTempY = 0
    figurePointList = []
    # for apertureTempX in range(imgSize[0]):
    #     for apertureTempY in range(imgSize[1]):
    while True:
        if apertureTempX >= imgSize[0]:
            apertureTempX = 0
            break
        while True:
            if apertureTempY >= imgSize[1]:
                apertureTempY = 0
                break
            x = apertureTempX
            y = apertureTempY
            flag = False
            if pixels[x, y] == whiteColor:
                if not imageFigures['figures']:
                    flag = True
                else:
                    countFigure = 0
                    for figure in imageFigures['figures']:
                        if (x, y) in figure['pontList']:
                            break
                        else:
                            countFigure += 1
                    if countFigure == len(imageFigures['figures']):
                        flag = True
            if flag:
                figurePointList = findPointsInApperture(pixels, x, y, imgSize, filterSize, img, imageFigures)
                # print(figurePointList)
                # return
                imageFigures['figures'].append({
                    'pontList': figurePointList
                })
            apertureTempY += filterSize
        apertureTempX += filterSize

                # print(imageFigures)
                # return
    print(imageFigures)
            # for point in pointList:
            #     if !point['wasApertureCenter']:
            #         x = point['point'][0]
            #         y = point['point'][1]
            #         wasApertureCenterList.append(point['point'])
            #     print(pointList)
            #
            # for i in range(filterSize):
            #     for j in range(filterSize):
            #         pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
            #         if pixelPosX == -1 or pixelPosY == -1:
            #             continue
            #         if pixels[pixelPosX, pixelPosY] == whiteColor:
            #             x = pixelPosX
            #             y = pixelPosY
            #             imageFigures['figures'][-1]['pontList'].append({
            #                 'x': x,
            #                 'y': y,
            #                 'wasApertureCenter': True
            #             })
            #         else:
            #             minValue = pixels[pixelPosX, pixelPosY]

            # avg = (minValue + maxValue) / 2
            # for i in range(filterSize):
            #     for j in range(filterSize):
            #         pixelPosX, pixelPosY = getAperturePosition(x, y, imgSize, i, j, filterSize)
            #         if pixelPosX == -1 or pixelPosY == -1:
            #             continue
            #         if pixels[pixelPosX,pixelPosY] < avg:
            #             pixels[pixelPosX,pixelPosY] = 0
            #         else:
            #             pixels[pixelPosX,pixelPosY] = 255
    img.show()
    


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Find similar images.')
    parser.add_argument('-f', '--file',
        nargs='?',
        metavar='file',
        dest='file',
        const='./lab3test.png',
        default='./lab3test.png',
        help='a path to image')
    args = parser.parse_args()
    sys.exit(main(args.file))