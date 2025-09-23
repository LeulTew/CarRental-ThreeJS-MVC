using Google.Cloud.Speech.V1;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Carrental.Controllers
{
    public class SpeechToTextController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> POST()
        {
            try
            {
                using var reader = new BinaryReader(Request.Body);
                var audioData = reader.ReadBytes((int)Request.ContentLength);

                var speechClient = SpeechClient.Create();

                // Convert audioData byte array to ByteString
                ByteString audioByteString = ByteString.CopyFrom(audioData);

                var response = await speechClient.RecognizeAsync(new RecognitionConfig
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = 16000,
                    LanguageCode = "en-US",
                }, RecognitionAudio.FromBytes(audioByteString.ToByteArray()));

                var transcription = "";
                foreach (var result in response.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        transcription += alternative.Transcript + Environment.NewLine;
                    }
                }

                return Ok(transcription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
