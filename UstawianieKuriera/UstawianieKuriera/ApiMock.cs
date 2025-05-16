namespace UstawianieKuriera;
internal class ApiMock
{
  public static string ZlecPrzesylke() {
    var random = new Random();
    var rnd = random.Next(1,10);
    if(rnd == 5) throw new Exception("nie można zlecić");
    System.Threading.Thread.Sleep(1000);
    return Guid.NewGuid().ToString();
  }
}
