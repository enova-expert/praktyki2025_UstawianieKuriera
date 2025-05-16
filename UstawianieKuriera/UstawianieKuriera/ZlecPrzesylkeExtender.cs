using System.Collections.Generic;
namespace UstawianieKuriera;
public class ZlecPrzesylkeExtender
{
  private DokumentHandlowy _Dokument;
  [Context] public DokumentHandlowy Dokument { 
    get => _Dokument; 
    set {
      _Dokument = value;
      KurierInfo.Dokument = value;
  }}

  public KurierInfoWorker KurierInfo { get; } = new();

  public bool JestKurier => !string.IsNullOrEmpty(Dokument.Kurier());
  public bool JestJużPrzesyłka => !string.IsNullOrWhiteSpace(Dokument.PrzesyłkaId()) 
    && !Dokument.PrzesyłkaId().StartsWith(__ZnacznikBłędu);

  const string __ZnacznikBłędu = "---";
  public void ZlećPrzesyłkę() {
    if(Dokument == null) throw new Exception("Wskaż dokument.");
    if(JestJużPrzesyłka) throw new Exception("Przesyłka została już zlecona.");
    string id;
    try { id = ApiMock.ZlecPrzesylke(); }
    catch(Exception ex) { id = __ZnacznikBłędu + ex.Message; }
    using(var tran = Dokument.Session.Logout(true)) {
      Dokument.SetPrzesyłka(id);
      tran.CommitUI();
    }
  }

  private void UpdateFeatures() {
    foreach(FeatureDefinition definition in (SubTable<FeatureDefinition>)this.ZapisGlowny.Features.Definitions) {
      if(!((IEnumerable<FeatureAlgorithm>)new FeatureAlgorithm[2]
      {
        FeatureAlgorithm.Get,
        FeatureAlgorithm.GetArgs
      }).Contains<FeatureAlgorithm>(definition.Algorithm))
        this.Features[definition.Name] = this.ZapisGlowny.Features[definition.Name];
    }
  }


}
