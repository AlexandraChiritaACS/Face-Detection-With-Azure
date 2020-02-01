using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ImageProcessorCore;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
namespace FaceDetect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("78396eceef024cacb6cb12186cf8ee3e", @"https://westeurope.api.cognitive.microsoft.com/face/v1.0");

        static async void initPersonGroup()
        {
            string SubscriptionKey = "78396eceef024cacb6cb12186cf8ee3e";
            // Use your own subscription endpoint corresponding to the subscription key.
            string SubscriptionRegion = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/";
            FaceServiceClient faceServiceClient = new FaceServiceClient(SubscriptionKey, SubscriptionRegion);
            string personGroupId = "myfriends";
            
            //await faceServiceClient.CreatePersonGroupAsync(personGroupId, "My Friends");


            CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                // Id of the PersonGroup that the person belonged to
                personGroupId,
                // Name of the person
                "Serj"
            );

            // Creare instanta persoana
            CreatePersonResult friend2 = await faceServiceClient.CreatePersonAsync(
                // Id of the PersonGroup that the person belonged to
                personGroupId,
                // Name of the person
                "Daron"
            );


            const string friend1ImageDir = @"E:\System\Serj";

            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend1.PersonId, s);
                }
            }

            const string friend2ImageDir = @"E:\System\Daron";

            foreach (string imagePath in Directory.GetFiles(friend2ImageDir, "*.jpg"))
            {`
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend2.PersonId, s);
                }
            }

            // Training the neural network
            TrainingStatus trainingStatus = null;
            for (int i = 0; i <= 1000; i++)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }


            }

            string testImageFile = @"C:\Users\dell\source\repos\FaceDetect\FaceDetect\system-picture.jpg";

            using (Stream s = File.OpenRead(testImageFile))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();

                var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);

                foreach (var identifyResult in results)
                {
                    Console.WriteLine("Result of face: {0}", identifyResult.FaceId);
                    if (identifyResult.Candidates.Length == 0)
                    {
                        Console.WriteLine("Person not identified");
                        
                        // Blur person's face
                    }
                    else
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                        Console.WriteLine("Identified as {0}", person.Name);
                        // Highlight person's face.

                    }
                }
            }

            Console.ReadLine();
        }

        private async Task<FaceRectangle[]> UploadAndDetectFaces(string imageFilePath)
        {
            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    return faceRects.ToArray();
                }
            }
            catch (Exception)
            { 
                return new FaceRectangle[0];
            }
        }


        public MainWindow()
        {
            InitializeComponent();
            Title = "Face Detection and Recognition";
            var openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";

            /*bool? result = openDlg.ShowDialog();
            if (!(bool)result)
            {
                return;
            }*/

            string filePath = openDlg.FileName;
            Uri fileUri = new Uri(@"C:\Users\dell\source\repos\FaceDetect\FaceDetect\system-picture.jpg");
            BitmapImage bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.Default;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();
            FacePhoto.Source = bitmapSource;

            initIdentify(bitmapSource);

            
        }

        async void initIdentify(BitmapImage bitmapSource)
        {
            string SubscriptionKey = "78396eceef024cacb6cb12186cf8ee3e";
            // Use your own subscription endpoint corresponding to the subscription key.
            string SubscriptionRegion = "https://westeurope.api.cognitive.microsoft.com/face/v1.0/";
            FaceServiceClient faceServiceClient = new FaceServiceClient(SubscriptionKey, SubscriptionRegion);
            string personGroupId = "myfriends";

            //await faceServiceClient.CreatePersonGroupAsync(personGroupId, "My Friends");


            CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                // Id of the PersonGroup that the person belonged to
                personGroupId,
                // Name of the person
                "Serj"
            );

            CreatePersonResult friend2 = await faceServiceClient.CreatePersonAsync(
                // Id of the PersonGroup that the person belonged to
                personGroupId,
                // Name of the person
                "Daron"
            );


            const string friend1ImageDir = @"E:\System\Serj";

            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend1.PersonId, s);
                }
            }

            const string friend2ImageDir = @"E:\System\Daron";

            foreach (string imagePath in Directory.GetFiles(friend2ImageDir, "*.jpg"))
            {
                using (Stream s = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, friend2.PersonId, s);
                }
            }

            // Training the neural network
            TrainingStatus trainingStatus = null;
            for (int i = 0; i <= 1000; i++)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }


            }

            string testImageFile = @"C:\Users\dell\source\repos\FaceDetect\FaceDetect\system-picture.jpg";


            Title = "Detecting...";
            FaceRectangle[] faceRects = await UploadAndDetectFaces(@"C:\Users\dell\source\repos\FaceDetect\FaceDetect\system-picture.jpg");
            Title = String.Format("Detection Finished. {0} face(s) detected", faceRects.Length);
            using (Stream s = File.OpenRead(testImageFile))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();

                var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);


                if (faceRects.Length > 0)
                {
                    DrawingVisual visual = new DrawingVisual();
                    DrawingContext drawingContext = visual.RenderOpen();
                    drawingContext.DrawImage(bitmapSource,
                        new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                    double dpi = bitmapSource.DpiX;
                    double resizeFactor = 96 / dpi;
                    FileStream stream = File.OpenRead(@"C:\Users\dell\source\repos\FaceDetect\FaceDetect\system1-picture.jpg");
                    FileStream output = File.OpenWrite(@"C:\Users\dell\source\repos\FaceDetect\FaceDetect\system2-picture.jpg");
                    Image<ImageProcessorCore.Color, uint> image = new Image<ImageProcessorCore.Color, uint>(stream);
                    ImageProcessorCore.EdgeDetection E = new EdgeDetection();
                    for (int i = 0; i < faceRects.Length; i++)
                    {
                        FaceRectangle faceRect = (FaceRectangle)faceRects.GetValue(i);
                        IdentifyResult identifyResult = (IdentifyResult)results.GetValue(i);

                     

                        if (identifyResult.Candidates.Length == 0)
                        {
                                var rectangle = new ImageProcessorCore.Rectangle(
                                        faceRect.Left,
                                        faceRect.Top,
                                        faceRect.Width,
                                        faceRect.Height);

                                    image = image.BoxBlur(20, rectangle);
                                    
                           
                            


                            drawingContext.DrawRectangle(
                         Brushes.Transparent,
                         new Pen(Brushes.Green, 2),
                         new Rect(
                             faceRect.Left * resizeFactor,
                             faceRect.Top * resizeFactor,
                             faceRect.Width * resizeFactor,
                             faceRect.Height * resizeFactor
                             )
                     );

                        }

                        else
                        {
                            //drawingContext.PushOpacity(0);


                            drawingContext.DrawRectangle(
                         Brushes.Transparent,
                         new Pen(Brushes.Red, 2),
                         new Rect(
                             faceRect.Left * resizeFactor,
                             faceRect.Top * resizeFactor,
                             faceRect.Width * resizeFactor,
                             faceRect.Height * resizeFactor
                             )
                     );
                            // Get top 1 among all candidates returned
                            var candidateId = identifyResult.Candidates[0].PersonId;
                            var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                            
                            

                            // Highlight person's face.


                            ImageProcessorCore.Rectangle rectangle = new ImageProcessorCore.Rectangle(
                                        faceRect.Left,
                                        faceRect.Top,
                                        faceRect.Width,
                                        faceRect.Height
                                        );

                            TextBox textbox = new TextBox();
                            textbox.Text = person.Name;
                            
                            image = image.BlackWhite(rectangle);
                            
                            String testString = person.Name;
                            FormattedText formattedText = new FormattedText(
                    testString,
                   CultureInfo.GetCultureInfo("en-us"),
                   FlowDirection.LeftToRight,
                   new Typeface("Verdana"),
                   30,
                   Brushes.Red);
                   drawingContext.DrawText(formattedText, new System.Windows.Point(faceRect.Left * resizeFactor - 40,faceRect.Top * resizeFactor - 40));
                   
                        }
                    }
                    image.SaveAsJpeg(output);
                    stream.Dispose();
                    output.Dispose();
                    drawingContext.Close();
                    RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                        (int)(bitmapSource.PixelWidth * (resizeFactor)),
                        (int)(bitmapSource.PixelHeight * (resizeFactor)),
                        96,
                        96,
                        PixelFormats.Pbgra32);

                    faceWithRectBitmap.Render(visual);
                    FacePhoto.Source = faceWithRectBitmap;
                    
                }

                InitializeComponent();
            }


        }

    }
}
