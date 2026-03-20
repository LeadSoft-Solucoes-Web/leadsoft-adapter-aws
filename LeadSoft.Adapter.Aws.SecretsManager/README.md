# LeadSoft® Adapter Aws Secrets Manager

Este pacote `Open Source` serve como um uma interface simples para adicionar como injeção de dependência, a integração com o AWS Secrets Manager, de forma mais enxuta. Ele é parte da nossa iniciativa de compartilhar conhecimento e recursos com a comunidade de desenvolvimento, permitindo que outros desenvolvedores possam se beneficiar do nosso trabalho e contribuir para o crescimento da comunidade de desenvolvimento.

Este pacote é mantido pela [LeadSoft®](https://leadsoft.com.br/), uma empresa de tecnologia que oferece soluções inovadoras para o mercado. Se você tiver alguma dúvida ou sugestão, não hesite em entrar em contato conosco.

#### [Nuget.Org: LeadSoft.Adapter.Aws.SecretsManager](https://www.nuget.org/packages/LeadSoft.Adapter.Aws.SecretsManager)
#### [GitHub Repo: leadsoft-adapter-aws](https://github.com/LeadSoft-Solucoes-Web/leadsoft-adapter-aws)

## Principais características
- Compatível com .NET 10.0.
- Chamadas assíncronas com `async`/`await`.
- Integração simples com _Dependency Injection_ (DI) do .NET.
- Tratamento centralizado de erros e respostas HTTP.
- Open Source (MIT License).

## Métodos disponíveis
- `Task<string> GetSecretValueAsync(string aKey)`
    - Obtém uma chave do AWS Secrets Manager pelo nome da chave.
- `Task<Dictionary<string, string>> GetSecretsAsync();`
    - Obtém a lista de todas as chaves do AWS Secrets Manager. (Este método é destinado para uso apenas em ambientes não produtivos.)

---

## Instalação
Pelo CLI do .NET:

```bash
dotnet add package LeadSoft.Adapter.Aws.SecretsManager
```

Ou via NuGet Package Manager no Visual Studio (pesquise por `LeadSoft.Adapter.Aws.SecretsManager`).

## Versionamento e compatibilidade
- Projeto direcionado para .NET 10.0. Verifique a compatibilidade do pacote com sua aplicação.
- Seguir práticas de versionamento semântico: breaking changes → major, novas features → minor, correções → patch.

## Licença
Consulte o arquivo de licença no repositório para detalhes sobre uso e redistribuição.

## Contribuição

Se você deseja contribuir para este projeto, sinta-se à vontade para enviar pull requests ou abrir issues. Estamos sempre abertos a sugestões e melhorias.

---

###  Desenvolvimento  
Desenvolvido pelo time da LeadSoft® Soluções Web.
* [Lucas Resende Tavares](https://www.linkedin.com/in/lucasrtavares/)
  
#### Nossa empresa
**LeadSoft®** é uma marca registrada pertencente à **Lucas R Tavares Tech Ltda** | CNPJ: 31.706.323/0001-29

#### Como nos encontrar:
- [Nosso Site](https://www.leadsoft.inf.br)
- [GitHub](https://github.com/LeadSoft-Solucoes-Web)
- [LinkedIn](https://www.linkedin.com/company/leadsoft-solucoes-web)
- [Behance](https://www.behance.net/leadsofsolue)
- [YouTube](https://www.youtube.com/@LeadsoftSolucoesWeb)
- [Instagram](https://www.instagram.com/leadsoft.inf/)
- [Facebook](https://www.facebook.com/leadsoft.inf.br)

#### INFORMAÇÕES DE CONTATO  Se você tiver alguma dúvida sobre estes Termos ou Serviços, entre em contato conosco em
[developers@leadsoft.inf.br](mailto:developers@leadsoft.inf.br).
[developers@leadsoft.inf.br](mailto:developers@leadsoft.inf.br).