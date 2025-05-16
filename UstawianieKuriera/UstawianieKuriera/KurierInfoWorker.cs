[assembly: Worker(typeof(KurierInfoWorker),typeof(DokumentHandlowy))]
namespace UstawianieKuriera;
public class KurierInfoWorker
{
  [Context] public DokumentHandlowy Dokument { get; set; }
  public string Kurier => Dokument.Kurier();
  public bool JestKurier => !string.IsNullOrEmpty(Kurier);
  public string Region => Dokument.Region();
  public Date PoprzedniaWysyłka { get {
      if(Dokument == null || string.IsNullOrWhiteSpace(Kurier)) return Date.Empty;
      var fakturySprzedaży = new FieldCondition.Equal("Kategoria",KategoriaHandlowa.Sprzedaż);
      var tenSamKurier = new FieldCondition.Equal("Features.Kurier",Kurier);
      var wcześniejszaData = new FieldCondition.LessEqual("Data",Dokument.Data);
      return Dokument.Module.DokHandlowe.WgKontrahent[Dokument.Kontrahent] 
        [fakturySprzedaży & tenSamKurier & wcześniejszaData]
        .Where(d => d != Dokument).OrderBy(d => d.Data).FirstOrDefault()?.Data ?? Date.Empty;
  }}
}