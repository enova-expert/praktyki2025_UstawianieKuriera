using Soneta.Business;
using Soneta.Waluty;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Soneta.Ksiega;

[Soneta.Types.Caption("Zapis księgowy - walutowy")]
public sealed class ZapisWalutowy : ZapisKsiegowy
{
  internal ZapisWalutowy(ZapisKsiegowy zapisGlowny)
    : base(zapisGlowny.Okres,zapisGlowny.Dekret,zapisGlowny.TypDziennika,TypZapisu.Walutowy) {
    this.ZapisGlowny = zapisGlowny;
  }

  public ZapisWalutowy(RowCreator creator)
    : base(creator) {
  }

  protected override void OnAdded() {
    base.OnAdded();
    this.Init();
  }

  protected override bool CalcReadOnly() => true;

  private void Init() {
    Waluta waluta = WalutyModule.GetInstance((ISessionable)this.Session).Waluty[this.ZapisGlowny.KwotaOperacji.Symbol];
    this.Lp = this.ZapisGlowny.Lp + 1;
    this.Konto = (KontoBase)this.ZapisGlowny.Konto[waluta];
    this.SetKwotaOperacji(this.ZapisGlowny.Strona,this.ZapisGlowny.KwotaOperacji);
    this.Opis = this.ZapisGlowny.Opis;
    this.DataPodatkowa = this.ZapisGlowny.DataPodatkowa;
    this.UpdateFeatures();
  }

  private void UpdateFeatures() {
    foreach(FeatureDefinition definition in this.ZapisGlowny.Features.Definitions) {
      if(! new[] { FeatureAlgorithm.Get,FeatureAlgorithm.GetArgs }.Contains(definition.Algorithm))
        this.Features[definition.Name] = this.ZapisGlowny.Features[definition.Name];
    }
  }
}
