using System;
using System.Linq;
using System.Text;
using Soneta.Business;
using Soneta.CRM;
using Soneta.Handel;
using Soneta.Types;
using UstawianieKuriera;

[assembly: Worker(typeof(RaportKurierow),typeof(DokHandlowe))]
namespace UstawianieKuriera;

public class RaportKurierow
{
  [Context] public DokumentHandlowy[] Dokumenty { get; set; }


  [Action("Generuj raport",
    Mode = ActionMode.SingleSession,
    Target = ActionTarget.ToolbarWithText)]  
  public object UtworzRaport(Context cx) {
    if(!Dokumenty?.Any() ?? false) return "Zaznacz faktury.";

    var raport = Dokumenty!.Select(d => d.Features.GetString("Kurier"))
      .Where(k=>!string.IsNullOrWhiteSpace(k))
      .GroupBy(k => k)
      .Select(k => new { kurier = k.Key,liczba = k.Count() });

    var nazwaRaportu = Date.Today.ToString("yyyy-MM-dd")
      + " - Raport kurierów ";
    var raportTxt = nazwaRaportu + Environment.NewLine
      + string.Join(Environment.NewLine,
        raport.Select(k => k.kurier + ": " + k.liczba).ToArray());

#if DEBUG
    new Log("DebugInfo",true).WriteLine(raportTxt);
#endif

    return new NamedStream(nazwaRaportu+".txt",
      Encoding.UTF8.GetBytes(raportTxt));
  }
}
