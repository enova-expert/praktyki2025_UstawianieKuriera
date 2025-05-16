[assembly: Worker(typeof(RaportKurierow),typeof(DokHandlowe))]
namespace UstawianieKuriera;
public class RaportKurierow
{
  [Context] public DokumentHandlowy[] Dokumenty { get; set; }
  [Context] public Params Parametry { get; set; }


  [Action("Generuj raport",
    Mode = ActionMode.SingleSession,
    Target = ActionTarget.ToolbarWithText)]  
  public object UtworzRaport() {
    if(!Dokumenty?.Any() ?? false) return "Zaznacz faktury.";
    var doks = Dokumenty!.Where(d => !string.IsNullOrWhiteSpace(d.Kurier())).ToArray();
    var okres = new FromTo(Dokumenty.Select(d => d.Data).Min(),Dokumenty.Select(d => d.Data).Max());
    var raport = doks.Select(d=> new { 
          kurier = d.Kurier(), 
          jestPrzesyłka = d.JestPrzesyłka(), 
          iserror=d.JestBłądPrzesyłki() } )
      .GroupBy(k => k.kurier)
      .Select(k => new { 
        kurier = k.Key, 
        razem = k.Count(), 
        poprawnych = k.Count(p => p.jestPrzesyłka), 
        błędnych = k.Count(p => p.iserror),
        brak = k.Count(p=> !p.jestPrzesyłka && !p.iserror)
      });

    var raportTxt = Parametry.TytułRaportu + " za okres: " + okres + env.NewLine
      + string.Join(env.NewLine, raport.Select(k => k.kurier + ": " + k.razem 
            + " (w tym błędów: "+k.błędnych+")").ToArray()) + env.NewLine
      + "Raport przygotował: " + Dokumenty.First().Session.Login.Operator.Name;

#if DEBUG
    new Log("DebugInfo",true).WriteLine(raportTxt);
#endif

    return new NamedStream(Parametry.TytułRaportu + ".txt",Encoding.UTF8.GetBytes(raportTxt));
  }

  public class Params {
    public string TytułRaportu { get; set; } = 
      Date.Today.ToString("yyyy-MM-dd") + " - Raport kurierów";
  }
}
