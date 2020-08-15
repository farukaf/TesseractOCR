using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace TesseractOCR.Controllers
{
    public class OCRRequisition
    {
        public string FileExt { get; set; }
        public string Base64 { get; set; }
        public byte[] ByteArray { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class OCRController : ControllerBase
    {
        public const string folderName = "images/";
        public const string trainedDataFolderName = "TesseractData";

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("YAY");
        }


        [HttpPost]
        public IActionResult Post(OCRRequisition request)
        {
            try
            {
                if (request.ByteArray == null || !request.ByteArray.Any())
                {
                    if (string.IsNullOrWhiteSpace(request.Base64))
                    {
                        throw new Exception("No data received");
                    }

                    request.ByteArray = Convert.FromBase64String(request.Base64);
                }

                var filePath = folderName +  Guid.NewGuid().ToString() + "." + request.FileExt;
                System.IO.File.WriteAllBytes(filePath, request.ByteArray);

                string tessPath = Path.Combine(trainedDataFolderName, "");
                string result = "";
                using (var engine = new TesseractEngine(tessPath, "eng", EngineMode.Default))
                {
                    var page = engine.Process(Pix.LoadFromFile(filePath));
                    result = page.GetText();
                }

                return Ok(String.IsNullOrWhiteSpace(result) ? "Empty" : result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
