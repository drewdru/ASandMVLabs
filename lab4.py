"""
    @package lab4
    Find similar images
"""
import os
import sys
import math
import argparse
from multiprocessing import Process, Queue
from PIL import Image

def rmsDifference(img1, img2, size):
    """
        Calculate root mean square difference of two images
        @param img1 The first image
        @param img2 The second image
        @param size The images width and height
        @return root mean square difference of two images
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
        @param size The image width and height
        @return array is upper left 8x8 block of DCTMatrix
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
def getImageHash(img, size, queue):
    """
        Calculate image pHash
        @param img The image
        @param size The image width and height
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
    hashString = "{0:0>4X}".format(int(hashString, 2))
    queue.put(hashString)

# may be optimized: https://habrahabr.ru/post/211264/
def isSimilarPHash(hashString1, hashString2):
    """
        Check difference of hashes
        @param hashString1 The hash string
        @param hashString2 The hash string
        @return True if count of hashes difference < 16
    """
    if len(hashString1) != 16 or len(hashString2) != 16:
        raise Exception('One of two strings not a 64-bit hash')
    differenceSum = 0
    for indx in range(16):
        if hashString1[indx] != hashString2[indx]:
            differenceSum += 1
        if differenceSum > 5:
            return False
    return True


def findSimilarImagesByPHash(imgDir):
    """
        Find similar images by pHash in directory
        @param imgDir The path to directory with images
    """
    size = 32, 32
    imageList = os.listdir(imgDir)
    for index, inImage1 in enumerate(imageList):
        img1Path = imgDir + inImage1
        img1 = Image.open(img1Path)
        img1 = img1.resize(size, Image.ANTIALIAS)
        img1 = img1.convert(mode='RGB')
        startINDX = index+1
        for inImage2 in imageList[startINDX:]:
            img2Path = imgDir + inImage2
            img2 = Image.open(img2Path)
            img2 = img2.resize(size, Image.ANTIALIAS)
            img2 = img2.convert(mode='RGB')

            q1 = Queue()
            q2 = Queue()
            p1 = Process(target=getImageHash, args=(img1, size, q1))
            p1.start()
            p2 = Process(target=getImageHash, args=(img2, size, q2))
            p2.start()
            img1PHash = q1.get()
            img2PHash = q2.get()
            p1.join()
            p2.join()

            if isSimilarPHash(img1PHash, img2PHash):
                similarImagesPaths = '{}\n{}\n\n'.format(img1Path, img2Path)
                print(similarImagesPaths)
                with open('similarImages.txt', 'a+') as f:
                    f.write(similarImagesPaths)


def main(method=0, imgDir=None):
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
    if imgDir is None:
        imgDir = './defaultImgDir/'
    if method == 1:
        findSimilarImagesByRMSD(imgDir)
    if method == 2:
        findSimilarImagesByPHash(imgDir)
    print('Paths to Similar images wrtite to similarImages.txt')


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Find similar images.')
    parser.add_argument('-d', '--directory',
        nargs='?',
        metavar='directory',
        dest='directory',
        # action='store_const',
        const='./defaultImgDir/',
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
    args = parser.parse_args()
    sys.exit(main(args.method, args.directory))
