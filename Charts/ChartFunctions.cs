using System;
using System.IO;
using System.Windows.Forms;
using ModulusFE.LineStudies;
using ModulusFE;
using System.Windows.Media;
using System.Windows;

namespace M4.Charts
{
    public class ChartFunctions
    {
        public static void DrawLineStudy(StockChartX _stockChartX, string sLineStudyType)
        {
            LineStudy.StudyTypeEnum studyTypeEnum;
            object[] args = new object[0];
            double strokeThicknes = 1;

            if (sLineStudyType == "ImageFromFile")
            {
                if (File.Exists(@"Images\smiley.bmp"))
                {
                    studyTypeEnum = LineStudy.StudyTypeEnum.ImageObject;
                    switch (sLineStudyType)
                    {
                        case "ImageFromFile":
                            args = new[] {@"Images\smiley.bmp"}; //load a file from disk
                            break;
                    }
                }
                else
                {
                    studyTypeEnum = LineStudy.StudyTypeEnum.ImageObject;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = false;
                    openFileDialog.Filter = "Bitmap|*.bmp|Jpeg|*.jpg;*.jpeg|PNG|*.png|Icon|*.ico|GIF|*.gif";
                    DialogResult dialogResult = openFileDialog.ShowDialog();
                    if(dialogResult == DialogResult.OK)
                    {
                        string fileName = openFileDialog.FileName;
                        args = new[] {fileName};
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                studyTypeEnum = (LineStudy.StudyTypeEnum)Enum.Parse(typeof(LineStudy.StudyTypeEnum), sLineStudyType);

                //set some extra parameters to line studies
                switch (studyTypeEnum)
                {
                    case LineStudy.StudyTypeEnum.StaticText:
                        args = new object[] { "Some text for testing" };
                        strokeThicknes = 12; //for text objects is FontSize
                        break;
                    case LineStudy.StudyTypeEnum.VerticalLine:
                        //when first parameter is false, vertical line will display DataTime instead on record number
                        args = new object[]
                     {
                       false, //true - show record number, false - show datetime
                       true, //true - show text with line, false - show only line
                       "d", //custom date format, when args[0] == false. See MSDN:DateTime.ToString(string) for legal values
                     };
                        break;
                    default:
                        break;
                }
            }

            string studyName = studyTypeEnum.ToString();
            int count = _stockChartX.GetLineStudyCountByType(studyTypeEnum);
            if (count > 0)
                studyName += count;
            LineStudy lineStudy = _stockChartX.AddLineStudy(studyTypeEnum, studyName, System.Windows.Media.Brushes.Red, args);
            lineStudy.StrokeThickness = strokeThicknes;

            //if linestudy is a text object we can change its text directly
            if (lineStudy.GetType() == typeof(StaticText))
                ((StaticText)lineStudy).Text = "Some other text for testing";
            //by default rectangle and ellipse have transparent background.
            //but since they implement the IShapeAble interface we can change the default background
            if (lineStudy is IShapeAble)
                ((IShapeAble)lineStudy).Fill = new LinearGradientBrush
                {
                    Opacity = 0.3,
                    StartPoint = new System.Windows.Point(0, 0.5),
                    EndPoint = new System.Windows.Point(1, 0.5),
                    GradientStops = new GradientStopCollection
                                                             {
                                                               new GradientStop(Colors.Yellow, 0),
                                                               new GradientStop(Colors.LightSteelBlue, 0.5),
                                                               new GradientStop(Colors.Blue, 1)
                                                             }
                };
        }

    }
}
