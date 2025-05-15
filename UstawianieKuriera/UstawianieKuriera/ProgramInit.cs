using Soneta.Business;
using Soneta.Business.App;
using UstawianieKuriera;

[assembly: ProgramInitializer(typeof(ProgramInit))]
[assembly: Service(typeof(ILoginListener), typeof(ProgramInit), ServiceScope.Login)]
namespace UstawianieKuriera;

class ProgramInit : ILoginListener, IProgramInitializer
{
  static ProgramInit() {
    Soneta.Handel.HandelModule.DokumentHandlowySchema
      //taka dziwaczna z pozoru konstrukcja odczytuje wartość cechy
      //po czym przypisuje ją do niej ponownie czym wywołuje cały kod
      //na niej zdefiniowany do weryfikacji poprawności,
      //który zadziała już na nowowybranym kontrahencie. 
      .AddKontrahentAfterEdit(row => { 
        row.Features["Kurier"] = row.Features["Kurier"]; });
  }

  public void Initialize() { }
  public void BeforeLogin(Session session) { }
  public void AfterLogin(Login login) { }
  public void BeforeUnlogin(Login login) { }  
}

