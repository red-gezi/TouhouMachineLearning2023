// See https://aka.ms/new-console-template for more information
using PlantUml.Net;
using System.Drawing;
using System.IO;
List<Image> images=new List<Image>();
var factory = new RendererFactory();
var renderer = factory.CreateRenderer(new PlantUmlSettings());
string path = @"D:\vs2022-Project\TouhouMachineLearning2022\ImageSummary.txt";
var infos = File.ReadAllText(path).Split("######\r\n").ToList();
infos.Remove("");
for (int i = 0; i < infos.Count(); i++)
{
    var bytes = await renderer.RenderAsync(infos[i].Replace("*", "_"), OutputFormat.Png);
    File.WriteAllBytes(i + ".png", bytes);
    images.Add(Image.FromFile(i + ".png"));
}
CombinImage(images);

void CombinImage(List<Image> images)
{
    var width = images.Max(image => image.Width);
    var height = images.Sum(image => image.Height);
    // 初始化画布(最终的拼图画布)并设置宽高
    Bitmap bitMap = new Bitmap(width, height);
    // 初始化画板
    Graphics g = Graphics.FromImage(bitMap);
    // 将画布涂为白色(底部颜色可自行设置)
    g.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
    for (int i = 0; i < images.Count; i++)
    {

        g.DrawImage(images[i], 0, images.Take(i).Sum(image => image.Height));

    }
    images.ForEach(image => image.Dispose());
    //保存
    bitMap.Save("合并.png");
    bitMap.Dispose();
}
