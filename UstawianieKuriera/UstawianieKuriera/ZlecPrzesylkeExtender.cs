[assembly: Worker(typeof(ZlecPrzesylkeExtender))]
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

  public string PrzesyłkaID => Dokument.PrzesyłkaId();
  public bool JestJużPrzesyłka => Dokument.JestPrzesyłka();
  public string CzasZlecenia => Dokument.PrzesyłkaCzas();

  
  public bool NieMożnaZlecić => !KurierInfo.JestKurier || JestJużPrzesyłka;

  public void ZlećPrzesyłkę() {
    if(Dokument == null) throw new Exception("Wskaż dokument.");
    if(JestJużPrzesyłka) throw new Exception("Przesyłka została już zlecona.");
    string id = null;
    Exception ex = null;
    try { id = ApiMock.ZlecPrzesylke(); }
    catch(Exception e) { ex = e; }
    Dokument.Session.ExecuteInTransaction(()=> {
      if(ex == null) Dokument.SetPrzesyłka(id); 
      else Dokument.SetPrzesyłka(ex);
    });
  }

  public UIElement GetUI() {
    if(!KurierInfo.JestKurier) return null;
    var krier = KurierInfo.Kurier;
    var elems = new List<UIElement>();
    switch(krier.ToLower()) {
      case "dhl":
        elems.Add(CreateField("Godzina dostawy","Features.KurierGodzinaDostawy",10));
        break;
      case "ups":
        elems.Add(CreateField("Telefon odbiorcy","Features.KurierTelefon", 20));
        break;
      case "inpost":
        elems.Add(CreateField("Paczkomat docelowy","Features.KurierPaczkomat",15));
        break;
    }
    return CreateGroup("Parametry " + krier,krier.Length + 3, elems.ToArray());
  }
  UIElement CreateField(string caption,string editValue,int width) =>
    new FieldElement {
      CaptionHtml = caption,
      LabelWidth = "20",
      Width = width.ToString(),
      EditValue = "{Dokument." + editValue + "}"
    };
  UIElement CreateGroup(string caption,int labelWidth,UIElement[] elements) =>
    new GroupContainer(elements) {
      CaptionHtml = caption,
      LabelWidth = labelWidth.ToString()
    };
}
