namespace UstawianieKuriera;
public static class FeaturesExtensions 
{
  public static string Kurier(this DokumentHandlowy dok) => 
    dok?.Features.GetString("Kurier") ?? "";

  public static string Region(this Kontrahent knt) => 
    knt?.Features.GetString("Region") ?? "";

  public static string Region(this DokumentHandlowy dok) => Region(dok.Kontrahent);

  public static string PrzesyłkaCzas(this DokumentHandlowy dok) => 
    dok?.Features.GetString("CzasZlecenia") ?? "";

  public static string PrzesyłkaId(this DokumentHandlowy dok) => 
    dok?.Features.GetString("NumerPrzesylki") ?? "";
  public static bool JestPrzesyłka(this DokumentHandlowy dok) => 
    Guid.TryParse(PrzesyłkaId(dok),out _);
  public static bool JestBłądPrzesyłki(this DokumentHandlowy dok) => 
    PrzesyłkaId(dok).StartsWith(__ZnacznikBłędu);

  const string __ZnacznikBłędu = "---";
  public static void SetPrzesyłka(this DokumentHandlowy dok,Exception e) =>
    SetPrzesyłka(dok,__ZnacznikBłędu + e.Message);
  public static void SetPrzesyłka(this DokumentHandlowy dok, string id) {
    if(dok == null) throw new ArgumentNullException(nameof(dok));
    dok.Features["NumerPrzesylki"] = id;
    dok.Features["CzasZlecenia"] = 
      string.IsNullOrWhiteSpace(id) ? "" : Date.Now.ToString("s").Replace("T"," ");
  }
}
