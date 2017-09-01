"""
    @package lab4
    Find similar images
"""
import os
import sys
import math
import argparse
from PIL import Image

def rmsDifference(img1, img2, size):
    """
        Calculate root mean square difference of two images

        @param img1 The first image
        @param img2 The second image
        @param size The width and height
        @return Root mean square difference of two images
    """
    res = 0
    for i in range(size[0]):
        for j in range(size[1]):
            try:
                dif = img1[i, j] - img2[i, j]
                res += pow(dif, 2)
            except IndexError:
                break
    res = math.sqrt(res) / 256
    return res


def findSimilarImagesByRMSD(imgDir):
    """
        Find similar images by RMSD in directory

        @param imgDir The path to directory with images
    """
    size = 32, 32
    imageList = os.listdir(imgDir)
    for index, inImage1 in enumerate(imageList):
        img1Path = imgDir + inImage1
        img1 = Image.open(img1Path)
        img1 = img1.resize(size, Image.ANTIALIAS)
        img1 = img1.convert(mode='L')
        startINDX = index + 1
        for inImage2 in imageList[startINDX:]:
            img2Path = imgDir + inImage2
            img2 = Image.open(img2Path)
            img2 = img2.resize(size, Image.ANTIALIAS)
            img2 = img2.convert(mode='L')
            rmsDiff = rmsDifference(img1.load(), img2.load(), img1.size)
            if rmsDiff < 1:
                similarImagesPaths = '{}\n{}\n\n'.format(img1Path, img2Path)
                print(similarImagesPaths)
                with open('similarImages.txt', 'a+') as f:
                    f.write(similarImagesPaths)


def DCT(img, size):
    """
        Calculate discrete cosine transform

        @param img The image
        @param size The width and height
        @return List of upper left 8x8 block of DCTMatrix
    """
    DCTMatrix = [[0 for x in range(size[0])] for y in range(size[1])]
    for u in range(size[0]):
        for v in range(size[1]):
            for i in range(size[0]):
                for j in range(size[1]):
                    r, g, b = img.getpixel((i, j))
                    S = (r + g + b) // 3
                    val1 = math.pi/size[0]*(i+1./2.)*u
                    val2 = math.pi/size[1]*(j+1./2.)*v
                    DCTMatrix[u][v] += S * math.cos(val1) * math.cos(val2)

    matrix = [[0 for x in range(8)] for y in range(8)]
    for i in range(8):
        for j in range(8):
            matrix[i][j] = DCTMatrix[i][j]
    return matrix


# pHash https://habrahabr.ru/post/120562/
def getImageHash(img, size, isBin):
    """
        Calculate image pHash

        @param img The image
        @param size The width and height
        @return 64-bit hash string
    """
    img = img.resize(size, Image.ANTIALIAS)
    matrix = DCT(img, size)
    average = 0
    for i in range(8):
        for j in range(8):
            if i == 0 and j == 0:
                continue
            average += matrix[i][j]
    average = average / 63

    hashString = ''
    for i in range(8):
        for j in range(8):
            if matrix[i][j] >= average:
                hashString += '1'
            else:
                hashString += '0'
    if isBin:
        return hashString
    return "{0:0>4X}".format(int(hashString, 2))

# may be optimized: https://habrahabr.ru/post/211264/
def checkHammingDistance(hashString1, hashString2, hashLength, HammingDistance):
    """
        Check difference of hashes

        @param hashString1 The hash string
        @param hashString2 The hash string
        @return Return True if count of hashes difference < hashLength
    """
    if (len(hashString1) != hashLength) or (len(hashString2) != hashLength):
        raise Exception('One of two strings not a 64-bit hash')
    differenceSum = 0
    for indx in range(hashLength):
        if hashString1[indx] != hashString2[indx]:
            differenceSum += 1
        if differenceSum > HammingDistance:
            return False
    return True


def findSimilarImagesByPHash(imgDir, isBin, HammingDistance):
    """
        Find similar images by pHash in directory

        @param imgDir The path to directory with images
    """
    hashLength = 16
    if isBin:
        hashLength = 64
    if (HammingDistance > hashLength) or (HammingDistance > hashLength):
        raise Exception('Incorrect HammingDistance\n'
            'HammingDistance <= 64 if isBin\n'
            'Or HammingDistance <= 64 if !isBin')
    size = 32, 32
    imageList = os.listdir(imgDir)
    imgInfoList = []
    print('Get images hash')
    imageListLength = len(imageList)
    for indx, inImage in enumerate(imageList):
        sys.stdout.write('\r')
        # the exact output you're looking for:
        sys.stdout.write('img {} of {}'.format(indx + 1, imageListLength))
        sys.stdout.flush()
        imgPath = imgDir + inImage
        img = Image.open(imgPath)
        img = img.resize(size, Image.ANTIALIAS)
        img = img.convert(mode='RGB')
        imgHash = getImageHash(img, size, isBin)
        imgInfoList.append({'hash': imgHash, 'path': imgPath})
    print('\nFind similar images by pHash')
    for index, imgInfo1 in enumerate(imgInfoList):
        startINDX = index+1
        for imgInfo2 in imgInfoList[startINDX:]:
            if checkHammingDistance(imgInfo1['hash'], imgInfo2['hash'],
                    hashLength, HammingDistance):
                similarImagesPaths = '{}\n{}\n\n'.format(imgInfo1['path'], imgInfo2['path'])
                print(similarImagesPaths)
                with open('similarImages.txt', 'a+') as f:
                    f.write(similarImagesPaths)


def main(method=0, imgDir='./defaultImgDir/', isBin=False, HammingDistance=5):
    """ main function """
    if method == 0:
        while True:
            try:
                method = int(input(
                    'Find similar images\n'
                    '1. Root mean square difference of images\n'
                    '2. Similar images by pHash\n'
                    'Choose a method: '))
                break
            except ValueError:
                print('Please write a number [1-2]')
    if method == 1:
        findSimilarImagesByRMSD(imgDir)
    if method == 2:
        findSimilarImagesByPHash(imgDir, isBin, HammingDistance)
    print('Paths to Similar images wrtite to similarImages.txt')


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Find similar images.')
    parser.add_argument('-d', '--directory',
        nargs='?',
        metavar='directory',
        dest='directory',
        # action='store_const',
        const='./defaultImgDir/',
        default='./defaultImgDir/',
        help='a directory path for find similar images')
    parser.add_argument('-m', '--method',
        nargs='?',
        type=int,
        metavar='numberOfMethod',
        dest='method',
        const=0,
        default=0,
        help='''
            method number:
            1.Root mean square difference of images
            2.Similar images by pHash
            ''')
    parser.add_argument('-b', '--bin',
        nargs='?',
        type=bool,
        metavar='isBin',
        dest='isBin',
        const=False,
        default=False,
        help='is binary pHash')
    parser.add_argument('--hamming',
        nargs='?',
        type=int,
        metavar='HammingDistance',
        dest='HammingDistance',
        const=5,
        default=5,
        help='Hamming distance')
    args = parser.parse_args()
    sys.exit(main(args.method, args.directory, args.isBin, args.HammingDistance))
