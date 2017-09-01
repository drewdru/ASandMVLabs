"""
    @package histogram
    Segmentations with histogram
"""

def getHistogram(pixels, size):
    """
        Get histogram from pixels

        @param pixels The 8-bit pixels, black and white
        @param size The width and height
        @return color histogram
    """
    histogram = []
    for i in range(256):
        histogram.append(0)
    for i in range(size[0]):
        for j in range(size[1]):
            color = pixels[i, j]
            histogram[color] += 1
    for i in range(256):
        histogram[i] /= size[0]*size[1]
    return histogram


def otsuPeak(histogram, k):
    """
        Get histogram from pixels

        @param histogram The color histogram
        @param size The width and height
        @param size The width and height
        @return list of peak color
    """
    peak = []
    w1 = 0
    w2 = 0
    mu1 = 0
    mu2 = 0
    mu = 0
    for indx, v in enumerate(histogram):
        if indx <= k:
            w1 += v
        else:
            w2 += v
    for indx, v in enumerate(histogram):
        value = indx*v
        if indx <= k:
            mu1 += value/w1
        else:
            mu2 += value/w2
        mu += (value/w1 + value/w2)
    nu = (w1*w2*((mu2-mu1)**2))/(mu**2)
    for indx, v in enumerate(histogram):
        if v < nu:
            peak.append(indx-1)
    return peak


def histogramSegmentation(img, size, method, k):
    """ http://www.ijcset.net/docs/Volumes/volume2issue1/ijcset2012020103.pdf """
    pixels = img.load()
    histogram = getHistogram(pixels, size)
    if method == 'otsu':
        peak = otsuPeak(histogram, k)
        for i in range(size[0]):
            for j in range(size[1]):
                if pixels[i, j] in peak:
                    pixels[i, j] = 255
                else:
                    pixels[i, j] = 0
