using Soneta.Business.App;

[assembly: ProgramInitializer(typeof(ProgramInit))]
[assembly: Service(typeof(ILoginListener), typeof(ProgramInit), ServiceScope.Login)]
namespace UstawianieKuriera;

class ProgramInit : ILoginListener, IProgramInitializer
{
  static ProgramInit() {
    Soneta.Handel.HandelModule.DokumentHandlowySchema
      //taka dziwaczna z pozoru konstrukcja odczytuje wartość cechy
      //po czym przypisuje ją do niej ponownie czym wywołuje cały kod settera
      //na niej zdefiniowana jest weryfikacja poprawności,
      //która zadziała już na nowowybranym kontrahencie. 
      .AddKontrahentAfterEdit(row => {
        //najlepiej byłoby użyć (rozbudować) FeaturesExtension
        row.Features["Kurier"] = row.Features["Kurier"]; }); 
  }

  public void Initialize() { }
  public void BeforeLogin(Session session) { }
  public void AfterLogin(Login login) { }
  public void BeforeUnlogin(Login login) { }  
}

