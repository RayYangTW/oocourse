using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace personal_project.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoController : ControllerBase
{


  private readonly ILogger<VideoController> _logger;

  public VideoController(ILogger<VideoController> logger)
  {
    _logger = logger;
  }

  [HttpGet("/generate")]
  public IActionResult GetRoom()
  {
    string roomId = GenerateRandomRoomId(10);
    return Redirect($"/room.html?roomId={roomId}");
  }



  private string GenerateRandomRoomId(int length)
  {
    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    Random random = new Random();

    StringBuilder sb = new StringBuilder();
    for (int i = 0; i < length; i++)
    {
      int index = random.Next(chars.Length);
      sb.Append(chars[index]);
    }

    return sb.ToString();
  }
}
