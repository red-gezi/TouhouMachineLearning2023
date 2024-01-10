using Microsoft.AspNetCore.Mvc;
using System.IO;

[ApiController]
[Route("Download")]
public class Api : ControllerBase
{

    [HttpGet()]
    public IActionResult Get()
    {
        // 处理GET请求逻辑
        return Ok("yes");
    }
    [HttpGet("{type}/{fileName}")]
    public IActionResult Get(string type, string fileName)
    {
        string path = type switch
        {
            //AB包资源
            "PC_Release" => $"AssetBundles/PC_Release/{fileName}",
            "PC_Test" => $"AssetBundles/PC_Test/{fileName}",
            "Android" => $"AssetBundles/Android/{fileName}",
            //Dll和Apk
            "PC_Release_Dll" => $"AssetBundles/DllOrAPK/PC_Release/{fileName}",
            "PC_Test_Dll" => $"AssetBundles/DllOrAPK/PC_Test/{fileName}",
            "Apk" => $"AssetBundles/DllOrAPK/Android/{fileName}",
            _ => $"AssetBundles/{type}/{fileName}" // 默认路径
        };
        if (System.IO.File.Exists(path))
        {
            return File(System.IO.File.ReadAllBytes(path), "application/octet-stream");
        }
        else
        {
            return NotFound();
        }

    }
    [HttpGet("{*filePath}")]
    public IActionResult Get(string filePath)
    {
        string path = $"AssetBundles/{filePath}";
        if (System.IO.File.Exists(path))
        {

            // 如果是文件，则返回文件内容
            return File(System.IO.File.ReadAllBytes(path), "application/octet-stream");
        }
        else if (Directory.Exists(path))
        {
            // 如果是文件夹，则返回文件夹的内容
            string[] files = Directory.GetFiles(path);
            return Ok(files);
        }
        else
        {
            return NotFound();
        }
    }
}
