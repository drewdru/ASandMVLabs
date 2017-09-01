using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ClassLibraryNeuralNetworks;

namespace WordsRecognition
{
    public partial class MainForm : Form
    {
        private const int BlobSize = 15;

        //        private readonly Dictionary<int, char> _characters = new Dictionary<int, char>
        //        {
        //            { 1, 'A' },  { 27, 'А'}, { 55, '0' }, { 65, '!' },
        //            { 2, 'B' },  { 28, 'Б'}, { 56, '1' }, { 66, '@' },
        //            { 3, 'C' },  { 29, 'В'}, { 57, '2' }, { 67, '#' },
        //            { 4, 'D' },  { 30, 'Г'}, { 58, '3' }, { 68, '$' },
        //            { 5, 'E' },  { 31, 'Д'}, { 59, '4' }, { 69, '%' },
        //            { 6, 'F' },  { 32, 'Е'}, { 60, '5' }, { 70, '^' },
        //            { 7, 'G' },  { 33, 'Ё'}, { 61, '6' }, { 71, '&' },
        //            { 8, 'H' },  { 34, 'Ж'}, { 62, '7' }, { 72, '*' },
        //            { 9, 'I' },  { 35, 'З'}, { 63, '8' }, { 73, '(' },
        //            { 10, 'J' }, { 36, 'И'}, { 64, '9' }, { 74, ')' },
        //            { 11, 'K' }, { 37, 'Й'},              { 75, ';' },
        //            { 12, 'L' }, { 38, 'К'},              { 76, '"' },
        //            { 13, 'M' }, { 39, 'Л'},
        //            { 14, 'N' }, { 40, 'М'},
        //            { 15, 'O' }, { 41, 'Н'},
        //            { 16, 'P' }, { 42, 'П'},
        //            { 17, 'Q' }, { 43, 'Р'},
        //            { 18, 'R' }, { 44, 'С'},
        //            { 19, 'S' }, { 45, 'Т'},
        //            { 20, 'T' }, { 46, 'У'},
        //            { 21, 'U' }, { 47, 'Ф'},
        //            { 22, 'V' }, { 48, 'Х'},
        //            { 23, 'W' }, { 49, 'Ц'},
        //            { 24, 'X' }, { 50, 'Ч'},
        //            { 25, 'Y' }, { 51, 'Щ'},
        //            { 26, 'Z' }, { 52, 'Э'},
        //                         { 53, 'Ю'},
        //                         { 54, 'Я'},
        //
        //        };

        private readonly Dictionary<int, char> _characters = new Dictionary<int, char>
        {
            {1, 'А'}, {2, 'Б'}, {3, 'В'}, {4, 'Г'}, {5, 'Д'}, {6, 'Е'}, {7, 'Ё'}, {8, 'Ж'}, {9, 'З'}, {10, 'И'},
            {11, 'Й'}, {12, 'К'}, {13, 'Л'}, {14, 'М'}, {15, 'Н'}, {16, 'О'}, {17, 'П'}, {18, 'Р'}, {19, 'С'}, {20, 'Т'},
            {21, 'У'}, {22, 'Ф'}, {23, 'Х'}, {24, 'Ц'}, {25, 'Ч'}, {26, 'Ш'}, {27, 'Щ'}, {28, 'Ъ'}, {29, 'Ы'}, {30, 'Ь'},
            {31, 'Э'}, {32, 'Ю'}, {33, 'Я'}
        };

        public MainForm()
        {
            InitializeComponent();

#if DEBUG
            Load += (sender, args) =>
            {
//                foreach (var file in Directory.GetFiles("temp"))
//                {
//                    File.Delete(file);
//                }
//                foreach (var file in Directory.GetFiles("bmp"))
//                {
//                    OnImageSelected(file);
//                    OnResolveClicked(null, null);
//                }
//
//                var files = Directory.GetFiles("bmp")
//                            .Select(Path.GetFileNameWithoutExtension)
//                            .Except(Directory.GetFiles("temp").Select(Path.GetFileNameWithoutExtension))
//                            .ToList();
                                OnImageSelected("Resources//1.bmp");
//                                OnImageSelected("Resources//2.bmp");
                //                OnImageSelected("Resources//3.bmp");
                                OnResolveClicked(null, null);
            };
#endif

            tcRoot.SelectedIndexChanged += OnSelectedTabChanged;
        }


        private void OnSelectedTabChanged(object sender, EventArgs e)
        {
            UnbindHandlers();

            btnResolve.Click += OnResolveClicked;
            btnOpen.Click += OnOpenClicked;
        }

        private void OnOpenClicked(object sender, EventArgs eventArgs)
        {
            var fileName = CreateSelectImageDialog();
            OnImageSelected(fileName);
        }

        private void OnImageSelected(string fileName)
        {
            pbSource.Image = Image.FromFile(fileName);
            llPath.Text = fileName;
        }

        private void OnResolveClicked(object sender, EventArgs eventArgs)
        {
            DetectWords();
        }

        private void DetectWords()
        {
            var pixels = PixelDataConverter.Parse(pbSource.Image);
            int width = pbSource.Image.Width;
            int height = pbSource.Image.Height;

            var nsPath = "Resources//5.nw";
            var ns = new NeuralNW(nsPath);

            Bitmap newBitmap = new Bitmap(pbSource.Image.Width, pbSource.Image.Height);
            Graphics graphics = Graphics.FromImage(newBitmap);
            graphics.DrawImage(pbSource.Image, 0, 0);

            int uniqueIx = 0;

            WordsExtractor extractor = new WordsExtractor();
            List<Sentence> extractedSentences = extractor.ExtractSentences(pixels, width, height, 2);
            foreach (var extractedSentence in extractedSentences)
            {
                var extractedWords = extractor.ExtractWords(extractedSentence, 1);
                foreach (var extractedWord in extractedWords)
                {
                    foreach (var character in extractedWord.Characters)
                    {
                        Character resizedCharacter = ResizeCharacter(character);
                        var binaryResult = ImageProcessor.ApplyBinaryFilter(resizedCharacter.Pixels, 125);

//                        var startOffset = extractedSentence.StartY;
//                        graphics.DrawRectangle(new Pen(Color.Red, 1f), 
//                            character.StartX, character.StartY + startOffset,
//                            character.EndX, character.EndY);
//
//                        graphics.DrawRectangle(new Pen(Color.Blue, 1f), 
//                            extractedSentence.StartX, extractedSentence.StartY, 
//                            extractedSentence.EndX, extractedSentence.EndY);

                        var vector = PixelDataConverter.ToVector(resizedCharacter.Pixels);
                        double[] result;
                        ns.NetOUT(vector, out result);
                        using (var a = File.Create("Resources\\r.txt"))
                        {
                            using (var writer = new StreamWriter(a))
                            {
                                foreach (var d in vector)
                                {
                                    writer.Write(d.ToString(new CultureInfo("ru-RU")) + "\n");
//                                    writer.Write(d + "\n ");
                                }
                            }
                        }

                        var recognitionResult = result
                            .Select((c, index) => new { Value = c, Index = index + 1 })
                            .OrderByDescending(c => c.Value)
                            .First()
                            .Index;

                        tbResult.Text += _characters[recognitionResult];
                        var characterImg = PixelDataConverter.ToImage(binaryResult, BlobSize, BlobSize);
//                        var resultFilePath = $"temp//{Path.GetFileNameWithoutExtension(llPath.Text)}.bmp";
//                        characterImg.Save(resultFilePath, ImageFormat.Bmp);
                        ilWordsImagesSource.Images.Add(characterImg);
                        lvLetters.Items.Add(_characters[recognitionResult].ToString(), uniqueIx++);
                    }
                    tbResult.Text += " ";
                }
                tbResult.Text += "\n";
            }

            pbSource.Image = newBitmap;

            return;
//
//            var sentecesSeek = DivideByHorizontal(pixels, width, height);
//            var rowsPointers = sentecesSeek.Keys.Select(c => c).ToArray(); // include
//            var endRowPointers = sentecesSeek.Values.Select(c => c).ToArray(); // include
//
//            for (int i = 0; i < sentecesSeek.Count; i++)
//            {
//                int localHeight;
//                if (sentecesSeek.Count == 1)
//                {
//                    localHeight = endRowPointers[0] - rowsPointers[0] + 1;
//                }
//                else
//                {
//                    if (i == sentecesSeek.Count - 1)
//                        localHeight = height - endRowPointers[i];
//                    else
//                        localHeight = endRowPointers[i] - rowsPointers[i] + 1;
//                }
//
//                List<Character> seeks = DivideByVertical(pixels
//                        .Skip(width*rowsPointers[i])
//                        .Take(localHeight*width)
//                        .ToArray(), width, localHeight)
//                    .Select(c =>
//                    {
//                        c.StartY = rowsPointers[i];
//                        c.EndY += rowsPointers[i];
//                        return c;
//                    }).ToList();
//
//                var letters = seeks.Where(c => c.Type == Character.CharacterType.LETTER).ToList();
//                var spaces = seeks.Where(c => c.Type == Character.CharacterType.SPACE).ToList();
//
//                Bitmap newBitmap = new Bitmap(pbSource.Image.Width, pbSource.Image.Height);
//                Graphics graphics = Graphics.FromImage(newBitmap);
//                graphics.DrawImage(pbSource.Image, 0, 0);
//
////                graphics.DrawRectangle(new Pen(Color.Blue, 3f), 
////                    0, rowsPointers[i],
////                    width, endRowPointers[i] - rowsPointers[i]);
//
//                foreach (var character in letters)
//                {
//                    graphics.DrawRectangle(new Pen(Color.Red, 1f),
//                        character.StartX,
//                        character.StartY,
//                        character.Width,
//                        character.Height);
//                }
//
//                foreach (var space in spaces)
//                {
//                    graphics.DrawRectangle(new Pen(Color.Green, 1f),
//                        space.StartX,
//                        space.StartY,
//                        space.Width,
//                        space.Height);
//                }
//
//                pbSource.Image = newBitmap;
//
//                letters = letters.Select(ResizeCharacter).ToList();
//
//                foreach (var letter in letters.Union(spaces).OrderBy(c => c.StartX))
//                {
//                    if (letter.Type == Character.CharacterType.SPACE)
//                    {
//                        tbResult.Text += " ";
//                    }
//                    else
//                    {
//                        var vector = ConvertPixelsToVector(letter.Pixels);
//                        double[] result;
//                        ns.NetOUT(vector, out result);
//
//                        var recognitionResult = result
//                            .Select((c, index) => new {Value = c, Index = index})
//                            .OrderByDescending(c => c.Value)
//                            .First()
//                            .Index;
//
//                        tbResult.Text += _characters[recognitionResult];
//                    }
//                }
//
//                tbResult.Text += "\n";
//            }
        }

        //        private Character ResizeCharacter(Character character)
        //        {
        //            int width = character.EndX - character.StartX;
        //            int height = character.EndY - character.EndY;
        //            var pixels = character.Pixels;
        //
        //            int diffX = Math.Abs(BLOB_SIZE - width)/2;
        //            int diffY = Math.Abs(BLOB_SIZE - height)/2;
        //
        //            PixelData[] result = new PixelData[BLOB_SIZE*BLOB_SIZE];
        //            for (int i = 0; i < BLOB_SIZE; i++)
        //            {
        //                for (int j = 0; j < BLOB_SIZE; j++)
        //                {
        //                    if ((i < diffX || i > width - diffX) &&
        //                        (j < diffY || j > height - diffY))
        //                    {
        //                        result[i*BLOB_SIZE + j] = PixelData.White;
        //                    }
        //                    else
        //                    {
        //                        result[i*BLOB_SIZE + j] = pixels[(i + diffX)*width + j + diffY];
        //                    }
        //                }
        //            }
        //
        //            character.Pixels = result;
        //            character.StartY -= diffY;
        //            character.StartX -= diffX;
        //            character.EndX += diffX;
        //            character.EndY += diffY;
        //
        //            return character;
        //        }

        public Character ResizeCharacter(Character character)
        {
            Image btm = PixelDataConverter.ToImage(character.Pixels, character.Width, character.Height);
            var resizedImage = ResizeImage(btm, BlobSize, BlobSize);
            var pixelDatas = PixelDataConverter.Parse(resizedImage);
            return new Character
            {
                Pixels = pixelDatas,
                Type = character.Type
            };
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        public Dictionary<int, int> DivideByHorizontal(PixelData[] pixels, int width, int height)
        {
            int spaceInPixels = 2;

            List<int> notEmptyLines = new List<int>();
            for (int i = 0; i < height; i++)
            {
                bool isAnyPixelsFound = false;
                for (int j = 0; j < width; j++)
                {
                    if (!pixels[i*width + j].IsWhite())
                    {
                        isAnyPixelsFound = true;
                        break;
                    }
                }

                if (isAnyPixelsFound)
                {
                    notEmptyLines.Add(i);
                }
            }

            Dictionary<int, int> seeks = new Dictionary<int, int>();
            int prevLineSeek = notEmptyLines.First();
            for (int i = 0; i < notEmptyLines.Count; i++)
            {
                if (i == notEmptyLines.Count - 1)
                {
                    seeks.Add(prevLineSeek, notEmptyLines[i]);
                }
                else if (Math.Abs(notEmptyLines[i] - notEmptyLines[i + 1]) >= spaceInPixels)
                {
                    seeks.Add(prevLineSeek, notEmptyLines[i]);
                    prevLineSeek = notEmptyLines[i + 1];
                }
            }

            return seeks;
        }

        public List<Character> DivideByVertical(PixelData[] rowPixels, int width, int height)
        {
            int spaceInPixels = 5;
            rowPixels = ImageProcessor.ApplyBinaryFilter(rowPixels, 200);

            List<int> blankLines = new List<int>();
            List<Character> characters = new List<Character>();

            Character current = null;
            for (int i = 0; i < width; i++)
            {
                bool isAnyPixelsFound = false;
                for (int j = 0; j < height; j++)
                {
                    if (!rowPixels[j*width + i].IsWhite())
                    {
                        if (current == null)
                        {
                            current = new Character();
                            current.StartX = i;
                            current.StartY = j;
                            current.EndY = j;
                            current.EndX = i;
                        }
                        else
                        {
                            if (i > current.EndX)
                            {
                                current.EndX = i;
                            }
                            if (j < current.StartY)
                            {
                                current.StartY = j;
                            }
                            if (j > current.EndY)
                            {
                                current.EndY = j;
                            }
                        }
                        isAnyPixelsFound = true;
                    }
                }

                if (!isAnyPixelsFound)
                {
                    blankLines.Add(i);
                    if (current != null)
                    {
                        var lastLetter = characters.LastOrDefault();
                        if (lastLetter != null)
                        {
                            if (Math.Abs(lastLetter.EndX - current.EndX) >= spaceInPixels)
                            {
                                characters.Add(current);
                            }
                            else
                            {
                                lastLetter.EndX = current.EndX;
                                lastLetter.EndY = Math.Max(current.EndY, lastLetter.EndY);
                                lastLetter.StartY = Math.Min(current.StartY, lastLetter.StartY);
                            }
                        }
                        else
                        {
                            characters.Add(current);
                        }

                        current = null;
                    }
                }
            }

            if (current != null)
            {
                characters.Add(current);
                current = null;
            }

            foreach (var character in characters)
            {
                var pixelsByLetter = new List<PixelData>(character.Width * character.Height);
                for (int i = character.StartY; i < character.EndY; i++)
                {
                    for (int j = character.StartX; j < character.EndX; j++)
                    {
                        pixelsByLetter.Add(rowPixels[i * (character.Width) + j]);
                    }
                }
                character.Pixels = pixelsByLetter.ToArray();
//                character.Pixels = rowPixels
//                    .Skip(character.StartX - 1 + Math.Max(width*character.StartY - 1, 0))
//                    .Take(width - character.StartX + (width - character.EndX) +
//                          width*(character.EndY - character.StartY))
//                    .ToArray();
            }

            if (!characters.Any()) return new List<Character>();
            if (!blankLines.Any()) return characters;

            int minStartY = characters.Min(c => c.StartY);
            int maxEndY = characters.Max(c => c.EndY);

            blankLines.RemoveAll(c => characters.Any(v => v.StartX <= c && v.EndX >= c));

            current = new Character
            {
                StartX = blankLines.FirstOrDefault(),
                EndX = blankLines.FirstOrDefault(),
                StartY = minStartY,
                EndY = maxEndY
            };
            for (int i = 0; i < blankLines.Count; i++)
            {
                if (i == blankLines.Count - 1)
                {
                    if (current == null)
                    {
                        current = new Character
                        {
                            StartX = blankLines[i],
                            StartY = minStartY,
                            EndY = maxEndY
                        };
                    }
                    current.EndX = blankLines[i];
                }
                else if (blankLines[i + 1] - blankLines[i] == 1)
                {
                    if (current == null)
                    {
                        current = new Character();
                        current.StartX = blankLines[i];
                        current.StartY = minStartY;
                        current.EndY = maxEndY;
                    }
                    else
                    {
                        current.EndX = blankLines[i + 1];
                    }
                }
                else if (current != null)
                {
                    current.Type = Character.CharacterType.SPACE;
                    characters.Add(current);
                    current = null;
                }
                else
                {
                    current = new Character();
                    current.Type = Character.CharacterType.SPACE;
                    current.StartX = blankLines[i];
                    current.EndX = blankLines[i];
                    current.StartY = minStartY;
                    current.EndY = maxEndY;
                    characters.Add(current);
                    current = null;
                }
            }

            if (current != null)
            {
                current.Type = Character.CharacterType.SPACE;
                current.EndX = blankLines.Last();
                characters.Add(current);
            }

            return characters;
        }


        private void UnbindHandlers()
        {
            btnOpen.Click -= OnOpenClicked;
            btnResolve.Click -= OnResolveClicked;
        }

        private string CreateSelectImageDialog()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp",
                Multiselect = false,
                InitialDirectory = Environment.CurrentDirectory
            };
            dlg.ShowDialog();

            return dlg.FileName;
        }

    }

    public class PixelData
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public PixelData()
        {
        }

        public PixelData(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override bool Equals(object obj) => Equals((PixelData)obj);

        protected bool Equals(PixelData other)
        {
            return Red == other.Red && Green == other.Green && Blue == other.Blue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Red;
                hashCode = (hashCode * 397) ^ Green;
                hashCode = (hashCode * 397) ^ Blue;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{nameof(Red)}: {Red}, {nameof(Green)}: {Green}, {nameof(Blue)}: {Blue}";
        }

        public static PixelData White = new PixelData(255, 255, 255);
        public static PixelData Black = new PixelData(0, 0, 0);

        public bool IsWhite() => Equals(White);
        public bool IsBlack() => Equals(Black);
    }

    public static class PixelDataConverter
    {
        public static PixelData[] Parse(Image img)
        {
            var bmp = (Bitmap)img;

            // Retrieve the width and height of the image (just the once). 
            int width = bmp.Width, height = bmp.Height;

            //                var rect = new Rectangle(0, 0, width, height);
            //                var bmpData = new BitmapData();
            //
            //                bmpData = bmp.LockBits(rect,
            //                    ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb,
            //                    bmpData);
            //
            //                var size = height * width;
            //                List<uint> buffer = new List<uint>();
            //                unsafe
            //                {
            //                    var tempBuff = (uint*)bmpData.Scan0.ToPointer();
            //                    for (int i = 0; i < size; i++)
            //                    {
            //                        buffer.Add(tempBuff[i]);
            //                    }
            //                }
            //
            //                bmp.UnlockBits(bmpData);
            //
            //                PixelData[] pixelData = new PixelData[buffer.Count];
            //                for (int i = 0; i < pixelData.Length; i++)
            //                {
            //                    int sR = (int)((buffer[i] >> 16) & 255);
            //                    int sG = (int)((buffer[i] >> 8) & 255);
            //                    int sB = (int)(buffer[i] & 255);
            //                    pixelData[i] = new PixelData(sR, sG, sB);
            //                }
            //
            //                return pixelData;

            // Lock the bitmap so we can access its internal data. 
            var lockedBmp = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite,
                PixelFormat.Format32bppArgb);
            try
            {
                // Create an array of 32-bit integers to store the rowPixels in. 
                // One array element is one pixel. 
                // "Stride" is the total width of the image in rowPixels and may be larger than the width of the image data * 4 due to alignment/padding.
                var pixelData = new int[lockedBmp.Height * lockedBmp.Stride / 4];

                // Copy the data from the bitmap to our array. 
                // BitmapData.Scan0 is the address of the first pixel in the image. 
                Marshal.Copy(lockedBmp.Scan0, pixelData, 0, pixelData.Length);

                // Loop over each pixel in the image. 
                for (int i = 0; i < pixelData.Length; ++i)
                {
                    // Retrieve the current pixel data as a Color structure. 
                    var pixelColour = Color.FromArgb(pixelData[i]);

                    // Adjust and store the new colour. 
                    pixelData[i] = Color.FromArgb(
                        pixelColour.A,
                        Math.Max((byte)0, pixelColour.R),
                        Math.Max((byte)0, pixelColour.G),
                        Math.Max((byte)0, pixelColour.B)
                    ).ToArgb();
                }

                // Copy the data back from our array into the image: 
                Marshal.Copy(pixelData, 0, lockedBmp.Scan0, pixelData.Length);

                return pixelData.Select(c => new PixelData
                {
                    Red = c >> 16 & 255,
                    Green = c >> 8 & 255,
                    Blue = c & 255
                }).ToArray();
            }
            finally
            {
                // Unlock the bitmap when we're done. 
                bmp.UnlockBits(lockedBmp);
            }
        }

        public static double[] ToVector(PixelData[] pixels)
        {
            return pixels.Select(c => (c.Red + c.Green + c.Blue) /  3 >= 125 ? -0.5 : 0.5).ToArray();
        }

        public static Image ToImage(PixelData[] pixels, int width, int height)
        {
            PixelsValidator.Validate(pixels, width, height);

            int size = pixels.Length;
            unsafe
            {
                fixed (uint* buffer = new uint[size])
                {
                    var pData = pixels.Select(c => (uint)(c.Red << 16 | c.Green << 8 | c.Blue)).ToArray();

                    for (int i = 0; i < size; i++)
                    {
                        buffer[i] = pData[i];
                    }

                    return new Bitmap(width, height, width * sizeof(uint), PixelFormat.Format32bppRgb, new IntPtr(buffer));
                    //                        btm.Save("tmp.bmp", ImageFormat.Bmp);
                }
            }

            //                return new Bitmap("tmp.png");

            //                Bitmap bm;
            //                using (var mStream = new MemoryStream())
            //                {
            //                    mStream.Write(pData, 0, pData.Length);
            //                    mStream.Seek(0, SeekOrigin.Begin);
            //                    using (var image = Image.FromStream(mStream))
            //                    {
            //                        // see if this works.
            //                        // handle the image as you wish, return it, process it or something else.
            //                    }
            //                    bm = new Bitmap(mStream);
            //                    mStream.Dispose();
            //                }
            //                return bm;
        }
    }

    public static class PixelsValidator
    {
        public static void Validate(PixelData[] pixels, int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentException("Width must be greater than zero");
            }
            if (height <= 0)
            {
                throw new ArgumentException("Height must be greater than zero");
            }
            if (pixels == null)
            {
                throw new ArgumentNullException(nameof(pixels));
            }
            if (!pixels.Any())
            {
                throw new ArgumentException("Collection of pixels should be not empty");
            }
            if (pixels.Length != width * height)
            {
                throw new ArgumentException($"Size of pixels collection ({pixels.Length}) is not equal to given size ({width * height})");
            }
        }
    }


    public class WordsExtractor
    {
        public List<Sentence> ExtractSentences(PixelData[] pixels, int width, int height, int spaceInPixels = 2)
        {
            PixelsValidator.Validate(pixels, width, height);

            List<int> notEmptyLines = new List<int>();
            for (int i = 0; i < height; i++)
            {
                bool isAnyPixelsFound = false;
                for (int j = 0; j < width; j++)
                {
                    if (!pixels[i * width + j].IsWhite())
                    {
                        isAnyPixelsFound = true;
                        break;
                    }
                }

                if (isAnyPixelsFound)
                {
                    notEmptyLines.Add(i);
                }
            }

            if (!notEmptyLines.Any()) return new List<Sentence>();

            Dictionary<int, int> seeks = new Dictionary<int, int>();
            int prevLineSeek = notEmptyLines.First();
            for (int i = 0; i < notEmptyLines.Count; i++)
            {
                if (i == notEmptyLines.Count - 1)
                {
                    seeks.Add(prevLineSeek, notEmptyLines[i]);
                }
                else if (Math.Abs(notEmptyLines[i] - notEmptyLines[i + 1]) >= spaceInPixels)
                {
                    seeks.Add(prevLineSeek, notEmptyLines[i]);
                    prevLineSeek = notEmptyLines[i + 1];
                }
            }

            return seeks.Select(c => new Sentence
            {
                StartY = c.Key,
                StartX = 0,
                EndX = width - 1,
                EndY = c.Value,
                Pixels = pixels.Skip(c.Key * width).Take((c.Value - c.Key + 1) * width).ToArray()
            }).ToList();
        }

        public List<Word> ExtractWords(Sentence sentence, int spaceInPixels = 2)
        {
            int width = sentence.Width;
            int height = sentence.Height;
            var rowPixels = ImageProcessor.ApplyBinaryFilter(sentence.Pixels, 125);

            var spaces = ExtractSpaces(rowPixels, width, height, 0);
            var letters = ExtractLetters(rowPixels, spaces, width, height, spaceInPixels);

            foreach (var character in letters.Union(spaces))
            {
                if (character.Type == Character.CharacterType.SPACE)
                {
                    character.Pixels = new PixelData[0];
                }
                else
                {
                    var pixelsByLetter = new List<PixelData>(character.Width * character.Height);
                    for (int i = character.StartY; i <= character.EndY; i++)
                    {
                        for (int j = character.StartX; j <= character.EndX; j++)
                        {
                            pixelsByLetter.Add(rowPixels[i * width + j]);
                        }
                    }
                    character.Pixels = pixelsByLetter.ToArray();
                }
            }

            List<Word> words = new List<Word>();
            Word currentWord = null;
            foreach (var character in letters.Union(spaces).OrderBy(c => c.StartX))
            {
                if (currentWord == null)
                {
                    currentWord = new Word();
                }

                if (character.Type == Character.CharacterType.SPACE && currentWord.Characters.Any())
                {
                    words.Add(currentWord);
                    currentWord = null;
                }
                else if(character.Type != Character.CharacterType.SPACE)
                {
                    currentWord.Characters.Add(character);
                }
            }

            if (currentWord != null)
            {
                words.Add(currentWord);
            }

            return words;
        }

        public List<Character> ExtractLetters(PixelData[] rowPixels, List<Character> spaces, int width, int height, int spaceInPixels = 5)
        {
            PixelsValidator.Validate(rowPixels, width, height);

            List<Character> characters = new List<Character>();

            // Create letter characters for each non empty string
            Character current = null;
            for (int i = 0; i < width; i++)
            {
                if (spaces.Any(c => c.StartX <= i && i <= c.EndX))
                {
                    continue;
                }

                current = new Character
                {
                    StartY = height,
                    StartX = i,
                    EndX = i,
                    EndY = 0,
                    Type = Character.CharacterType.LETTER
                };

                for (int j = 0; j < height; j++)
                {
                    int pixelIx = j*width + i;
                    var currentPxl = rowPixels[pixelIx];

                    if (currentPxl.IsBlack())
                    {
                        current.StartY = Math.Min(current.StartY, j);
                        current.EndY = Math.Max(current.EndY, j);
                    }
                }

                current.StartX = Math.Min(current.StartX, i);
                current.EndX = Math.Max(current.EndX, i);

                characters.Add(current);
            }

            // Merge characters by given spaceInPixels
            current = null;
            List<Character> resultCharacters = new List<Character>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (current == null)
                {
                    current = new Character
                    {
                        StartY = characters[i].StartY,
                        StartX = characters[i].StartX,
                        EndX = characters[i].EndX,
                        EndY = characters[i].EndY,
                        Type = characters[i].Type,
                        Pixels = characters[i].Pixels
                    };
                }

                if (Math.Abs(current.EndX - characters[i].StartX) > spaceInPixels)
                {
                    resultCharacters.Add(current);
                    current = null;
                    i--;
                }
                else
                {
                    current.StartY = Math.Min(current.StartY, characters[i].StartY);
                    current.EndY = Math.Max(current.EndY, characters[i].EndY);
                    current.StartX = Math.Min(current.StartX, characters[i].StartX);
                    current.EndX = Math.Max(current.EndX, characters[i].EndX); 
                }

                if (i == characters.Count - 1 && current != null)
                {
                    current.StartY = Math.Min(current.StartY, characters[i].StartY);
                    current.EndY = Math.Max(current.EndY, characters[i].EndY);
                    current.StartX = Math.Min(current.StartX, characters[i].StartX);
                    current.EndX = Math.Max(current.EndX, characters[i].EndX);
                    resultCharacters.Add(current);
                }
            }

            return resultCharacters;
        }

        public List<Character> ExtractSpaces(PixelData[] rowPixels, int width, int height, int spaceInPixels = 5)
        {
            PixelsValidator.Validate(rowPixels, width, height);
            // Find empty vertical lines
            var blankLines = new List<int>();
            for (int i = 0; i < width; i++)
            {
                bool isAnyPixelsFound = false;
                for (int j = 0; j < height; j++)
                {
                    var currentPxl = rowPixels[j*width + i];
                    if (currentPxl.IsBlack())
                    {
                        isAnyPixelsFound = true;
                        break;
                    }
                }

                if (!isAnyPixelsFound)
                {
                    blankLines.Add(i);
                }
            }

            var spaces = new List<Character>();

            Character lastSpace = null;

            if (blankLines.Count == 1)
            {
                spaces.Add(new Character()
                {
                    StartX = blankLines[0],
                    StartY = 0,
                    EndX = blankLines[0],
                    EndY = height,
                    Type = Character.CharacterType.SPACE,
                    Pixels = new PixelData[0]
                });

                return spaces;
            }

            // Merge empty lines by space character
            for (int i = 0; i < blankLines.Count; i++)
            {
                if (lastSpace == null)
                {
                    lastSpace = new Character
                    {
                        StartX = blankLines[i],
                        StartY = 0,
                        EndX = blankLines[i],
                        EndY = height,
                        Type = Character.CharacterType.SPACE,
                        Pixels = new PixelData[0]
                    };
                }

                if (i == blankLines.Count - 1)
                {
                    lastSpace.EndX = blankLines[i];
                    if (lastSpace.Width >= spaceInPixels)
                    {
                        spaces.Add(lastSpace);
                    }
                }
                else
                {
                    if (Math.Abs(blankLines[i] - blankLines[i + 1]) == 1)
                    {
                        lastSpace.EndX = Math.Max(blankLines[i], blankLines[i + 1]);
                    }
                    else
                    {
                        if (lastSpace.Width >= spaceInPixels)
                        {
                            spaces.Add(lastSpace);
                        }
                        lastSpace = null;
                    }
                }
            }
            return spaces;
        }
    }

    public class ImageProcessor
    {
        public static PixelData[] ApplyBinaryFilter(PixelData[] pixels, int threshold)
        {
            foreach (var pixelData in pixels)
            {
                var pixelValue = (pixelData.Red + pixelData.Green + pixelData.Blue) / 3 >= threshold ? 255 : 0;

                pixelData.Red = pixelValue;
                pixelData.Green = pixelValue;
                pixelData.Blue = pixelValue;
            }

            return pixels;
        }
    }

    public class Word
    {
        public List<Character> Characters { get; set; } = new List<Character>();
    }

    public class Sentence : ILocated
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }

        public PixelData[] Pixels { get; set; }

        public int Height => EndY - StartY + 1;
        public int Width => EndX - StartX + 1;
    }

    public class Character : ILocated
    {
        public int StartX { get; set; }
        public int EndX { get; set; }
        public int StartY { get; set; }
        public int EndY { get; set; }

        public PixelData[] Pixels { get; set; }
        public CharacterType Type { get; set; } = CharacterType.LETTER;

        public int Width => EndX - StartX + 1;
        public int Height => EndY - StartY + 1;

        public enum CharacterType
        {
            SPACE,
            LETTER,
            DIGIT
        }
    }

    public interface ILocated
    {
        int StartX { get; set; }
        int StartY { get; set; }
        int EndX { get; set; }
        int EndY { get; set; }
    }
}